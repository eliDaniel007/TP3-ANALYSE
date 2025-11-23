using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class ProductSelectionWindow : Window
    {
        private List<Produit> allProducts;
        public Produit? SelectedProduct { get; private set; }
        public int Quantity { get; private set; }

        public ProductSelectionWindow(List<Produit> products)
        {
            InitializeComponent();
            allProducts = products;
            ProductsDataGrid.ItemsSource = allProducts;
            
            // Mettre le focus sur la recherche
            TxtSearch.Focus();
            TxtSearch.SelectAll();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText) || searchText == "rechercher un produit...")
            {
                ProductsDataGrid.ItemsSource = allProducts;
            }
            else
            {
                var filtered = allProducts.Where(p =>
                    p.Nom.ToLower().Contains(searchText) ||
                    p.SKU.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText)
                ).ToList();
                
                ProductsDataGrid.ItemsSource = filtered;
            }
        }

        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is Produit produit)
            {
                SelectedProduct = produit;
                TxtStockInfo.Text = $"Stock disponible: {produit.StockDisponible} unités";
                BtnAdd.IsEnabled = true;
            }
            else
            {
                SelectedProduct = null;
                TxtStockInfo.Text = "Stock disponible: -";
                BtnAdd.IsEnabled = false;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null)
            {
                MessageBox.Show(
                    "Veuillez sélectionner un produit.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Valider la quantité
            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show(
                    "Veuillez entrer une quantité valide (nombre entier positif).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // Vérifier le stock disponible
            if (quantity > SelectedProduct.StockDisponible)
            {
                MessageBox.Show(
                    $"Stock insuffisant!\n\n" +
                    $"Demandé: {quantity}\n" +
                    $"Disponible: {SelectedProduct.StockDisponible}\n\n" +
                    $"Veuillez réduire la quantité.",
                    "Stock insuffisant",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            Quantity = quantity;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

