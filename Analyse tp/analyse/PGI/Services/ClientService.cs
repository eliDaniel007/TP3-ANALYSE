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
        /// Vérifier si un email contient "client"
        /// </summary>
        public static bool IsClientEmail(string email)
        {
            return email.ToLower().Contains("client");
        }

        /// <summary>
        /// Obtenir un client par son ID
        /// </summary>
        public static Client? GetClientById(int id)
        {
            try
            {
                string query = @"
                    SELECT id, type, nom, courriel_contact, mot_de_passe, telephone, statut, date_creation
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
                        MotDePasse = row["mot_de_passe"] != DBNull.Value ? row["mot_de_passe"].ToString() : string.Empty,
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
        /// Obtenir tous les clients
        /// </summary>
        public static List<Client> GetAllClients()
        {
            var clients = new List<Client>();

            try
            {
                string query = @"
                    SELECT id, type, nom, courriel_contact, mot_de_passe, telephone, statut, date_creation
                    FROM clients
                    ORDER BY nom";

                var dt = DatabaseHelper.ExecuteQuery(query, null);

                foreach (DataRow row in dt.Rows)
                {
                    clients.Add(new Client
                    {
                        Id = Convert.ToInt32(row["id"]),
                        Type = row["type"].ToString() ?? string.Empty,
                        Nom = row["nom"].ToString() ?? string.Empty,
                        CourrielContact = row["courriel_contact"].ToString() ?? string.Empty,
                        MotDePasse = row["mot_de_passe"] != DBNull.Value ? row["mot_de_passe"].ToString() : string.Empty,
                        Telephone = row["telephone"] != DBNull.Value ? row["telephone"].ToString() : null,
                        Statut = row["statut"].ToString() ?? "Actif",
                        DateCreation = Convert.ToDateTime(row["date_creation"])
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des clients : {ex.Message}", ex);
            }

            return clients;
        }

        /// <summary>
        /// Créer un nouveau client (mode CRM avec validation complète)
        /// </summary>
        public static int CreerClient(Client client)
        {
            // Validation des champs obligatoires
            if (string.IsNullOrWhiteSpace(client.Nom))
                throw new Exception("Le nom du client est obligatoire.");
            
            if (string.IsNullOrWhiteSpace(client.CourrielContact))
                throw new Exception("L'adresse email est obligatoire.");
            
            if (!client.CourrielContact.Contains("@"))
                throw new Exception("L'adresse email n'est pas valide.");
            
            if (string.IsNullOrWhiteSpace(client.Telephone))
                throw new Exception("Le téléphone est obligatoire.");
            
            if (string.IsNullOrWhiteSpace(client.Type))
                throw new Exception("Le type de client est obligatoire.");

            // Vérifier l'unicité de l'email
            var existingClient = GetClientByEmail(client.CourrielContact);
            if (existingClient != null)
            {
                throw new Exception("Un client avec cette adresse email existe déjà.");
            }

            try
            {
                string query = @"
                    INSERT INTO clients (type, nom, courriel_contact, telephone, mot_de_passe, statut)
                    VALUES (@type, @nom, @email, @telephone, @password, @statut)";

                var parameters = new Dictionary<string, object>
                {
                    { "@type", client.Type },
                    { "@nom", client.Nom },
                    { "@email", client.CourrielContact },
                    { "@telephone", client.Telephone ?? string.Empty },
                    { "@password", client.MotDePasse ?? string.Empty },
                    { "@statut", string.IsNullOrWhiteSpace(client.Statut) ? "Prospect" : client.Statut }
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                int newId = (int)DatabaseHelper.GetLastInsertId();
                
                // Créer une interaction de bienvenue
                InteractionClientService.CreerInteraction(new Models.InteractionClient
                {
                    ClientId = newId,
                    TypeInteraction = "Email",
                    Sujet = "Bienvenue",
                    Description = $"Bienvenue chez NordikAdventures! Nous sommes ravis de vous compter parmi nos clients.",
                    DateInteraction = DateTime.Now
                });
                
                return newId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la création du client: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour un client
        /// </summary>
        public static bool UpdateClient(Client client)
        {
            // Validation
            if (client.Id <= 0)
                throw new Exception("ID client invalide.");
            
            if (string.IsNullOrWhiteSpace(client.Nom))
                throw new Exception("Le nom du client est obligatoire.");
            
            if (string.IsNullOrWhiteSpace(client.CourrielContact))
                throw new Exception("L'adresse email est obligatoire.");
            
            if (string.IsNullOrWhiteSpace(client.Telephone))
                throw new Exception("Le téléphone est obligatoire.");

            try
            {
                string query = @"
                    UPDATE clients SET
                        type = @type,
                        nom = @nom,
                        courriel_contact = @email,
                        telephone = @telephone,
                        statut = @statut
                    WHERE id = @id";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", client.Id },
                    { "@type", client.Type },
                    { "@nom", client.Nom },
                    { "@email", client.CourrielContact },
                    { "@telephone", client.Telephone ?? string.Empty },
                    { "@statut", client.Statut }
                };

                return DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour du client: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Changer le statut d'un client (avec création d'interaction)
        /// </summary>
        public static bool ChangerStatut(int clientId, string nouveauStatut)
        {
            // Validation du statut
            var statutsValides = new[] { "Prospect", "Actif", "Fidèle", "Inactif" };
            if (!Array.Exists(statutsValides, s => s == nouveauStatut))
            {
                throw new Exception($"Statut invalide. Doit être: {string.Join(", ", statutsValides)}");
            }

            try
            {
                var client = GetClientById(clientId);
                if (client == null)
                    throw new Exception("Client introuvable.");

                string ancienStatut = client.Statut;
                
                // Mettre à jour le statut
                string query = "UPDATE clients SET statut = @statut WHERE id = @id";
                var parameters = new Dictionary<string, object>
                {
                    { "@id", clientId },
                    { "@statut", nouveauStatut }
                };

                bool success = DatabaseHelper.ExecuteNonQuery(query, parameters) > 0;
                
                if (success)
                {
                    // Créer une interaction pour documenter le changement
                    InteractionClientService.CreerInteraction(new Models.InteractionClient
                    {
                        ClientId = clientId,
                        TypeInteraction = "Note",
                        Sujet = $"Changement de statut: {ancienStatut} → {nouveauStatut}",
                        Description = $"Statut du client modifié manuellement de '{ancienStatut}' à '{nouveauStatut}'.",
                        DateInteraction = DateTime.Now
                    });
                }
                
                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du changement de statut: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Désactiver un client (ne peut pas être supprimé s'il a des ventes)
        /// </summary>
        public static bool DesactiverClient(int clientId)
        {
            try
            {
                // Vérifier si le client a des ventes
                string checkQuery = @"
                    SELECT COUNT(*) FROM factures WHERE client_id = @clientId";
                
                var checkParams = new Dictionary<string, object>
                {
                    { "@clientId", clientId }
                };
                
                int nbVentes = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));
                
                if (nbVentes > 0)
                {
                    // Désactiver seulement (ne pas supprimer)
                    return ChangerStatut(clientId, "Inactif");
                }
                else
                {
                    // Aucune vente, on peut supprimer
                    string deleteQuery = "DELETE FROM clients WHERE id = @clientId";
                    return DatabaseHelper.ExecuteNonQuery(deleteQuery, checkParams) > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la désactivation du client: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Marquer les clients inactifs (appel procédure stockée)
        /// </summary>
        public static void MarquerClientsInactifs()
        {
            try
            {
                string query = "CALL sp_marquer_clients_inactifs()";
                DatabaseHelper.ExecuteNonQuery(query, null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors du marquage des clients inactifs: {ex.Message}", ex);
            }
        }
    }
}

