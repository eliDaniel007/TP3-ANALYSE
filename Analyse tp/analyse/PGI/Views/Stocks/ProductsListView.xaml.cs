using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class ProductsListView : UserControl
    {
        private List<Produit> allProduits;

        public ProductsListView()
        {
            InitializeComponent();
            LoadCategories();
            LoadFournisseurs();
            LoadProduits();
        }

        public void LoadProduits()
        {
            try
            {
                // Charger les produits depuis la base de données
                allProduits = ProduitService.GetAllProduits();
                
                // Log dans la console (pour debug)
                Console.WriteLine($"✅ {allProduits.Count} produits chargés depuis MySQL");
                
                // Appliquer les filtres actuels
                ApplyFilters();
            }
            catch (Exception ex)
            {
                // Log de l'erreur
                Console.WriteLine($"❌ Erreur BDD (produits): {ex.Message}");
                
                // Charger des données d'exemple en cas d'erreur de connexion
            LoadSampleData();
            }
        }

        private void DisplayProduits(List<Produit> produits)
        {
            // Convertir les produits en format pour affichage
            var displayProducts = produits.Select(p => new ProductDisplay
            {
                Id = p.Id,
                SKU = p.SKU,
                Nom = p.Nom,
                Categorie = p.NomCategorie,
                Fournisseur = p.NomFournisseur,
                Prix = $"{p.Prix:N2} $",
                Cout = $"{p.Cout:N2} $",
                Stock = p.StockDisponible.ToString(),
                Seuil = p.SeuilReapprovisionnement.ToString(),
                Statut = p.Statut == "Inactif" ? "Inactif" : (p.StockDisponible <= p.SeuilReapprovisionnement ? "À commander" : "Actif"),
                StatutColor = p.Statut == "Inactif" 
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"))
                    : (p.StockDisponible <= p.SeuilReapprovisionnement 
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"))
                        : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"))),
                IsInactif = p.Statut == "Inactif",
                ProduitStatut = p.Statut
            }).ToList();

            ProductsDataGrid.ItemsSource = displayProducts;
        }

        private void LoadSampleData()
        {
            // Données d'exemple pour la maquette (si pas de connexion BDD)
            var products = new List<ProductDisplay>
            {
                new ProductDisplay { SKU = "VES-001", Nom = "Veste d'hiver Nordique", Categorie = "Vêtements", Fournisseur = "Nordic Supplies", Prix = "225 $", Cout = "150 $", Stock = "45", Seuil = "20", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
                new ProductDisplay { SKU = "BOT-012", Nom = "Bottes de randonnée", Categorie = "Chaussures", Fournisseur = "Adventure Co.", Prix = "180 $", Cout = "110 $", Stock = "12", Seuil = "15", Statut = "À commander", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")) },
                new ProductDisplay { SKU = "SAC-045", Nom = "Sac à dos 45L", Categorie = "Équipement", Fournisseur = "Mountain Gear", Prix = "130 $", Cout = "80 $", Stock = "28", Seuil = "10", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
            };

            ProductsDataGrid.ItemsSource = products;
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Rechercher par nom ou SKU...")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = "Rechercher par nom ou SKU...";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire produit (mode ajout)
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                parent.NavigateToProductForm(null);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le produit sélectionné
            var button = sender as Button;
            var product = button?.DataContext as ProductDisplay;
            
            if (product != null)
            {
                // Naviguer vers le formulaire d'édition avec l'ID du produit
                var parent = FindParentStocksMainView(this);
                if (parent != null)
                {
                    parent.NavigateToProductForm(product.Id);
                }
            }
        }

        private void BtnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le produit sélectionné
            var button = sender as Button;
            var product = button?.DataContext as ProductDisplay;
            
            if (product != null)
            {
                try
                {
                    // Charger le produit depuis la BDD pour obtenir le statut actuel
                    var produit = ProduitService.GetProduitById(product.Id);
                    if (produit == null)
                    {
                        MessageBox.Show("Produit introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Déterminer le nouveau statut
                    string nouveauStatut;
                    string message;
                    if (produit.Statut == "Actif")
                    {
                        nouveauStatut = "Inactif";
                        message = $"⚠️ Voulez-vous vraiment désactiver ce produit ?\n\n" +
                                 $"SKU : {product.SKU}\n" +
                                 $"Nom : {product.Nom}\n\n" +
                                 $"Le produit ne sera plus visible dans les ventes.";
                    }
                    else
                    {
                        nouveauStatut = "Actif";
                        message = $"✅ Voulez-vous réactiver ce produit ?\n\n" +
                                 $"SKU : {product.SKU}\n" +
                                 $"Nom : {product.Nom}\n\n" +
                                 $"Le produit sera à nouveau disponible pour les ventes.";
                    }

                    var result = MessageBox.Show(
                        message,
                        nouveauStatut == "Inactif" ? "Confirmer la désactivation" : "Confirmer la réactivation",
                        MessageBoxButton.YesNo,
                        nouveauStatut == "Inactif" ? MessageBoxImage.Warning : MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // Mettre à jour le statut
                        produit.Statut = nouveauStatut;
                        bool success = ProduitService.UpdateProduit(produit);

                        if (success)
                        {
                            // Recharger la liste
                            LoadProduits();

                            MessageBox.Show(
                                $"✅ Le produit '{product.Nom}' a été {(nouveauStatut == "Inactif" ? "désactivé" : "réactivé")} avec succès.",
                                nouveauStatut == "Inactif" ? "Désactivation réussie" : "Réactivation réussie",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                $"❌ Erreur lors de la {(nouveauStatut == "Inactif" ? "désactivation" : "réactivation")} du produit.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"❌ Erreur : {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    Console.WriteLine($"Erreur lors du changement de statut: {ex.Message}");
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le produit sélectionné
            var button = sender as Button;
            var product = button?.DataContext as ProductDisplay;
            
            if (product != null)
            {
                var result = MessageBox.Show(
                    $"⚠️ Voulez-vous vraiment supprimer ce produit ?\n\n" +
                    $"SKU : {product.SKU}\n" +
                    $"Nom : {product.Nom}\n" +
                    $"Catégorie : {product.Categorie}\n\n" +
                    $"Cette action est irréversible !",
                    "Confirmer la suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Supprimer de la base de données
                        bool success = ProduitService.DeleteProduit(product.Id);
                    
                        if (success)
                    {
                            // Recharger la liste
                            LoadProduits();

                    MessageBox.Show(
                        $"✅ Le produit '{product.Nom}' a été supprimé avec succès.",
                        "Suppression réussie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                        }
                        else
                        {
                            MessageBox.Show(
                                "❌ Erreur lors de la suppression du produit.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"❌ Erreur lors de la suppression : {ex.Message}",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        private void LoadCategories()
        {
            try
            {
                var categories = CategorieService.GetActiveCategories();
                CmbCategorie.Items.Clear();
                var allCat = new ComboBoxItem { Content = "Toutes les catégories" };
                CmbCategorie.Items.Add(allCat);
                foreach (var cat in categories)
                {
                    CmbCategorie.Items.Add(new ComboBoxItem { Content = cat.Nom });
                }
                CmbCategorie.SelectedItem = allCat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des catégories: {ex.Message}");
            }
        }

        private void LoadFournisseurs()
        {
            try
            {
                var fournisseurs = FournisseurService.GetActiveFournisseurs();
                CmbFournisseur.Items.Clear();
                var allFour = new ComboBoxItem { Content = "Tous les fournisseurs" };
                CmbFournisseur.Items.Add(allFour);
                foreach (var four in fournisseurs)
                {
                    CmbFournisseur.Items.Add(new ComboBoxItem { Content = four.Nom });
                }
                CmbFournisseur.SelectedItem = allFour;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des fournisseurs: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            if (allProduits == null) return;

            var filtered = allProduits.AsEnumerable();

            // Filtre par recherche (nom ou SKU)
            if (TxtSearch.Text != "Rechercher par nom ou SKU..." && !string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                var searchText = TxtSearch.Text.ToLower();
                filtered = filtered.Where(p =>
                    p.Nom.ToLower().Contains(searchText) ||
                    p.SKU.ToLower().Contains(searchText)
                );
            }

            // Filtre par catégorie
            if (CmbCategorie.SelectedItem is ComboBoxItem selectedCat && selectedCat.Content.ToString() != "Toutes les catégories")
            {
                var categorieName = selectedCat.Content.ToString();
                filtered = filtered.Where(p => p.NomCategorie == categorieName);
            }

            // Filtre par fournisseur
            if (CmbFournisseur.SelectedItem is ComboBoxItem selectedFour && selectedFour.Content.ToString() != "Tous les fournisseurs")
            {
                var fournisseurName = selectedFour.Content.ToString();
                filtered = filtered.Where(p => p.NomFournisseur == fournisseurName);
                }

            // Filtre par statut
            if (CmbStatut.SelectedItem is ComboBoxItem selectedStatut && selectedStatut.Content.ToString() != "Tous les statuts")
            {
                var statutName = selectedStatut.Content.ToString();
                if (statutName == "À commander")
                {
                    filtered = filtered.Where(p => p.StockDisponible <= p.SeuilReapprovisionnement);
                }
                else if (statutName == "Actif")
                    {
                    filtered = filtered.Where(p => p.StockDisponible > p.SeuilReapprovisionnement && p.Statut == "Actif");
                }
                else if (statutName == "Inactif")
                {
                    filtered = filtered.Where(p => p.Statut == "Inactif");
                }
            }

            DisplayProduits(filtered.ToList());
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbCategorie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbFournisseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbStatut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            // Désactiver temporairement les événements pour éviter les appels multiples
            TxtSearch.TextChanged -= TxtSearch_TextChanged;
            CmbCategorie.SelectionChanged -= CmbCategorie_SelectionChanged;
            CmbFournisseur.SelectionChanged -= CmbFournisseur_SelectionChanged;
            CmbStatut.SelectionChanged -= CmbStatut_SelectionChanged;

            // Réinitialiser la recherche
            TxtSearch.Text = "Rechercher par nom ou SKU...";
            TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));

            // Réinitialiser les filtres
            if (CmbCategorie.Items.Count > 0)
            {
                CmbCategorie.SelectedItem = CmbCategorie.Items[0]; // "Toutes les catégories"
            }
            
            if (CmbFournisseur.Items.Count > 0)
            {
                CmbFournisseur.SelectedItem = CmbFournisseur.Items[0]; // "Tous les fournisseurs"
            }
            
            if (CmbStatut.Items.Count > 0)
            {
                CmbStatut.SelectedItem = CmbStatut.Items[0]; // "Tous les statuts"
            }

            // Réactiver les événements
            TxtSearch.TextChanged += TxtSearch_TextChanged;
            CmbCategorie.SelectionChanged += CmbCategorie_SelectionChanged;
            CmbFournisseur.SelectionChanged += CmbFournisseur_SelectionChanged;
            CmbStatut.SelectionChanged += CmbStatut_SelectionChanged;

                // Réafficher tous les produits
                if (allProduits != null)
                {
                    DisplayProduits(allProduits);
            }
        }

        private StocksMainView FindParentStocksMainView(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is StocksMainView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as StocksMainView;
        }
    }

    // Classe pour l'affichage des produits dans le DataGrid
    public class ProductDisplay
    {
        public int Id { get; set; }
        public string SKU { get; set; }
        public string Nom { get; set; }
        public string Categorie { get; set; }
        public string Fournisseur { get; set; }
        public string Prix { get; set; }
        public string Cout { get; set; }
        public string Stock { get; set; }
        public string Seuil { get; set; }
        public string Statut { get; set; }
        public Brush StatutColor { get; set; }
        public bool IsInactif { get; set; }
        public string ProduitStatut { get; set; }
    }
}

