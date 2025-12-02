using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI
{
    public partial class ClientShoppingWindow : Window
    {
        private string clientName;
        private string clientEmail;
        private int clientId;
        private Panier panier;
        private List<Produit> tousProduits;
        private List<Commande> commandesClient;

        public ClientShoppingWindow(string name, string email)
        {
            InitializeComponent();
            this.clientName = name;
            this.clientEmail = email;
            this.panier = new Panier();
            
            // Récupérer l'ID du client
            var client = ClientService.GetClientByEmail(email);
            if (client != null)
            {
                this.clientId = client.Id;
            }
            
            WelcomeText.Text = $"Bienvenue, {name} !";
            
            // Charger les données
            LoadProduits();
            LoadCategories();
            LoadCommandes();
            
            // Initialiser l'interface
            UpdatePanierHeader();
            
            // Attendre que les contrôles soient complètement initialisés avant d'afficher le catalogue
            this.Loaded += (s, e) => 
            {
                // Attendre un peu plus pour s'assurer que tous les contrôles sont prêts
                Dispatcher.BeginInvoke(new Action(() => 
                {
                    AfficherCatalogue();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            };
        }

        #region Chargement des données

        private void LoadProduits()
        {
            try
            {
                tousProduits = ProduitService.GetAllProduits()
                    .Where(p => p.Statut == "Actif" && p.StockDisponible > 0)
                    .ToList();
                
                Console.WriteLine($"✅ {tousProduits.Count} produits chargés pour le client");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des produits : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                tousProduits = new List<Produit>();
            }
        }

        private void LoadCategories()
        {
            try
            {
                if (CmbCategorie == null) return;
                
                var categories = CategorieService.GetActiveCategories();
                CmbCategorie.Items.Clear();
                CmbCategorie.Items.Add(new ComboBoxItem { Content = "Toutes les catégories" });
                
                foreach (var cat in categories)
                {
                    CmbCategorie.Items.Add(new ComboBoxItem { Content = cat.Nom });
                }
                
                if (CmbCategorie.Items.Count > 0)
                {
                    CmbCategorie.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des catégories: {ex.Message}");
            }
        }

        private void LoadCommandes()
        {
            try
            {
                commandesClient = CommandeService.GetCommandesByClient(clientId);
                Console.WriteLine($"✅ {commandesClient.Count} commandes chargées pour le client");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des commandes : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                commandesClient = new List<Commande>();
            }
        }

        #endregion

        #region Navigation entre onglets

        private void BtnTabCatalogue_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnTabCatalogue);
            AfficherCatalogue();
        }

        private void BtnTabPanier_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnTabPanier);
            AfficherPanier();
        }

        private void BtnTabCommandes_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnTabCommandes);
            AfficherCommandes();
        }

        private void BtnTabHistorique_Click(object sender, RoutedEventArgs e)
        {
            SetActiveTab(BtnTabHistorique);
            AfficherHistorique();
        }

        private void SetActiveTab(Button activeButton)
        {
            // Réinitialiser tous les styles
            BtnTabCatalogue.Style = (Style)FindResource("TabButtonStyle");
            BtnTabPanier.Style = (Style)FindResource("TabButtonStyle");
            BtnTabCommandes.Style = (Style)FindResource("TabButtonStyle");
            BtnTabHistorique.Style = (Style)FindResource("TabButtonStyle");
            
            // Appliquer le style actif
            activeButton.Style = (Style)FindResource("ActiveTabButtonStyle");
            
            // Afficher/masquer les grilles
            CatalogueGrid.Visibility = activeButton == BtnTabCatalogue ? Visibility.Visible : Visibility.Collapsed;
            PanierGrid.Visibility = activeButton == BtnTabPanier ? Visibility.Visible : Visibility.Collapsed;
            CommandesGrid.Visibility = activeButton == BtnTabCommandes ? Visibility.Visible : Visibility.Collapsed;
            HistoriqueGrid.Visibility = activeButton == BtnTabHistorique ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnPanierHeader_Click(object sender, RoutedEventArgs e)
        {
            BtnTabPanier_Click(sender, e);
        }

        #endregion

        #region Catalogue

        private void AfficherCatalogue()
        {
            // Vérifier que tousProduits n'est pas null
            if (tousProduits == null)
            {
                tousProduits = new List<Produit>();
            }
            
            var produitsAffiches = tousProduits.AsEnumerable();
            
            // Filtre par recherche
            if (TxtSearch != null && TxtSearch.Text != "Rechercher un produit..." && !string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                var searchText = TxtSearch.Text.ToLower();
                produitsAffiches = produitsAffiches.Where(p =>
                    p.Nom.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.SKU.ToLower().Contains(searchText)
                );
            }
            
            // Filtre par catégorie
            if (CmbCategorie != null && CmbCategorie.SelectedItem != null && 
                CmbCategorie.SelectedItem is ComboBoxItem selectedCat && 
                selectedCat.Content != null &&
                selectedCat.Content.ToString() != "Toutes les catégories")
            {
                var categorieName = selectedCat.Content.ToString();
                produitsAffiches = produitsAffiches.Where(p => p.NomCategorie == categorieName);
            }
            
            // Convertir en format d'affichage avec les images
            var produitsDisplay = produitsAffiches.Select(p => new
            {
                p.Id,
                p.Nom,
                p.Description,
                p.Prix,
                p.StockDisponible,
                p.NomCategorie,
                p.SKU,
                p.ImagePath, // Ajout du chemin de l'image
                StockInfo = p.StockDisponible > 0 
                    ? $"✓ En stock ({p.StockDisponible} disponibles)" 
                    : "✗ Rupture de stock"
            }).ToList();
            
            // Vérifier que le contrôle est initialisé avant d'assigner
            if (ProductsItemsControl != null)
            {
                ProductsItemsControl.ItemsSource = produitsDisplay;
            }
            else
            {
                Console.WriteLine("⚠️ ProductsItemsControl n'est pas encore initialisé");
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            AfficherCatalogue();
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Rechercher un produit...")
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = "Rechercher un produit...";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void CmbCategorie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AfficherCatalogue();
        }

        private void BtnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext == null) return;
            
            try
            {
                // Récupérer les données du produit depuis le DataContext
                dynamic produitData = button.DataContext;
                int produitId = produitData.Id;
                var produit = tousProduits.FirstOrDefault(p => p.Id == produitId);
                
                if (produit == null)
                {
                    MessageBox.Show("Produit introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                // Vérifier le stock
                if (produit.StockDisponible <= 0)
                {
                    MessageBox.Show("Ce produit n'est plus en stock.", "Stock indisponible", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Créer un item de panier
                var panierItem = new PanierItem
                {
                    ProduitId = produit.Id,
                    SKU = produit.SKU,
                    Nom = produit.Nom,
                    Description = produit.Description,
                    Prix = produit.Prix,
                    Quantite = 1,
                    StockDisponible = produit.StockDisponible,
                    Categorie = produit.NomCategorie
                };
                
                // Ajouter au panier
                panier.AjouterProduit(panierItem);
                
                UpdatePanierHeader();
                MessageBox.Show($"✅ {produit.Nom} ajouté au panier !", 
                    "Produit ajouté", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Panier

        private void AfficherPanier()
        {
            if (panier.EstVide)
            {
                PanierItemsControl.ItemsSource = null;
                TxtPanierVide.Visibility = Visibility.Visible;
                BtnPasserCommande.IsEnabled = false;
            }
            else
            {
                PanierItemsControl.ItemsSource = panier.Items;
                TxtPanierVide.Visibility = Visibility.Collapsed;
                BtnPasserCommande.IsEnabled = true;
            }
            
            UpdatePanierSummary();
        }

        private void UpdatePanierSummary()
        {
            decimal sousTotal = panier.Total;
            decimal tps = sousTotal * 0.05m;
            decimal tvq = sousTotal * 0.09975m;
            decimal total = sousTotal + tps + tvq;
            
            TxtSousTotal.Text = sousTotal.ToString("C");
            TxtTPS.Text = tps.ToString("C");
            TxtTVQ.Text = tvq.ToString("C");
            TxtTotal.Text = total.ToString("C");
        }

        private void BtnIncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is PanierItem item)
            {
                try
                {
                    panier.ModifierQuantite(item.ProduitId, item.Quantite + 1);
                    AfficherPanier();
                    UpdatePanierHeader();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is PanierItem item)
            {
                try
                {
                    panier.ModifierQuantite(item.ProduitId, item.Quantite - 1);
                    AfficherPanier();
                    UpdatePanierHeader();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnRemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is PanierItem item)
            {
                var result = MessageBox.Show(
                    $"Voulez-vous retirer {item.Nom} du panier ?",
                    "Confirmer",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    panier.RetirerProduit(item.ProduitId);
                    AfficherPanier();
                    UpdatePanierHeader();
                }
            }
        }

        private void BtnViderPanier_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous vider complètement votre panier ?",
                "Confirmer",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                panier.Vider();
                AfficherPanier();
                UpdatePanierHeader();
            }
        }

        private void BtnPasserCommande_Click(object sender, RoutedEventArgs e)
        {
            if (panier.EstVide)
            {
                MessageBox.Show("Votre panier est vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            // Demander l'adresse de livraison
            var adresseWindow = new Window
            {
                Title = "Adresse de livraison",
                Width = 500,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            
            var stackPanel = new StackPanel { Margin = new Thickness(20) };
            
            stackPanel.Children.Add(new TextBlock 
            { 
                Text = "Adresse de livraison", 
                FontSize = 18, 
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            });
            
            var txtAdresse = new TextBox 
            { 
                Height = 100, 
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(0, 0, 0, 15)
            };
            stackPanel.Children.Add(txtAdresse);
            
            var btnConfirmer = new Button 
            { 
                Content = "Confirmer la commande", 
                Height = 40,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold
            };
            
            string adresseLivraison = "";
            btnConfirmer.Click += (s, args) =>
            {
                adresseLivraison = txtAdresse.Text;
                adresseWindow.DialogResult = true;
                adresseWindow.Close();
            };
            
            stackPanel.Children.Add(btnConfirmer);
            adresseWindow.Content = stackPanel;
            
            if (adresseWindow.ShowDialog() == true)
            {
                try
                {
                    // Calculer le total avec taxes
                    decimal sousTotal = panier.Total;
                    decimal tps = sousTotal * 0.05m;
                    decimal tvq = sousTotal * 0.09975m;
                    decimal totalAvecTaxes = sousTotal + tps + tvq;
                    
                    // Créer la commande
                    var commande = new Commande
                    {
                        ClientId = clientId,
                        DateCommande = DateTime.Now,
                        Statut = "Confirmée",
                        MontantTotal = totalAvecTaxes,
                        AdresseLivraison = adresseLivraison,
                        Notes = ""
                    };
                    
                    // Convertir les items du panier en lignes de commande
                    foreach (var item in panier.Items)
                    {
                        commande.Lignes.Add(new LigneCommande
                        {
                            ProduitId = item.ProduitId,
                            Quantite = item.Quantite,
                            PrixUnitaire = item.Prix,
                            SousTotal = item.SousTotal
                        });
                    }
                    
                    // Enregistrer la commande
                    int commandeId = CommandeService.CreateCommande(commande);
                    
                    MessageBox.Show(
                        $"✅ Commande créée avec succès !\n\n" +
                        $"Numéro de commande : {commande.NumeroCommande}\n" +
                        $"Montant total : {commande.MontantTotal:C}",
                        "Commande confirmée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    // Vider le panier
                    panier.Vider();
                    AfficherPanier();
                    UpdatePanierHeader();
                    
                    // Recharger les commandes
                    LoadCommandes();
                    
                    // Aller à l'onglet commandes
                    BtnTabCommandes_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la création de la commande : {ex.Message}", 
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdatePanierHeader()
        {
            BtnPanierHeader.Content = $"🛒 Panier ({panier.NombreArticles})";
        }

        #endregion

        #region Commandes

        private void AfficherCommandes()
        {
            // Filtrer les commandes non annulées
            var commandesActives = commandesClient
                .Where(c => c.Statut != "Annulée")
                .OrderByDescending(c => c.DateCommande)
                .ToList();
            
            CommandesDataGrid.ItemsSource = commandesActives;
        }

        private void BtnVoirDetailsCommande_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag is Commande commande)
            {
                var detailsWindow = new Window
                {
                    Title = $"Détails de la commande {commande.NumeroCommande}",
                    Width = 700,
                    Height = 500,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };
                
                var scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                var stackPanel = new StackPanel { Margin = new Thickness(20) };
                
                // Informations générales
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = $"Commande {commande.NumeroCommande}", 
                    FontSize = 20, 
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 15)
                });
                
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = $"Date : {commande.DateCommande:dd/MM/yyyy HH:mm}", 
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 5)
                });
                
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = $"Statut : {commande.Statut}", 
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 5)
                });
                
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = $"Adresse de livraison : {commande.AdresseLivraison ?? "Non spécifiée"}", 
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 20),
                    TextWrapping = TextWrapping.Wrap
                });
                
                // Lignes de commande
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = "Articles :", 
                    FontSize = 16, 
                    FontWeight = FontWeights.SemiBold,
                    Margin = new Thickness(0, 0, 0, 10)
                });
                
                foreach (var ligne in commande.Lignes)
                {
                    var border = new Border
                    {
                        Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8FAFC")),
                        Margin = new Thickness(0, 0, 0, 10),
                        Padding = new Thickness(10)
                    };
                    
                    var lignePanel = new StackPanel();
                    
                    lignePanel.Children.Add(new TextBlock 
                    { 
                        Text = $"{ligne.NomProduit} ({ligne.SKU})", 
                        FontSize = 14,
                        FontWeight = FontWeights.SemiBold
                    });
                    
                    lignePanel.Children.Add(new TextBlock 
                    { 
                        Text = $"Quantité : {ligne.Quantite} × {ligne.PrixUnitaire:C} = {ligne.SousTotal:C}", 
                        FontSize = 13,
                        Margin = new Thickness(0, 5, 0, 0)
                    });
                    
                    border.Child = lignePanel;
                    stackPanel.Children.Add(border);
                }
                
                // Total
                stackPanel.Children.Add(new TextBlock 
                { 
                    Text = $"Total : {commande.MontantTotal:C}", 
                    FontSize = 18, 
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 20, 0, 0)
                });
                
                scrollViewer.Content = stackPanel;
                detailsWindow.Content = scrollViewer;
                detailsWindow.ShowDialog();
            }
        }

        #endregion

        #region Historique

        private void AfficherHistorique()
        {
            // Toutes les commandes (y compris annulées)
            var historique = commandesClient
                .OrderByDescending(c => c.DateCommande)
                .Select(c => new
                {
                    c.NumeroCommande,
                    c.DateCommande,
                    c.Statut,
                    c.MontantTotal,
                    NombreArticles = c.Lignes.Sum(l => l.Quantite)
                })
                .ToList();
            
            HistoriqueDataGrid.ItemsSource = historique;
        }

        #endregion

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous vous déconnecter ?",
                "Déconnexion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
            }
        }
    }
}
