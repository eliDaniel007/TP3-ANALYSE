using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class EvaluationClientService
    {
        /// <summary>
        /// Créer une nouvelle évaluation client (trigger créera alerte si note ≤ 2)
        /// </summary>
        public static int CreerEvaluation(EvaluationClient evaluation)
        {
            // Validation: note entre 1 et 5
            if (evaluation.NoteSatisfaction < 1 || evaluation.NoteSatisfaction > 5)
            {
                throw new Exception("La note de satisfaction doit être entre 1 et 5.");
            }

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO evaluations_clients 
                        (client_id, facture_id, note_satisfaction, commentaire, date_evaluation)
                        VALUES 
                        (@clientId, @factureId, @note, @commentaire, @date);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", evaluation.ClientId);
                        cmd.Parameters.AddWithValue("@factureId", evaluation.FactureId.HasValue ? evaluation.FactureId.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@note", evaluation.NoteSatisfaction);
                        cmd.Parameters.AddWithValue("@commentaire", evaluation.Commentaire ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@date", evaluation.DateEvaluation);
                        
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création de l'évaluation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir toutes les évaluations d'un client
        /// </summary>
        public static List<EvaluationClient> GetEvaluationsByClient(int clientId)
        {
            var evaluations = new List<EvaluationClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ev.*,
                            c.nom AS nom_client,
                            f.numero_facture
                        FROM evaluations_clients ev
                        INNER JOIN clients c ON ev.client_id = c.id
                        LEFT JOIN factures f ON ev.facture_id = f.id
                        WHERE ev.client_id = @clientId
                        ORDER BY ev.date_evaluation DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                evaluations.Add(MapEvaluationFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des évaluations: {ex.Message}", ex);
            }

            return evaluations;
        }

        /// <summary>
        /// Obtenir toutes les évaluations récentes
        /// </summary>
        public static List<EvaluationClient> GetAllEvaluations(int limit = 100)
        {
            var evaluations = new List<EvaluationClient>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            ev.*,
                            c.nom AS nom_client,
                            f.numero_facture
                        FROM evaluations_clients ev
                        INNER JOIN clients c ON ev.client_id = c.id
                        LEFT JOIN factures f ON ev.facture_id = f.id
                        ORDER BY ev.date_evaluation DESC
                        LIMIT @limit";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@limit", limit);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                evaluations.Add(MapEvaluationFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des évaluations: {ex.Message}", ex);
            }

            return evaluations;
        }

        private static EvaluationClient MapEvaluationFromReader(MySqlDataReader reader)
        {
            return new EvaluationClient
            {
                Id = reader.GetInt32("id"),
                ClientId = reader.GetInt32("client_id"),
                FactureId = reader.IsDBNull(reader.GetOrdinal("facture_id")) ? null : reader.GetInt32("facture_id"),
                NoteSatisfaction = reader.GetInt32("note_satisfaction"),
                Commentaire = reader.IsDBNull(reader.GetOrdinal("commentaire")) ? null : reader.GetString("commentaire"),
                DateEvaluation = reader.GetDateTime("date_evaluation"),
                AlerteGeneree = reader.GetBoolean("alerte_generee"),
                NomClient = reader.GetString("nom_client"),
                NumeroFacture = reader.IsDBNull(reader.GetOrdinal("numero_facture")) ? null : reader.GetString("numero_facture")
            };
        }
    }
}

