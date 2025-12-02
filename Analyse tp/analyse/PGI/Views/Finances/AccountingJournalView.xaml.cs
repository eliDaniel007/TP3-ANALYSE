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
            var entries = new List<JournalEntry>
            {
                new JournalEntry { Date = "2025-01-28", Transaction = "Vente #F2025-1045", Compte = "Ventes - Produits", Debit = "", Credit = "4 250,00 $" },
                new JournalEntry { Date = "2025-01-28", Transaction = "Vente #F2025-1045", Compte = "Comptes clients", Debit = "4 250,00 $", Credit = "" },
                new JournalEntry { Date = "2025-01-25", Transaction = "Achat #A2025-0105", Compte = "Inventaire", Debit = "15 450,00 $", Credit = "" },
                new JournalEntry { Date = "2025-01-25", Transaction = "Achat #A2025-0105", Compte = "Comptes fournisseurs", Debit = "", Credit = "15 450,00 $" },
                new JournalEntry { Date = "2025-01-20", Transaction = "Paie Janvier 2025", Compte = "Salaires", Debit = "45 000,00 $", Credit = "" },
                new JournalEntry { Date = "2025-01-20", Transaction = "Paie Janvier 2025", Compte = "Banque", Debit = "", Credit = "45 000,00 $" },
            };

            JournalDataGrid.ItemsSource = entries;
            
            // Calculer les totaux
            double totalDebit = 64950.00;
            double totalCredit = 64950.00;
            
            TxtTotalDebit.Text = $"{totalDebit:N2} $";
            TxtTotalCredit.Text = $"{totalCredit:N2} $";
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

