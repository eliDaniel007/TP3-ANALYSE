using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class MouvementStockService
    {
        /// <summary>
        /// Obtenir tous les mouvements de stock pour un produit
        /// </summary>
        public static List<MouvementStock> GetMouvementsByProduitId(int produitId)
        {
            var mouvements = new List<MouvementStock>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ms.id,
                            ms.produit_id,
                            ms.type_mouvement,
                            ms.quantite,
                            ms.raison,
                            ms.date_mouvement,
                            ms.note_utilisateur,
                            ms.employe_id,
                            p.nom AS produit_nom,
                            p.sku AS produit_sku,
                            CONCAT(e.prenom, ' ', e.nom) AS employe_nom
                        FROM mouvements_stock ms
                        LEFT JOIN produits p ON ms.produit_id = p.id
                        LEFT JOIN employes e ON ms.employe_id = e.id
                        WHERE ms.produit_id = @produitId
                        ORDER BY ms.date_mouvement DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@produitId", produitId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mouvements.Add(new MouvementStock
                                {
                                    Id = reader.GetInt32("id"),
                                    ProduitId = reader.GetInt32("produit_id"),
                                    TypeMouvement = reader.GetString("type_mouvement"),
                                    Quantite = reader.GetInt32("quantite"),
                                    Raison = reader.GetString("raison"),
                                    DateMouvement = reader.GetDateTime("date_mouvement"),
                                    NoteUtilisateur = reader.IsDBNull(reader.GetOrdinal("note_utilisateur")) 
                                        ? null : reader.GetString("note_utilisateur"),
                                    EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) 
                                        ? null : reader.GetInt32("employe_id"),
                                    NomProduit = reader.IsDBNull(reader.GetOrdinal("produit_nom")) 
                                        ? null : reader.GetString("produit_nom"),
                                    SKUProduit = reader.IsDBNull(reader.GetOrdinal("produit_sku")) 
                                        ? null : reader.GetString("produit_sku"),
                                    NomEmploye = reader.IsDBNull(reader.GetOrdinal("employe_nom")) 
                                        ? "Système" : reader.GetString("employe_nom")
                                });
                            }
                        }
                    }
                }

                return mouvements;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des mouvements de stock: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir tous les mouvements de stock (tous produits)
        /// </summary>
        public static List<MouvementStock> GetAllMouvements()
        {
            var mouvements = new List<MouvementStock>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ms.id,
                            ms.produit_id,
                            ms.type_mouvement,
                            ms.quantite,
                            ms.raison,
                            ms.date_mouvement,
                            ms.note_utilisateur,
                            ms.employe_id,
                            p.nom AS produit_nom,
                            p.sku AS produit_sku,
                            CONCAT(e.prenom, ' ', e.nom) AS employe_nom
                        FROM mouvements_stock ms
                        LEFT JOIN produits p ON ms.produit_id = p.id
                        LEFT JOIN employes e ON ms.employe_id = e.id
                        ORDER BY ms.date_mouvement DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            mouvements.Add(new MouvementStock
                            {
                                Id = reader.GetInt32("id"),
                                ProduitId = reader.GetInt32("produit_id"),
                                TypeMouvement = reader.GetString("type_mouvement"),
                                Quantite = reader.GetInt32("quantite"),
                                Raison = reader.GetString("raison"),
                                DateMouvement = reader.GetDateTime("date_mouvement"),
                                NoteUtilisateur = reader.IsDBNull(reader.GetOrdinal("note_utilisateur")) 
                                    ? null : reader.GetString("note_utilisateur"),
                                EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) 
                                    ? null : reader.GetInt32("employe_id"),
                                NomProduit = reader.IsDBNull(reader.GetOrdinal("produit_nom")) 
                                    ? null : reader.GetString("produit_nom"),
                                SKUProduit = reader.IsDBNull(reader.GetOrdinal("produit_sku")) 
                                    ? null : reader.GetString("produit_sku"),
                                NomEmploye = reader.IsDBNull(reader.GetOrdinal("employe_nom")) 
                                    ? "Système" : reader.GetString("employe_nom")
                            });
                        }
                    }
                }

                return mouvements;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des mouvements de stock: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ajouter un mouvement de stock
        /// </summary>
        public static int AddMouvementStock(MouvementStock mouvement)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                
                // S'assurer que la date est définie
                if (mouvement.DateMouvement == default)
                {
                    mouvement.DateMouvement = DateTime.Now;
                }

                    string query = @"
                        INSERT INTO mouvements_stock 
                        (produit_id, type_mouvement, quantite, raison, date_mouvement, note_utilisateur, employe_id)
                        VALUES 
                        (@produitId, @typeMouvement, @quantite, @raison, @dateMouvement, @noteUtilisateur, @employeId);
                        SELECT LAST_INSERT_ID();";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@produitId", mouvement.ProduitId);
                        cmd.Parameters.AddWithValue("@typeMouvement", mouvement.TypeMouvement);
                        cmd.Parameters.AddWithValue("@quantite", mouvement.Quantite);
                        cmd.Parameters.AddWithValue("@raison", mouvement.Raison);
                        cmd.Parameters.AddWithValue("@dateMouvement", mouvement.DateMouvement);
                        cmd.Parameters.AddWithValue("@noteUtilisateur", 
                            string.IsNullOrEmpty(mouvement.NoteUtilisateur) ? DBNull.Value : mouvement.NoteUtilisateur);
                        cmd.Parameters.AddWithValue("@employeId", 
                            mouvement.EmployeId.HasValue ? mouvement.EmployeId.Value : DBNull.Value);

                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout du mouvement de stock: {ex.Message}", ex);
            }
        }
    }
}
