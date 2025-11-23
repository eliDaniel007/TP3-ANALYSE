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
    public partial class PurchasesListView : UserControl
    {
        public PurchasesListView()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                var commandes = CommandeFournisseurService.GetAllCommandes();
                
                var purchases = commandes.Select(c => new Purchase
                {
                    Id = c.Id,
                    NumeroCommande = c.NumeroCommande,
                    Date = c.DateCommande.ToString("yyyy-MM-dd"),
                    Fournisseur = c.NomFournisseur,
                    MontantTotal = $"{c.MontantTotal:N2} $",
                    Statut = c.Statut,
                    StatutColor = GetStatutColor(c.Statut),
                    CommandeObject = c
                }).ToList();
                
                PurchasesDataGrid.ItemsSource = purchases;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des commandes:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                PurchasesDataGrid.ItemsSource = new List<Purchase>();
            }
        }

        private Brush GetStatutColor(string statut)
        {
            return statut switch
            {
                "En attente" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),
                "Partiellement reçue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                "Reçue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                "Annulée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"))
            };
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
            
            if (purchase != null && purchase.CommandeObject != null)
            {
                var commande = purchase.CommandeObject;
                
                // Afficher les détails de la commande
                var detailsWindow = new PurchaseDetailsWindow(commande);
                detailsWindow.ShowDialog();
                
                // Recharger la liste
                LoadData();
            }
        }

        private void BtnReceive_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null && purchase.CommandeObject != null)
            {
                var commande = purchase.CommandeObject;
                
                if (commande.Statut == "Annulée")
                {
                    MessageBox.Show(
                        "Impossible de réceptionner une commande annulée.",
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                
                if (commande.Statut == "Reçue")
                {
                    MessageBox.Show(
                        "Cette commande a déjà été complètement reçue.",
                        "Information",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    return;
                }
                
                // Ouvrir une fenêtre de réception
                var receptionWindow = new PurchaseReceptionWindow(commande);
                if (receptionWindow.ShowDialog() == true)
                {
                    MessageBox.Show(
                        "✅ Commande réceptionnée avec succès !\nLe stock a été mis à jour.",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    LoadData();
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var purchase = button?.DataContext as Purchase;
            
            if (purchase != null && purchase.CommandeObject != null)
            {
                var commande = purchase.CommandeObject;
                
                if (commande.Statut != "Reçue")
                {
                    MessageBox.Show(
                        "⚠️ Cette commande n'a pas encore été complètement reçue.\n" +
                        "Statut actuel : " + commande.Statut,
                        "Attention",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                var result = MessageBox.Show(
                    $"Confirmer la réception de la commande {commande.NumeroCommande} ?\n\n" +
                    "Cette action confirme que tout est conforme.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show(
                        "✅ Commande confirmée !",
                        "Succès",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                    LoadData();
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
        public int Id { get; set; }
        public string NumeroCommande { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Fournisseur { get; set; } = string.Empty;
        public string MontantTotal { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public CommandeFournisseur? CommandeObject { get; set; }
    }
}

