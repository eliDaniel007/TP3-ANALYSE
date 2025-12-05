using System;
using System.Windows;

namespace PGI.Views.Finances
{
    public partial class RefundWindow : Window
    {
        public double MontantRemboursement { get; private set; }
        public string Motif { get; private set; } = string.Empty;
        public string DateRemboursement { get; private set; } = string.Empty;
        private int _factureId;
        private decimal _montantTotalPaye;
        private decimal _montantTotalFacture;
        private decimal _montantDejaRembourse;

        public RefundWindow(string numeroFacture, string client, decimal montantTotalFacture, decimal montantTotalPaye, decimal montantDejaRembourse, int factureId)
        {
            InitializeComponent();
            
            _factureId = factureId;
            _montantTotalPaye = montantTotalPaye;
            _montantTotalFacture = montantTotalFacture;
            _montantDejaRembourse = montantDejaRembourse;
            
            TxtNumeroFacture.Text = numeroFacture;
            TxtClient.Text = client;
            TxtMontantTotal.Text = $"{montantTotalFacture:N2} $";
            TxtMontantPaye.Text = $"{montantTotalPaye:N2} $";
            
            // Afficher le montant déjà remboursé si > 0
            if (montantDejaRembourse > 0)
            {
                BorderMontantRembourse.Visibility = Visibility.Visible;
                TxtMontantRembourse.Text = $"{montantDejaRembourse:N2} $";
            }
            
            // Montant maximum remboursable = montant payé - montant déjà remboursé
            // On peut rembourser jusqu'à la totalité du montant payé
            decimal montantMaxRemboursable = montantTotalPaye - montantDejaRembourse;
            
            // S'assurer que le montant maximum est au moins égal au montant total payé si aucun remboursement n'a été fait
            if (montantDejaRembourse == 0 && montantTotalPaye > 0)
            {
                montantMaxRemboursable = montantTotalPaye;
            }
            
            // Initialiser avec le montant maximum remboursable (permet de rembourser la totalité)
            TxtMontantRemboursement.Text = montantMaxRemboursable > 0 ? montantMaxRemboursable.ToString("F2") : "0.00";
            TxtMontantMax.Text = $"(Maximum: {montantMaxRemboursable:N2} $)";
            DpDateRemboursement.SelectedDate = DateTime.Now;
        }

        private void TxtMontantRemboursement_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Validation visuelle en temps réel
            if (string.IsNullOrWhiteSpace(TxtMontantRemboursement.Text))
                return;

            string montantText = TxtMontantRemboursement.Text.Trim().Replace(" ", "").Replace("$", "");
            montantText = montantText.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            
            if (decimal.TryParse(montantText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out decimal montant))
            {
                decimal montantMaxRemboursable = _montantTotalPaye - _montantDejaRembourse;
                if (montant > montantMaxRemboursable)
                {
                    TxtMontantRemboursement.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626"));
                }
                else
                {
                    TxtMontantRemboursement.BorderBrush = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#E2E8F0"));
                }
            }
        }

        private void BtnRefund_Click(object sender, RoutedEventArgs e)
        {
            // Validation du montant (gérer les formats français et anglais)
            string montantText = TxtMontantRemboursement.Text.Trim().Replace(" ", "").Replace("$", "");
            if (string.IsNullOrWhiteSpace(montantText))
            {
                MessageBox.Show(
                    "Veuillez entrer un montant à rembourser.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Essayer de parser avec le format français (virgule) ou anglais (point)
            montantText = montantText.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!decimal.TryParse(montantText, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out decimal montant) || montant <= 0)
            {
                MessageBox.Show(
                    "Veuillez entrer un montant valide (ex: 100,50 ou 100.50).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Calculer le montant maximum remboursable
            decimal montantMaxRemboursable = _montantTotalPaye - _montantDejaRembourse;
            
            // S'assurer qu'on peut rembourser la totalité si aucun remboursement n'a été fait
            if (_montantDejaRembourse == 0 && _montantTotalPaye > 0)
            {
                montantMaxRemboursable = _montantTotalPaye;
            }
            
            if (montant > montantMaxRemboursable)
            {
                MessageBox.Show(
                    $"Le montant à rembourser ({montant:N2} $) ne peut pas être supérieur au montant remboursable ({montantMaxRemboursable:N2} $).\n\n" +
                    $"Montant payé : {_montantTotalPaye:N2} $\n" +
                    $"Déjà remboursé : {_montantDejaRembourse:N2} $\n" +
                    $"Montant remboursable restant : {montantMaxRemboursable:N2} $",
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

            // Vérifier que la date est sélectionnée
            if (!DpDateRemboursement.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Veuillez sélectionner une date de remboursement.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            DateTime dateRemboursement = DpDateRemboursement.SelectedDate.Value;

            // Confirmation
            var result = MessageBox.Show(
                $"⚠️ Confirmer le remboursement ?\n\n" +
                $"Facture : {TxtNumeroFacture.Text}\n" +
                $"Client : {TxtClient.Text}\n" +
                $"Montant à rembourser : {montant:N2} $\n" +
                $"Motif : {TxtMotif.Text}\n" +
                $"Date : {dateRemboursement:dd/MM/yyyy}\n\n" +
                $"Cette action créera une écriture comptable de remboursement et mettra à jour le statut de la facture.",
                "Confirmer le remboursement",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    // Enregistrer le remboursement dans la base de données
                    PGI.Services.PaiementService.EnregistrerRemboursement(
                        _factureId,
                        montant,
                        TxtMotif.Text,
                        dateRemboursement,
                        null // TODO: Récupérer l'ID de l'employé connecté
                    );

                    // Sauvegarder les informations pour l'affichage
                    MontantRemboursement = (double)montant;
                    Motif = TxtMotif.Text;
                    DateRemboursement = dateRemboursement.ToString("yyyy-MM-dd");

                    this.DialogResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors de l'enregistrement du remboursement : {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

