using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class CommandeFournisseurService
    {
        /// <summary>
        /// Générer un nouveau numéro de commande unique
        /// </summary>
        public static string GenererNumeroCommande()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("CALL sp_generer_numero_commande(@numero)", conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@numero", MySqlDbType.VarChar, 50)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        });
                        
                        cmd.ExecuteNonQuery();
                        return cmd.Parameters["@numero"].Value.ToString() ?? $"CMD-{DateTime.Now.Year}-0001";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur génération numéro: {ex.Message}");
                return $"CMD-{DateTime.Now.Year}-{DateTime.Now.Ticks % 10000:D4}";
            }
        }

        /// <summary>
        /// Créer une nouvelle commande fournisseur
        /// </summary>
        public static int CreerCommande(CommandeFournisseur commande, List<LigneCommandeFournisseur> lignes)
        {
            if (lignes == null || lignes.Count == 0)
                throw new Exception("La commande doit contenir au moins une ligne");

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Calculer les totaux
                        decimal sousTotal = 0;
                        foreach (var ligne in lignes)
                        {
                            sousTotal += ligne.QuantiteCommandee * ligne.PrixUnitaire;
                        }
                        
                        var (tps, tvq, total) = TaxesService.CalculerTaxes(sousTotal);
                        
                        // 2. Insérer la commande
                        string insertCommande = @"
                            INSERT INTO commandes_fournisseurs 
                            (numero_commande, date_commande, date_livraison_prevue, fournisseur_id, employe_id,
                             sous_total, montant_tps, montant_tvq, montant_total, statut, note_interne)
                            VALUES 
                            (@numero, @dateCommande, @dateLivraison, @fournisseurId, @employeId,
                             @sousTotal, @tps, @tvq, @total, @statut, @note);
                            SELECT LAST_INSERT_ID();";
                        
                        int commandeId;
                        using (var cmd = new MySqlCommand(insertCommande, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@numero", commande.NumeroCommande);
                            cmd.Parameters.AddWithValue("@dateCommande", commande.DateCommande);
                            cmd.Parameters.AddWithValue("@dateLivraison", commande.DateLivraisonPrevue.HasValue ? commande.DateLivraisonPrevue.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@fournisseurId", commande.FournisseurId);
                            cmd.Parameters.AddWithValue("@employeId", commande.EmployeId.HasValue ? commande.EmployeId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@sousTotal", sousTotal);
                            cmd.Parameters.AddWithValue("@tps", tps);
                            cmd.Parameters.AddWithValue("@tvq", tvq);
                            cmd.Parameters.AddWithValue("@total", total);
                            cmd.Parameters.AddWithValue("@statut", "En attente");
                            cmd.Parameters.AddWithValue("@note", commande.NoteInterne ?? (object)DBNull.Value);
                            
                            commandeId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        
                        // 3. Insérer les lignes de commande
                        foreach (var ligne in lignes)
                        {
                            string insertLigne = @"
                                INSERT INTO lignes_commandes_fournisseurs 
                                (commande_id, produit_id, sku, description, quantite_commandee, quantite_recue, prix_unitaire, montant_ligne)
                                VALUES 
                                (@commandeId, @produitId, @sku, @description, @qteCommande, 0, @prix, @montant)";
                            
                            using (var cmd = new MySqlCommand(insertLigne, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@commandeId", commandeId);
                                cmd.Parameters.AddWithValue("@produitId", ligne.ProduitId);
                                cmd.Parameters.AddWithValue("@sku", ligne.SKU);
                                cmd.Parameters.AddWithValue("@description", ligne.Description);
                                cmd.Parameters.AddWithValue("@qteCommande", ligne.QuantiteCommandee);
                                cmd.Parameters.AddWithValue("@prix", ligne.PrixUnitaire);
                                cmd.Parameters.AddWithValue("@montant", ligne.QuantiteCommandee * ligne.PrixUnitaire);
                                
                                cmd.ExecuteNonQuery();
                            }
                        }
                        
                        transaction.Commit();
                        return commandeId;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Recevoir une commande (complète ou partielle) et mettre à jour le stock
        /// </summary>
        public static bool RecevoirCommande(int commandeId, Dictionary<int, int> quantitesRecues, int? employeId)
        {
            var commande = GetCommandeById(commandeId);
            if (commande == null)
                throw new Exception("Commande introuvable");
            
            if (commande.Statut == "Annulée")
                throw new Exception("Impossible de recevoir une commande annulée");
            
            if (commande.Statut == "Reçue")
                throw new Exception("Cette commande a déjà été complètement reçue");

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        bool commandeComplete = true;
                        
                        // Pour chaque ligne de commande
                        foreach (var ligne in commande.Lignes)
                        {
                            if (quantitesRecues.ContainsKey(ligne.Id))
                            {
                                int qteRecue = quantitesRecues[ligne.Id];
                                
                                if (qteRecue > 0)
                                {
                                    // Mettre à jour la quantité reçue
                                    string updateLigne = @"
                                        UPDATE lignes_commandes_fournisseurs 
                                        SET quantite_recue = quantite_recue + @qteRecue
                                        WHERE id = @id";
                                    
                                    using (var cmd = new MySqlCommand(updateLigne, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@qteRecue", qteRecue);
                                        cmd.Parameters.AddWithValue("@id", ligne.Id);
                                        cmd.ExecuteNonQuery();
                                    }
                                    
                                    // Augmenter le stock
                                    string updateStock = @"
                                        UPDATE niveaux_stock 
                                        SET qte_disponible = qte_disponible + @quantite 
                                        WHERE produit_id = @produitId";
                                    
                                    using (var cmd = new MySqlCommand(updateStock, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@quantite", qteRecue);
                                        cmd.Parameters.AddWithValue("@produitId", ligne.ProduitId);
                                        cmd.ExecuteNonQuery();
                                    }
                                    
                                    // Créer un mouvement de stock
                                    string insertMouvement = @"
                                        INSERT INTO mouvements_stock 
                                        (produit_id, type_mouvement, quantite, raison, note_utilisateur, employe_id, date_mouvement)
                                        VALUES 
                                        (@produitId, 'ENTREE', @quantite, 'reception_commande', @note, @employeId, NOW())";
                                    
                                    using (var cmd = new MySqlCommand(insertMouvement, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@produitId", ligne.ProduitId);
                                        cmd.Parameters.AddWithValue("@quantite", qteRecue);
                                        cmd.Parameters.AddWithValue("@note", $"Réception commande {commande.NumeroCommande}");
                                        cmd.Parameters.AddWithValue("@employeId", employeId.HasValue ? employeId.Value : DBNull.Value);
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            
                            // Vérifier si cette ligne est complète
                            int nouvelleQteRecue = ligne.QuantiteRecue + (quantitesRecues.ContainsKey(ligne.Id) ? quantitesRecues[ligne.Id] : 0);
                            if (nouvelleQteRecue < ligne.QuantiteCommandee)
                            {
                                commandeComplete = false;
                            }
                        }
                        
                        // Mettre à jour le statut de la commande
                        string nouveauStatut = commandeComplete ? "Reçue" : "Partiellement reçue";
                        DateTime? dateReception = commandeComplete ? DateTime.Now : (DateTime?)null;
                        
                        string updateCommande = @"
                            UPDATE commandes_fournisseurs 
                            SET statut = @statut, date_reception = @dateReception
                            WHERE id = @id";
                        
                        using (var cmd = new MySqlCommand(updateCommande, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@statut", nouveauStatut);
                            cmd.Parameters.AddWithValue("@dateReception", dateReception.HasValue ? dateReception.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@id", commandeId);
                            cmd.ExecuteNonQuery();
                        }
                        
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Obtenir toutes les commandes fournisseurs
        /// </summary>
        public static List<CommandeFournisseur> GetAllCommandes()
        {
            var commandes = new List<CommandeFournisseur>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            cf.*,
                            f.nom AS nom_fournisseur,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM commandes_fournisseurs cf
                        INNER JOIN fournisseurs f ON cf.fournisseur_id = f.id
                        LEFT JOIN employes e ON cf.employe_id = e.id
                        ORDER BY cf.date_commande DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            commandes.Add(MapCommandeFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des commandes: {ex.Message}", ex);
            }

            return commandes;
        }

        /// <summary>
        /// Obtenir une commande par ID avec ses lignes
        /// </summary>
        public static CommandeFournisseur? GetCommandeById(int id)
        {
            CommandeFournisseur? commande = null;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    
                    // Récupérer la commande
                    string queryCommande = @"
                        SELECT 
                            cf.*,
                            f.nom AS nom_fournisseur,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM commandes_fournisseurs cf
                        INNER JOIN fournisseurs f ON cf.fournisseur_id = f.id
                        LEFT JOIN employes e ON cf.employe_id = e.id
                        WHERE cf.id = @id";
                    
                    using (var cmd = new MySqlCommand(queryCommande, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                commande = MapCommandeFromReader(reader);
                            }
                        }
                    }
                    
                    if (commande != null)
                    {
                        // Récupérer les lignes
                        string queryLignes = @"
                            SELECT 
                                lcf.*,
                                p.nom AS nom_produit
                            FROM lignes_commandes_fournisseurs lcf
                            INNER JOIN produits p ON lcf.produit_id = p.id
                            WHERE lcf.commande_id = @commandeId";
                        
                        using (var cmd = new MySqlCommand(queryLignes, conn))
                        {
                            cmd.Parameters.AddWithValue("@commandeId", id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    commande.Lignes.Add(new LigneCommandeFournisseur
                                    {
                                        Id = reader.GetInt32("id"),
                                        CommandeId = reader.GetInt32("commande_id"),
                                        ProduitId = reader.GetInt32("produit_id"),
                                        SKU = reader.GetString("sku"),
                                        Description = reader.GetString("description"),
                                        QuantiteCommandee = reader.GetInt32("quantite_commandee"),
                                        QuantiteRecue = reader.GetInt32("quantite_recue"),
                                        PrixUnitaire = reader.GetDecimal("prix_unitaire"),
                                        MontantLigne = reader.GetDecimal("montant_ligne"),
                                        NomProduit = reader.GetString("nom_produit")
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la commande: {ex.Message}", ex);
            }

            return commande;
        }

        /// <summary>
        /// Mapper une commande depuis un DataReader
        /// </summary>
        private static CommandeFournisseur MapCommandeFromReader(MySqlDataReader reader)
        {
            return new CommandeFournisseur
            {
                Id = reader.GetInt32("id"),
                NumeroCommande = reader.GetString("numero_commande"),
                DateCommande = reader.GetDateTime("date_commande"),
                DateLivraisonPrevue = reader.IsDBNull(reader.GetOrdinal("date_livraison_prevue")) ? null : reader.GetDateTime("date_livraison_prevue"),
                DateReception = reader.IsDBNull(reader.GetOrdinal("date_reception")) ? null : reader.GetDateTime("date_reception"),
                FournisseurId = reader.GetInt32("fournisseur_id"),
                EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) ? null : reader.GetInt32("employe_id"),
                SousTotal = reader.GetDecimal("sous_total"),
                MontantTPS = reader.GetDecimal("montant_tps"),
                MontantTVQ = reader.GetDecimal("montant_tvq"),
                MontantTotal = reader.GetDecimal("montant_total"),
                Statut = reader.GetString("statut"),
                NoteInterne = reader.IsDBNull(reader.GetOrdinal("note_interne")) ? null : reader.GetString("note_interne"),
                NomFournisseur = reader.GetString("nom_fournisseur"),
                NomEmploye = reader.IsDBNull(reader.GetOrdinal("nom_employe")) ? "N/A" : reader.GetString("nom_employe")
            };
        }
    }
}

