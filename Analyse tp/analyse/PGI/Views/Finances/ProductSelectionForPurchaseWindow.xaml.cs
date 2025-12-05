using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class ProductSelectionForPurchaseWindow : Window
    {
        private List<ProductDisplay> allProducts;
        private int? fournisseurId;
        public Produit? SelectedProduct { get; private set; }
        public int Quantite { get; private set; }

        public ProductSelectionForPurchaseWindow(int? fournisseurId = null)
        {
            InitializeComponent();
            this.fournisseurId = fournisseurId;
            
            // Mettre à jour le titre si un fournisseur est spécifié
            if (fournisseurId.HasValue)
            {
                try
                {
                    var fournisseur = FournisseurService.GetFournisseurById(fournisseurId.Value);
                    if (fournisseur != null)
                    {
                        this.Title = $"Sélectionner un produit - {fournisseur.Nom}";
                    }
                }
                catch
                {
                    // Ignorer l'erreur, garder le titre par défaut
                }
            }
            
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var produits = ProduitService.GetAllProduits();
                
                // Filtrer par fournisseur si spécifié
                var produitsFiltres = produits.Where(p => p.Statut == "Actif");
                
                if (fournisseurId.HasValue)
                {
                    produitsFiltres = produitsFiltres.Where(p => p.FournisseurId == fournisseurId.Value);
                }
                
                allProducts = produitsFiltres
                    .Select(p => new ProductDisplay
                    {
                        Id = p.Id,
                        SKU = p.SKU,
                        Nom = p.Nom,
                        Cout = p.Cout,
                        CoutFormate = $"{p.Cout:N2} $",
                        NomFournisseur = p.NomFournisseur,
                        Produit = p
                    })
                    .ToList();
                
                ProductsDataGrid.ItemsSource = allProducts;
                
                // Afficher un message si aucun produit n'est disponible
                if (allProducts.Count == 0 && fournisseurId.HasValue)
                {
                    var fournisseur = FournisseurService.GetFournisseurById(fournisseurId.Value);
                    var nomFournisseur = fournisseur?.Nom ?? "ce fournisseur";
                    MessageBox.Show(
                        $"Aucun produit actif disponible pour {nomFournisseur}.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des produits:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                allProducts = new List<ProductDisplay>();
            }
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (allProducts == null) return;

            string searchText = TxtSearch.Text.Trim().ToLower();
            
            if (string.IsNullOrEmpty(searchText))
            {
                // Si un fournisseur est sélectionné, les produits sont déjà filtrés
                ProductsDataGrid.ItemsSource = allProducts;
            }
            else
            {
                var filtered = allProducts.Where(p =>
                    p.SKU.ToLower().Contains(searchText) ||
                    p.Nom.ToLower().Contains(searchText) ||
                    p.NomFournisseur.ToLower().Contains(searchText)
                ).ToList();
                
                ProductsDataGrid.ItemsSource = filtered;
            }
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ValiderSelection();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ValiderSelection();
        }

        private void ValiderSelection()
        {
            var selectedDisplay = ProductsDataGrid.SelectedItem as ProductDisplay;
            if (selectedDisplay == null)
            {
                MessageBox.Show(
                    "Veuillez sélectionner un produit.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (!int.TryParse(TxtQuantite.Text, out int quantite) || quantite <= 0)
            {
                MessageBox.Show(
                    "Veuillez entrer une quantité valide (nombre entier positif).",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                TxtQuantite.Focus();
                return;
            }

            SelectedProduct = selectedDisplay.Produit;
            Quantite = quantite;
            this.DialogResult = true;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private class ProductDisplay
        {
            public int Id { get; set; }
            public string SKU { get; set; } = string.Empty;
            public string Nom { get; set; } = string.Empty;
            public decimal Cout { get; set; }
            public string CoutFormate { get; set; } = string.Empty;
            public string NomFournisseur { get; set; } = string.Empty;
            public Produit Produit { get; set; } = null!;
        }
    }
}

