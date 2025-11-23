using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class PurchaseFormView : UserControl
    {
        private List<PurchaseLine> purchaseLines;
        private int? commandeIdEnCours = null; // Pour édition

        public PurchaseFormView()
        {
            InitializeComponent();
            purchaseLines = new List<PurchaseLine>();
            ProductsDataGrid.ItemsSource = purchaseLines;
            LoadFournisseurs();
        }

        private void LoadFournisseurs()
        {
            try
            {
                var fournisseurs = FournisseurService.GetAllFournisseurs()
                    .Where(f => f.Statut == "Actif")
                    .ToList();
                
                CmbFournisseur.Items.Clear();
                CmbFournisseur.Items.Add(new ComboBoxItem { Content = "Sélectionner un fournisseur...", Tag = null });
                
                foreach (var fournisseur in fournisseurs)
                {
                    CmbFournisseur.Items.Add(new ComboBoxItem 
                    { 
                        Content = fournisseur.Nom, 
                        Tag = fournisseur.Id 
                    });
                }
                
                CmbFournisseur.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des fournisseurs:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectionWindow = new ProductSelectionForPurchaseWindow();
            if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedProduct != null)
            {
                var produit = selectionWindow.SelectedProduct;
                int quantite = selectionWindow.Quantite;
                
                // Vérifier si le produit est déjà dans la liste
                var existingLine = purchaseLines.FirstOrDefault(l => l.ProduitId == produit.Id);
                if (existingLine != null)
                {
                    // Augmenter la quantité
                    existingLine.Quantite = (int.Parse(existingLine.Quantite) + quantite).ToString();
                    existingLine.Total = $"{(decimal.Parse(existingLine.CoutAchat.Replace(" $", "").Replace(",", ".")) * int.Parse(existingLine.Quantite)):N2} $";
                    ProductsDataGrid.Items.Refresh();
                }
                else
                {
                    // Ajouter nouvelle ligne
                    purchaseLines.Add(new PurchaseLine
                    {
                        ProduitId = produit.Id,
                        Produit = $"{produit.SKU} - {produit.Nom}",
                        CoutAchat = $"{produit.Cout:N2} $",
                        Quantite = quantite.ToString(),
                        Total = $"{produit.Cout * quantite:N2} $"
                    });
                    ProductsDataGrid.Items.Refresh();
                }
                
                CalculateTotal();
            }
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
            try
            {
                // Validation
                var selectedItem = CmbFournisseur.SelectedItem as ComboBoxItem;
                if (selectedItem == null || selectedItem.Tag == null)
                {
                    MessageBox.Show(
                        "Veuillez sélectionner un fournisseur.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                if (purchaseLines.Count == 0)
                {
                    MessageBox.Show(
                        "Veuillez ajouter au moins un produit à la commande.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                int fournisseurId = (int)selectedItem.Tag;
                
                // Créer la commande
                var commande = new CommandeFournisseur
                {
                    NumeroCommande = CommandeFournisseurService.GenererNumeroCommande(),
                    DateCommande = DateTime.Now,
                    DateLivraisonPrevue = DateTime.Now.AddDays(7), // 7 jours par défaut
                    FournisseurId = fournisseurId,
                    EmployeId = null, // TODO: Utiliser l'employé connecté
                    NoteInterne = null
                };

                var lignes = purchaseLines.Select(pl => new LigneCommandeFournisseur
                {
                    ProduitId = pl.ProduitId,
                    SKU = pl.Produit.Split('-')[0].Trim(),
                    Description = pl.Produit.Split('-').Length > 1 ? pl.Produit.Split('-')[1].Trim() : pl.Produit,
                    QuantiteCommandee = int.Parse(pl.Quantite),
                    QuantiteRecue = 0,
                    PrixUnitaire = decimal.Parse(pl.CoutAchat.Replace(" $", "").Replace(",", "."))
                }).ToList();

                commandeIdEnCours = CommandeFournisseurService.CreerCommande(commande, lignes);
                
                MessageBox.Show(
                    $"✅ Commande {commande.NumeroCommande} créée avec succès !",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                
                // Retourner à la liste
                RetournerALaListe();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la création de la commande:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            // Cette fonctionnalité est maintenant dans PurchasesListView
            // où on peut réceptionner une commande existante
            MessageBox.Show(
                "Pour réceptionner une commande, allez dans la liste des commandes fournisseurs et cliquez sur 'Réceptionner'.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void RetournerALaListe()
        {
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.BtnPurchases.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
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
        public int ProduitId { get; set; }
        public string Produit { get; set; } = string.Empty;
        public string CoutAchat { get; set; } = string.Empty;
        public string Quantite { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }
}

