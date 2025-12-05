using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class PurchaseFormView : UserControl
    {
        private List<PurchaseLine> purchaseLines;
        private int? _commandeId; // null = nouvelle commande, sinon = édition
        private CommandeFournisseur? _commandeExistante;

        public PurchaseFormView()
        {
            InitializeComponent();
            purchaseLines = new List<PurchaseLine>();
            ProductsDataGrid.ItemsSource = purchaseLines;
            LoadFournisseurs();
        }

        public PurchaseFormView(int commandeId) : this()
        {
            _commandeId = commandeId;
            LoadCommandeExistante();
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

                foreach (var f in fournisseurs)
                {
                    var item = new ComboBoxItem { Content = f.Nom, Tag = f.Id };
                    CmbFournisseur.Items.Add(item);
                }

                if (CmbFournisseur.Items.Count > 0)
                    CmbFournisseur.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des fournisseurs : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CmbFournisseur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Si le fournisseur change, vider la liste des produits (optionnel)
            // ou afficher un message pour informer l'utilisateur
            var selectedItem = CmbFournisseur.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Tag != null)
            {
                // Le fournisseur est sélectionné, les produits seront filtrés lors de l'ajout
                // On pourrait aussi vider la liste actuelle si on veut forcer à recommencer
                // purchaseLines.Clear();
                // ProductsDataGrid.Items.Refresh();
                // CalculateTotal();
            }
        }

        private void LoadCommandeExistante()
        {
            try
            {
                if (_commandeId.HasValue)
                {
                    _commandeExistante = CommandeFournisseurService.GetCommandeById(_commandeId.Value);
                    if (_commandeExistante == null)
                    {
                        MessageBox.Show("Commande introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        NavigateBack();
                        return;
                    }

                    // Vérifier que la commande peut être modifiée
                    if (_commandeExistante.Statut == "Reçue" || _commandeExistante.Statut == "Fermée" || _commandeExistante.Statut == "Annulée")
                    {
                        MessageBox.Show(
                            $"Cette commande ne peut pas être modifiée.\nStatut actuel : {_commandeExistante.Statut}",
                            "Modification impossible",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        NavigateBack();
                        return;
                    }

                    // Mettre à jour le titre et le statut
                    TxtFormTitle.Text = $"📦 Modifier la Commande {_commandeExistante.NumeroCommande}";
                    TxtStatut.Text = _commandeExistante.Statut.ToUpper();
                    
                    // Changer la couleur selon le statut
                    if (_commandeExistante.Statut == "En attente")
                    {
                        BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                    }
                    else if (_commandeExistante.Statut == "Partiellement reçue")
                    {
                        BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                    }

                    // Charger les données de la commande
                    var fournisseurItem = CmbFournisseur.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => item.Tag != null && (int)item.Tag == _commandeExistante.FournisseurId);
                    if (fournisseurItem != null)
                        CmbFournisseur.SelectedItem = fournisseurItem;

                    // Désactiver le changement de fournisseur si la commande a déjà des lignes reçues
                    bool hasReception = _commandeExistante.Lignes.Any(l => l.QuantiteRecue > 0);
                    if (hasReception)
                    {
                        CmbFournisseur.IsEnabled = false;
                        MessageBox.Show(
                            "Cette commande a déjà des produits reçus. Le fournisseur ne peut pas être modifié.",
                            "Information",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }

                    // Charger les lignes
                    purchaseLines.Clear();
                    foreach (var ligne in _commandeExistante.Lignes)
                    {
                        var produit = ProduitService.GetProduitById(ligne.ProduitId);
                        var purchaseLine = new PurchaseLine
                        {
                            LigneId = ligne.Id,
                            ProduitId = ligne.ProduitId,
                            Produit = produit?.Nom ?? ligne.Description,
                            SKU = ligne.SKU,
                            CoutAchat = ligne.PrixUnitaire.ToString("N2") + " $",
                            Quantite = ligne.QuantiteCommandee.ToString(),
                            Total = (ligne.PrixUnitaire * ligne.QuantiteCommandee).ToString("N2") + " $",
                            QuantiteRecue = ligne.QuantiteRecue // Conserver la quantité déjà reçue
                        };
                        purchaseLines.Add(purchaseLine);
                    }

                    ProductsDataGrid.Items.Refresh();
                    CalculateTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la commande : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier qu'un fournisseur est sélectionné
            if (CmbFournisseur.SelectedItem == null || (CmbFournisseur.SelectedItem as ComboBoxItem)?.Tag == null)
            {
                MessageBox.Show(
                    "Veuillez d'abord sélectionner un fournisseur avant d'ajouter des produits.",
                    "Fournisseur requis",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            int fournisseurId = (int)((ComboBoxItem)CmbFournisseur.SelectedItem).Tag;
            var selectionWindow = new ProductSelectionForPurchaseWindow(fournisseurId);
            if (selectionWindow.ShowDialog() == true && selectionWindow.SelectedProduct != null)
            {
                var produit = selectionWindow.SelectedProduct;
                var quantite = selectionWindow.Quantite;

                // Vérifier que le produit appartient bien au fournisseur sélectionné
                if (produit.FournisseurId != fournisseurId)
                {
                    MessageBox.Show(
                        "Ce produit n'appartient pas au fournisseur sélectionné.",
                        "Erreur de validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Vérifier si le produit est déjà dans la liste
                var existingLine = purchaseLines.FirstOrDefault(l => l.ProduitId == produit.Id);
                if (existingLine != null)
                {
                    // Augmenter la quantité
                    int currentQty = int.TryParse(existingLine.Quantite, out int qty) ? qty : 0;
                    existingLine.Quantite = (currentQty + quantite).ToString();
                    existingLine.Total = (produit.Cout * (currentQty + quantite)).ToString("N2") + " $";
                }
                else
                {
                    // Ajouter une nouvelle ligne
                    var newLine = new PurchaseLine
                    {
                        ProduitId = produit.Id,
                        Produit = produit.Nom,
                        SKU = produit.SKU,
                        CoutAchat = produit.Cout.ToString("N2") + " $",
                        Quantite = quantite.ToString(),
                        Total = (produit.Cout * quantite).ToString("N2") + " $"
                    };
                    purchaseLines.Add(newLine);
                }

                ProductsDataGrid.Items.Refresh();
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
            decimal total = 0;
            foreach (var line in purchaseLines)
            {
                string totalStr = line.Total.Replace(" $", "").Replace(" ", "").Replace(",", ".");
                if (decimal.TryParse(totalStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal lineTotal))
                {
                    total += lineTotal;
                }
            }
            TxtTotal.Text = $"{total:N2} $";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (CmbFournisseur.SelectedItem == null || (CmbFournisseur.SelectedItem as ComboBoxItem)?.Tag == null)
            {
                MessageBox.Show("Veuillez sélectionner un fournisseur.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (purchaseLines.Count == 0)
            {
                MessageBox.Show("Veuillez ajouter au moins un produit à la commande.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int fournisseurId = (int)((ComboBoxItem)CmbFournisseur.SelectedItem).Tag;

                // Créer les lignes de commande
                var lignes = new List<LigneCommandeFournisseur>();
                foreach (var line in purchaseLines)
                {
                    if (!int.TryParse(line.Quantite, out int quantite) || quantite <= 0)
                        continue;

                    string coutStr = line.CoutAchat.Replace(" $", "").Replace(" ", "").Replace(",", ".");
                    if (!decimal.TryParse(coutStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal prixUnitaire))
                        continue;

                    var produit = ProduitService.GetProduitById(line.ProduitId);
                    lignes.Add(new LigneCommandeFournisseur
                    {
                        ProduitId = line.ProduitId,
                        SKU = line.SKU,
                        Description = produit?.Nom ?? line.Produit,
                        QuantiteCommandee = quantite,
                        PrixUnitaire = prixUnitaire
                    });
                }

                if (lignes.Count == 0)
                {
                    MessageBox.Show("Aucune ligne valide dans la commande.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (_commandeId.HasValue && _commandeExistante != null)
                {
                    // Modifier une commande existante
                    var commande = new CommandeFournisseur
                    {
                        FournisseurId = fournisseurId,
                        NoteInterne = _commandeExistante.NoteInterne
                    };

                    CommandeFournisseurService.ModifierCommande(_commandeId.Value, commande, lignes);
                    MessageBox.Show($"✅ Commande modifiée avec succès !\n\nN° : {_commandeExistante.NumeroCommande}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Revenir à la liste
                    NavigateBack();
                }
                else
                {
                    // Créer une nouvelle commande
                    var commande = new CommandeFournisseur
                    {
                        NumeroCommande = CommandeFournisseurService.GenererNumeroCommande(),
                        DateCommande = DateTime.Now,
                        FournisseurId = fournisseurId,
                        Statut = "En attente",
                        NoteInterne = null
                    };

                    int commandeId = CommandeFournisseurService.CreerCommande(commande, lignes);
                    MessageBox.Show($"✅ Commande créée avec succès !\n\nN° : {commande.NumeroCommande}", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Revenir à la liste
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            if (!_commandeId.HasValue)
            {
                MessageBox.Show("Veuillez d'abord sauvegarder la commande avant de la réceptionner.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var commande = CommandeFournisseurService.GetCommandeById(_commandeId.Value);
                if (commande == null)
                {
                    MessageBox.Show("Commande introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var receptionWindow = new PurchaseReceptionWindow(commande);
                if (receptionWindow.ShowDialog() == true)
                {
                    MessageBox.Show("✅ Commande réceptionnée avec succès !\nStock mis à jour.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigateBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la réception : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigateBack();
        }

        private void NavigateBack()
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
        public int? LigneId { get; set; } // Pour l'édition
        public int ProduitId { get; set; }
        public string Produit { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CoutAchat { get; set; } = string.Empty;
        public string Quantite { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public int QuantiteRecue { get; set; } = 0; // Quantité déjà reçue (pour la modification)
    }
}

