using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class PurchaseFormView : UserControl
    {
        private List<PurchaseLine> purchaseLines;

        public PurchaseFormView()
        {
            InitializeComponent();
            purchaseLines = new List<PurchaseLine>();
            ProductsDataGrid.ItemsSource = purchaseLines;
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var newLine = new PurchaseLine
            {
                Produit = "Veste d'hiver Nordique",
                CoutAchat = "150,00 $",
                Quantite = "10",
                Total = "1 500,00 $"
            };

            purchaseLines.Add(newLine);
            ProductsDataGrid.Items.Refresh();
            CalculateTotal();
        }

        private void BtnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var line = button?.DataContext as PurchaseLine;
            
            if (line != null)
            {
                purchaseLines.Remove(line);
                ProductsDataGrid.Items.Refresh();
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            double total = 0;
            foreach (var line in purchaseLines)
            {
                string totalStr = line.Total.Replace(" $", "").Replace(",", ".");
                if (double.TryParse(totalStr, out double lineTotal))
                {
                    total += lineTotal;
                }
            }
            TxtTotal.Text = $"{total:N2} $";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("✅ Commande sauvegardée !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Réceptionner cette commande ?\n\n" +
                "Actions automatiques :\n" +
                "1. Mise à jour du stock (mouvement IN)\n" +
                "2. Création d'une facture fournisseur",
                "Réceptionner",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("✅ Commande réceptionnée !\nStock mis à jour.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.BtnPurchases.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }

        private FinancesMainView? FindParentFinancesMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is FinancesMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as FinancesMainView;
        }
    }

    public class PurchaseLine
    {
        public string Produit { get; set; } = string.Empty;
        public string CoutAchat { get; set; } = string.Empty;
        public string Quantite { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }
}

