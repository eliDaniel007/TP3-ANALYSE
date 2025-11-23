using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class SalesListView : UserControl
    {
        private List<Sale> allSales;

        public SalesListView()
        {
            InitializeComponent();
            LoadFactures();
        }

        public void LoadFactures()
        {
            try
            {
                // Charger les vraies factures depuis la base de données
                var factures = FactureService.GetAllFactures();
                
                allSales = factures.Select(f => new Sale
                {
                    Id = f.Id,
                    NumeroFacture = f.NumeroFacture,
                    Date = f.DateFacture.ToString("yyyy-MM-dd"),
                    Client = f.NomClient,
                    MontantTotal = $"{f.MontantTotal:N2} $",
                    Statut = GetStatutAffichage(f),
                    StatutColor = GetStatutColor(f),
                    FactureObject = f // Garder la référence complète
                }).ToList();

                SalesDataGrid.ItemsSource = allSales;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des factures:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                
                // Fallback sur données vides
                allSales = new List<Sale>();
                SalesDataGrid.ItemsSource = allSales;
            }
        }

        private string GetStatutAffichage(Facture facture)
        {
            if (facture.Statut == "Annulée")
                return "Annulée";
            
            if (facture.StatutPaiement == "Payée")
                return "Payée";
            
            if (facture.StatutPaiement == "Partielle")
                return "Partielle";
            
            // Vérifier si en retard
            if (facture.DateEcheance < DateTime.Now && facture.StatutPaiement == "Impayée")
                return "En retard";
            
            return "Impayée";
        }

        private Brush GetStatutColor(Facture facture)
        {
            if (facture.Statut == "Annulée")
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
            
            if (facture.StatutPaiement == "Payée")
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
            
            if (facture.StatutPaiement == "Partielle")
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
            
            // En retard
            if (facture.DateEcheance < DateTime.Now && facture.StatutPaiement == "Impayée")
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
            
            // Impayée mais pas encore en retard
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
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
        }

        private void CmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allSales == null) return;
            
            var selectedItem = CmbStatusFilter.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;
            
            string filter = selectedItem.Content.ToString() ?? "";
            
            if (filter == "Tous les statuts")
            {
                SalesDataGrid.ItemsSource = allSales;
            }
            else
            {
                var filtered = allSales.Where(s => s.Statut == filter).ToList();
                SalesDataGrid.ItemsSource = filtered;
            }
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
                if (sale.Statut == "Annulée")
                {
                    MessageBox.Show(
                        "Impossible d'enregistrer un paiement pour une facture annulée.",
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                if (sale.Statut == "Payée")
                {
                    MessageBox.Show(
                        "Cette facture est déjà entièrement payée.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                // Récupérer la facture complète
                var facture = sale.FactureObject;
                if (facture != null)
                {
                    var paymentWindow = new PaymentWindow(
                        sale.Id,
                        sale.NumeroFacture, 
                        sale.Client, 
                        (double)facture.MontantTotal, 
                        (double)facture.MontantDu
                    );
                    
                    if (paymentWindow.ShowDialog() == true)
                    {
                        // Recharger la liste pour afficher les nouveaux statuts
                        LoadFactures();
                    }
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

                // Extraire le montant (simulation)
                double montantPaye = 4250.00;
                
                var refundWindow = new RefundWindow(sale.NumeroFacture, sale.Client, montantPaye);
                if (refundWindow.ShowDialog() == true)
                {
                    // Mettre à jour le statut
                    MessageBox.Show(
                        "Le remboursement a été effectué.\nLa facture reste dans l'historique.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
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
                // Vérifier que ce n'est pas déjà annulée
                if (sale.Statut == "Annulée")
                {
                    MessageBox.Show(
                        "Cette facture est déjà annulée.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }

                var result = MessageBox.Show(
                    $"⚠️ Voulez-vous vraiment annuler cette facture ?\n\n" +
                    $"N° : {sale.NumeroFacture}\n" +
                    $"Client : {sale.Client}\n" +
                    $"Montant : {sale.MontantTotal}\n\n" +
                    $"Les produits seront remis en stock.\n" +
                    $"Cette action est irréversible !",
                    "Confirmer l'annulation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Demander le motif d'annulation
                        string motif = Microsoft.VisualBasic.Interaction.InputBox(
                            "Veuillez entrer le motif de l'annulation:",
                            "Motif d'annulation",
                            "Annulation demandée par le client"
                        );

                        if (string.IsNullOrWhiteSpace(motif))
                        {
                            MessageBox.Show(
                                "Le motif d'annulation est obligatoire.",
                                "Validation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                            return;
                        }

                        // Annuler dans la base de données
                        bool success = FactureService.AnnulerFacture(sale.Id, motif);

                        if (success)
                        {
                            MessageBox.Show(
                                $"✅ La facture '{sale.NumeroFacture}' a été annulée.\n\n" +
                                $"Les produits ont été remis en stock.",
                                "Annulation réussie",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );

                            // Recharger la liste
                            LoadFactures();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Erreur lors de l'annulation:\n{ex.Message}",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
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

    // Classe modèle pour les ventes (affichage)
    public class Sale
    {
        public int Id { get; set; }
        public string NumeroFacture { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string MontantTotal { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public Facture? FactureObject { get; set; } // Référence complète
    }
}

