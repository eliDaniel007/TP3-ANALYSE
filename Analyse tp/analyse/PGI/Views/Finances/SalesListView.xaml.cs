using System.Collections.Generic;
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
        }

        private void CmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Implémenter le filtrage par statut
            MessageBox.Show(
                "Filtrage par statut à implémenter avec la base de données.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
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
                // Extraire les montants (simulation)
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

