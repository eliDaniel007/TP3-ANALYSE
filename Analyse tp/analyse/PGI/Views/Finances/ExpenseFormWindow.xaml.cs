using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class ExpenseFormWindow : Window
    {
        private int? _depenseId; // null = nouvelle dépense, sinon = édition

        public ExpenseFormWindow(int? depenseId = null)
        {
            InitializeComponent();
            _depenseId = depenseId;
            
            if (depenseId.HasValue)
            {
                TxtTitle.Text = "✏️ Modifier la Dépense";
                BtnSave.Content = "Modifier";
                LoadDepenseExistante();
            }
            
            DpDateDepense.SelectedDate = DateTime.Now;
        }

        private void LoadDepenseExistante()
        {
            try
            {
                if (_depenseId.HasValue)
                {
                    var depense = DepenseService.GetDepenseById(_depenseId.Value);
                    if (depense == null)
                    {
                        MessageBox.Show("Dépense introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                        return;
                    }

                    // Vérifier que c'est une dépense générale (pas une commande fournisseur)
                    if (depense.Source != "Generale")
                    {
                        MessageBox.Show(
                            "Cette dépense provient d'une commande fournisseur et ne peut pas être modifiée ici.",
                            "Modification impossible",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        this.Close();
                        return;
                    }

                    TxtDescription.Text = depense.Description;
                    
                    // Sélectionner la catégorie
                    foreach (ComboBoxItem item in CmbCategorie.Items)
                    {
                        if (item.Content.ToString() == depense.Categorie)
                        {
                            CmbCategorie.SelectedItem = item;
                            break;
                        }
                    }

                    TxtMontant.Text = depense.Montant.ToString("F2");
                    DpDateDepense.SelectedDate = depense.DateDepense;

                    // Sélectionner le statut
                    foreach (ComboBoxItem item in CmbStatutPaiement.Items)
                    {
                        if (item.Content.ToString() == depense.StatutPaiement)
                        {
                            CmbStatutPaiement.SelectedItem = item;
                            break;
                        }
                    }

                    // Sélectionner le mode de paiement
                    if (!string.IsNullOrEmpty(depense.ModePaiement))
                    {
                        foreach (ComboBoxItem item in CmbModePaiement.Items)
                        {
                            if (item.Content.ToString() == depense.ModePaiement)
                            {
                                CmbModePaiement.SelectedItem = item;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la dépense : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtDescription.Text))
            {
                MessageBox.Show("Veuillez entrer une description.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDescription.Focus();
                return;
            }

            if (CmbCategorie.SelectedItem == null)
            {
                MessageBox.Show("Veuillez sélectionner une catégorie.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtMontant.Text) || !decimal.TryParse(TxtMontant.Text, out decimal montant) || montant <= 0)
            {
                MessageBox.Show("Veuillez entrer un montant valide (nombre positif).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtMontant.Focus();
                return;
            }

            if (!DpDateDepense.SelectedDate.HasValue)
            {
                MessageBox.Show("Veuillez sélectionner une date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var depense = new Depense
                {
                    Description = TxtDescription.Text.Trim(),
                    Categorie = (CmbCategorie.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty,
                    Montant = montant,
                    DateDepense = DpDateDepense.SelectedDate.Value,
                    StatutPaiement = (CmbStatutPaiement.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "En attente",
                    ModePaiement = (CmbModePaiement.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                if (string.IsNullOrWhiteSpace(depense.ModePaiement))
                    depense.ModePaiement = null;

                if (_depenseId.HasValue)
                {
                    depense.Id = _depenseId.Value;
                    DepenseService.ModifierDepense(depense);
                    MessageBox.Show("✅ Dépense modifiée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    DepenseService.AjouterDepense(depense);
                    MessageBox.Show("✅ Dépense ajoutée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

