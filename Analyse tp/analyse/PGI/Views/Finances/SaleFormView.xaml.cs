using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class SaleFormView : UserControl
    {
        private List<SaleLine> saleLines;
        private List<Client> clients;
        private List<Produit> produits;
        private string numeroFacture;

        public SaleFormView()
        {
            InitializeComponent();
            saleLines = new List<SaleLine>();
            ProductsDataGrid.ItemsSource = saleLines;
            
            // Générer le numéro de facture
            numeroFacture = FactureService.GenererNumeroFacture();
            // TxtNumeroFacture.Text = numeroFacture; // TODO: Ajouter ce TextBox dans le XAML si nécessaire
            
            // Charger les données
            LoadClients();
            LoadProduits();
            
            CalculateTotals();
        }

        private void LoadClients()
        {
            try
            {
                clients = ClientService.GetAllClients();
                
                // Ajouter un élément par défaut
                CmbClient.Items.Clear();
                CmbClient.Items.Add(new ComboBoxItem { Content = "Sélectionner un client..." });
                
                // Ajouter les clients actifs uniquement
                foreach (var client in clients.Where(c => c.Statut == "Actif"))
                {
                    var item = new ComboBoxItem 
                    { 
                        Content = client.Nom,
                        Tag = client
                    };
                    CmbClient.Items.Add(item);
                }
                
                CmbClient.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des clients:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void LoadProduits()
        {
            try
            {
                produits = ProduitService.GetAllProduits()
                    .Where(p => p.Statut == "Actif")
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des produits:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                produits = new List<Produit>();
            }
        }

        private void CmbClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = CmbClient.SelectedItem as ComboBoxItem;
            if (selectedItem != null && selectedItem.Tag is Client client)
            {
                // Afficher les vraies informations du client
                TxtClientInfo.Text = $"📧 Email: {client.CourrielContact}\n" +
                                   $"📞 Téléphone: {client.Telephone ?? "N/A"}\n" +
                                   $"📊 Statut: {client.Statut}\n" +
                                   $"📅 Client depuis: {client.DateCreation:dd/MM/yyyy}";
            }
            else
            {
                TxtClientInfo.Text = "Sélectionnez un client pour voir ses informations";
            }
        }

        private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Créer une fenêtre de sélection de produits
            var selectWindow = new ProductSelectionWindow(produits);
            if (selectWindow.ShowDialog() == true && selectWindow.SelectedProduct != null)
            {
                var produit = selectWindow.SelectedProduct;
                int quantite = selectWindow.Quantity;
                
                // Vérifier si le produit existe déjà dans la facture
                var existingLine = saleLines.FirstOrDefault(l => l.ProduitId == produit.Id);
                if (existingLine != null)
                {
                    // Augmenter la quantité
                    int newQte = int.Parse(existingLine.Quantite) + quantite;
                    existingLine.Quantite = newQte.ToString();
                    existingLine.Total = $"{newQte * produit.Prix:N2} $";
                }
                else
                {
                    // Ajouter une nouvelle ligne
                    var newLine = new SaleLine
                    {
                        ProduitId = produit.Id,
                        Produit = produit.Nom,
                        SKU = produit.SKU,
                        PrixUnitaire = $"{produit.Prix:N2} $",
                        Quantite = quantite.ToString(),
                        Total = $"{quantite * produit.Prix:N2} $",
                        PrixUnitaireDecimal = produit.Prix,
                        StockDisponible = produit.StockDisponible
                    };
                    saleLines.Add(newLine);
                }
                
                ProductsDataGrid.Items.Refresh();
                CalculateTotals();
            }
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
            decimal subtotal = 0;
            foreach (var line in saleLines)
            {
                subtotal += line.PrixUnitaireDecimal * decimal.Parse(line.Quantite);
            }

            // Calcul des taxes avec les taux réels de la base de données
            var (tps, tvq, total) = TaxesService.CalculerTaxes(subtotal);

            // Mise à jour de l'affichage
            TxtSubtotal.Text = $"{subtotal:N2} $";
            TxtTPS.Text = $"{tps:N2} $";
            TxtTVQ.Text = $"{tvq:N2} $";
            TxtTotal.Text = $"{total:N2} $";
            TxtMontantDu.Text = $"{total:N2} $";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation client
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

            var selectedItem = CmbClient.SelectedItem as ComboBoxItem;
            var client = selectedItem?.Tag as Client;
            if (client == null)
            {
                MessageBox.Show("Erreur: Client invalide", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation produits
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

            try
            {
                // Créer la facture
                var facture = new Facture
                {
                    NumeroFacture = numeroFacture,
                    DateFacture = DateTime.Now,
                    DateEcheance = DateTime.Now.AddDays(30), // 30 jours par défaut
                    ClientId = client.Id,
                    EmployeId = null, // TODO: Récupérer l'employé connecté
                    ConditionsPaiement = "Net 30 jours"
                };

                // Créer les lignes de facture
                var lignes = new List<LigneFacture>();
                foreach (var line in saleLines)
                {
                    lignes.Add(new LigneFacture
                    {
                        ProduitId = line.ProduitId,
                        SKU = line.SKU,
                        Description = line.Produit,
                        Quantite = int.Parse(line.Quantite),
                        PrixUnitaire = line.PrixUnitaireDecimal
                    });
                }

                // Sauvegarder dans la base de données
                // Cette méthode effectue toutes les validations automatiquement:
                // - Stock disponible
                // - Client actif
                // - Factures en retard
                // - Mise à jour du stock
                // - Création des mouvements
                int factureId = FactureService.CreerFacture(facture, lignes);

                MessageBox.Show(
                    $"✅ Facture {numeroFacture} créée avec succès !\n\n" +
                    $"Client : {client.Nom}\n" +
                    $"Total : {TxtTotal.Text}\n" +
                    $"Nombre de lignes : {lignes.Count}\n\n" +
                    "Le stock a été automatiquement mis à jour.",
                    "Succès",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                // Retourner à la liste des ventes
                NavigateBackToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"❌ Erreur lors de la création de la facture:\n\n{ex.Message}\n\n" +
                    "Vérifiez que:\n" +
                    "- Le stock est suffisant pour tous les produits\n" +
                    "- Le client n'a pas de factures en retard\n" +
                    "- Le client est actif",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
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

    // Classe modèle pour les lignes de facture (affichage)
    public class SaleLine
    {
        public int ProduitId { get; set; }
        public string Produit { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string PrixUnitaire { get; set; } = string.Empty;
        public string Quantite { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;
        public decimal PrixUnitaireDecimal { get; set; }
        public int StockDisponible { get; set; }
    }
}

