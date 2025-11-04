using System.Windows;

namespace PGI.Views.Finances
{
    public partial class TaxSettingsWindow : Window
    {
        public static double TPSRate { get; set; } = 5.0;
        public static double TVQRate { get; set; } = 9.975;

        public TaxSettingsWindow()
        {
            InitializeComponent();
            
            // Charger les valeurs actuelles
            TxtTPS.Text = TPSRate.ToString("0.000");
            TxtTVQ.Text = TVQRate.ToString("0.000");
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

            // Sauvegarder les nouveaux taux
            TPSRate = tpsValue;
            TVQRate = tvqValue;

            // TODO: Enregistrer dans la base de données

            MessageBox.Show(
                $"✅ Paramètres fiscaux mis à jour !\n\n" +
                $"TPS : {TPSRate}%\n" +
                $"TVQ : {TVQRate}%\n\n" +
                $"Ces taux seront appliqués aux prochaines factures.",
                "Succès",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

