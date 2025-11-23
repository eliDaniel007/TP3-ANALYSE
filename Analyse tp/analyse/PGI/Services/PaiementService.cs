using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class PaiementService
    {
        /// <summary>
        /// Enregistrer un paiement avec validation
        /// </summary>
        public static int EnregistrerPaiement(Paiement paiement)
        {
            // VALIDATION: Vérifier que le montant ne dépasse pas le montant dû
            var facture = FactureService.GetFactureById(paiement.FactureId);
            if (facture == null)
                throw new Exception("Facture introuvable");
            
            if (facture.Statut == "Annulée")
                throw new Exception("Impossible d'enregistrer un paiement pour une facture annulée");
            
            if (paiement.Montant > facture.MontantDu)
                throw new Exception($"Le montant du paiement ({paiement.Montant:C}) dépasse le montant dû ({facture.MontantDu:C})");
            
            if (paiement.Montant <= 0)
                throw new Exception("Le montant du paiement doit être supérieur à zéro");

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO paiements 
                        (facture_id, date_paiement, montant, mode_paiement, numero_reference, note, employe_id)
                        VALUES 
                        (@factureId, @date, @montant, @mode, @reference, @note, @employeId);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@factureId", paiement.FactureId);
                        cmd.Parameters.AddWithValue("@date", paiement.DatePaiement);
                        cmd.Parameters.AddWithValue("@montant", paiement.Montant);
                        cmd.Parameters.AddWithValue("@mode", paiement.ModePaiement);
                        cmd.Parameters.AddWithValue("@reference", paiement.NumeroReference ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@note", paiement.Note ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@employeId", paiement.EmployeId.HasValue ? paiement.EmployeId.Value : DBNull.Value);
                        
                        int paiementId = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        // Le trigger se charge de mettre à jour la facture automatiquement
                        
                        return paiementId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'enregistrement du paiement: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtenir tous les paiements d'une facture
        /// </summary>
        public static List<Paiement> GetPaiementsByFactureId(int factureId)
        {
            var paiements = new List<Paiement>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.*,
                            f.numero_facture,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON p.employe_id = e.id
                        WHERE p.facture_id = @factureId
                        ORDER BY p.date_paiement DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@factureId", factureId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paiements.Add(MapPaiementFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des paiements: {ex.Message}", ex);
            }

            return paiements;
        }

        /// <summary>
        /// Obtenir tous les paiements (historique complet)
        /// </summary>
        public static List<Paiement> GetAllPaiements()
        {
            var paiements = new List<Paiement>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.*,
                            f.numero_facture,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON p.employe_id = e.id
                        ORDER BY p.date_paiement DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            paiements.Add(MapPaiementFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des paiements: {ex.Message}", ex);
            }

            return paiements;
        }

        /// <summary>
        /// Obtenir les paiements d'une période
        /// </summary>
        public static List<Paiement> GetPaiementsByPeriode(DateTime dateDebut, DateTime dateFin)
        {
            var paiements = new List<Paiement>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.*,
                            f.numero_facture,
                            c.nom AS nom_client,
                            CONCAT(e.prenom, ' ', e.nom) AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
                        LEFT JOIN employes e ON p.employe_id = e.id
                        WHERE p.date_paiement BETWEEN @dateDebut AND @dateFin
                        ORDER BY p.date_paiement DESC";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paiements.Add(MapPaiementFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération des paiements: {ex.Message}", ex);
            }

            return paiements;
        }

        /// <summary>
        /// Mapper un paiement depuis un DataReader
        /// </summary>
        private static Paiement MapPaiementFromReader(MySqlDataReader reader)
        {
            return new Paiement
            {
                Id = reader.GetInt32("id"),
                FactureId = reader.GetInt32("facture_id"),
                DatePaiement = reader.GetDateTime("date_paiement"),
                Montant = reader.GetDecimal("montant"),
                ModePaiement = reader.GetString("mode_paiement"),
                NumeroReference = reader.IsDBNull(reader.GetOrdinal("numero_reference")) ? null : reader.GetString("numero_reference"),
                Note = reader.IsDBNull(reader.GetOrdinal("note")) ? null : reader.GetString("note"),
                EmployeId = reader.IsDBNull(reader.GetOrdinal("employe_id")) ? null : reader.GetInt32("employe_id"),
                NumeroFacture = reader.GetString("numero_facture"),
                NomClient = reader.GetString("nom_client"),
                NomEmploye = reader.IsDBNull(reader.GetOrdinal("nom_employe")) ? "N/A" : reader.GetString("nom_employe")
            };
        }
    }
}

