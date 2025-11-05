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
            LoadProduits();
        }

        private void LoadProduits()
        {
            try
            {
                // Charger les produits depuis la base de données
                allProduits = ProduitService.GetAllProduits();
                DisplayProduits(allProduits);
                
                // Log dans la console (pour debug)
                Console.WriteLine($"✅ {allProduits.Count} produits chargés depuis MySQL");
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
                Statut = p.StockDisponible <= p.SeuilReapprovisionnement ? "À commander" : "Actif",
                StatutColor = p.StockDisponible <= p.SeuilReapprovisionnement 
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"))
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
            // Naviguer vers le formulaire produit
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                parent.NavigateToProductForm();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le produit sélectionné
            var button = sender as Button;
            var product = button?.DataContext as ProductDisplay;
            
            if (product != null)
            {
                // Naviguer vers le formulaire d'édition
                var parent = FindParentStocksMainView(this);
                if (parent != null)
                {
                    parent.NavigateToProductForm();
                    // TODO: Passer l'ID du produit au formulaire
                    MessageBox.Show(
                        $"Modification du produit : {product.Nom}\nSKU : {product.SKU}",
                        "Édition",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
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

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearch.Text != "Rechercher par nom ou SKU..." && !string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                try
                {
                    // Rechercher dans la base de données
                    var produits = ProduitService.SearchProduits(TxtSearch.Text);
                    DisplayProduits(produits);
                }
                catch
                {
                    // Recherche locale en cas d'erreur
                    if (allProduits != null)
                    {
                        var filtered = allProduits.Where(p =>
                            p.Nom.Contains(TxtSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                            p.SKU.Contains(TxtSearch.Text, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                        DisplayProduits(filtered);
                    }
                }
            }
            else if (string.IsNullOrWhiteSpace(TxtSearch.Text) || TxtSearch.Text == "Rechercher par nom ou SKU...")
            {
                // Réafficher tous les produits
                if (allProduits != null)
                {
                    DisplayProduits(allProduits);
                }
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
    }
}

