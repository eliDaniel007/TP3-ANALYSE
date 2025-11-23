using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class PaymentWindow : Window
    {
        private int factureId;
        public double MontantPaye { get; private set; }
        public string MethodePaiement { get; private set; } = string.Empty;
        public string DatePaiement { get; private set; } = string.Empty;

        public PaymentWindow(int factureId, string numeroFacture, string client, double montantTotal, double montantDu)
        {
            InitializeComponent();
            
            this.factureId = factureId;
            
            TxtNumeroFacture.Text = numeroFacture;
            TxtClient.Text = client;
            TxtMontantTotal.Text = $"{montantTotal:N2} $";
            TxtMontantDu.Text = $"{montantDu:N2} $";
            TxtMontantPaye.Text = montantDu.ToString("0.00");
            
            // Définir la date par défaut à aujourd'hui
            TxtDatePaiement.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation du montant
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

            string montantDuStr = TxtMontantDu.Text.Replace(" $", "").Replace(",", "").Replace(" ", "");
            if (!double.TryParse(montantDuStr, out double montantDu))
            {
                MessageBox.Show("Erreur lors de la lecture du montant dû.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

            // Validation de la date
            if (!DateTime.TryParse(TxtDatePaiement.Text, out DateTime datePaiement))
            {
                MessageBox.Show(
                    "Veuillez entrer une date valide (format: AAAA-MM-JJ).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            try
            {
                // Sauvegarder les informations
                MontantPaye = montant;
                MethodePaiement = ((ComboBoxItem)CmbMethode.SelectedItem).Content.ToString() ?? "Comptant";
                DatePaiement = TxtDatePaiement.Text;

                // Créer le paiement
                var paiement = new Paiement
                {
                    FactureId = this.factureId,
                    Montant = (decimal)montant,
                    ModePaiement = MethodePaiement,
                    DatePaiement = datePaiement,
                    NumeroReference = null, // TODO: Ajouter TxtNumeroReference dans le XAML si nécessaire
                    Note = null, // TODO: Ajouter TxtNote dans le XAML si nécessaire
                    EmployeId = null // TODO: Récupérer l'employé connecté
                };

                // Enregistrer dans la base de données
                int paiementId = PaiementService.EnregistrerPaiement(paiement);

                MessageBox.Show(
                    $"✅ Paiement enregistré avec succès !\n\n" +
                    $"Montant : {MontantPaye:N2} $\n" +
                    $"Méthode : {MethodePaiement}\n" +
                    $"Date : {DatePaiement}\n\n" +
                    $"Le statut de la facture a été mis à jour automatiquement.",
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
                    $"Erreur lors de l'enregistrement du paiement:\n{ex.Message}",
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

