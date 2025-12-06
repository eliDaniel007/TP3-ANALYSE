using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class JournalComptableService
    {
        /// <summary>
        /// Obtenir toutes les écritures comptables pour une période donnée depuis la table journal_comptable
        /// </summary>
        public static List<JournalEntryDisplay> GetJournalEntries(DateTime dateDebut, DateTime dateFin)
        {
            var entries = new List<JournalEntryDisplay>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            id,
                            date_ecriture,
                            description,
                            compte,
                            debit,
                            credit,
                            reference,
                            type_transaction
                        FROM journal_comptable
                        WHERE date_ecriture BETWEEN @dateDebut AND @dateFin
                        ORDER BY date_ecriture, id";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut.Date);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin.Date);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                entries.Add(new JournalEntryDisplay
                                {
                                    Date = reader.GetDateTime("date_ecriture").ToString("yyyy-MM-dd"),
                                    Transaction = reader.GetString("description"),
                                    Compte = reader.GetString("compte"),
                                    Debit = reader.GetDecimal("debit") > 0 ? reader.GetDecimal("debit").ToString("C") : "",
                                    Credit = reader.GetDecimal("credit") > 0 ? reader.GetDecimal("credit").ToString("C") : ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Si la table n'existe pas encore, retourner une liste vide
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la récupération du journal comptable: {ex.Message}");
                return new List<JournalEntryDisplay>();
            }

            return entries;
        }

        /// <summary>
        /// Calculer les totaux Débit et Crédit
        /// </summary>
        public static (decimal totalDebit, decimal totalCredit) CalculateTotals(List<JournalEntryDisplay> entries)
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            foreach (var entry in entries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Debit))
                {
                    if (decimal.TryParse(entry.Debit.Replace("$", "").Replace(",", "").Trim(), out decimal debit))
                    {
                        totalDebit += debit;
                    }
                }

                if (!string.IsNullOrWhiteSpace(entry.Credit))
                {
                    if (decimal.TryParse(entry.Credit.Replace("$", "").Replace(",", "").Trim(), out decimal credit))
                    {
                        totalCredit += credit;
                    }
                }
            }

            return (totalDebit, totalCredit);
        }
    }

    /// <summary>
    /// Classe d'affichage pour le journal comptable (utilisée dans la vue)
    /// </summary>
    public class JournalEntryDisplay
    {
        public string Date { get; set; } = string.Empty;
        public string Transaction { get; set; } = string.Empty;
        public string Compte { get; set; } = string.Empty;
        public string Debit { get; set; } = string.Empty;
        public string Credit { get; set; } = string.Empty;
    }
}

