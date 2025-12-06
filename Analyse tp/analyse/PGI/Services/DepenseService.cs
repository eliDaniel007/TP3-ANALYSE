using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;

namespace PGI.Services
{
    public class DepenseService
    {
        public static List<Depense> GetAllDepenses()
        {
            var depenses = new List<Depense>();
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    // Union entre la table 'depenses' (frais généraux) et 'commandes_fournisseurs' (achats stock)
                    string query = @"
                        SELECT id, description, categorie, montant, date_depense, statut_paiement, mode_paiement, 'Generale' as source
                        FROM depenses
                        UNION ALL
                        SELECT id, CONCAT('Achat Stock: ', numero_commande) as description, 'Achat Stock' as categorie, montant_total as montant, date_commande as date_depense, 
                        CASE 
                            WHEN statut IN ('Envoyée', 'Partiellement reçue', 'Reçue', 'Fermée') THEN 'Payée'
                            WHEN date_reception IS NOT NULL THEN 'Payée'
                            WHEN statut = 'En attente' THEN 'Payée'  -- Les commandes fournisseurs sont considérées comme payées dès leur création (engagement financier)
                            ELSE 'En attente' 
                        END as statut_paiement, 
                        'Virement' as mode_paiement, 'Stock' as source
                        FROM commandes_fournisseurs
                        WHERE statut != 'Annulée'
                        ORDER BY date_depense DESC";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            depenses.Add(new Depense
                            {
                                Id = reader.GetInt32("id"),
                                Description = reader.GetString("description"),
                                Categorie = reader.GetString("categorie"),
                                Montant = reader.GetDecimal("montant"),
                                DateDepense = reader.GetDateTime("date_depense"),
                                StatutPaiement = reader.GetString("statut_paiement"),
                                ModePaiement = reader.IsDBNull(reader.GetOrdinal("mode_paiement")) ? null : reader.GetString("mode_paiement"),
                                Source = reader.GetString("source")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return depenses;
        }

        public static int AjouterDepense(Depense depense)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO depenses (description, categorie, montant, date_depense, statut_paiement, mode_paiement) 
                               VALUES (@desc, @cat, @montant, @date, @statut, @mode);
                               SELECT LAST_INSERT_ID();";
                
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@desc", depense.Description);
                    cmd.Parameters.AddWithValue("@cat", depense.Categorie);
                    cmd.Parameters.AddWithValue("@montant", depense.Montant);
                    cmd.Parameters.AddWithValue("@date", depense.DateDepense);
                    cmd.Parameters.AddWithValue("@statut", depense.StatutPaiement);
                    cmd.Parameters.AddWithValue("@mode", depense.ModePaiement ?? (object)DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>
        /// Obtenir une dépense par son ID
        /// </summary>
        public static Depense? GetDepenseById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT id, description, categorie, montant, date_depense, statut_paiement, mode_paiement, 'Generale' as source
                        FROM depenses
                        WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Depense
                                {
                                    Id = reader.GetInt32("id"),
                                    Description = reader.GetString("description"),
                                    Categorie = reader.GetString("categorie"),
                                    Montant = reader.GetDecimal("montant"),
                                    DateDepense = reader.GetDateTime("date_depense"),
                                    StatutPaiement = reader.GetString("statut_paiement"),
                                    ModePaiement = reader.IsDBNull(reader.GetOrdinal("mode_paiement")) ? null : reader.GetString("mode_paiement"),
                                    Source = reader.GetString("source")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la dépense: {ex.Message}", ex);
            }
            return null;
        }

        /// <summary>
        /// Modifier une dépense existante
        /// </summary>
        public static bool ModifierDepense(Depense depense)
        {
            if (depense.Id == 0)
                throw new Exception("L'ID de la dépense est requis pour la modification");

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        UPDATE depenses 
                        SET description = @desc, 
                            categorie = @cat, 
                            montant = @montant, 
                            date_depense = @date, 
                            statut_paiement = @statut, 
                            mode_paiement = @mode
                        WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desc", depense.Description);
                        cmd.Parameters.AddWithValue("@cat", depense.Categorie);
                        cmd.Parameters.AddWithValue("@montant", depense.Montant);
                        cmd.Parameters.AddWithValue("@date", depense.DateDepense);
                        cmd.Parameters.AddWithValue("@statut", depense.StatutPaiement);
                        cmd.Parameters.AddWithValue("@mode", depense.ModePaiement ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@id", depense.Id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la modification de la dépense: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprimer une dépense
        /// </summary>
        public static bool SupprimerDepense(int depenseId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM depenses WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", depenseId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de la dépense: {ex.Message}", ex);
            }
        }
    }

    public class Depense
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public decimal Montant { get; set; }
        public DateTime DateDepense { get; set; }
        public string StatutPaiement { get; set; } = "En attente";
        public string? ModePaiement { get; set; }
        public string Source { get; set; } = "Generale"; // 'Generale' ou 'Stock'
    }
}