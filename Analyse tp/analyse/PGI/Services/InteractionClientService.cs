using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class InteractionClientService
    {
        /// <summary>
        /// Créer une nouvelle interaction client
        /// </summary>
        public static int CreerInteraction(InteractionClient interaction)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO interactions_clients 
                        (client_id, employe_id, type_interaction, sujet, description, date_interaction, resultat_action)
                        VALUES 
                        (@clientId, @employeId, @type, @sujet, @description, @date, @resultat);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", interaction.ClientId);
                        cmd.Parameters.AddWithValue("@employeId", interaction.EmployeId.HasValue ? interaction.EmployeId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@type", interaction.TypeInteraction);
                        cmd.Parameters.AddWithValue("@sujet", interaction.Sujet);
                        cmd.Parameters.AddWithValue("@description", interaction.Description);
                        cmd.Parameters.AddWithValue("@date", interaction.DateInteraction);
                        cmd.Parameters.AddWithValue("@resultat", interaction.ResultatAction ?? (object)DBNull.Value);
                        
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'interaction: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir toutes les interactions d'un client
        /// </summary>
        public static List<InteractionClient> GetInteractionsByClient(int clientId)
        {
            var interactions = new List<InteractionClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ic.*,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM interactions_clients ic
                        INNER JOIN clients c ON ic.client_id = c.id
                        LEFT JOIN employes e ON ic.employe_id = e.id
                        WHERE ic.client_id = @clientId
                        ORDER BY ic.date_interaction DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                interactions.Add(MapInteractionFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des interactions: {ex.Message}", ex);
            }

            return interactions;
        }

        /// <summary>
        /// Obtenir toutes les interactions récentes
        /// </summary>
        public static List<InteractionClient> GetInteractionsRecentes(int limit = 50)
        {
            var interactions = new List<InteractionClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ic.*,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM interactions_clients ic
                        INNER JOIN clients c ON ic.client_id = c.id
                        LEFT JOIN employes e ON ic.employe_id = e.id
                        ORDER BY ic.date_interaction DESC
                        LIMIT @limit";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                interactions.Add(MapInteractionFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des interactions récentes: {ex.Message}", ex);
            }

            return interactions;
        }

        private static InteractionClient MapInteractionFromReader(MySqlDataReader reader)
        {
            return new InteractionClient
            {
                Id = reader.GetInt32("id"),
                ClientId = reader.GetInt32("client_id"),
                EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) ? null : reader.GetInt32("employe_id"),
                TypeInteraction = reader.GetString("type_interaction"),
                Sujet = reader.GetString("sujet"),
                Description = reader.GetString("description"),
                DateInteraction = reader.GetDateTime("date_interaction"),
                ResultatAction = reader.IsDBNull(reader.GetOrdinal("resultat_action")) ? null : reader.GetString("resultat_action"),
                DateCreation = reader.GetDateTime("date_creation"),
                NomClient = reader.GetString("nom_client"),
                NomEmploye = reader.IsDBNull(reader.GetOrdinal("nom_employe")) ? "Système" : reader.GetString("nom_employe")
            };
        }
    }
}

