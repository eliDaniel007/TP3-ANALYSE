using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class FinancesDashboardView : UserControl
    {
        public FinancesDashboardView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Données d'exemple pour les factures impayées
            var unpaidInvoices = new List<UnpaidInvoice>
            {
                new UnpaidInvoice { NumeroFacture = "F2025-0987", Client = "Aventure Nordique Inc.", MontantDu = "4 250 $", JoursRetard = "45" },
                new UnpaidInvoice { NumeroFacture = "F2025-0912", Client = "Plein Air Québec", MontantDu = "3 890 $", JoursRetard = "38" },
                new UnpaidInvoice { NumeroFacture = "F2025-0856", Client = "Sports Extrêmes", MontantDu = "6 120 $", JoursRetard = "52" },
            };

            // TODO: Assigner au DataGrid (nécessite un x:Name sur le DataGrid)
        }

        private void BtnViewUnpaidInvoices_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la liste des ventes avec filtre "En retard"
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.BtnSales.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }

        private void BtnNewSale_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire de nouvelle vente
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToSaleForm();
            }
        }

        private void BtnNewPurchase_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire de nouvelle commande fournisseur
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToPurchaseForm();
            }
        }

        private void BtnRecordPayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Fonctionnalité 'Enregistrer un paiement' à implémenter.\n\n" +
                "Cette fonction ouvrira un pop-up pour enregistrer un paiement sur une facture existante.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnTaxSettings_Click(object sender, RoutedEventArgs e)
        {
            var taxSettingsWindow = new TaxSettingsWindow();
            taxSettingsWindow.ShowDialog();
        }

        private FinancesMainView? FindParentFinancesMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is FinancesMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as FinancesMainView;
        }
    }

    // Classe modèle pour les factures impayées
    public class UnpaidInvoice
    {
        public string NumeroFacture { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string MontantDu { get; set; } = string.Empty;
        public string JoursRetard { get; set; } = string.Empty;
    }
}

