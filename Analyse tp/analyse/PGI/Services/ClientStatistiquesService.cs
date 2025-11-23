using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class ClientStatistiquesService
    {
        /// <summary>
        /// Obtenir les statistiques d'un client
        /// </summary>
        public static ClientStatistiques? GetStatistiquesClient(int clientId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM vue_statistiques_clients WHERE client_id = @clientId";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapStatistiquesFromReader(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques du client: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Obtenir les statistiques de tous les clients
        /// </summary>
        public static List<ClientStatistiques> GetAllStatistiques()
        {
            var statistiques = new List<ClientStatistiques>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM vue_statistiques_clients ORDER BY score_composite DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            statistiques.Add(MapStatistiquesFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des statistiques: {ex.Message}", ex);
            }

            return statistiques;
        }

        /// <summary>
        /// Calculer les KPIs globaux CRM
        /// </summary>
        public static Dictionary<string, object> GetKPIsGlobaux()
        {
            var kpis = new Dictionary<string, object>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    
                    // Taux de fidélisation (clients fidèles / clients actifs)
                    string queryFidelisation = @"
                        SELECT 
                            COUNT(CASE WHEN statut = 'Fidèle' THEN 1 END) AS fideles,
                            COUNT(CASE WHEN statut IN ('Actif', 'Fidèle') THEN 1 END) AS actifs
                        FROM clients";
                    
                    using (var cmd = new MySqlCommand(queryFidelisation, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int fideles = reader.GetInt32("fideles");
                            int actifs = reader.GetInt32("actifs");
                            decimal tauxFidelisation = actifs > 0 ? (decimal)fideles / actifs * 100 : 0;
                            
                            kpis["NombreClientsFideles"] = fideles;
                            kpis["NombreClientsActifs"] = actifs;
                            kpis["TauxFidelisation"] = tauxFidelisation;
                        }
                    }
                    
                    // Taux de conversion (prospects → clients actifs)
                    string queryConversion = @"
                        SELECT 
                            COUNT(CASE WHEN statut = 'Prospect' THEN 1 END) AS prospects,
                            COUNT(CASE WHEN statut IN ('Actif', 'Fidèle') THEN 1 END) AS convertis,
                            COUNT(*) AS total
                        FROM clients";
                    
                    using (var cmd = new MySqlCommand(queryConversion, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int prospects = reader.GetInt32("prospects");
                            int total = reader.GetInt32("total");
                            int convertis = reader.GetInt32("convertis");
                            decimal tauxConversion = (prospects + convertis) > 0 ? (decimal)convertis / (prospects + convertis) * 100 : 0;
                            
                            kpis["NombreProspects"] = prospects;
                            kpis["TauxConversion"] = tauxConversion;
                        }
                    }
                    
                    // Panier moyen et CA total
                    string queryCA = @"
                        SELECT 
                            COALESCE(AVG(montant_total), 0) AS panier_moyen,
                            COALESCE(SUM(montant_total), 0) AS ca_total,
                            COUNT(*) AS nb_factures
                        FROM factures
                        WHERE statut = 'Active'";
                    
                    using (var cmd = new MySqlCommand(queryCA, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            kpis["PanierMoyen"] = reader.GetDecimal("panier_moyen");
                            kpis["ChiffreAffairesTotal"] = reader.GetDecimal("ca_total");
                            kpis["NombreFactures"] = reader.GetInt32("nb_factures");
                        }
                    }
                    
                    // Alertes ouvertes
                    string queryAlertes = @"
                        SELECT COUNT(*) AS nb_alertes_ouvertes
                        FROM alertes_service_client
                        WHERE statut IN ('Ouverte', 'En cours')";
                    
                    using (var cmd = new MySqlCommand(queryAlertes, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            kpis["AlertesOuvertes"] = reader.GetInt32("nb_alertes_ouvertes");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du calcul des KPIs: {ex.Message}", ex);
            }

            return kpis;
        }

        /// <summary>
        /// Vérifier si un client peut passer commande
        /// </summary>
        public static bool ClientPeutCommander(int clientId)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT fn_client_peut_commander(@clientId) AS peut_commander";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clientId", clientId);
                        var result = cmd.ExecuteScalar();
                        return Convert.ToBoolean(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la vérification de l'éligibilité du client: {ex.Message}", ex);
            }
        }

        private static ClientStatistiques MapStatistiquesFromReader(MySqlDataReader reader)
        {
            return new ClientStatistiques
            {
                ClientId = reader.GetInt32("client_id"),
                NomClient = reader.GetString("nom_client"),
                Statut = reader.GetString("statut"),
                NombreCommandes = reader.GetInt32("nombre_commandes"),
                ChiffreAffairesTotal = reader.GetDecimal("chiffre_affaires_total"),
                PanierMoyen = reader.GetDecimal("panier_moyen"),
                DateDerniereCommande = reader.IsDBNull(reader.GetOrdinal("date_derniere_commande")) ? null : reader.GetDateTime("date_derniere_commande"),
                JoursSanActivite = reader.IsDBNull(reader.GetOrdinal("jours_sans_activite")) ? 0 : reader.GetInt32("jours_sans_activite"),
                NoteSatisfactionMoyenne = reader.GetDecimal("note_satisfaction_moyenne"),
                NombreInteractions = reader.GetInt32("nombre_interactions"),
                NombreReclamations = reader.GetInt32("nombre_reclamations"),
                MontantImpaye = reader.GetDecimal("montant_impaye"),
                RetardPaiement = reader.GetBoolean("retard_paiement"),
                ScoreComposite = reader.GetDecimal("score_composite")
            };
        }
    }
}

