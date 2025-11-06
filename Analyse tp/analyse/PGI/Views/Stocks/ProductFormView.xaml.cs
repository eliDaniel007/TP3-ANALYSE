using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class ProductFormView : UserControl
    {
        private int? productId;
        private bool isEditMode;
        private Produit currentProduct;

        public ProductFormView()
        {
            InitializeComponent();
            productId = null;
            isEditMode = false;
            LoadCategories();
            LoadFournisseurs();
        }

        // Constructeur pour l'édition (appelé depuis StocksMainView)
        public ProductFormView(int id) : this()
        {
            productId = id;
            isEditMode = true;
            LoadProduct(id);
        }

        private void LoadCategories()
        {
            try
            {
                var categories = CategorieService.GetActiveCategories();
                CmbCategorie.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des catégories: {ex.Message}");
                MessageBox.Show($"Erreur lors du chargement des catégories: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFournisseurs()
        {
            try
            {
                var fournisseurs = FournisseurService.GetActiveFournisseurs();
                CmbFournisseur.ItemsSource = fournisseurs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des fournisseurs: {ex.Message}");
                MessageBox.Show($"Erreur lors du chargement des fournisseurs: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProduct(int id)
        {
            try
            {
                currentProduct = ProduitService.GetProduitById(id);
                if (currentProduct == null)
                {
                    MessageBox.Show("Produit introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    BtnBack_Click(null, null);
                    return;
                }

                // Remplir les champs
                TxtNom.Text = currentProduct.Nom;
                TxtSKU.Text = currentProduct.SKU;
                TxtDescription.Text = currentProduct.Description;
                TxtCout.Text = currentProduct.Cout.ToString("F2");
                TxtPrix.Text = currentProduct.Prix.ToString("F2");
                TxtSeuil.Text = currentProduct.SeuilReapprovisionnement.ToString();
                TxtStockMinimum.Text = currentProduct.StockMinimum.ToString();
                TxtPoids.Text = currentProduct.PoidsKg.ToString("F3");

                // Sélectionner la catégorie
                CmbCategorie.SelectedValue = currentProduct.CategorieId;

                // Sélectionner le fournisseur
                CmbFournisseur.SelectedValue = currentProduct.FournisseurId;

                // Sélectionner le statut
                var statutItem = CmbStatut.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == currentProduct.Statut);
                if (statutItem != null)
                {
                    CmbStatut.SelectedItem = statutItem;
                }

                // Afficher les stocks
                TxtStockActuel.Text = $"{currentProduct.StockDisponible + currentProduct.StockReservee} unités";
                TxtStockReserve.Text = $"{currentProduct.StockReservee} unités";
                TxtStockDisponible.Text = $"{currentProduct.StockDisponible} unités";

                // Calculer et afficher la marge
                CalculateMarge(null, null);

                // Mettre à jour le titre
                var titleTextBlock = this.FindName("TitleTextBlock") as TextBlock;
                if (titleTextBlock != null)
                {
                    titleTextBlock.Text = "Modifier le produit";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du produit: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur lors du chargement du produit: {ex.Message}");
            }
        }

        private void CalculateMarge(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (decimal.TryParse(TxtCout.Text, out decimal cout) && 
                    decimal.TryParse(TxtPrix.Text, out decimal prix) && 
                    prix > 0)
                {
                    decimal marge = ((prix - cout) / prix) * 100;
                    TxtMarge.Text = $"{marge:F2} %";
                }
                else
                {
                    TxtMarge.Text = "0.00 %";
                }
            }
            catch
            {
                TxtMarge.Text = "0.00 %";
            }
        }

        private void CmbFournisseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbFournisseur.SelectedItem is Fournisseur fournisseur)
            {
                TxtDelaiLivraison.Text = $"• Délai de livraison: {fournisseur.DelaiLivraisonJours} jours";
                TxtEscompte.Text = $"• Escompte: {fournisseur.PourcentageEscompte:F1}%";
                BorderFournisseurInfo.Visibility = Visibility.Visible;
            }
            else
            {
                BorderFournisseurInfo.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
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
            // Validation des champs obligatoires
            if (string.IsNullOrWhiteSpace(TxtNom.Text))
            {
                MessageBox.Show("Le nom du produit est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtNom.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtSKU.Text))
            {
                MessageBox.Show("Le SKU est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtSKU.Focus();
                return;
            }

            if (CmbCategorie.SelectedValue == null)
            {
                MessageBox.Show("La catégorie est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbCategorie.Focus();
                return;
            }

            if (CmbFournisseur.SelectedValue == null)
            {
                MessageBox.Show("Le fournisseur est obligatoire.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                CmbFournisseur.Focus();
                return;
            }

            if (!decimal.TryParse(TxtCout.Text, out decimal cout) || cout <= 0)
            {
                MessageBox.Show("Le coût d'achat doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCout.Focus();
                return;
            }

            if (!decimal.TryParse(TxtPrix.Text, out decimal prix) || prix <= 0)
            {
                MessageBox.Show("Le prix de vente doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrix.Focus();
                return;
            }

            if (prix <= cout)
            {
                MessageBox.Show("Le prix de vente doit être supérieur au coût d'achat.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPrix.Focus();
                return;
            }

            if (!int.TryParse(TxtSeuil.Text, out int seuil) || seuil < 0)
            {
                MessageBox.Show("Le seuil de réapprovisionnement doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtSeuil.Focus();
                return;
            }

            if (!int.TryParse(TxtStockMinimum.Text, out int stockMin) || stockMin < 0)
            {
                MessageBox.Show("Le stock minimum doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtStockMinimum.Focus();
                return;
            }

            if (!decimal.TryParse(TxtPoids.Text, out decimal poids) || poids < 0)
            {
                MessageBox.Show("Le poids doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtPoids.Focus();
                return;
            }

            try
            {
                // Créer ou mettre à jour le produit
                var produit = new Produit
                {
                    Id = isEditMode ? productId.Value : 0,
                    SKU = TxtSKU.Text.Trim(),
                    Nom = TxtNom.Text.Trim(),
                    Description = TxtDescription.Text.Trim(),
                    CategorieId = (int)CmbCategorie.SelectedValue,
                    FournisseurId = (int)CmbFournisseur.SelectedValue,
                    Cout = cout,
                    Prix = prix,
                    MargeBrute = ((prix - cout) / prix) * 100,
                    SeuilReapprovisionnement = seuil,
                    StockMinimum = stockMin,
                    PoidsKg = poids,
                    Statut = ((ComboBoxItem)CmbStatut.SelectedItem)?.Content.ToString() ?? "Actif",
                    DateEntreeStock = isEditMode ? currentProduct?.DateEntreeStock : DateTime.Now
                };

                bool success;
                if (isEditMode)
                {
                    success = ProduitService.UpdateProduit(produit);
                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ Le produit '{produit.Nom}' a été modifié avec succès.",
                            "Modification réussie",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                else
                {
                    int newId = ProduitService.AddProduit(produit);
                    success = newId > 0;
                    if (success)
                    {
                        MessageBox.Show(
                            $"✅ Le produit '{produit.Nom}' a été ajouté avec succès.",
                            "Ajout réussi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }

                if (!success)
                {
                    MessageBox.Show(
                        "❌ Erreur lors de l'enregistrement du produit.",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }

                // Rafraîchir la liste des produits avant de retourner
                var parent = FindParentStocksMainView(this);
                if (parent != null)
                {
                    parent.RefreshProductsList();
                }

                // Retourner à la liste
                BtnBack_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Erreur lors de l'enregistrement : {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                Console.WriteLine($"Erreur lors de l'enregistrement du produit: {ex.Message}");
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
}
