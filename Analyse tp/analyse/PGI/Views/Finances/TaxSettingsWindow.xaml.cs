using System;
using System.Windows;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class TaxSettingsWindow : Window
    {
        public TaxSettingsWindow()
        {
            InitializeComponent();
            LoadTaxRates();
        }

        private void LoadTaxRates()
        {
            try
            {
                var tauxTPS = TaxesService.GetTauxTPS();
                var tauxTVQ = TaxesService.GetTauxTVQ();
                
                // Afficher en pourcentage (ex: 0.05 -> 5.0)
                TxtTPS.Text = (tauxTPS * 100).ToString("0.000");
                TxtTVQ.Text = (tauxTVQ * 100).ToString("0.000");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des taux : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!decimal.TryParse(TxtTPS.Text, out decimal tpsValue) || tpsValue < 0 || tpsValue > 100)
            {
                MessageBox.Show(
                    "Veuillez entrer un taux TPS valide (0-100%).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (!decimal.TryParse(TxtTVQ.Text, out decimal tvqValue) || tvqValue < 0 || tvqValue > 100)
            {
                MessageBox.Show(
                    "Veuillez entrer un taux TVQ valide (0-100%).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                // Convertir en décimal (ex: 5.0% -> 0.05)
                decimal tauxTPS = tpsValue / 100m;
                decimal tauxTVQ = tvqValue / 100m;

                // Sauvegarder dans la base de données
                TaxesService.UpdateTaxRate("TPS", tauxTPS);
                TaxesService.UpdateTaxRate("TVQ", tauxTVQ);

                MessageBox.Show(
                    $"✅ Paramètres fiscaux mis à jour !\n\n" +
                    $"TPS : {tpsValue}%\n" +
                    $"TVQ : {tvqValue}%\n\n" +
                    $"Ces taux seront appliqués aux prochaines factures.",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

