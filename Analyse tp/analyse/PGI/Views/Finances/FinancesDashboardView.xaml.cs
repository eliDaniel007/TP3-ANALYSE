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

        private void LoadDashboardData()
        {
            try
            {
                DateTime fin = DateTime.Now;
                DateTime debut = fin.AddDays(-30);

                // A. Indicateurs Financiers (KPIs)
                decimal ventes = DashboardService.GetVentesTotales(debut, fin);
                decimal depenses = DashboardService.GetDepensesExploitation(debut, fin);
                decimal coutVentes = DashboardService.GetCoutMarchandisesVendues(debut, fin);
                
                decimal margeBrute = ventes - coutVentes;
                decimal profitNet = margeBrute - depenses;

                TxtVentes.Text = $"{ventes:C}";
                TxtDepenses.Text = $"{depenses:C}";
                TxtMarge.Text = $"{margeBrute:C}";
                TxtProfitNet.Text = $"{profitNet:C}";

                // B. Suivi de Trésorerie (Listes)
                GridFacturesAttente.ItemsSource = DashboardService.GetFacturesEnAttente().DefaultView;
                GridTransactions.ItemsSource = DashboardService.GetDernieresTransactions().DefaultView;

                // C. Rapports (Taxes)
                decimal taxes = DashboardService.GetTaxesAPayer();
                TxtTaxes.Text = $"{taxes:C}";
            }
            catch (Exception ex)
            {
                // En production, logger l'erreur. Ici on ne bloque pas l'UI.
                Console.WriteLine($"Erreur Dashboard Finance: {ex.Message}");
            }
        }
    }
}
