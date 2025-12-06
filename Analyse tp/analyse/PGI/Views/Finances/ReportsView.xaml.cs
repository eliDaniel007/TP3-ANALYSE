using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class ReportsView : UserControl
    {
        public ReportsView()
        {
            InitializeComponent();
            // Initialiser avec le mois en cours
            TxtTaxDateDebut.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtTaxDateFin.SelectedDate = DateTime.Now;
            TxtSalesDateDebut.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            TxtSalesDateFin.SelectedDate = DateTime.Now;
        }

        private void BtnGenerateTaxPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dateDebut = TxtTaxDateDebut.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dateFin = TxtTaxDateFin.SelectedDate ?? DateTime.Now;

                var rapport = RapportService.GetRapportTaxes(dateDebut, dateFin);
                
                if (rapport == null || rapport.Rows.Count == 0)
                {
                    MessageBox.Show("Aucune donnée trouvée pour cette période.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string fileName = $"Rapport_Taxes_{dateDebut:yyyyMMdd}_{dateFin:yyyyMMdd}.txt";
                string? filePath = RapportService.GetSaveFilePath(fileName, "Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*");

                if (filePath != null)
                {
                    string title = $"RAPPORT DE TAXES (TPS/TVQ)\nPériode : {dateDebut:yyyy-MM-dd} à {dateFin:yyyy-MM-dd}";
                    RapportService.ExportToPDF(rapport, filePath, title);
                    MessageBox.Show($"✅ Rapport de taxes généré avec succès !\n\n{filePath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du rapport : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateTaxCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dateDebut = TxtTaxDateDebut.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dateFin = TxtTaxDateFin.SelectedDate ?? DateTime.Now;

                var rapport = RapportService.GetRapportTaxes(dateDebut, dateFin);
                
                if (rapport == null || rapport.Rows.Count == 0)
                {
                    MessageBox.Show("Aucune donnée trouvée pour cette période.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string fileName = $"Rapport_Taxes_{dateDebut:yyyyMMdd}_{dateFin:yyyyMMdd}.csv";
                string? filePath = RapportService.GetSaveFilePath(fileName, "Fichiers CSV (*.csv)|*.csv|Tous les fichiers (*.*)|*.*");

                if (filePath != null)
                {
                    string title = $"RAPPORT DE TAXES (TPS/TVQ) - Période : {dateDebut:yyyy-MM-dd} à {dateFin:yyyy-MM-dd}";
                    RapportService.ExportToCSV(rapport, filePath, title);
                    MessageBox.Show($"✅ Rapport de taxes généré avec succès !\n\n{filePath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du rapport : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateSalesPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dateDebut = TxtSalesDateDebut.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dateFin = TxtSalesDateFin.SelectedDate ?? DateTime.Now;

                var rapport = RapportService.GetRapportVentes(dateDebut, dateFin);
                
                if (rapport == null || rapport.Rows.Count == 0)
                {
                    MessageBox.Show("Aucune donnée trouvée pour cette période.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string fileName = $"Rapport_Ventes_{dateDebut:yyyyMMdd}_{dateFin:yyyyMMdd}.txt";
                string? filePath = RapportService.GetSaveFilePath(fileName, "Fichiers texte (*.txt)|*.txt|Tous les fichiers (*.*)|*.*");

                if (filePath != null)
                {
                    string title = $"RAPPORT DES VENTES\nPériode : {dateDebut:yyyy-MM-dd} à {dateFin:yyyy-MM-dd}";
                    RapportService.ExportToPDF(rapport, filePath, title);
                    MessageBox.Show($"✅ Rapport des ventes généré avec succès !\n\n{filePath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du rapport : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGenerateSalesCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime dateDebut = TxtSalesDateDebut.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dateFin = TxtSalesDateFin.SelectedDate ?? DateTime.Now;

                var rapport = RapportService.GetRapportVentes(dateDebut, dateFin);
                
                if (rapport == null || rapport.Rows.Count == 0)
                {
                    MessageBox.Show("Aucune donnée trouvée pour cette période.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string fileName = $"Rapport_Ventes_{dateDebut:yyyyMMdd}_{dateFin:yyyyMMdd}.csv";
                string? filePath = RapportService.GetSaveFilePath(fileName, "Fichiers CSV (*.csv)|*.csv|Tous les fichiers (*.*)|*.*");

                if (filePath != null)
                {
                    string title = $"RAPPORT DES VENTES - Période : {dateDebut:yyyy-MM-dd} à {dateFin:yyyy-MM-dd}";
                    RapportService.ExportToCSV(rapport, filePath, title);
                    MessageBox.Show($"✅ Rapport des ventes généré avec succès !\n\n{filePath}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du rapport : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnTaxSettings_Click(object sender, RoutedEventArgs e)
        {
            var taxSettingsWindow = new TaxSettingsWindow();
            taxSettingsWindow.ShowDialog();
        }

    }
}

