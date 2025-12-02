using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Finances
{
    public partial class PurchasesListView : UserControl
    {
        public PurchasesListView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var purchases = new List<Purchase>
            {
                new Purchase { NumeroCommande = "A2025-0105", Date = "2025-01-25", Fournisseur = "Nordic Supplies", MontantTotal = "15 450 $", Statut = "Envoyée", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")) },
                new Purchase { NumeroCommande = "A2025-0104", Date = "2025-01-22", Fournisseur = "Adventure Co.", MontantTotal = "22 890 $", Statut = "Reçue", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) },
                new Purchase { NumeroCommande = "A2025-0103", Date = "2025-01-20", Fournisseur = "Mountain Gear", MontantTotal = "8 320 $", Statut = "Fermée", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")) },
            };

            PurchasesDataGrid.ItemsSource = purchases;
        }

        private void BtnAddPurchase_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToPurchaseForm();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                MessageBox.Show($"Modifier la commande : {purchase.NumeroCommande}", "Édition", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null)
            {
                var result = MessageBox.Show(
                    $"Réceptionner la commande {purchase.NumeroCommande} ?\n\n" +
                    $"Actions automatiques :\n" +
                    $"1. Mise à jour du stock (mouvement IN)\n" +
                    $"2. Création d'une facture fournisseur à payer",
                    "Réceptionner la commande",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Mettre à jour le stock dans la base de données
                    // TODO: Créer une facture fournisseur
                    
                    purchase.Statut = "Reçue";
                    purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    PurchasesDataGrid.Items.Refresh();
                    
                    MessageBox.Show("✅ Commande réceptionnée !\nStock mis à jour et facture créée.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    // TODO: Mettre à jour le statut dans la base de données
                    
                    purchase.Statut = "Fermée";
                    purchase.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280"));
                    PurchasesDataGrid.Items.Refresh();
                    
                    MessageBox.Show("✅ Commande fermée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
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

