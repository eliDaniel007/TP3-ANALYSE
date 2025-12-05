using System;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class FinancesDashboardView : UserControl
    {
        public FinancesDashboardView()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void BtnRefresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        private void BtnRapportTaxes_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Naviguer vers la vue Rapports
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToReports();
            }
            else
            {
                System.Windows.MessageBox.Show("Impossible de naviguer vers les rapports.", "Erreur", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private void BtnBilanFinancier_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Naviguer vers la vue Rapports (qui contient le bilan financier)
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToReports();
            }
            else
            {
                System.Windows.MessageBox.Show("Impossible de naviguer vers les rapports.", "Erreur", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        private FinancesMainView? FindParentFinancesMainView(System.Windows.DependencyObject child)
        {
            System.Windows.DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is FinancesMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as FinancesMainView;
        }

        private void LoadDashboardData()
        {
            try
            {
                DateTime fin = DateTime.Now;
                DateTime debut = fin.AddDays(-30);

                // A. Indicateurs Financiers (KPIs) - 30 derniers jours
                decimal ventes = DashboardService.GetVentesTotales(debut, fin);
                decimal depenses = DashboardService.GetDepensesExploitation(debut, fin);
                decimal coutVentes = DashboardService.GetCoutMarchandisesVendues(debut, fin);
                
                decimal margeBrute = ventes - coutVentes;
                decimal profitNet = margeBrute - depenses;

                TxtVentes.Text = $"{ventes:C}";
                TxtDepenses.Text = $"{depenses:C}";
                TxtMarge.Text = $"{margeBrute:C}";
                TxtProfitNet.Text = $"{profitNet:C}";

                // B. Suivi de Trésorerie (Listes) - Toutes les données
                var facturesAttente = DashboardService.GetFacturesEnAttente();
                if (facturesAttente != null && facturesAttente.Rows.Count > 0)
                {
                    GridFacturesAttente.ItemsSource = facturesAttente.DefaultView;
                }
                else
                {
                    GridFacturesAttente.ItemsSource = null;
                }

                var transactions = DashboardService.GetDernieresTransactions();
                if (transactions != null && transactions.Rows.Count > 0)
                {
                    GridTransactions.ItemsSource = transactions.DefaultView;
                }
                else
                {
                    GridTransactions.ItemsSource = null;
                }

                // C. Rapports (Taxes) - Toutes les données
                decimal taxes = DashboardService.GetTaxesAPayer();
                TxtTaxes.Text = $"{taxes:C}";
            }
            catch (Exception ex)
            {
                // Afficher l'erreur pour le débogage
                System.Windows.MessageBox.Show($"Erreur lors du chargement du tableau de bord : {ex.Message}\n\nDétails : {ex.StackTrace}", 
                    "Erreur Dashboard", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
