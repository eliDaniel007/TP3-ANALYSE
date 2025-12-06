using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    /// <summary>
    /// Service pour la comptabilité avec respect strict de la partie double
    /// </summary>
    public class AccountingService
    {
        /// <summary>
        /// Valider et enregistrer une transaction comptable
        /// Vérifie que Débit = Crédit avant d'enregistrer
        /// </summary>
        public static bool ValidateAndRecord(List<JournalEntry> entries)
        {
            if (entries == null || entries.Count == 0)
                throw new Exception("Aucune écriture à enregistrer");

            // Vérifier l'équilibre
            decimal totalDebit = entries.Sum(e => e.Debit);
            decimal totalCredit = entries.Sum(e => e.Credit);

            if (Math.Abs(totalDebit - totalCredit) > 0.01m) // Tolérance de 0.01$ pour arrondis
            {
                throw new Exception($"Transaction déséquilibrée : Débit ({totalDebit:C}) ≠ Crédit ({totalCredit:C})");
            }

            // Enregistrer toutes les écritures
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var entry in entries)
                            {
                                string query = @"
                                    INSERT INTO journal_comptable 
                                    (date_ecriture, description, compte, debit, credit, reference, type_transaction)
                                    VALUES 
                                    (@date, @description, @compte, @debit, @credit, @reference, @type)";

                                using (var cmd = new MySqlCommand(query, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@date", entry.Date);
                                    cmd.Parameters.AddWithValue("@description", entry.Description);
                                    cmd.Parameters.AddWithValue("@compte", entry.Compte);
                                    cmd.Parameters.AddWithValue("@debit", entry.Debit);
                                    cmd.Parameters.AddWithValue("@credit", entry.Credit);
                                    cmd.Parameters.AddWithValue("@reference", entry.Reference);
                                    cmd.Parameters.AddWithValue("@type", entry.TypeTransaction);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return true;
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
                throw new Exception($"Erreur lors de l'enregistrement comptable : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Enregistrer une vente
        /// Débiter "Comptes Clients" (Actif), Créditer "Ventes" (Revenu) + "Taxes à payer" (Passif)
        /// </summary>
        public static void EnregistrerVente(string numeroFacture, DateTime dateFacture, decimal montantHT, decimal montantTPS, decimal montantTVQ, decimal montantTotal, string nomClient)
        {
            var entries = new List<JournalEntry>();

            // 1. Débit : Comptes Clients (Actif - augmente)
            entries.Add(new JournalEntry
            {
                Date = dateFacture,
                Description = $"Vente {numeroFacture} - {nomClient}",
                Compte = "Comptes Clients",
                Debit = montantTotal,
                Credit = 0,
                Reference = numeroFacture,
                TypeTransaction = "Vente"
            });

            // 2. Crédit : Ventes (Revenu - augmente au crédit)
            entries.Add(new JournalEntry
            {
                Date = dateFacture,
                Description = $"Vente {numeroFacture} - {nomClient}",
                Compte = "Ventes",
                Debit = 0,
                Credit = montantHT,
                Reference = numeroFacture,
                TypeTransaction = "Vente"
            });

            // 3. Crédit : Taxes à payer TPS (Passif - augmente au crédit)
            if (montantTPS > 0)
            {
                entries.Add(new JournalEntry
                {
                    Date = dateFacture,
                    Description = $"TPS - {numeroFacture}",
                    Compte = "Taxes à payer - TPS",
                    Debit = 0,
                    Credit = montantTPS,
                    Reference = numeroFacture,
                    TypeTransaction = "Vente"
                });
            }

            // 4. Crédit : Taxes à payer TVQ (Passif - augmente au crédit)
            if (montantTVQ > 0)
            {
                entries.Add(new JournalEntry
                {
                    Date = dateFacture,
                    Description = $"TVQ - {numeroFacture}",
                    Compte = "Taxes à payer - TVQ",
                    Debit = 0,
                    Credit = montantTVQ,
                    Reference = numeroFacture,
                    TypeTransaction = "Vente"
                });
            }

            ValidateAndRecord(entries);
        }

        /// <summary>
        /// Enregistrer un achat de stock
        /// Débiter "Stock" (Actif), Créditer "Comptes Fournisseurs" (Passif)
        /// </summary>
        public static void EnregistrerAchatStock(string numeroCommande, DateTime dateCommande, decimal montantTotal, string nomFournisseur)
        {
            var entries = new List<JournalEntry>();

            // 1. Débit : Stock (Actif - augmente)
            entries.Add(new JournalEntry
            {
                Date = dateCommande,
                Description = $"Achat Stock {numeroCommande} - {nomFournisseur}",
                Compte = "Stock",
                Debit = montantTotal,
                Credit = 0,
                Reference = numeroCommande,
                TypeTransaction = "Achat"
            });

            // 2. Crédit : Comptes Fournisseurs (Passif - augmente au crédit)
            entries.Add(new JournalEntry
            {
                Date = dateCommande,
                Description = $"Achat Stock {numeroCommande} - {nomFournisseur}",
                Compte = "Comptes Fournisseurs",
                Debit = 0,
                Credit = montantTotal,
                Reference = numeroCommande,
                TypeTransaction = "Achat"
            });

            ValidateAndRecord(entries);
        }

        /// <summary>
        /// Payer un salaire
        /// Débiter "Salaires" (Dépense), Créditer "Banque" (Actif qui diminue)
        /// </summary>
        public static void PayerSalaire(string referencePaie, DateTime datePaiement, decimal montant, string nomEmploye)
        {
            var entries = new List<JournalEntry>();

            // 1. Débit : Salaires (Dépense - augmente au débit)
            entries.Add(new JournalEntry
            {
                Date = datePaiement,
                Description = $"Paie {referencePaie} - {nomEmploye}",
                Compte = "Salaires",
                Debit = montant,
                Credit = 0,
                Reference = referencePaie,
                TypeTransaction = "Salaire"
            });

            // 2. Crédit : Banque (Actif qui diminue - diminue au crédit)
            entries.Add(new JournalEntry
            {
                Date = datePaiement,
                Description = $"Paie {referencePaie} - {nomEmploye}",
                Compte = "Banque",
                Debit = 0,
                Credit = montant,
                Reference = referencePaie,
                TypeTransaction = "Salaire"
            });

            ValidateAndRecord(entries);
        }

        /// <summary>
        /// Enregistrer une dépense générale
        /// Débiter la catégorie de dépense, Créditer "Banque" (Actif qui diminue)
        /// </summary>
        public static void EnregistrerDepense(string description, DateTime dateDepense, decimal montant, string categorie)
        {
            var entries = new List<JournalEntry>();

            // 1. Débit : Dépense par catégorie (Dépense - augmente au débit)
            entries.Add(new JournalEntry
            {
                Date = dateDepense,
                Description = $"Dépense: {description}",
                Compte = $"Dépenses - {categorie}",
                Debit = montant,
                Credit = 0,
                Reference = description,
                TypeTransaction = "Dépense"
            });

            // 2. Crédit : Banque (Actif qui diminue - diminue au crédit)
            entries.Add(new JournalEntry
            {
                Date = dateDepense,
                Description = $"Dépense: {description}",
                Compte = "Banque",
                Debit = 0,
                Credit = montant,
                Reference = description,
                TypeTransaction = "Dépense"
            });

            ValidateAndRecord(entries);
        }
    }
}

