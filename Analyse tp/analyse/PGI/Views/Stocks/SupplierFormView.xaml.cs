using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class SupplierFormView : UserControl
    {
        private int? fournisseurId;
        private bool isEditMode;

        // Constructeur pour mode ajout
        public SupplierFormView()
        {
            InitializeComponent();
            isEditMode = false;
            TxtTitle.Text = "🏭 Ajouter un fournisseur";
        }

        // Constructeur pour mode édition
        public SupplierFormView(int id)
        {
            InitializeComponent();
            fournisseurId = id;
            isEditMode = true;
            TxtTitle.Text = "🏭 Modifier un fournisseur";
            LoadFournisseurData();
        }

        private void LoadFournisseurData()
        {
            try
            {
                var fournisseur = FournisseurService.GetFournisseurById(fournisseurId.Value);
                if (fournisseur != null)
                {
                    TxtCode.Text = fournisseur.Code;
                    TxtNom.Text = fournisseur.Nom;
                    TxtEmail.Text = fournisseur.CourrielContact;
                    TxtDelai.Text = fournisseur.DelaiLivraisonJours.ToString();
                    TxtEscompte.Text = fournisseur.PourcentageEscompte.ToString("0.##");
                    
                    // Sélectionner le statut
                    foreach (ComboBoxItem item in CmbStatut.Items)
                    {
                        if (item.Content.ToString() == fournisseur.Statut)
                        {
                            CmbStatut.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fournisseur : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur LoadFournisseurData: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (!ValidateForm())
            {
                return;
            }

            try
            {
                // Créer l'objet Fournisseur
                var fournisseur = new Fournisseur
                {
                    Code = TxtCode.Text.Trim(),
                    Nom = TxtNom.Text.Trim(),
                    CourrielContact = TxtEmail.Text.Trim(),
                    DelaiLivraisonJours = int.Parse(TxtDelai.Text.Trim()),
                    PourcentageEscompte = decimal.Parse(TxtEscompte.Text.Trim()),
                    Statut = ((ComboBoxItem)CmbStatut.SelectedItem).Content.ToString()
                };

                if (isEditMode && fournisseurId.HasValue)
                {
                    // Mode édition
                    fournisseur.Id = fournisseurId.Value;
                    bool success = FournisseurService.UpdateFournisseur(fournisseur);
                    
                    if (success)
                    {
                        MessageBox.Show("✅ Le fournisseur a été modifié avec succès.",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Retourner à la liste
                        ReturnToSuppliersList();
                    }
                    else
                    {
                        MessageBox.Show("❌ Erreur lors de la modification du fournisseur.",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Mode ajout
                    int newId = FournisseurService.AddFournisseur(fournisseur);
                    
                    if (newId > 0)
                    {
                        MessageBox.Show("✅ Le fournisseur a été ajouté avec succès.",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Retourner à la liste
                        ReturnToSuppliersList();
                    }
                    else
                    {
                        MessageBox.Show("❌ Erreur lors de l'ajout du fournisseur.",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur BtnSave_Click: {ex.Message}");
            }
        }

        private bool ValidateForm()
        {
            // Valider le code
            if (string.IsNullOrWhiteSpace(TxtCode.Text))
            {
                MessageBox.Show("⚠️ Le code du fournisseur est obligatoire.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCode.Focus();
                return false;
            }

            // Valider le nom
            if (string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                MessageBox.Show("⚠️ Le nom du fournisseur est obligatoire.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNom.Focus();
                return false;
            }

            // Valider l'email
            if (string.IsNullOrWhiteSpace(TxtEmail.Text))
            {
                MessageBox.Show("⚠️ L'email de contact est obligatoire.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return false;
            }

            // Valider le format de l'email
            if (!TxtEmail.Text.Contains("@") || !TxtEmail.Text.Contains("."))
            {
                MessageBox.Show("⚠️ L'email n'est pas valide.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEmail.Focus();
                return false;
            }

            // Valider le délai de livraison
            if (!int.TryParse(TxtDelai.Text, out int delai) || delai < 0)
            {
                MessageBox.Show("⚠️ Le délai de livraison doit être un nombre positif.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtDelai.Focus();
                return false;
            }

            // Valider l'escompte
            if (!decimal.TryParse(TxtEscompte.Text, out decimal escompte) || escompte < 0 || escompte > 100)
            {
                MessageBox.Show("⚠️ L'escompte doit être un nombre entre 0 et 100.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtEscompte.Focus();
                return false;
            }

            return true;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ReturnToSuppliersList();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous vraiment annuler ? Les modifications non enregistrées seront perdues.",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                ReturnToSuppliersList();
            }
        }

        private void ReturnToSuppliersList()
        {
            // Trouver le parent StocksMainView et rafraîchir la liste
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                parent.RefreshSuppliersList();
            }
        }

        private StocksMainView FindParentStocksMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is StocksMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as StocksMainView;
        }
    }
}
