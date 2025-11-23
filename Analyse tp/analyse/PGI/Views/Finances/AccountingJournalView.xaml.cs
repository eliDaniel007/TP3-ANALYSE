using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class AccountingJournalView : UserControl
    {
        private DateTime? dateDebut;
        private DateTime? dateFin;

        public AccountingJournalView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var entries = new List<JournalEntry>();
                
                // Charger les factures
                var factures = FactureService.GetAllFactures();
                
                // Filtrer par date si nécessaire
                if (dateDebut.HasValue)
                {
                    factures = factures.Where(f => f.DateFacture >= dateDebut.Value).ToList();
                }
                if (dateFin.HasValue)
                {
                    factures = factures.Where(f => f.DateFacture <= dateFin.Value).ToList();
                }
                
                // Générer des écritures pour chaque facture
                foreach (var facture in factures.OrderByDescending(f => f.DateFacture))
                {
                    // Écriture 1: Crédit Ventes
                    entries.Add(new JournalEntry
                    {
                        Date = facture.DateFacture.ToString("yyyy-MM-dd"),
                        Transaction = $"Vente #{facture.NumeroFacture}",
                        Compte = "Ventes - Produits",
                        Debit = "",
                        Credit = $"{facture.SousTotal:N2} $"
                    });
                    
                    // Écriture 2: Débit Comptes clients (ou Caisse si payé)
                    string compteDebit = facture.StatutPaiement == "Payée" ? "Caisse" : "Comptes clients";
                    entries.Add(new JournalEntry
                    {
                        Date = facture.DateFacture.ToString("yyyy-MM-dd"),
                        Transaction = $"Vente #{facture.NumeroFacture}",
                        Compte = compteDebit,
                        Debit = $"{facture.MontantTotal:N2} $",
                        Credit = ""
                    });
                    
                    // Écriture 3: TPS perçue
                    if (facture.MontantTPS > 0)
                    {
                        entries.Add(new JournalEntry
                        {
                            Date = facture.DateFacture.ToString("yyyy-MM-dd"),
                            Transaction = $"Vente #{facture.NumeroFacture}",
                            Compte = "TPS à payer",
                            Debit = "",
                            Credit = $"{facture.MontantTPS:N2} $"
                        });
                    }
                    
                    // Écriture 4: TVQ perçue
                    if (facture.MontantTVQ > 0)
                    {
                        entries.Add(new JournalEntry
                        {
                            Date = facture.DateFacture.ToString("yyyy-MM-dd"),
                            Transaction = $"Vente #{facture.NumeroFacture}",
                            Compte = "TVQ à payer",
                            Debit = "",
                            Credit = $"{facture.MontantTVQ:N2} $"
                        });
                    }
                }
                
                // Charger les commandes fournisseurs
                var commandes = CommandeFournisseurService.GetAllCommandes();
                
                if (dateDebut.HasValue)
                {
                    commandes = commandes.Where(c => c.DateCommande >= dateDebut.Value).ToList();
                }
                if (dateFin.HasValue)
                {
                    commandes = commandes.Where(c => c.DateCommande <= dateFin.Value).ToList();
                }
                
                // Générer des écritures pour chaque commande fournisseur
                foreach (var commande in commandes.Where(c => c.Statut == "Reçue").OrderByDescending(c => c.DateReception))
                {
                    // Écriture 1: Débit Inventaire
                    entries.Add(new JournalEntry
                    {
                        Date = commande.DateReception?.ToString("yyyy-MM-dd") ?? commande.DateCommande.ToString("yyyy-MM-dd"),
                        Transaction = $"Achat #{commande.NumeroCommande}",
                        Compte = "Inventaire",
                        Debit = $"{commande.SousTotal:N2} $",
                        Credit = ""
                    });
                    
                    // Écriture 2: Crédit Comptes fournisseurs
                    entries.Add(new JournalEntry
                    {
                        Date = commande.DateReception?.ToString("yyyy-MM-dd") ?? commande.DateCommande.ToString("yyyy-MM-dd"),
                        Transaction = $"Achat #{commande.NumeroCommande}",
                        Compte = "Comptes fournisseurs",
                        Debit = "",
                        Credit = $"{commande.MontantTotal:N2} $"
                    });
                    
                    // Écriture 3: Débit TPS à récupérer
                    if (commande.MontantTPS > 0)
                    {
                        entries.Add(new JournalEntry
                        {
                            Date = commande.DateReception?.ToString("yyyy-MM-dd") ?? commande.DateCommande.ToString("yyyy-MM-dd"),
                            Transaction = $"Achat #{commande.NumeroCommande}",
                            Compte = "TPS à récupérer",
                            Debit = $"{commande.MontantTPS:N2} $",
                            Credit = ""
                        });
                    }
                    
                    // Écriture 4: Débit TVQ à récupérer
                    if (commande.MontantTVQ > 0)
                    {
                        entries.Add(new JournalEntry
                        {
                            Date = commande.DateReception?.ToString("yyyy-MM-dd") ?? commande.DateCommande.ToString("yyyy-MM-dd"),
                            Transaction = $"Achat #{commande.NumeroCommande}",
                            Compte = "TVQ à récupérer",
                            Debit = $"{commande.MontantTVQ:N2} $",
                            Credit = ""
                        });
                    }
                }
                
                // Trier par date
                entries = entries.OrderByDescending(e => e.Date).ToList();
                
                JournalDataGrid.ItemsSource = entries;
                
                // Calculer les totaux
                double totalDebit = 0;
                double totalCredit = 0;
                
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.Debit))
                    {
                        string debitStr = entry.Debit.Replace(" $", "").Replace(",", "").Replace(" ", "");
                        if (double.TryParse(debitStr, out double debit))
                        {
                            totalDebit += debit;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(entry.Credit))
                    {
                        string creditStr = entry.Credit.Replace(" $", "").Replace(",", "").Replace(" ", "");
                        if (double.TryParse(creditStr, out double credit))
                        {
                            totalCredit += credit;
                        }
                    }
                }
                
                TxtTotalDebit.Text = $"{totalDebit:N2} $";
                TxtTotalCredit.Text = $"{totalCredit:N2} $";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement du journal comptable:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                
                JournalDataGrid.ItemsSource = new List<JournalEntry>();
                TxtTotalDebit.Text = "0,00 $";
                TxtTotalCredit.Text = "0,00 $";
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Filtrage par période à implémenter avec la base de données.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }

    public class JournalEntry
    {
        public string Date { get; set; } = string.Empty;
        public string Transaction { get; set; } = string.Empty;
        public string Compte { get; set; } = string.Empty;
        public string Debit { get; set; } = string.Empty;
        public string Credit { get; set; } = string.Empty;
    }
}

