using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class ClientService
    {
        /// <summary>
        /// Authentifier un client avec email et mot de passe
        /// </summary>
        public static (bool success, string nom, int clientId) Authenticate(string email, string password)
        {
            try
            {
                string query = @"
                    SELECT id, nom, mot_de_passe
                    FROM clients
                    WHERE courriel_contact = @email AND statut != 'Inactif'";

                var parameters = new Dictionary<string, object>
                {
                    { "@email", email }
                };

                var dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    string? dbPassword = dt.Rows[0]["mot_de_passe"] != DBNull.Value 
                        ? dt.Rows[0]["mot_de_passe"].ToString() 
                        : null;

                    // Vérification simple du mot de passe (pas de hashage)
                    if (dbPassword == password)
                    {
                        string nom = dt.Rows[0]["nom"]?.ToString() ?? string.Empty;
                        int id = Convert.ToInt32(dt.Rows[0]["id"]);

                        return (true, nom, id);
                    }
                }

                return (false, string.Empty, 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'authentification du client : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inscrire un nouveau client
        /// </summary>
        public static (bool success, string message, int clientId) Register(string nom, string email, string telephone, string password)
        {
            try
            {
                // Vérifier que l'email contient "client"
                if (!email.ToLower().Contains("client"))
                {
                    return (false, "❌ L'adresse email doit contenir le mot 'client'.", 0);
                }

                // Vérifier si l'email existe déjà
                string checkQuery = "SELECT COUNT(*) FROM clients WHERE courriel_contact = @email";
                var checkParams = new Dictionary<string, object>
                {
                    { "@email", email }
                };

                var count = DatabaseHelper.ExecuteScalar(checkQuery, checkParams);
                if (Convert.ToInt32(count) > 0)
                {
                    return (false, "❌ Cette adresse email est déjà utilisée.", 0);
                }

                // Insérer le nouveau client
                string insertQuery = @"
                    INSERT INTO clients (type, nom, courriel_contact, telephone, mot_de_passe, statut)
                    VALUES (@type, @nom, @email, @telephone, @password, @statut)";

                var insertParams = new Dictionary<string, object>
                {
                    { "@type", "Particulier" },
                    { "@nom", nom },
                    { "@email", email },
                    { "@telephone", telephone ?? string.Empty },
                    { "@password", password },
                    { "@statut", "Actif" }
                };

                DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
                int newClientId = (int)DatabaseHelper.GetLastInsertId();

                return (true, "✅ Inscription réussie !", newClientId);
            }
            catch (Exception ex)
            {
                return (false, $"❌ Erreur lors de l'inscription : {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Obtenir un client par son email
        /// </summary>
        public static Client GetClientByEmail(string email)
        {
            try
            {
                string query = @"
                    SELECT id, type, nom, courriel_contact, telephone, statut, date_creation
                    FROM clients
                    WHERE courriel_contact = @email";

                var parameters = new Dictionary<string, object>
                {
                    { "@email", email }
                };

                var dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return new Client
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Type = row["type"].ToString(),
                        Nom = row["nom"].ToString(),
                        CourrielContact = row["courriel_contact"].ToString(),
                        Telephone = row["telephone"] != DBNull.Value ? row["telephone"].ToString() : string.Empty,
                        Statut = row["statut"].ToString(),
                        DateCreation = Convert.ToDateTime(row["date_creation"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération du client : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir un client par son ID
        /// </summary>
        public static Client? GetClientById(int id)
        {
            try
            {
                string query = @"
                    SELECT id, type, nom, courriel_contact, telephone, statut, date_creation
                    FROM clients
                    WHERE id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", id }
                };

                var dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    return new Client
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Type = row["type"].ToString() ?? string.Empty,
                        Nom = row["nom"].ToString() ?? string.Empty,
                        CourrielContact = row["courriel_contact"].ToString() ?? string.Empty,
                        Telephone = row["telephone"] != DBNull.Value ? row["telephone"].ToString() : null,
                        Statut = row["statut"].ToString() ?? "Actif",
                        DateCreation = Convert.ToDateTime(row["date_creation"])
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération du client : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Vérifier si un email contient "client"
        /// </summary>
        public static bool IsClientEmail(string email)
        {
            return email.ToLower().Contains("client");
        }
    }
}

