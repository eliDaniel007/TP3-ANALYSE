using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class CategorieService
    {
        /// <summary>
        /// Obtenir toutes les catégories
        /// </summary>
        public static List<Categorie> GetAllCategories()
        {
            var categories = new List<Categorie>();
            
            string query = @"
                SELECT id, nom, statut, date_creation
                FROM categories
                ORDER BY nom";

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(MapDataRowToCategorie(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des catégories: {ex.Message}", ex);
            }

            return categories;
        }

        /// <summary>
        /// Obtenir une catégorie par son ID
        /// </summary>
        public static Categorie GetCategorieById(int id)
        {
            string query = "SELECT * FROM categories WHERE id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    return MapDataRowToCategorie(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la catégorie: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Ajouter une nouvelle catégorie
        /// </summary>
        public static int AddCategorie(Categorie categorie)
        {
            string query = @"
                INSERT INTO categories (nom, statut)
                VALUES (@nom, @statut)";

            var parameters = new Dictionary<string, object>
            {
                { "@nom", categorie.Nom },
                { "@statut", categorie.Statut }
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return (int)DatabaseHelper.GetLastInsertId();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de la catégorie: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour une catégorie
        /// </summary>
        public static bool UpdateCategorie(Categorie categorie)
        {
            string query = @"
                UPDATE categories SET
                    nom = @nom,
                    statut = @statut
                WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", categorie.Id },
                { "@nom", categorie.Nom },
                { "@statut", categorie.Statut }
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la catégorie: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprimer une catégorie
        /// </summary>
        public static bool DeleteCategorie(int id)
        {
            string query = "DELETE FROM categories WHERE id = @id";
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
                throw new Exception($"Erreur lors de la suppression de la catégorie: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir les catégories actives uniquement
        /// </summary>
        public static List<Categorie> GetActiveCategories()
        {
            var categories = new List<Categorie>();
            
            string query = @"
                SELECT id, nom, statut, date_creation
                FROM categories
                WHERE statut = 'Actif'
                ORDER BY nom";

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    categories.Add(MapDataRowToCategorie(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des catégories actives: {ex.Message}", ex);
            }

            return categories;
        }

        /// <summary>
        /// Mapper une ligne DataRow vers un objet Categorie
        /// </summary>
        private static Categorie MapDataRowToCategorie(DataRow row)
        {
            return new Categorie
            {
                Id = Convert.ToInt32(row["id"]),
                Nom = row["nom"].ToString(),
                Statut = row["statut"].ToString(),
                DateCreation = Convert.ToDateTime(row["date_creation"])
            };
        }
    }
}

