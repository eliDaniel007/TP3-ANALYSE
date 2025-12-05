using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class CampagneMarketingService
    {
        /// <summary>
        /// Créer une nouvelle campagne marketing
        /// </summary>
        public static int CreerCampagne(CampagneMarketing campagne)
        {
            // Validation: date fin >= date début
            if (campagne.DateFin < campagne.DateDebut)
            {
                throw new Exception("La date de fin doit être égale ou postérieure à la date de début.");
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO campagnes_marketing 
                        (nom_campagne, type, description, date_debut, date_fin, budget, nombre_destinataires, statut)
                        VALUES 
                        (@nom, @type, @description, @dateDebut, @dateFin, @budget, @nbDestinataires, @statut);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nom", campagne.NomCampagne);
                        cmd.Parameters.AddWithValue("@type", campagne.Type);
                        cmd.Parameters.AddWithValue("@description", campagne.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@dateDebut", campagne.DateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", campagne.DateFin);
                        cmd.Parameters.AddWithValue("@budget", campagne.Budget);
                        cmd.Parameters.AddWithValue("@nbDestinataires", campagne.NombreDestinataires);
                        cmd.Parameters.AddWithValue("@statut", campagne.Statut);
                        
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de la campagne: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour une campagne
        /// </summary>
        public static bool UpdateCampagne(CampagneMarketing campagne)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        UPDATE campagnes_marketing SET
                            nom_campagne = @nom,
                            type = @type,
                            description = @description,
                            date_debut = @dateDebut,
                            date_fin = @dateFin,
                            budget = @budget,
                            nombre_destinataires = @nbDestinataires,
                            nombre_reponses = @nbReponses,
                            statut = @statut
                        WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", campagne.Id);
                        cmd.Parameters.AddWithValue("@nom", campagne.NomCampagne);
                        cmd.Parameters.AddWithValue("@type", campagne.Type);
                        cmd.Parameters.AddWithValue("@description", campagne.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@dateDebut", campagne.DateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", campagne.DateFin);
                        cmd.Parameters.AddWithValue("@budget", campagne.Budget);
                        cmd.Parameters.AddWithValue("@nbDestinataires", campagne.NombreDestinataires);
                        cmd.Parameters.AddWithValue("@nbReponses", campagne.NombreReponses);
                        cmd.Parameters.AddWithValue("@statut", campagne.Statut);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la campagne: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Supprimer une campagne
        /// </summary>
        public static bool SupprimerCampagne(int campagneId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM campagnes_marketing WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", campagneId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de la campagne: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Clôturer une campagne (calcule automatiquement le taux de participation)
        /// </summary>
        public static bool CloturerCampagne(int campagneId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("CALL sp_cloture_campagne(@id)", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", campagneId);
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la clôture de la campagne: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir toutes les campagnes
        /// </summary>
        public static List<CampagneMarketing> GetAllCampagnes()
        {
            var campagnes = new List<CampagneMarketing>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM campagnes_marketing ORDER BY date_debut DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            campagnes.Add(MapCampagneFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des campagnes: {ex.Message}", ex);
            }

            return campagnes;
        }

        /// <summary>
        /// Obtenir une campagne par ID
        /// </summary>
        public static CampagneMarketing? GetCampagneById(int id)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM campagnes_marketing WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapCampagneFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de la campagne: {ex.Message}", ex);
            }

            return null;
        }

        private static CampagneMarketing MapCampagneFromReader(MySqlDataReader reader)
        {
            return new CampagneMarketing
            {
                Id = reader.GetInt32("id"),
                NomCampagne = reader.GetString("nom_campagne"),
                Type = reader.GetString("type"),
                Description = reader.IsDBNull(reader.GetOrdinal("description")) ? string.Empty : reader.GetString("description"),
                DateDebut = reader.GetDateTime("date_debut"),
                DateFin = reader.GetDateTime("date_fin"),
                Budget = reader.GetDecimal("budget"),
                NombreDestinataires = reader.GetInt32("nombre_destinataires"),
                NombreReponses = reader.GetInt32("nombre_reponses"),
                TauxParticipation = reader.GetDecimal("taux_participation"),
                Statut = reader.GetString("statut"),
                DateCreation = reader.GetDateTime("date_creation")
            };
        }
    }
}

