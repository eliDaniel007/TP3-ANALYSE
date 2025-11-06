using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
        }

        private void BtnGenerateTaxPDF_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Génération du rapport de taxes en PDF...\n\n" +
                $"Période : {TxtTaxDateDebut.Text} → {TxtTaxDateFin.Text}\n\n" +
                "Le fichier sera sauvegardé dans le dossier Documents/Rapports/",
                "Rapport de Taxes (PDF)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateTaxCSV_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Génération du rapport de taxes en CSV...\n\n" +
                $"Période : {TxtTaxDateDebut.Text} → {TxtTaxDateFin.Text}\n\n" +
                "Le fichier sera sauvegardé dans le dossier Documents/Rapports/",
                "Rapport de Taxes (CSV)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateSalesPDF_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Génération du rapport des ventes en PDF...\n\n" +
                $"Période : {TxtSalesDateDebut.Text} → {TxtSalesDateFin.Text}\n\n" +
                "Le fichier sera sauvegardé dans le dossier Documents/Rapports/",
                "Rapport des Ventes (PDF)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateSalesCSV_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Génération du rapport des ventes en CSV...\n\n" +
                $"Période : {TxtSalesDateDebut.Text} → {TxtSalesDateFin.Text}\n\n" +
                "Le fichier sera sauvegardé dans le dossier Documents/Rapports/",
                "Rapport des Ventes (CSV)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void CmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Recalculer les valeurs en fonction de la période sélectionnée
            // TODO: Implémenter avec données de la base
        }
    }
}

