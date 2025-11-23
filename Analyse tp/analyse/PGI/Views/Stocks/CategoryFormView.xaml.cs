using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class CategoryFormView : UserControl
    {
        private int? categoryId;
        private bool isEditMode;

        // Constructeur pour mode ajout
        public CategoryFormView()
        {
            InitializeComponent();
            isEditMode = false;
            TxtTitle.Text = "🏷️ Ajouter une catégorie";
        }

        // Constructeur pour mode édition
        public CategoryFormView(int id)
        {
            InitializeComponent();
            categoryId = id;
            isEditMode = true;
            TxtTitle.Text = "🏷️ Modifier une catégorie";
            LoadCategoryData();
        }

        private void LoadCategoryData()
        {
            try
            {
                var category = CategorieService.GetCategorieById(categoryId.Value);
                if (category != null)
                {
                    TxtNom.Text = category.Nom;
                    
                    // Sélectionner le statut
                    foreach (ComboBoxItem item in CmbStatut.Items)
                    {
                        if (item.Content.ToString() == category.Statut)
                        {
                            CmbStatut.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la catégorie : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur LoadCategoryData: {ex.Message}");
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
                // Créer l'objet Categorie
                var categorie = new Categorie
                {
                    Nom = TxtNom.Text.Trim(),
                    Statut = ((ComboBoxItem)CmbStatut.SelectedItem).Content.ToString()
                };

                if (isEditMode && categoryId.HasValue)
                {
                    // Mode édition
                    categorie.Id = categoryId.Value;
                    bool success = CategorieService.UpdateCategorie(categorie);
                    
                    if (success)
                    {
                        MessageBox.Show("✅ La catégorie a été modifiée avec succès.",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Retourner à la liste
                        ReturnToCategoriesList();
                    }
                    else
                    {
                        MessageBox.Show("❌ Erreur lors de la modification de la catégorie.",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Mode ajout
                    int newId = CategorieService.AddCategorie(categorie);
                    
                    if (newId > 0)
                    {
                        MessageBox.Show("✅ La catégorie a été ajoutée avec succès.",
                            "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Retourner à la liste
                        ReturnToCategoriesList();
                    }
                    else
                    {
                        MessageBox.Show("❌ Erreur lors de l'ajout de la catégorie.",
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
            // Valider le nom
            if (string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                MessageBox.Show("⚠️ Le nom de la catégorie est obligatoire.",
                    "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNom.Focus();
                return false;
            }

            return true;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            ReturnToCategoriesList();
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
                ReturnToCategoriesList();
            }
        }

        private void ReturnToCategoriesList()
        {
            // Trouver le parent StocksMainView et rafraîchir la liste
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                parent.RefreshCategoriesList();
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

