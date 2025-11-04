using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Stocks
{
    public partial class ProductsListView : UserControl
    {
        public ProductsListView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Données d'exemple pour la maquette
            var products = new List<Product>
            {
                new Product { SKU = "VES-001", Nom = "Veste d'hiver Nordique", Categorie = "Vêtements", Fournisseur = "Nordic Supplies", Prix = "225 $", Cout = "150 $", Stock = "45", Seuil = "20", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
                new Product { SKU = "BOT-012", Nom = "Bottes de randonnée", Categorie = "Chaussures", Fournisseur = "Adventure Co.", Prix = "180 $", Cout = "110 $", Stock = "12", Seuil = "15", Statut = "À commander", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")) },
                new Product { SKU = "SAC-045", Nom = "Sac à dos 45L", Categorie = "Équipement", Fournisseur = "Mountain Gear", Prix = "130 $", Cout = "80 $", Stock = "28", Seuil = "10", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
                new Product { SKU = "GAN-007", Nom = "Gants thermiques", Categorie = "Accessoires", Fournisseur = "Nordic Supplies", Prix = "45 $", Cout = "25 $", Stock = "67", Seuil = "30", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
                new Product { SKU = "TEN-021", Nom = "Tente 4 saisons", Categorie = "Équipement", Fournisseur = "Adventure Co.", Prix = "450 $", Cout = "280 $", Stock = "8", Seuil = "5", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
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
            var product = button?.DataContext as Product;
            
            if (product != null)
            {
                // Naviguer vers le formulaire d'édition
                var parent = FindParentStocksMainView(this);
                if (parent != null)
                {
                    parent.NavigateToProductForm();
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
            var product = button?.DataContext as Product;
            
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
                    // TODO: Supprimer de la base de données
                    
                    // Supprimer de la liste affichée
                    var products = ProductsDataGrid.ItemsSource as List<Product>;
                    if (products != null)
                    {
                        products.Remove(product);
                        ProductsDataGrid.Items.Refresh();
                    }

                    MessageBox.Show(
                        $"✅ Le produit '{product.Nom}' a été supprimé avec succès.",
                        "Suppression réussie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
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

    // Classe modèle pour les produits
    public class Product
    {
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

