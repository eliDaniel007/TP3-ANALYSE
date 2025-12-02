using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace PGI.Helpers
{
    /// <summary>
    /// Classe helper pour gérer la connexion à la base de données MySQL
    /// </summary>
    public class DatabaseHelper
    {
        // Connexion MySQL simple - Mot de passe : password
        // Les mots de passe des employés/clients sont stockés en clair dans la colonne mot_de_passe (pas de hashage)
        private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=password;AllowUserVariables=true;";

        /// <summary>
        /// Obtenir une nouvelle connexion MySQL
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Configurer la chaîne de connexion (connexion simple MySQL)
        /// </summary>
        public static void SetConnectionString(string server, string database, string uid, string pwd)
        {
            // Connexion simple à MySQL
            // Les mots de passe des employés/clients sont en clair dans la colonne mot_de_passe (pas de hashage)
            connectionString = $"Server={server};Database={database};Uid={uid};Pwd={pwd};AllowUserVariables=true;";
        }

        /// <summary>
        /// Tester la connexion à la base de données
        /// </summary>
        public static bool TestConnection(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Exécuter une requête SELECT et retourner un DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Ajouter les paramètres si fournis
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? (object)DBNull.Value);
                            }
                        }

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'exécution de la requête: {ex.Message}", ex);
            }
            return dataTable;
        }

        /// <summary>
        /// Exécuter une commande INSERT, UPDATE ou DELETE
        /// </summary>
        public static int ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Ajouter les paramètres si fournis
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? (object)DBNull.Value);
                            }
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'exécution de la commande: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exécuter une requête qui retourne une seule valeur (COUNT, MAX, etc.)
        /// </summary>
        public static object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        // Ajouter les paramètres si fournis
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? (object)DBNull.Value);
                            }
                        }

                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'exécution de la requête scalaire: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exécuter une procédure stockée
        /// </summary>
        public static DataTable ExecuteStoredProcedure(string procedureName, Dictionary<string, object> parameters = null)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(procedureName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Ajouter les paramètres si fournis
                        if (parameters != null)
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value ?? (object)DBNull.Value);
                            }
                        }

                        using (var adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'exécution de la procédure stockée: {ex.Message}", ex);
            }
            return dataTable;
        }

        /// <summary>
        /// Obtenir le dernier ID inséré
        /// </summary>
        public static long GetLastInsertId()
        {
            try
            {
                var result = ExecuteScalar("SELECT LAST_INSERT_ID()");
                return Convert.ToInt64(result);
            }
            catch
            {
                return 0;
            }
        }
    }
}

