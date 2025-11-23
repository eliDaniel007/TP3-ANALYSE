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
            LoadTaxesFromDatabase();
        }

        private void LoadTaxesFromDatabase()
        {
            try
            {
                var taxes = TaxesService.GetTaxes();
                TxtTPS.Text = (taxes.TauxTPS * 100).ToString("0.000");
                TxtTVQ.Text = (taxes.TauxTVQ * 100).ToString("0.000");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des taux de taxes:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                // Valeurs par défaut en cas d'erreur
                TxtTPS.Text = "5.000";
                TxtTVQ.Text = "9.975";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!double.TryParse(TxtTPS.Text, out double tpsValue) || tpsValue < 0 || tpsValue > 100)
            {
                MessageBox.Show(
                    "Veuillez entrer un taux TPS valide (0-100%).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (!double.TryParse(TxtTVQ.Text, out double tvqValue) || tvqValue < 0 || tvqValue > 100)
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
                // Sauvegarder dans la base de données (convertir % en décimal)
                TaxesService.UpdateTaxRate("TPS", (decimal)tpsValue / 100m);
                TaxesService.UpdateTaxRate("TVQ", (decimal)tvqValue / 100m);

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
                MessageBox.Show(
                    $"Erreur lors de la sauvegarde des taux de taxes:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

