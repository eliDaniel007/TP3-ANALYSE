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

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnProducts);
            LoadSubView("Views/Stocks/ProductsListView.xaml");
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
                var uri = new Uri("Views/Stocks/ProductFormView.xaml", UriKind.Relative);
                var control = System.Windows.Application.LoadComponent(uri) as UserControl;
                
                // TODO: Passer l'ID du produit au contrôle si nécessaire
                
                StocksContentArea.Content = control;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

