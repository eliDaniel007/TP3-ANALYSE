using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class AccountingJournalView : UserControl
    {
        public AccountingJournalView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            try
            {
                var transactions = PGI.Services.DashboardService.GetDernieresTransactions();
                var entries = new List<JournalEntry>();
                double totalDebit = 0;
                double totalCredit = 0;

                foreach (System.Data.DataRow row in transactions.Rows)
                {
                    string type = row["type"].ToString();
                    string reference = row["reference"].ToString();
                    string tiers = row["tiers"].ToString();
                    string date = Convert.ToDateTime(row["date_transaction"]).ToString("yyyy-MM-dd");
                    decimal montant = Convert.ToDecimal(row["montant"]);
                    
                    // Écriture 1 (Débit)
                    var entry1 = new JournalEntry
                    {
                        Date = date,
                        Transaction = $"{type} {reference} ({tiers})",
                        Compte = type == "Vente" ? "Banque / Clients" : "Achats / Stock",
                        Debit = type == "Vente" ? "" : montant.ToString("C"), // Achat = Débit Stock
                        Credit = type == "Vente" ? "" : "" // Attente ligne 2
                    };
                    // Ajustement logique Vente = Crédit Ventes, Débit Banque
                    if (type == "Vente")
                    {
                         entry1.Debit = montant.ToString("C");
                         totalDebit += (double)montant;
                    }
                    else
                    {
                         entry1.Debit = montant.ToString("C");
                         totalDebit += (double)montant;
                    }
                    entries.Add(entry1);

                    // Écriture 2 (Crédit)
                    var entry2 = new JournalEntry
                    {
                        Date = "",
                        Transaction = "",
                        Compte = type == "Vente" ? "Ventes marchandises" : "Banque / Fournisseurs",
                        Debit = "",
                        Credit = montant.ToString("C")
                    };
                    totalCredit += (double)montant;
                    entries.Add(entry2);
                }

                JournalDataGrid.ItemsSource = entries;
                
                TxtTotalDebit.Text = $"{totalDebit:C}";
                TxtTotalCredit.Text = $"{totalCredit:C}";
            }
            catch (System.Exception ex)
            {
                 MessageBox.Show($"Erreur lors du chargement du journal : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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

