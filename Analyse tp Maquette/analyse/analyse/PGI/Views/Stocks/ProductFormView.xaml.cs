using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Stocks
{
    public partial class ProductFormView : UserControl
    {
        public ProductFormView()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Retourner à la liste des produits
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                parent.BtnProducts.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            BtnBack_Click(sender, e);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Produit enregistré avec succès!", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            BtnBack_Click(sender, e);
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
}

