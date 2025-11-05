using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class CategoriesView : UserControl
    {
        private List<Categorie> categories;

        public CategoriesView()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                // Charger depuis la base de données
                categories = CategorieService.GetAllCategories();
                DisplayCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des catégories: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadSampleCategories();
            }
        }

        private void DisplayCategories()
        {
            CategoriesListPanel.Children.Clear();

            foreach (var category in categories)
            {
                var border = new Border
                {
                    Background = Brushes.Transparent,
                    BorderBrush = (Brush)new BrushConverter().ConvertFrom("#E2E8F0"),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(25, 20, 25, 20)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var textBlock = new TextBlock
                {
                    Text = $"{category.Nom} ({category.Statut})",
                    FontSize = 15,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#1E293B"),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(textBlock, 0);

                var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal };

                var editButton = new Button
                {
                    Content = "✏️ Modifier",
                    Background = Brushes.Transparent,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#669BBC"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Margin = new Thickness(0, 0, 10, 0),
                    Padding = new Thickness(10, 5, 10, 5),
                    Tag = category.Id
                };
                editButton.Click += EditButton_Click;

                var deleteButton = new Button
                {
                    Content = "🗑️ Supprimer",
                    Background = Brushes.Transparent,
                    Foreground = (Brush)new BrushConverter().ConvertFrom("#EF4444"),
                    BorderThickness = new Thickness(0),
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Padding = new Thickness(10, 5, 10, 5),
                    Tag = category.Id
                };
                deleteButton.Click += DeleteButton_Click;

                buttonPanel.Children.Add(editButton);
                buttonPanel.Children.Add(deleteButton);
                Grid.SetColumn(buttonPanel, 1);

                grid.Children.Add(textBlock);
                grid.Children.Add(buttonPanel);
                border.Child = grid;

                CategoriesListPanel.Children.Add(border);
            }
        }

        private void LoadSampleCategories()
        {
            // Données d'exemple si pas de connexion BDD
            categories = new List<Categorie>
            {
                new Categorie { Id = 1, Nom = "Vêtements", Statut = "Actif" },
                new Categorie { Id = 2, Nom = "Équipement", Statut = "Actif" },
                new Categorie { Id = 3, Nom = "Chaussures", Statut = "Actif" }
            };
            DisplayCategories();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int categoryId = (int)button.Tag;
            var category = categories.Find(c => c.Id == categoryId);
            
            if (category != null)
            {
                // TODO: Ouvrir une fenêtre d'édition
                MessageBox.Show($"Modification de la catégorie : {category.Nom}",
                    "Édition", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int categoryId = (int)button.Tag;
            var category = categories.Find(c => c.Id == categoryId);
            
            if (category != null)
            {
                var result = MessageBox.Show(
                    $"⚠️ Voulez-vous vraiment supprimer la catégorie '{category.Nom}' ?\n\n" +
                    $"Cette action est irréversible !",
                    "Confirmer la suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = CategorieService.DeleteCategorie(categoryId);
                        if (success)
                        {
                            LoadCategories();
                            MessageBox.Show($"✅ La catégorie '{category.Nom}' a été supprimée avec succès.",
                                "Suppression réussie", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ Erreur lors de la suppression : {ex.Message}",
                            "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            string nom = TxtNewCategory.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(nom) || nom == "Nom de la nouvelle catégorie")
            {
                MessageBox.Show("Veuillez entrer un nom de catégorie.", "Erreur",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newCategorie = new Categorie
                {
                    Nom = nom,
                    Statut = "Actif"
                };

                int id = CategorieService.AddCategorie(newCategorie);
                
                if (id > 0)
                {
                    TxtNewCategory.Text = "Nom de la nouvelle catégorie";
                    LoadCategories();
                    MessageBox.Show($"✅ La catégorie '{nom}' a été ajoutée avec succès.",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur lors de l'ajout : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtNewCategory_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtNewCategory.Text == "Nom de la nouvelle catégorie")
            {
                TxtNewCategory.Text = "";
                TxtNewCategory.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtNewCategory_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNewCategory.Text))
            {
                TxtNewCategory.Text = "Nom de la nouvelle catégorie";
                TxtNewCategory.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }
    }
}

