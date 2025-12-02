using System;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Stocks
{
    public partial class StocksMainView : UserControl
    {
        private Button activeSubNavButton;

        public StocksMainView()
        {
            InitializeComponent();
            // Charger le tableau de bord par défaut
            NavigateToDashboard();
        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private ProductsListView currentProductsListView;

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnProducts);
            LoadSubView("Views/Stocks/ProductsListView.xaml");
            
            // Sauvegarder la référence pour rafraîchir plus tard
            if (StocksContentArea.Content is ProductsListView productsView)
            {
                currentProductsListView = productsView;
            }
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnSuppliers);
            LoadSubView("Views/Stocks/SuppliersView.xaml");
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnCategories);
            LoadSubView("Views/Stocks/CategoriesView.xaml");
        }

        private void NavigateToDashboard()
        {
            SetActiveSubNavButton(BtnDashboard);
            LoadSubView("Views/Stocks/StocksDashboardView.xaml");
        }

        private void SetActiveSubNavButton(Button button)
        {
            // Réinitialiser le style de l'ancien bouton actif
            if (activeSubNavButton != null)
            {
                activeSubNavButton.Style = (Style)FindResource("SubNavButtonStyle");
            }

            // Appliquer le style actif au nouveau bouton
            button.Style = (Style)FindResource("ActiveSubNavButtonStyle");
            activeSubNavButton = button;
        }

        private void LoadSubView(string viewPath)
        {
            try
            {
                // Charger le UserControl depuis le chemin XAML
                var uri = new Uri(viewPath, UriKind.Relative);
                var control = System.Windows.Application.LoadComponent(uri) as UserControl;
                StocksContentArea.Content = control;
            }
            catch (Exception ex)
            {
                var textBlock = new TextBlock
                {
                    Text = $"Erreur lors du chargement de la vue : {ex.Message}",
                    FontSize = 14,
                    Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626")
                    ),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(20)
                };
                StocksContentArea.Content = textBlock;
            }
        }

        // Méthode publique pour naviguer vers la fiche produit (appelée depuis ProductsListView)
        public void NavigateToProductForm(int? productId = null)
        {
            try
            {
                ProductFormView formView;
                if (productId.HasValue)
                {
                    // Mode édition : passer l'ID du produit
                    formView = new ProductFormView(productId.Value);
                }
                else
                {
                    // Mode ajout : nouveau produit
                    formView = new ProductFormView();
                }
                
                StocksContentArea.Content = formView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur lors de la navigation vers ProductFormView: {ex.Message}");
            }
        }

        // Méthode publique pour rafraîchir la liste des produits (appelée depuis ProductFormView après sauvegarde)
        public void RefreshProductsList()
        {
            // Si on est déjà sur la liste des produits, la recharger
            if (StocksContentArea.Content is ProductsListView productsView)
            {
                productsView.LoadProduits();
            }
            else
            {
                // Sinon, recharger la vue
                BtnProducts_Click(null, null);
            }
        }

        // Méthode publique pour naviguer vers le formulaire fournisseur (appelée depuis SuppliersView)
        public void NavigateToSupplierForm(int? fournisseurId = null)
        {
            try
            {
                SupplierFormView formView;
                if (fournisseurId.HasValue)
                {
                    // Mode édition : passer l'ID du fournisseur
                    formView = new SupplierFormView(fournisseurId.Value);
                }
                else
                {
                    // Mode ajout : nouveau fournisseur
                    formView = new SupplierFormView();
                }
                
                StocksContentArea.Content = formView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur lors de la navigation vers SupplierFormView: {ex.Message}");
            }
        }

        // Méthode publique pour rafraîchir la liste des fournisseurs (appelée depuis SupplierFormView après sauvegarde)
        public void RefreshSuppliersList()
        {
            // Si on est déjà sur la liste des fournisseurs, la recharger
            if (StocksContentArea.Content is SuppliersView suppliersView)
            {
                suppliersView.LoadFournisseurs();
            }
            else
            {
                // Sinon, recharger la vue
                BtnSuppliers_Click(null, null);
            }
        }

        // Méthode publique pour rafraîchir la liste des catégories (appelée depuis CategoryFormView après sauvegarde)
        public void RefreshCategoriesList()
        {
            // Si on est déjà sur la liste des catégories, la recharger
            if (StocksContentArea.Content is CategoriesView categoriesView)
            {
                categoriesView.LoadCategories();
            }
            else
            {
                // Sinon, recharger la vue
                BtnCategories_Click(null, null);
            }
        }

        // Méthode publique pour naviguer vers la liste des produits (appelée depuis ProductHistoryView)
        public void NavigateToProductsList()
        {
            BtnProducts_Click(null, null);
        }
    }
}

