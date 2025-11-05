using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;

namespace PGI.Services
{
    public class EmployeService
    {
        /// <summary>
        /// Authentifier un employé avec email et mot de passe
        /// </summary>
        public static (bool success, string nom, string prenom, string role) Authenticate(string email, string password)
        {
            try
            {
                string query = @"
                    SELECT id, nom, prenom, role_systeme, mot_de_passe
                    FROM employes
                    WHERE courriel = @email AND statut = 'Actif'";

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
                        string prenom = dt.Rows[0]["prenom"]?.ToString() ?? string.Empty;
                        string role = dt.Rows[0]["role_systeme"]?.ToString() ?? "Employé";

                        return (true, nom, prenom, role);
                    }
                }

                return (false, string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'authentification : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir un employé par son email
        /// </summary>
        public static DataRow GetEmployeByEmail(string email)
        {
            try
            {
                string query = @"
                    SELECT id, matricule, nom, prenom, courriel, telephone, 
                           departement, poste, role_systeme, statut
                    FROM employes
                    WHERE courriel = @email";

                var parameters = new Dictionary<string, object>
                {
                    { "@email", email }
                };

                var dt = DatabaseHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'employé : {ex.Message}", ex);
            }
        }
    }
}

