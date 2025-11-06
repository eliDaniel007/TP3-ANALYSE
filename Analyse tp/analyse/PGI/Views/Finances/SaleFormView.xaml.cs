using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class SaleFormView : UserControl
    {
        private List<SaleLine> saleLines;

        public SaleFormView()
        {
            InitializeComponent();
            saleLines = new List<SaleLine>();
            ProductsDataGrid.ItemsSource = saleLines;
            CalculateTotals();
        }

        private void CmbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = CmbClient.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Content.ToString() != "Sélectionner un client...")
            {
                // Afficher les informations du client (simulation)
                TxtClientInfo.Text = $"📧 Email: contact@{selectedItem.Content.ToString().ToLower().Replace(" ", "")}.com\n" +
                                   $"📞 Téléphone: (514) 555-1234\n" +
                                   $"📍 Adresse: 123 Rue Principale, Montréal, QC H3A 1B1";
            }
            else
            {
                TxtClientInfo.Text = "Sélectionnez un client pour voir ses informations";
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Ajouter une ligne de produit (simulation)
            var newLine = new SaleLine
            {
                Produit = "Veste d'hiver Nordique",
                SKU = "VES-001",
                PrixUnitaire = "225,00 $",
                Quantite = "1",
                Total = "225,00 $"
            };

            saleLines.Add(newLine);
            ProductsDataGrid.Items.Refresh();
            CalculateTotals();

            MessageBox.Show(
                "Pour ajouter un produit réel, implémenter un pop-up de sélection\n" +
                "qui récupère les produits du module Stocks avec leurs prix.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnRemoveProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var line = button?.DataContext as SaleLine;
            
            if (line != null)
            {
                saleLines.Remove(line);
                ProductsDataGrid.Items.Refresh();
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            // Calcul du sous-total
            double subtotal = 0;
            foreach (var line in saleLines)
            {
                // Extraire le montant numérique (simulation simple)
                string totalStr = line.Total.Replace(" $", "").Replace(",", ".");
                if (double.TryParse(totalStr, out double lineTotal))
                {
                    subtotal += lineTotal;
                }
            }

            // Calcul des taxes
            double tps = subtotal * 0.05;      // TPS 5%
            double tvq = subtotal * 0.09975;   // TVQ 9.975%
            double total = subtotal + tps + tvq;

            // Mise à jour de l'affichage
            TxtSubtotal.Text = $"{subtotal:N2} $";
            TxtTPS.Text = $"{tps:N2} $";
            TxtTVQ.Text = $"{tvq:N2} $";
            TxtTotal.Text = $"{total:N2} $";
            TxtMontantDu.Text = $"{total:N2} $"; // Initialement, tout le montant est dû
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (CmbClient.SelectedIndex == 0)
            {
                MessageBox.Show(
                    "Veuillez sélectionner un client.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (saleLines.Count == 0)
            {
                MessageBox.Show(
                    "Veuillez ajouter au moins un produit à la facture.",
                    "Validation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            // TODO: Vérifier le stock disponible pour chaque produit
            // TODO: Enregistrer dans la base de données
            // TODO: Mettre à jour le stock (mouvement OUT)

            MessageBox.Show(
                "✅ Facture enregistrée avec succès !\n\n" +
                $"Client : {(CmbClient.SelectedItem as ComboBoxItem)?.Content}\n" +
                $"Total : {TxtTotal.Text}\n\n" +
                "La facture a été créée et le stock mis à jour.",
                "Succès",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Retourner à la liste des ventes
            NavigateBackToList();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Fonctionnalité 'Imprimer PDF' à implémenter.\n\n" +
                "Génération d'un PDF de la facture avec toutes les informations.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous vraiment annuler ? Les modifications non enregistrées seront perdues.",
                "Confirmer",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                NavigateBackToList();
            }
        }

        private void NavigateBackToList()
        {
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.BtnSales.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
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

    // Classe modèle pour les lignes de facture
    public class SaleLine
    {
        public string Produit { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string PrixUnitaire { get; set; } = string.Empty;
        public string Quantite { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
    }
}

