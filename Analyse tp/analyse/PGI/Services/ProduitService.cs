using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class ProduitService
    {
        /// <summary>
        /// Obtenir tous les produits avec leurs informations liées
        /// </summary>
        public static List<Produit> GetAllProduits()
        {
            var produits = new List<Produit>();
            
            string query = @"
                SELECT 
                    p.*,
                    c.nom AS nom_categorie,
                    f.nom AS nom_fournisseur,
                    COALESCE(SUM(ns.qte_disponible), 0) AS stock_disponible,
                    COALESCE(SUM(ns.qte_reservee), 0) AS stock_reservee
                FROM produits p
                LEFT JOIN categories c ON p.categorie_id = c.id
                LEFT JOIN fournisseurs f ON p.fournisseur_id = f.id
                LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
                GROUP BY p.id
                ORDER BY p.nom";

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    produits.Add(MapDataRowToProduit(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des produits: {ex.Message}", ex);
            }

            return produits;
        }

        /// <summary>
        /// Obtenir un produit par son ID
        /// </summary>
        public static Produit GetProduitById(int id)
        {
            string query = @"
                SELECT 
                    p.*, 
                    c.nom AS nom_categorie,
                    f.nom AS nom_fournisseur,
                    COALESCE(SUM(ns.qte_disponible), 0) AS stock_disponible,
                    COALESCE(SUM(ns.qte_reservee), 0) AS stock_reservee
                FROM produits p
                LEFT JOIN categories c ON p.categorie_id = c.id
                LEFT JOIN fournisseurs f ON p.fournisseur_id = f.id
                LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
                WHERE p.id = @id
                GROUP BY p.id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    return MapDataRowToProduit(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération du produit: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Ajouter un nouveau produit
        /// </summary>
        public static int AddProduit(Produit produit)
        {
            string query = @"
                INSERT INTO produits (
                    sku, categorie_id, nom, description, cout, prix, marge_brute,
                    seuil_reapprovisionnement, stock_minimum, poids_kg, 
                    fournisseur_id, statut, date_entree_stock
                ) VALUES (
                    @sku, @categorie_id, @nom, @description, @cout, @prix, @marge_brute,
                    @seuil_reapprovisionnement, @stock_minimum, @poids_kg,
                    @fournisseur_id, @statut, @date_entree_stock
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@sku", produit.SKU },
                { "@categorie_id", produit.CategorieId },
                { "@nom", produit.Nom },
                { "@description", produit.Description },
                { "@cout", produit.Cout },
                { "@prix", produit.Prix },
                { "@marge_brute", produit.MargeBrute },
                { "@seuil_reapprovisionnement", produit.SeuilReapprovisionnement },
                { "@stock_minimum", produit.StockMinimum },
                { "@poids_kg", produit.PoidsKg },
                { "@fournisseur_id", produit.FournisseurId },
                { "@statut", produit.Statut },
                { "@date_entree_stock", produit.DateEntreeStock }
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return (int)DatabaseHelper.GetLastInsertId();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout du produit: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour un produit
        /// </summary>
        public static bool UpdateProduit(Produit produit)
        {
            string query = @"
                UPDATE produits SET
                    sku = @sku,
                    categorie_id = @categorie_id,
                    nom = @nom,
                    description = @description,
                    cout = @cout,
                    prix = @prix,
                    marge_brute = @marge_brute,
                    seuil_reapprovisionnement = @seuil_reapprovisionnement,
                    stock_minimum = @stock_minimum,
                    poids_kg = @poids_kg,
                    fournisseur_id = @fournisseur_id,
                    statut = @statut,
                    date_entree_stock = @date_entree_stock
                WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", produit.Id },
                { "@sku", produit.SKU },
                { "@categorie_id", produit.CategorieId },
                { "@nom", produit.Nom },
                { "@description", produit.Description },
                { "@cout", produit.Cout },
                { "@prix", produit.Prix },
                { "@marge_brute", produit.MargeBrute },
                { "@seuil_reapprovisionnement", produit.SeuilReapprovisionnement },
                { "@stock_minimum", produit.StockMinimum },
                { "@poids_kg", produit.PoidsKg },
                { "@fournisseur_id", produit.FournisseurId },
                { "@statut", produit.Statut },
                { "@date_entree_stock", produit.DateEntreeStock }
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du produit: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprimer un produit
        /// </summary>
        public static bool DeleteProduit(int id)
        {
            string query = "DELETE FROM produits WHERE id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression du produit: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Rechercher des produits
        /// </summary>
        public static List<Produit> SearchProduits(string searchTerm)
        {
            var produits = new List<Produit>();
            
            string query = @"
                SELECT 
                    p.*, 
                    c.nom AS nom_categorie,
                    f.nom AS nom_fournisseur,
                    COALESCE(SUM(ns.qte_disponible), 0) AS stock_disponible,
                    COALESCE(SUM(ns.qte_reservee), 0) AS stock_reservee
                FROM produits p
                LEFT JOIN categories c ON p.categorie_id = c.id
                LEFT JOIN fournisseurs f ON p.fournisseur_id = f.id
                LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
                WHERE p.nom LIKE @searchTerm OR p.sku LIKE @searchTerm OR p.description LIKE @searchTerm
                GROUP BY p.id
                ORDER BY p.nom";

            var parameters = new Dictionary<string, object>
            {
                { "@searchTerm", $"%{searchTerm}%" }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                foreach (DataRow row in dt.Rows)
                {
                    produits.Add(MapDataRowToProduit(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la recherche de produits: {ex.Message}", ex);
            }

            return produits;
        }

        /// <summary>
        /// Compter le nombre de produits associés à un fournisseur
        /// </summary>
        public static int CountProduitsByFournisseur(int fournisseurId)
        {
            string query = "SELECT COUNT(*) FROM produits WHERE fournisseur_id = @fournisseur_id";
            var parameters = new Dictionary<string, object>
            {
                { "@fournisseur_id", fournisseurId }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du comptage des produits: {ex.Message}", ex);
            }

            return 0;
        }

        /// <summary>
        /// Mapper une ligne DataRow vers un objet Produit
        /// </summary>
        private static Produit MapDataRowToProduit(DataRow row)
        {
            try
            {
                return new Produit
                {
                    Id = Convert.ToInt32(row["id"]),
                    SKU = row["sku"]?.ToString() ?? string.Empty,
                    CategorieId = row.Table.Columns.Contains("categorie_id") ? Convert.ToInt32(row["categorie_id"]) : 0,
                    Nom = row["nom"]?.ToString() ?? string.Empty,
                    Description = row["description"] != DBNull.Value ? row["description"].ToString() : string.Empty,
                    Cout = Convert.ToDecimal(row["cout"]),
                    Prix = Convert.ToDecimal(row["prix"]),
                    MargeBrute = Convert.ToDecimal(row["marge_brute"]),
                    SeuilReapprovisionnement = Convert.ToInt32(row["seuil_reapprovisionnement"]),
                    StockMinimum = Convert.ToInt32(row["stock_minimum"]),
                    PoidsKg = Convert.ToDecimal(row["poids_kg"]),
                    FournisseurId = row.Table.Columns.Contains("fournisseur_id") ? Convert.ToInt32(row["fournisseur_id"]) : 0,
                    Statut = row["statut"]?.ToString() ?? "Actif",
                    DateEntreeStock = row["date_entree_stock"] != DBNull.Value ? Convert.ToDateTime(row["date_entree_stock"]) : (DateTime?)null,
                    DateCreation = Convert.ToDateTime(row["date_creation"]),
                    NomCategorie = row.Table.Columns.Contains("nom_categorie") && row["nom_categorie"] != DBNull.Value ? row["nom_categorie"].ToString() : string.Empty,
                    NomFournisseur = row.Table.Columns.Contains("nom_fournisseur") && row["nom_fournisseur"] != DBNull.Value ? row["nom_fournisseur"].ToString() : string.Empty,
                    StockDisponible = row.Table.Columns.Contains("stock_disponible") ? Convert.ToInt32(row["stock_disponible"]) : 0,
                    StockReservee = row.Table.Columns.Contains("stock_reservee") ? Convert.ToInt32(row["stock_reservee"]) : 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du mapping du produit (ligne {row["id"]}): {ex.Message}", ex);
            }
        }
    }
}

