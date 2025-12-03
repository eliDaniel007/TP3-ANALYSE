using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class FactureService
    {
        /// <summary>
        /// Générer un nouveau numéro de facture unique
        /// </summary>
        public static string GenererNumeroFacture()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("CALL sp_generer_numero_facture(@numero)", conn))
                    {
                        cmd.Parameters.Add(new MySqlParameter("@numero", MySqlDbType.VarChar, 50)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        });
                        
                        cmd.ExecuteNonQuery();
                        return cmd.Parameters["@numero"].Value.ToString() ?? $"FAC-{DateTime.Now.Year}-0001";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur génération numéro: {ex.Message}");
                // Fallback: générer manuellement
                return $"FAC-{DateTime.Now.Year}-{DateTime.Now.Ticks % 10000:D4}";
            }
        }

        /// <summary>
        /// Créer une nouvelle facture avec validation
        /// </summary>
        public static int CreerFacture(Facture facture, List<LigneFacture> lignes)
        {
            // VALIDATION 1: Vérifier que le client est actif
            var client = ClientService.GetClientById(facture.ClientId);
            if (client == null)
                throw new Exception("Client introuvable");
            
            if (client.Statut == "Inactif")
                throw new Exception("Impossible de créer une facture pour un client inactif");
            
            // VALIDATION 2: Vérifier si le client a des factures en retard
            if (ClientAFacturesEnRetard(facture.ClientId))
                throw new Exception("Le client a des factures en retard. Vente refusée.");
            
            // VALIDATION 3: Vérifier le stock disponible pour tous les produits
            foreach (var ligne in lignes)
            {
                if (!VerifierStockDisponible(ligne.ProduitId, ligne.Quantite))
                {
                    var produit = ProduitService.GetProduitById(ligne.ProduitId);
                    throw new Exception($"Stock insuffisant pour le produit '{produit?.Nom}'. Stock disponible: {GetStockDisponible(ligne.ProduitId)}");
                }
            }

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Insérer la facture
                        string insertFacture = @"
                            INSERT INTO factures 
                            (numero_facture, date_facture, date_echeance, client_id, employe_id, 
                             sous_total, montant_tps, montant_tvq, montant_total, montant_du, 
                             statut_paiement, statut, note_interne, conditions_paiement)
                            VALUES 
                            (@numero, @dateFacture, @dateEcheance, @clientId, @employeId, 
                             @sousTotal, @tps, @tvq, @total, @du, 
                             @statutPaiement, @statut, @note, @conditions);
                            SELECT LAST_INSERT_ID();";
                        
                        int factureId;
                        using (var cmd = new MySqlCommand(insertFacture, conn, transaction))
                        {
                            // Calculer les totaux
                            decimal sousTotal = 0;
                            foreach (var ligne in lignes)
                            {
                                sousTotal += ligne.Quantite * ligne.PrixUnitaire;
                            }
                            
                            var (tps, tvq, total) = TaxesService.CalculerTaxes(sousTotal);
                            
                            cmd.Parameters.AddWithValue("@numero", facture.NumeroFacture);
                            cmd.Parameters.AddWithValue("@dateFacture", facture.DateFacture);
                            cmd.Parameters.AddWithValue("@dateEcheance", facture.DateEcheance);
                            cmd.Parameters.AddWithValue("@clientId", facture.ClientId);
                            cmd.Parameters.AddWithValue("@employeId", facture.EmployeId.HasValue ? facture.EmployeId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@sousTotal", sousTotal);
                            cmd.Parameters.AddWithValue("@tps", tps);
                            cmd.Parameters.AddWithValue("@tvq", tvq);
                            cmd.Parameters.AddWithValue("@total", total);
                            cmd.Parameters.AddWithValue("@du", total);
                            cmd.Parameters.AddWithValue("@statutPaiement", "Impayée");
                            cmd.Parameters.AddWithValue("@statut", "Active");
                            cmd.Parameters.AddWithValue("@note", facture.NoteInterne ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@conditions", facture.ConditionsPaiement);
                            
                            factureId = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        
                        // 2. Insérer les lignes de facture
                        foreach (var ligne in lignes)
                        {
                            string insertLigne = @"
                                INSERT INTO lignes_factures 
                                (facture_id, produit_id, sku, description, quantite, prix_unitaire, montant_ligne)
                                VALUES 
                                (@factureId, @produitId, @sku, @description, @quantite, @prix, @montant)";
                            
                            using (var cmd = new MySqlCommand(insertLigne, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@factureId", factureId);
                                cmd.Parameters.AddWithValue("@produitId", ligne.ProduitId);
                                cmd.Parameters.AddWithValue("@sku", ligne.SKU);
                                cmd.Parameters.AddWithValue("@description", ligne.Description);
                                cmd.Parameters.AddWithValue("@quantite", ligne.Quantite);
                                cmd.Parameters.AddWithValue("@prix", ligne.PrixUnitaire);
                                cmd.Parameters.AddWithValue("@montant", ligne.Quantite * ligne.PrixUnitaire);
                                
                                cmd.ExecuteNonQuery();
                            }
                            
                            // 3. Mettre à jour le stock (sortie)
                            ReduireStock(ligne.ProduitId, ligne.Quantite, conn, transaction);
                            
                            // 4. Créer un mouvement de stock
                            CreerMouvementStock(ligne.ProduitId, -ligne.Quantite, "vente", 
                                $"Vente facture {facture.NumeroFacture}", facture.EmployeId, conn, transaction);
                        }
                        
                        transaction.Commit();
                        return factureId;
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
        /// Vérifier si un client a des factures en retard
        /// </summary>
        private static bool ClientAFacturesEnRetard(int clientId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM factures 
                        WHERE client_id = @clientId 
                        AND statut = 'Active'
                        AND statut_paiement IN ('Impayée', 'Partielle')
                        AND date_echeance < CURDATE()";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false; // En cas d'erreur, on autorise quand même
            }
        }

        /// <summary>
        /// Vérifier si le stock est suffisant
        /// </summary>
        private static bool VerifierStockDisponible(int produitId, int quantiteVoulue)
        {
            int stockDisponible = GetStockDisponible(produitId);
            return stockDisponible >= quantiteVoulue;
        }

        /// <summary>
        /// Obtenir le stock disponible d'un produit
        /// </summary>
        private static int GetStockDisponible(int produitId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT qte_disponible FROM niveaux_stock WHERE produit_id = @produitId";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@produitId", produitId);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Réduire le stock lors d'une vente
        /// </summary>
        private static void ReduireStock(int produitId, int quantite, MySqlConnection conn, MySqlTransaction transaction)
        {
            // Vérifier d'abord si une ligne existe pour ce produit
            string checkQuery = "SELECT COUNT(*) FROM niveaux_stock WHERE produit_id = @produitId";
            int count = 0;
            
            using (var checkCmd = new MySqlCommand(checkQuery, conn, transaction))
            {
                checkCmd.Parameters.AddWithValue("@produitId", produitId);
                var result = checkCmd.ExecuteScalar();
                count = result != null ? Convert.ToInt32(result) : 0;
            }
            
            if (count > 0)
            {
                // Mettre à jour le stock existant
                string updateQuery = @"
                    UPDATE niveaux_stock 
                    SET qte_disponible = qte_disponible - @quantite 
                    WHERE produit_id = @produitId";
                
                using (var cmd = new MySqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@quantite", quantite);
                    cmd.Parameters.AddWithValue("@produitId", produitId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    
                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Impossible de mettre à jour le stock pour le produit ID {produitId}");
                    }
                }
            }
            else
            {
                // Si aucune ligne n'existe, cela ne devrait pas arriver car le stock a été vérifié avant
                // Mais on lance une exception pour signaler le problème
                throw new Exception($"Aucune ligne de stock trouvée pour le produit ID {produitId}");
            }
        }

        /// <summary>
        /// Créer un mouvement de stock
        /// </summary>
        private static void CreerMouvementStock(int produitId, int quantite, string raison, string note, int? employeId, MySqlConnection conn, MySqlTransaction transaction)
        {
            string query = @"
                INSERT INTO mouvements_stock 
                (produit_id, type_mouvement, quantite, raison, note_utilisateur, employe_id, date_mouvement)
                VALUES 
                (@produitId, @type, @quantite, @raison, @note, @employeId, NOW())";
            
            using (var cmd = new MySqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@produitId", produitId);
                cmd.Parameters.AddWithValue("@type", quantite > 0 ? "ENTREE" : "SORTIE");
                cmd.Parameters.AddWithValue("@quantite", Math.Abs(quantite));
                cmd.Parameters.AddWithValue("@raison", raison);
                cmd.Parameters.AddWithValue("@note", note);
                cmd.Parameters.AddWithValue("@employeId", employeId.HasValue ? employeId.Value : DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtenir toutes les factures
        /// </summary>
        public static List<Facture> GetAllFactures()
        {
            var factures = new List<Facture>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            f.*,
                            c.nom AS nom_client,
                            c.courriel_contact AS courriel_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_vendeur
                        FROM factures f
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON f.employe_id = e.id
                        ORDER BY f.date_facture DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            factures.Add(MapFactureFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des factures: {ex.Message}", ex);
            }

            return factures;
        }

        /// <summary>
        /// Obtenir une facture par ID avec ses lignes
        /// </summary>
        public static Facture? GetFactureById(int id)
        {
            Facture? facture = null;

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    
                    // Récupérer la facture
                    string queryFacture = @"
                        SELECT 
                            f.*,
                            c.nom AS nom_client,
                            c.courriel_contact AS courriel_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_vendeur
                        FROM factures f
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON f.employe_id = e.id
                        WHERE f.id = @id";
                    
                    using (var cmd = new MySqlCommand(queryFacture, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                facture = MapFactureFromReader(reader);
                            }
                        }
                    }
                    
                    if (facture != null)
                    {
                        // Récupérer les lignes
                        string queryLignes = @"
                            SELECT 
                                lf.*,
                                p.nom AS nom_produit
                            FROM lignes_factures lf
                            INNER JOIN produits p ON lf.produit_id = p.id
                            WHERE lf.facture_id = @factureId";
                        
                        using (var cmd = new MySqlCommand(queryLignes, conn))
                        {
                            cmd.Parameters.AddWithValue("@factureId", id);
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    facture.Lignes.Add(new LigneFacture
                                    {
                                        Id = reader.GetInt32("id"),
                                        FactureId = reader.GetInt32("facture_id"),
                                        ProduitId = reader.GetInt32("produit_id"),
                                        SKU = reader.GetString("sku"),
                                        Description = reader.GetString("description"),
                                        Quantite = reader.GetInt32("quantite"),
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
                throw new Exception($"Erreur lors de la récupération de la facture: {ex.Message}", ex);
            }

            return facture;
        }

        /// <summary>
        /// Annuler une facture (ne peut pas être supprimée)
        /// </summary>
        public static bool AnnulerFacture(int id, string motif)
        {
            try
            {
                // Vérifier que la facture est impayée
                var facture = GetFactureById(id);
                if (facture == null)
                    throw new Exception("Facture introuvable");
                
                if (facture.StatutPaiement != "Impayée")
                    throw new Exception("Seules les factures impayées peuvent être annulées");

                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Annuler la facture
                            string query = "UPDATE factures SET statut = 'Annulée', note_interne = CONCAT(IFNULL(note_interne, ''), '\n[Annulation] ', @motif) WHERE id = @id";
                            using (var cmd = new MySqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@motif", motif);
                                cmd.Parameters.AddWithValue("@id", id);
                                cmd.ExecuteNonQuery();
                            }
                            
                            // 2. Remettre les produits en stock
                            foreach (var ligne in facture.Lignes)
                            {
                                string updateStock = "UPDATE niveaux_stock SET qte_disponible = qte_disponible + @quantite WHERE produit_id = @produitId";
                                using (var cmd = new MySqlCommand(updateStock, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@quantite", ligne.Quantite);
                                    cmd.Parameters.AddWithValue("@produitId", ligne.ProduitId);
                                    cmd.ExecuteNonQuery();
                                }
                                
                                // Créer un mouvement de stock pour la remise en stock
                                CreerMouvementStock(ligne.ProduitId, ligne.Quantite, "annulation_vente", 
                                    $"Annulation facture {facture.NumeroFacture}: {motif}", null, conn, transaction);
                            }
                            
                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'annulation de la facture: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mapper une facture depuis un DataReader
        /// </summary>
        private static Facture MapFactureFromReader(MySqlDataReader reader)
        {
            return new Facture
            {
                Id = reader.GetInt32("id"),
                NumeroFacture = reader.GetString("numero_facture"),
                DateFacture = reader.GetDateTime("date_facture"),
                DateEcheance = reader.GetDateTime("date_echeance"),
                ClientId = reader.GetInt32("client_id"),
                EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) ? null : reader.GetInt32("employe_id"),
                SousTotal = reader.GetDecimal("sous_total"),
                MontantTPS = reader.GetDecimal("montant_tps"),
                MontantTVQ = reader.GetDecimal("montant_tvq"),
                MontantTotal = reader.GetDecimal("montant_total"),
                MontantPaye = reader.GetDecimal("montant_paye"),
                MontantDu = reader.GetDecimal("montant_du"),
                StatutPaiement = reader.GetString("statut_paiement"),
                Statut = reader.GetString("statut"),
                NoteInterne = reader.IsDBNull(reader.GetOrdinal("note_interne")) ? null : reader.GetString("note_interne"),
                ConditionsPaiement = reader.IsDBNull(reader.GetOrdinal("conditions_paiement")) ? null : reader.GetString("conditions_paiement"),
                NomClient = reader.GetString("nom_client"),
                CourrielClient = reader.GetString("courriel_client"),
                NomVendeur = reader.IsDBNull(reader.GetOrdinal("nom_vendeur")) ? "N/A" : reader.GetString("nom_vendeur")
            };
        }

        /// <summary>
        /// Obtenir toutes les factures d'un client
        /// </summary>
        public static List<Facture> GetFacturesByClient(int clientId)
        {
            var factures = new List<Facture>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            f.*,
                            c.nom AS nom_client,
                            c.courriel_contact AS courriel_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_vendeur
                        FROM factures f
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON f.employe_id = e.id
                        WHERE f.client_id = @clientId
                        ORDER BY f.date_facture DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                factures.Add(MapFactureFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des factures du client: {ex.Message}", ex);
            }

            return factures;
        }
    }
}

