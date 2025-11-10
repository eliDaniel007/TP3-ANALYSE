using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class CommandeService
    {
        /// <summary>
        /// Créer une nouvelle commande
        /// </summary>
        public static int CreateCommande(Commande commande)
        {
            try
            {
                // Générer un numéro de commande unique
                string numeroCommande = GenerateNumeroCommande();
                
                string query = @"
                    INSERT INTO commandes_vente (
                        client_id, numero_commande, date_commande, statut, 
                        montant_total, adresse_livraison, notes
                    ) VALUES (
                        @client_id, @numero_commande, @date_commande, @statut,
                        @montant_total, @adresse_livraison, @notes
                    )";

                var parameters = new Dictionary<string, object>
                {
                    { "@client_id", commande.ClientId },
                    { "@numero_commande", numeroCommande },
                    { "@date_commande", commande.DateCommande },
                    { "@statut", commande.Statut },
                    { "@montant_total", commande.MontantTotal },
                    { "@adresse_livraison", commande.AdresseLivraison ?? (object)DBNull.Value },
                    { "@notes", commande.Notes ?? (object)DBNull.Value }
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                int commandeId = (int)DatabaseHelper.GetLastInsertId();

                // Insérer les lignes de commande
                foreach (var ligne in commande.Lignes)
                {
                    CreateLigneCommande(commandeId, ligne);
                }

                return commandeId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de la commande : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Créer une ligne de commande
        /// </summary>
        private static void CreateLigneCommande(int commandeId, LigneCommande ligne)
        {
            string query = @"
                INSERT INTO lignes_commande_vente (
                    commande_id, produit_id, quantite, prix_unitaire, sous_total
                ) VALUES (
                    @commande_id, @produit_id, @quantite, @prix_unitaire, @sous_total
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@commande_id", commandeId },
                { "@produit_id", ligne.ProduitId },
                { "@quantite", ligne.Quantite },
                { "@prix_unitaire", ligne.PrixUnitaire },
                { "@sous_total", ligne.SousTotal }
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Obtenir toutes les commandes d'un client
        /// </summary>
        public static List<Commande> GetCommandesByClient(int clientId)
        {
            var commandes = new List<Commande>();

            string query = @"
                SELECT 
                    cv.*,
                    c.nom AS nom_client
                FROM commandes_vente cv
                INNER JOIN clients c ON cv.client_id = c.id
                WHERE cv.client_id = @client_id
                ORDER BY cv.date_commande DESC";

            var parameters = new Dictionary<string, object>
            {
                { "@client_id", clientId }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                foreach (DataRow row in dt.Rows)
                {
                    var commande = MapDataRowToCommande(row);
                    commande.Lignes = GetLignesCommande(commande.Id);
                    commandes.Add(commande);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des commandes : {ex.Message}", ex);
            }

            return commandes;
        }

        /// <summary>
        /// Obtenir une commande par son ID
        /// </summary>
        public static Commande GetCommandeById(int commandeId)
        {
            string query = @"
                SELECT 
                    cv.*,
                    c.nom AS nom_client
                FROM commandes_vente cv
                INNER JOIN clients c ON cv.client_id = c.id
                WHERE cv.id = @commande_id";

            var parameters = new Dictionary<string, object>
            {
                { "@commande_id", commandeId }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    var commande = MapDataRowToCommande(dt.Rows[0]);
                    commande.Lignes = GetLignesCommande(commande.Id);
                    return commande;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la commande : {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Obtenir les lignes d'une commande
        /// </summary>
        private static List<LigneCommande> GetLignesCommande(int commandeId)
        {
            var lignes = new List<LigneCommande>();

            string query = @"
                SELECT 
                    lcv.*,
                    p.nom AS nom_produit,
                    p.sku AS sku
                FROM lignes_commande_vente lcv
                INNER JOIN produits p ON lcv.produit_id = p.id
                WHERE lcv.commande_id = @commande_id";

            var parameters = new Dictionary<string, object>
            {
                { "@commande_id", commandeId }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                foreach (DataRow row in dt.Rows)
                {
                    lignes.Add(MapDataRowToLigneCommande(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des lignes de commande : {ex.Message}", ex);
            }

            return lignes;
        }

        /// <summary>
        /// Mettre à jour le statut d'une commande
        /// </summary>
        public static bool UpdateStatutCommande(int commandeId, string nouveauStatut)
        {
            string query = @"
                UPDATE commandes_vente 
                SET statut = @statut 
                WHERE id = @commande_id";

            var parameters = new Dictionary<string, object>
            {
                { "@commande_id", commandeId },
                { "@statut", nouveauStatut }
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du statut : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Générer un numéro de commande unique
        /// </summary>
        private static string GenerateNumeroCommande()
        {
            string prefix = "CMD";
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("00");
            
            // Obtenir le dernier numéro de commande du mois
            string query = @"
                SELECT COUNT(*) 
                FROM commandes_vente 
                WHERE YEAR(date_commande) = @year 
                AND MONTH(date_commande) = @month";

            var parameters = new Dictionary<string, object>
            {
                { "@year", year },
                { "@month", month }
            };

            try
            {
                var count = DatabaseHelper.ExecuteScalar(query, parameters);
                int numero = Convert.ToInt32(count) + 1;
                return $"{prefix}-{year}{month}-{numero:0000}";
            }
            catch
            {
                return $"{prefix}-{year}{month}-0001";
            }
        }

        /// <summary>
        /// Mapper une ligne DataRow vers un objet Commande
        /// </summary>
        private static Commande MapDataRowToCommande(DataRow row)
        {
            return new Commande
            {
                Id = Convert.ToInt32(row["id"]),
                ClientId = Convert.ToInt32(row["client_id"]),
                NumeroCommande = row["numero_commande"]?.ToString() ?? string.Empty,
                DateCommande = Convert.ToDateTime(row["date_commande"]),
                Statut = row["statut"]?.ToString() ?? "Brouillon",
                MontantTotal = Convert.ToDecimal(row["montant_total"]),
                AdresseLivraison = row["adresse_livraison"] != DBNull.Value ? row["adresse_livraison"].ToString() : null,
                Notes = row["notes"] != DBNull.Value ? row["notes"].ToString() : null,
                NomClient = row.Table.Columns.Contains("nom_client") && row["nom_client"] != DBNull.Value 
                    ? row["nom_client"].ToString() 
                    : string.Empty
            };
        }

        /// <summary>
        /// Mapper une ligne DataRow vers un objet LigneCommande
        /// </summary>
        private static LigneCommande MapDataRowToLigneCommande(DataRow row)
        {
            return new LigneCommande
            {
                Id = Convert.ToInt32(row["id"]),
                CommandeId = Convert.ToInt32(row["commande_id"]),
                ProduitId = Convert.ToInt32(row["produit_id"]),
                Quantite = Convert.ToInt32(row["quantite"]),
                PrixUnitaire = Convert.ToDecimal(row["prix_unitaire"]),
                SousTotal = Convert.ToDecimal(row["sous_total"]),
                NomProduit = row.Table.Columns.Contains("nom_produit") && row["nom_produit"] != DBNull.Value 
                    ? row["nom_produit"].ToString() 
                    : string.Empty,
                SKU = row.Table.Columns.Contains("sku") && row["sku"] != DBNull.Value 
                    ? row["sku"].ToString() 
                    : string.Empty
            };
        }
    }
}


