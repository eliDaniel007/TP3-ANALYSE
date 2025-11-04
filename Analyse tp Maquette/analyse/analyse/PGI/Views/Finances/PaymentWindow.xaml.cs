using System;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class PaymentWindow : Window
    {
        public double MontantPaye { get; private set; }
        public string MethodePaiement { get; private set; } = string.Empty;
        public string DatePaiement { get; private set; } = string.Empty;

        public PaymentWindow(string numeroFacture, string client, double montantTotal, double montantDu)
        {
            InitializeComponent();
            
            TxtNumeroFacture.Text = numeroFacture;
            TxtClient.Text = client;
            TxtMontantTotal.Text = $"{montantTotal:N2} $";
            TxtMontantDu.Text = $"{montantDu:N2} $";
            TxtMontantPaye.Text = montantDu.ToString("0.00");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!double.TryParse(TxtMontantPaye.Text, out double montant) || montant <= 0)
            {
                MessageBox.Show(
                    "Veuillez entrer un montant valide.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            double montantDu = double.Parse(TxtMontantDu.Text.Replace(" $", "").Replace(",", "."));
            if (montant > montantDu)
            {
                MessageBox.Show(
                    $"Le montant payé ({montant:N2} $) ne peut pas être supérieur au montant dû ({montantDu:N2} $).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Sauvegarder les informations
            MontantPaye = montant;
            MethodePaiement = ((ComboBoxItem)CmbMethode.SelectedItem).Content.ToString() ?? "Comptant";
            DatePaiement = TxtDatePaiement.Text;

            // TODO: Enregistrer dans la base de données

            MessageBox.Show(
                $"✅ Paiement enregistré avec succès !\n\n" +
                $"Montant : {MontantPaye:N2} $\n" +
                $"Méthode : {MethodePaiement}\n" +
                $"Date : {DatePaiement}",
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

