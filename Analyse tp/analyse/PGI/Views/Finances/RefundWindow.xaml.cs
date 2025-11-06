using System;
using System.Windows;

namespace PGI.Views.Finances
{
    public partial class RefundWindow : Window
    {
        public double MontantRemboursement { get; private set; }
        public string Motif { get; private set; } = string.Empty;
        public string DateRemboursement { get; private set; } = string.Empty;

        public RefundWindow(string numeroFacture, string client, double montantPaye)
        {
            InitializeComponent();
            
            TxtNumeroFacture.Text = numeroFacture;
            TxtClient.Text = client;
            TxtMontantPaye.Text = $"{montantPaye:N2} $";
            TxtMontantRemboursement.Text = montantPaye.ToString("0.00");
        }

        private void BtnRefund_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!double.TryParse(TxtMontantRemboursement.Text, out double montant) || montant <= 0)
            {
                MessageBox.Show(
                    "Veuillez entrer un montant valide.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            double montantPaye = double.Parse(TxtMontantPaye.Text.Replace(" $", "").Replace(",", "."));
            if (montant > montantPaye)
            {
                MessageBox.Show(
                    $"Le montant à rembourser ({montant:N2} $) ne peut pas être supérieur au montant payé ({montantPaye:N2} $).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtMotif.Text))
            {
                MessageBox.Show(
                    "Veuillez indiquer un motif pour le remboursement.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Confirmation
            var result = MessageBox.Show(
                $"⚠️ Confirmer le remboursement ?\n\n" +
                $"Montant : {montant:N2} $\n" +
                $"Motif : {TxtMotif.Text}\n\n" +
                $"Cette action créera une écriture comptable de remboursement.",
                "Confirmer le remboursement",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // Sauvegarder les informations
                MontantRemboursement = montant;
                Motif = TxtMotif.Text;
                DateRemboursement = TxtDateRemboursement.Text;

                // TODO: Enregistrer dans la base de données
                // TODO: Créer l'écriture comptable de remboursement

                MessageBox.Show(
                    $"✅ Remboursement effectué avec succès !\n\n" +
                    $"Montant : {MontantRemboursement:N2} $\n" +
                    $"Date : {DateRemboursement}",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                this.DialogResult = true;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

