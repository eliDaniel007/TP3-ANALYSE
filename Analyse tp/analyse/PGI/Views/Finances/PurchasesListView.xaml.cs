using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Finances
{
    public partial class PurchasesListView : UserControl
    {
        private List<Purchase> allPurchases;
        private const string searchPlaceholder = "Rechercher par N° Commande ou Fournisseur...";

        public PurchasesListView()
        {
            InitializeComponent();
            InitializePlaceholder();
            LoadData();
        }

        private void InitializePlaceholder()
        {
            TxtSearch.Text = searchPlaceholder;
            TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            TxtSearch.GotFocus += TxtSearch_GotFocus;
            TxtSearch.LostFocus += TxtSearch_LostFocus;
            TxtSearch.TextChanged += TxtSearch_TextChanged;
            CmbStatusFilter.SelectionChanged += CmbStatusFilter_SelectionChanged;
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == searchPlaceholder)
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = searchPlaceholder;
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearch.Text != searchPlaceholder)
            {
                ApplyFilters();
            }
        }

        private void CmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allPurchases == null) return;

            var filtered = allPurchases.AsEnumerable();

            // Filtre par recherche
            var searchText = TxtSearch.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != searchPlaceholder)
            {
                filtered = filtered.Where(p =>
                    p.NumeroCommande.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    p.Fournisseur.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filtre par statut
            var selectedStatus = (CmbStatusFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Tous les statuts")
            {
                var statusWithoutEmoji = selectedStatus.Replace("📝", "").Replace("📤", "").Replace("📦", "").Replace("✅", "").Trim();
                filtered = filtered.Where(p => p.Statut == statusWithoutEmoji || p.Statut.Contains(statusWithoutEmoji));
            }

            PurchasesDataGrid.ItemsSource = filtered.ToList();
        }

        private void LoadData()
        {
            try
            {
                var commandes = PGI.Services.CommandeFournisseurService.GetAllCommandes();
                allPurchases = new List<Purchase>();

                foreach (var c in commandes)
                {
                    var purchase = new Purchase
                    {
                        NumeroCommande = c.NumeroCommande,
                        Date = c.DateCommande.ToString("yyyy-MM-dd"),
                        Fournisseur = c.NomFournisseur,
                        MontantTotal = c.MontantTotal.ToString("C"),
                        Statut = c.Statut
                    };

                    // Définir la couleur selon le statut
                    if (purchase.Statut == "Reçue")
                        purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    else if (purchase.Statut == "Envoyée" || purchase.Statut == "En attente" || purchase.Statut == "Partiellement reçue")
                        purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                    else if (purchase.Statut == "Annulée" || purchase.Statut == "Fermée")
                        purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
                    else
                        purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B"));

                    allPurchases.Add(purchase);
                }

                PurchasesDataGrid.ItemsSource = allPurchases;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des achats : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddPurchase_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToPurchaseForm();
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                try
                {
                    var commande = PGI.Services.CommandeFournisseurService.GetCommandeByNumero(purchase.NumeroCommande);
                    if (commande != null)
                    {
                        var detailsWindow = new PurchaseDetailsWindow(commande);
                        detailsWindow.Owner = Window.GetWindow(this);
                        detailsWindow.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                try
                {
                    var commande = PGI.Services.CommandeFournisseurService.GetCommandeByNumero(purchase.NumeroCommande);
                    if (commande == null)
                    {
                        MessageBox.Show("Commande introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Vérifier si la commande peut être modifiée
                    if (commande.Statut == "Reçue" || commande.Statut == "Fermée" || commande.Statut == "Annulée")
                    {
                        MessageBox.Show(
                            $"⚠️ Cette commande ne peut pas être modifiée.\nStatut : {commande.Statut}",
                            "Modification impossible",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    // Naviguer vers le formulaire d'édition
                    var parent = FindParentFinancesMainView(this);
                    if (parent != null)
                    {
                        parent.NavigateToPurchaseForm(commande.Id);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                try
                {
                    var commande = PGI.Services.CommandeFournisseurService.GetCommandeByNumero(purchase.NumeroCommande);
                    if (commande == null)
                    {
                        MessageBox.Show("Commande introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Vérifier si la commande peut être réceptionnée
                    if (commande.Statut == "Reçue" || commande.Statut == "Fermée" || commande.Statut == "Annulée")
                    {
                        MessageBox.Show(
                            $"⚠️ Cette commande ne peut pas être réceptionnée.\nStatut : {commande.Statut}",
                            "Réception impossible",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    // Ouvrir la fenêtre de réception
                    var receptionWindow = new PurchaseReceptionWindow(commande);
                    if (receptionWindow.ShowDialog() == true)
                    {
                        // Recharger les données
                        LoadData();
                        MessageBox.Show("✅ Commande réceptionnée avec succès !\nStock mis à jour.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la réception : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                if (purchase.Statut != "Reçue")
                {
                    MessageBox.Show(
                        "⚠️ Vous devez d'abord réceptionner la commande avant de la fermer.",
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                var result = MessageBox.Show(
                    $"Fermer la commande {purchase.NumeroCommande} ?\n\n" +
                    $"Cette action marque la commande comme terminée.\n" +
                    $"Elle ne pourra plus être modifiée.",
                    "Fermer la commande",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var commande = PGI.Services.CommandeFournisseurService.GetCommandeByNumero(purchase.NumeroCommande);
                        if (commande != null)
                        {
                            PGI.Services.CommandeFournisseurService.FermerCommande(commande.Id);
                            
                            // Recharger les données
                            LoadData();
                            MessageBox.Show("✅ Commande fermée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la fermeture : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void PurchasesDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PurchasesDataGrid.SelectedItem is Purchase purchase)
            {
                try
                {
                    var commande = PGI.Services.CommandeFournisseurService.GetCommandeByNumero(purchase.NumeroCommande);
                    if (commande != null)
                    {
                        var detailsWindow = new PurchaseDetailsWindow(commande);
                        detailsWindow.Owner = Window.GetWindow(this);
                        detailsWindow.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private FinancesMainView? FindParentFinancesMainView(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is FinancesMainView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as FinancesMainView;
        }
    }

    public class Purchase
    {
        public string NumeroCommande { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Fournisseur { get; set; } = string.Empty;
        public string MontantTotal { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
    }
}
