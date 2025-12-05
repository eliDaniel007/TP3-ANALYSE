using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Finances
{
    public partial class SalesListView : UserControl
    {
        private List<Sale> allSales;

        public SalesListView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            try 
            {
                var factures = PGI.Services.FactureService.GetAllFactures();
                allSales = new List<Sale>();

                foreach (var f in factures)
                {
                    var sale = new Sale 
                    { 
                        NumeroFacture = f.NumeroFacture, 
                        Date = f.DateFacture.ToString("yyyy-MM-dd"), 
                        Client = f.NomClient, 
                        MontantTotal = f.MontantTotal.ToString("C"), 
                        Statut = f.StatutPaiement 
                    };

                    // Définir la couleur selon le statut
                    if (sale.Statut == "Payée") 
                        sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    else if (sale.Statut == "Partielle") 
                        sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                    else if (sale.Statut == "En retard" || sale.Statut == "Annulée") 
                        sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                    else
                        sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")); // Impayée ou autre

                    allSales.Add(sale);
                }

                SalesDataGrid.ItemsSource = allSales;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des ventes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Rechercher par N° Facture ou Client...")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = "Rechercher par N° Facture ou Client...";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
            ApplyFilters();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearch.Text != "Rechercher par N° Facture ou Client...")
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
            if (allSales == null) return;

            var filtered = allSales.AsEnumerable();

            // Filtre par recherche
            var searchText = TxtSearch.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != "Rechercher par N° Facture ou Client...")
            {
                filtered = filtered.Where(s => 
                    s.NumeroFacture.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    s.Client.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filtre par statut
            var selectedStatus = (CmbStatusFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "Tous les statuts")
            {
                // Retirer les emojis pour la comparaison
                var statusWithoutEmoji = selectedStatus.Replace("✅", "").Replace("🟡", "").Replace("🔴", "").Replace("❌", "").Trim();
                filtered = filtered.Where(s => s.Statut == statusWithoutEmoji);
            }

            SalesDataGrid.ItemsSource = filtered.ToList();
        }

        private void BtnAddSale_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire de nouvelle vente
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToSaleForm();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var sale = button?.DataContext as Sale;
            
            if (sale != null)
            {
                // Vérifier si la facture est déjà payée
                if (sale.Statut == "Payée")
                {
                    MessageBox.Show(
                        "⚠️ Cette facture est déjà payée. Vous ne pouvez pas la modifier.\n\n" +
                        "Pour corriger une facture payée, vous devez créer une note de crédit ou un remboursement.",
                        "Modification impossible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Vérifier si la facture est annulée
                if (sale.Statut == "Annulée")
                {
                    MessageBox.Show(
                        "⚠️ Cette facture est annulée. Vous ne pouvez pas la modifier.",
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
                    parent.NavigateToSaleForm();
                    MessageBox.Show(
                        $"Modification de la facture : {sale.NumeroFacture}\nClient : {sale.Client}",
                        "Édition",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
            }
        }

        private void BtnPayment_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var sale = button?.DataContext as Sale;
            
            if (sale != null)
            {
                // Vérifier si la facture est déjà payée
                if (sale.Statut == "Payée")
                {
                    MessageBox.Show(
                        "⚠️ Cette facture est déjà payée. Vous ne pouvez pas enregistrer un nouveau paiement.\n\n" +
                        "Si vous devez effectuer un remboursement, utilisez le bouton de remboursement.",
                        "Facture déjà payée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Vérifier si la facture est annulée
                if (sale.Statut == "Annulée")
                {
                    MessageBox.Show(
                        "⚠️ Cette facture est annulée. Vous ne pouvez pas enregistrer un paiement.",
                        "Facture annulée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Extraire les montants (simulation - TODO: récupérer depuis la base de données)
                double montantTotal = 4250.00;
                double montantDu = 4250.00;
                
                var paymentWindow = new PaymentWindow(sale.NumeroFacture, sale.Client, montantTotal, montantDu);
                if (paymentWindow.ShowDialog() == true)
                {
                    // Mettre à jour le statut
                    sale.Statut = "Payée";
                    sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    SalesDataGrid.Items.Refresh();
                }
            }
        }

        private void BtnRefund_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var sale = button?.DataContext as Sale;
            
            if (sale != null)
            {
                if (sale.Statut != "Payée")
                {
                    MessageBox.Show(
                        "⚠️ Seules les factures payées peuvent être remboursées.",
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                try
                {
                    // Récupérer la facture depuis la base de données
                    var facture = PGI.Services.FactureService.GetFactureByNumero(sale.NumeroFacture);
                    if (facture == null)
                    {
                        MessageBox.Show(
                            "Facture introuvable dans la base de données.",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }

                    // Calculer les montants
                    // Le montant payé = MontantTotal - MontantDu
                    // Si la facture est "Payée", alors MontantDu = 0, donc MontantPaye = MontantTotal
                    decimal montantTotalPaye = facture.MontantTotal - facture.MontantDu;
                    
                    // Si MontantPaye est disponible dans la facture, l'utiliser (plus fiable)
                    if (facture.MontantPaye > 0)
                    {
                        montantTotalPaye = facture.MontantPaye;
                    }
                    
                    // Si la facture est marquée comme "Payée" mais montantTotalPaye est 0, utiliser le montant total
                    if (facture.StatutPaiement == "Payée" && montantTotalPaye == 0)
                    {
                        montantTotalPaye = facture.MontantTotal;
                    }
                    
                    // Calculer le montant déjà remboursé depuis les paiements négatifs (remboursements)
                    var paiements = PGI.Services.PaiementService.GetPaiementsByFactureId(facture.Id);
                    decimal montantDejaRembourse = Math.Abs(paiements.Where(p => p.Montant < 0).Sum(p => p.Montant));
                    
                    var refundWindow = new RefundWindow(
                        sale.NumeroFacture, 
                        sale.Client, 
                        facture.MontantTotal,
                        montantTotalPaye, 
                        montantDejaRembourse,
                        facture.Id
                    );
                    if (refundWindow.ShowDialog() == true)
                    {
                        // Recharger les données pour mettre à jour le statut
                        LoadSampleData();
                        MessageBox.Show(
                            $"✅ Remboursement enregistré avec succès !\n\n" +
                            $"Montant remboursé : {refundWindow.MontantRemboursement:N2} $\n" +
                            $"La facture a été mise à jour.",
                            "Remboursement effectué",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors du remboursement : {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var sale = button?.DataContext as Sale;
            
            if (sale != null)
            {
                var result = MessageBox.Show(
                    $"⚠️ Voulez-vous vraiment annuler cette facture ?\n\n" +
                    $"N° : {sale.NumeroFacture}\n" +
                    $"Client : {sale.Client}\n" +
                    $"Montant : {sale.MontantTotal}\n\n" +
                    $"Cette action est irréversible !",
                    "Confirmer l'annulation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Annuler dans la base de données
                    sale.Statut = "Annulée";
                    sale.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
                    SalesDataGrid.Items.Refresh();

                    MessageBox.Show(
                        $"✅ La facture '{sale.NumeroFacture}' a été annulée.",
                        "Annulation réussie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
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

    // Classe modèle pour les ventes
    public class Sale
    {
        public string NumeroFacture { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string MontantTotal { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
    }
}

