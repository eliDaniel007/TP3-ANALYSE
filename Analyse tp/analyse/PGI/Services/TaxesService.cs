using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class TaxesService
    {
        /// <summary>
        /// Obtenir le taux de TPS actif
        /// </summary>
        public static decimal GetTauxTPS()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT taux FROM parametres_taxes WHERE nom_taxe = 'TPS' AND actif = TRUE LIMIT 1";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0.05m; // Défaut 5%
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetTauxTPS: {ex.Message}");
                return 0.05m; // Taux par défaut
            }
        }

        /// <summary>
        /// Obtenir le taux de TVQ actif
        /// </summary>
        public static decimal GetTauxTVQ()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT taux FROM parametres_taxes WHERE nom_taxe = 'TVQ' AND actif = TRUE LIMIT 1";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDecimal(result) : 0.09975m; // Défaut 9.975%
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetTauxTVQ: {ex.Message}");
                return 0.09975m; // Taux par défaut
            }
        }

        /// <summary>
        /// Calculer les taxes sur un montant
        /// </summary>
        public static (decimal tps, decimal tvq, decimal total) CalculerTaxes(decimal sousTotal)
        {
            decimal tauxTPS = GetTauxTPS();
            decimal tauxTVQ = GetTauxTVQ();
            
            decimal montantTPS = Math.Round(sousTotal * tauxTPS, 2);
            decimal montantTVQ = Math.Round(sousTotal * tauxTVQ, 2);
            decimal total = sousTotal + montantTPS + montantTVQ;
            
            return (montantTPS, montantTVQ, total);
        }

        /// <summary>
        /// Obtenir tous les paramètres de taxes
        /// </summary>
        public static List<ParametresTaxes> GetAllTaxes()
        {
            var taxes = new List<ParametresTaxes>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM parametres_taxes ORDER BY nom_taxe";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            taxes.Add(new ParametresTaxes
                            {
                                Id = reader.GetInt32("id"),
                                NomTaxe = reader.GetString("nom_taxe"),
                                Taux = reader.GetDecimal("taux"),
                                Actif = reader.GetBoolean("actif"),
                                DateEffective = reader.GetDateTime("date_effective"),
                                Description = reader.IsDBNull(reader.GetOrdinal("description")) 
                                    ? null : reader.GetString("description")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des taxes: {ex.Message}", ex);
            }

            return taxes;
        }

        /// <summary>
        /// Mettre à jour un taux de taxe
        /// </summary>
        public static bool UpdateTaxe(int id, decimal nouveauTaux)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE parametres_taxes SET taux = @taux WHERE id = @id";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taux", nouveauTaux);
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la taxe: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Mettre à jour un taux de taxe par son nom
        /// </summary>
        public static bool UpdateTaxRate(string nomTaxe, decimal nouveauTaux)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE parametres_taxes SET taux = @taux WHERE nom_taxe = @nomTaxe";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taux", nouveauTaux);
                        cmd.Parameters.AddWithValue("@nomTaxe", nomTaxe);
                        
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de la taxe {nomTaxe}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir les taux de taxes actifs (TPS et TVQ)
        /// </summary>
        public static (decimal TauxTPS, decimal TauxTVQ) GetTaxes()
        {
            return (GetTauxTPS(), GetTauxTVQ());
        }
    }
}

