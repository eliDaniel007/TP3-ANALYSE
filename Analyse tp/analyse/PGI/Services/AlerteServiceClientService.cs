using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class AlerteServiceClientService
    {
        /// <summary>
        /// Créer une nouvelle alerte
        /// </summary>
        public static int CreerAlerte(AlerteServiceClient alerte)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO alertes_service_client 
                        (client_id, evaluation_id, type_alerte, description, priorite, statut, employe_assigne_id)
                        VALUES 
                        (@clientId, @evaluationId, @type, @description, @priorite, @statut, @employeId);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", alerte.ClientId);
                        cmd.Parameters.AddWithValue("@evaluationId", alerte.EvaluationId.HasValue ? alerte.EvaluationId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@type", alerte.TypeAlerte);
                        cmd.Parameters.AddWithValue("@description", alerte.Description);
                        cmd.Parameters.AddWithValue("@priorite", alerte.Priorite);
                        cmd.Parameters.AddWithValue("@statut", alerte.Statut);
                        cmd.Parameters.AddWithValue("@employeId", alerte.EmployeAssigneId.HasValue ? alerte.EmployeAssigneId.Value : DBNull.Value);
                        
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'alerte: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Assigner une alerte à un employé
        /// </summary>
        public static bool AssignerAlerte(int alerteId, int employeId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        UPDATE alertes_service_client SET
                            employe_assigne_id = @employeId,
                            statut = 'En cours'
                        WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", alerteId);
                        cmd.Parameters.AddWithValue("@employeId", employeId);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'assignation de l'alerte: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Résoudre une alerte
        /// </summary>
        public static bool ResoudreAlerte(int alerteId, string noteResolution)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        UPDATE alertes_service_client SET
                            statut = 'Résolue',
                            date_resolution = @dateResolution,
                            note_resolution = @note
                        WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", alerteId);
                        cmd.Parameters.AddWithValue("@dateResolution", DateTime.Now);
                        cmd.Parameters.AddWithValue("@note", noteResolution);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la résolution de l'alerte: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir toutes les alertes
        /// </summary>
        public static List<AlerteServiceClient> GetAllAlertes(string? filtreStatut = null)
        {
            var alertes = new List<AlerteServiceClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            a.*,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM alertes_service_client a
                        INNER JOIN clients c ON a.client_id = c.id
                        LEFT JOIN employes e ON a.employe_assigne_id = e.id
                        " + (string.IsNullOrEmpty(filtreStatut) ? "" : "WHERE a.statut = @statut ") + @"
                        ORDER BY 
                            CASE a.priorite
                                WHEN 'Urgente' THEN 1
                                WHEN 'Haute' THEN 2
                                WHEN 'Moyenne' THEN 3
                                WHEN 'Basse' THEN 4
                            END,
                            a.date_creation DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(filtreStatut))
                        {
                            cmd.Parameters.AddWithValue("@statut", filtreStatut);
                        }
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                alertes.Add(MapAlerteFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des alertes: {ex.Message}", ex);
            }

            return alertes;
        }

        /// <summary>
        /// Obtenir les alertes d'un client
        /// </summary>
        public static List<AlerteServiceClient> GetAlertesByClient(int clientId)
        {
            var alertes = new List<AlerteServiceClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            a.*,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM alertes_service_client a
                        INNER JOIN clients c ON a.client_id = c.id
                        LEFT JOIN employes e ON a.employe_assigne_id = e.id
                        WHERE a.client_id = @clientId
                        ORDER BY a.date_creation DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                alertes.Add(MapAlerteFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des alertes du client: {ex.Message}", ex);
            }

            return alertes;
        }

        private static AlerteServiceClient MapAlerteFromReader(MySqlDataReader reader)
        {
            return new AlerteServiceClient
            {
                Id = reader.GetInt32("id"),
                ClientId = reader.GetInt32("client_id"),
                EvaluationId = reader.IsDBNull(reader.GetOrdinal("evaluation_id")) ? null : reader.GetInt32("evaluation_id"),
                TypeAlerte = reader.GetString("type_alerte"),
                Description = reader.GetString("description"),
                Priorite = reader.GetString("priorite"),
                Statut = reader.GetString("statut"),
                EmployeAssigneId = reader.IsDBNull(reader.GetOrdinal("employe_assigne_id")) ? null : reader.GetInt32("employe_assigne_id"),
                DateCreation = reader.GetDateTime("date_creation"),
                DateResolution = reader.IsDBNull(reader.GetOrdinal("date_resolution")) ? null : reader.GetDateTime("date_resolution"),
                NoteResolution = reader.IsDBNull(reader.GetOrdinal("note_resolution")) ? null : reader.GetString("note_resolution"),
                NomClient = reader.GetString("nom_client"),
                NomEmployeAssigne = reader.IsDBNull(reader.GetOrdinal("nom_employe")) ? "Non assigné" : reader.GetString("nom_employe")
            };
        }
    }
}

