using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class FournisseurService
    {
        /// <summary>
        /// Obtenir tous les fournisseurs
        /// </summary>
        public static List<Fournisseur> GetAllFournisseurs()
        {
            var fournisseurs = new List<Fournisseur>();
            
            string query = @"
                SELECT id, nom, code, courriel_contact, delai_livraison_jours,
                       pourcentage_escompte, statut, date_creation
                FROM fournisseurs
                ORDER BY nom";

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    fournisseurs.Add(MapDataRowToFournisseur(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des fournisseurs: {ex.Message}", ex);
            }

            return fournisseurs;
        }

        /// <summary>
        /// Obtenir un fournisseur par son ID
        /// </summary>
        public static Fournisseur GetFournisseurById(int id)
        {
            string query = "SELECT * FROM fournisseurs WHERE id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", id }
            };

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    return MapDataRowToFournisseur(dt.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération du fournisseur: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Ajouter un nouveau fournisseur
        /// </summary>
        public static int AddFournisseur(Fournisseur fournisseur)
        {
            string query = @"
                INSERT INTO fournisseurs (
                    nom, code, courriel_contact, delai_livraison_jours,
                    pourcentage_escompte, statut
                ) VALUES (
                    @nom, @code, @courriel_contact, @delai_livraison_jours,
                    @pourcentage_escompte, @statut
                )";

            var parameters = new Dictionary<string, object>
            {
                { "@nom", fournisseur.Nom },
                { "@code", fournisseur.Code },
                { "@courriel_contact", fournisseur.CourrielContact },
                { "@delai_livraison_jours", fournisseur.DelaiLivraisonJours },
                { "@pourcentage_escompte", fournisseur.PourcentageEscompte },
                { "@statut", fournisseur.Statut }
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                return (int)DatabaseHelper.GetLastInsertId();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout du fournisseur: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour un fournisseur
        /// </summary>
        public static bool UpdateFournisseur(Fournisseur fournisseur)
        {
            string query = @"
                UPDATE fournisseurs SET
                    nom = @nom,
                    code = @code,
                    courriel_contact = @courriel_contact,
                    delai_livraison_jours = @delai_livraison_jours,
                    pourcentage_escompte = @pourcentage_escompte,
                    statut = @statut
                WHERE id = @id";

            var parameters = new Dictionary<string, object>
            {
                { "@id", fournisseur.Id },
                { "@nom", fournisseur.Nom },
                { "@code", fournisseur.Code },
                { "@courriel_contact", fournisseur.CourrielContact },
                { "@delai_livraison_jours", fournisseur.DelaiLivraisonJours },
                { "@pourcentage_escompte", fournisseur.PourcentageEscompte },
                { "@statut", fournisseur.Statut }
            };

            try
            {
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du fournisseur: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprimer un fournisseur
        /// </summary>
        public static bool DeleteFournisseur(int id)
        {
            // Vérifier d'abord s'il y a des produits associés
            try
            {
                int produitCount = ProduitService.CountProduitsByFournisseur(id);
                if (produitCount > 0)
                {
                    throw new Exception($"Impossible de supprimer ce fournisseur car {produitCount} produit(s) y sont associé(s). Veuillez d'abord supprimer ou réassigner les produits.");
                }
            }
            catch (Exception ex)
            {
                // Si l'erreur est déjà notre message personnalisé, la relancer
                if (ex.Message.Contains("produit(s)"))
                {
                    throw;
                }
                // Sinon, continuer avec la suppression (au cas où la vérification échoue)
            }

            string query = "DELETE FROM fournisseurs WHERE id = @id";
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
                throw new Exception($"Erreur lors de la suppression du fournisseur: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir les fournisseurs actifs uniquement
        /// </summary>
        public static List<Fournisseur> GetActiveFournisseurs()
        {
            var fournisseurs = new List<Fournisseur>();
            
            string query = @"
                SELECT id, nom, code, courriel_contact, delai_livraison_jours,
                       pourcentage_escompte, statut, date_creation
                FROM fournisseurs
                WHERE statut = 'Actif'
                ORDER BY nom";

            try
            {
                var dt = DatabaseHelper.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    fournisseurs.Add(MapDataRowToFournisseur(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des fournisseurs actifs: {ex.Message}", ex);
            }

            return fournisseurs;
        }

        /// <summary>
        /// Mapper une ligne DataRow vers un objet Fournisseur
        /// </summary>
        private static Fournisseur MapDataRowToFournisseur(DataRow row)
        {
            return new Fournisseur
            {
                Id = Convert.ToInt32(row["id"]),
                Nom = row["nom"].ToString(),
                Code = row["code"].ToString(),
                CourrielContact = row["courriel_contact"] != DBNull.Value ? row["courriel_contact"].ToString() : string.Empty,
                DelaiLivraisonJours = Convert.ToInt32(row["delai_livraison_jours"]),
                PourcentageEscompte = Convert.ToDecimal(row["pourcentage_escompte"]),
                Statut = row["statut"].ToString(),
                DateCreation = Convert.ToDateTime(row["date_creation"])
            };
        }
    }
}

