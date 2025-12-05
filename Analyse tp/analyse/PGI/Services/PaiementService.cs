using System;
using System.Collections.Generic;
using System.Linq;
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
                        (facture_id, date_paiement, montant, numero_reference, note)
                        VALUES 
                        (@factureId, @date, @montant, @reference, @note);
                        SELECT LAST_INSERT_ID();";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@factureId", paiement.FactureId);
                        cmd.Parameters.AddWithValue("@date", paiement.DatePaiement);
                        cmd.Parameters.AddWithValue("@montant", paiement.Montant);
                        cmd.Parameters.AddWithValue("@reference", paiement.NumeroReference ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@note", paiement.Note ?? (object)DBNull.Value);
                        
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
                            NULL AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
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
                            NULL AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
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
                            NULL AS nom_employe
                        FROM paiements p
                        INNER JOIN factures f ON p.facture_id = f.id
                        INNER JOIN clients c ON f.client_id = c.id
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
        /// Enregistrer un remboursement
        /// </summary>
        public static int EnregistrerRemboursement(int factureId, decimal montant, string motif, DateTime dateRemboursement, int? employeId = null)
        {
            // VALIDATION: Vérifier que la facture est payée
            var facture = FactureService.GetFactureById(factureId);
            if (facture == null)
                throw new Exception("Facture introuvable");
            
            if (facture.StatutPaiement != "Payée")
                throw new Exception("Seules les factures payées peuvent être remboursées");
            
            // Calculer le montant total payé
            // Utiliser d'abord le champ montant_paye de la facture (plus fiable)
            decimal montantTotalPaye = facture.MontantPaye;
            
            // Si montant_paye est 0, calculer depuis MontantTotal - MontantDu
            if (montantTotalPaye == 0)
            {
                montantTotalPaye = facture.MontantTotal - facture.MontantDu;
            }
            
            // Si toujours 0 et que la facture est payée, utiliser MontantTotal
            if (montantTotalPaye == 0 && facture.StatutPaiement == "Payée")
            {
                montantTotalPaye = facture.MontantTotal;
            }
            
            // Calculer le montant déjà remboursé depuis les paiements négatifs
            var paiements = GetPaiementsByFactureId(factureId);
            decimal montantDejaRembourse = Math.Abs(paiements.Where(p => p.Montant < 0).Sum(p => p.Montant));
            
            // Montant maximum remboursable = montant payé - montant déjà remboursé
            decimal montantMaxRemboursable = montantTotalPaye - montantDejaRembourse;
            
            if (montant > montantMaxRemboursable)
                throw new Exception($"Le montant du remboursement ({montant:C}) ne peut pas être supérieur au montant remboursable ({montantMaxRemboursable:C}). Montant payé: {montantTotalPaye:C}, Déjà remboursé: {montantDejaRembourse:C}");
            
            if (montant <= 0)
                throw new Exception("Le montant du remboursement doit être supérieur à zéro");

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // 1. Mettre à jour directement la facture (pas besoin d'enregistrer dans paiements si la contrainte empêche les montants négatifs)
                            // Le remboursement augmente le montant dû et diminue le montant payé
                            decimal nouveauMontantDu = facture.MontantDu + montant; // Le montant dû augmente
                            decimal nouveauMontantPaye = facture.MontantPaye - montant; // Le montant payé diminue
                            
                            // Déterminer le nouveau statut
                            string nouveauStatut;
                            if (nouveauMontantDu >= facture.MontantTotal)
                            {
                                nouveauStatut = "Impayée";
                            }
                            else if (nouveauMontantDu > 0)
                            {
                                nouveauStatut = "Partielle";
                            }
                            else
                            {
                                nouveauStatut = "Payée"; // Ne devrait pas arriver si on rembourse une facture payée
                            }
                            
                            // S'assurer que les montants ne deviennent pas négatifs
                            if (nouveauMontantPaye < 0) nouveauMontantPaye = 0;
                            
                            string updateFacture = @"
                                UPDATE factures 
                                SET montant_du = @montantDu, 
                                    montant_paye = @montantPaye,
                                    statut_paiement = @statut,
                                    note_interne = CONCAT(IFNULL(note_interne, ''), '\n[Remboursement ', @date, '] ', @motif, ' - Montant: ', @montant)
                                WHERE id = @factureId";
                            
                            using (var cmd = new MySqlCommand(updateFacture, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@montantDu", nouveauMontantDu);
                                cmd.Parameters.AddWithValue("@montantPaye", nouveauMontantPaye);
                                cmd.Parameters.AddWithValue("@statut", nouveauStatut);
                                cmd.Parameters.AddWithValue("@motif", motif);
                                cmd.Parameters.AddWithValue("@montant", montant);
                                cmd.Parameters.AddWithValue("@date", dateRemboursement.ToString("yyyy-MM-dd"));
                                cmd.Parameters.AddWithValue("@factureId", factureId);
                                cmd.ExecuteNonQuery();
                            }
                            
                            // 2. Optionnel: Enregistrer le remboursement dans une table séparée ou dans la note
                            // Pour l'instant, on enregistre juste dans la note de la facture
                            int remboursementId = 0; // Pas d'ID de paiement car on n'insère pas dans paiements
                            
                            transaction.Commit();
                            return remboursementId;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'enregistrement du remboursement: {ex.Message}", ex);
            }
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
                ModePaiement = "Comptant", // Valeur par défaut car la colonne mode_paiement n'existe pas dans la table
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

