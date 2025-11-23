using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class ProductHistoryView : UserControl
    {
        private int produitId;
        private Produit produit;

        public ProductHistoryView(int id)
        {
            InitializeComponent();
            produitId = id;
            LoadProduct();
            LoadMovements();
        }

        private void LoadProduct()
        {
            try
            {
                produit = ProduitService.GetProduitById(produitId);
                if (produit != null)
                {
                    TxtTitle.Text = $"🕐 Historique des mouvements - {produit.Nom} ({produit.SKU})";
                    Console.WriteLine($"✅ Produit chargé : {produit.Nom} - Stock disponible: {produit.StockDisponible}, Stock réservé: {produit.StockReservee}");
                }
                else
                {
                    Console.WriteLine($"⚠️ Produit {produitId} introuvable");
                    MessageBox.Show($"Produit introuvable (ID: {produitId})",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors du chargement du produit: {ex.Message}");
                MessageBox.Show($"Erreur lors du chargement du produit : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMovements()
        {
            try
            {
                var displayMovements = new List<MovementDisplay>();
                
                // Charger les mouvements depuis la base de données
                try
                {
                    var mouvements = MouvementStockService.GetMouvementsByProduitId(produitId);
                    Console.WriteLine($"✅ {mouvements.Count} mouvements chargés pour le produit {produitId}");
                    
                    // Convertir en format d'affichage
                    displayMovements = mouvements.Select(m => new MovementDisplay
                    {
                        DateMouvement = m.DateMouvement,
                        TypeMouvement = m.TypeMouvement,
                        TypeDisplay = m.TypeMouvement == "ENTREE" ? "ENTRÉE" : "SORTIE",
                        TypeColor = m.TypeMouvement == "ENTREE" 
                            ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"))
                            : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                        Quantite = m.Quantite,
                        QuantiteDisplay = m.TypeMouvement == "ENTREE" ? $"+{m.Quantite}" : $"-{m.Quantite}",
                        Raison = m.Raison,
                        RaisonDisplay = GetRaisonDisplay(m.Raison),
                        NoteUtilisateur = m.NoteUtilisateur ?? "",
                        NomEmploye = m.NomEmploye ?? "Système"
                    }).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Erreur lors du chargement des mouvements: {ex.Message}");
                    // Continuer même s'il n'y a pas de mouvements, on affichera au moins le stock actuel
                }

                // Toujours ajouter le stock actuel comme dernière entrée (en haut de la liste)
                if (produit != null)
                {
                    var stockActuel = new MovementDisplay
                    {
                        DateMouvement = DateTime.Now, // Date actuelle comme dernière entrée
                        TypeMouvement = "ENTREE",
                        TypeDisplay = "ENTRÉE",
                        TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                        Quantite = produit.StockDisponible,
                        QuantiteDisplay = $"+{produit.StockDisponible}",
                        Raison = "stock_actuel",
                        RaisonDisplay = "Stock actuel disponible",
                        NoteUtilisateur = $"Quantité actuelle en stock : {produit.StockDisponible} unités{(produit.StockReservee > 0 ? $". Stock réservé : {produit.StockReservee} unités" : "")}.",
                        NomEmploye = "Système"
                    };
                    
                    // Insérer au début de la liste (comme dernière entrée, donc en haut)
                    displayMovements.Insert(0, stockActuel);
                    
                    Console.WriteLine($"✅ Stock actuel ajouté : {produit.StockDisponible} unités");
                }
                else
                {
                    Console.WriteLine("⚠️ Produit non chargé, impossible d'afficher le stock actuel");
                }

                Console.WriteLine($"✅ Total de {displayMovements.Count} entrées à afficher");
                
                // Si aucune donnée, afficher un message dans l'interface
                if (displayMovements.Count == 0)
                {
                    Console.WriteLine("⚠️ Aucune donnée à afficher dans l'historique");
                    // Créer une entrée vide avec un message
                    var messageVide = new MovementDisplay
                    {
                        DateMouvement = DateTime.Now,
                        TypeMouvement = "INFO",
                        TypeDisplay = "INFO",
                        TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")),
                        Quantite = 0,
                        QuantiteDisplay = "-",
                        Raison = "aucun_mouvement",
                        RaisonDisplay = "Aucun mouvement enregistré",
                        NoteUtilisateur = "Il n'y a pas encore de mouvements de stock enregistrés pour ce produit.",
                        NomEmploye = "Système"
                    };
                    displayMovements.Add(messageVide);
                }
                
                MovementsDataGrid.ItemsSource = displayMovements;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'historique : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"❌ Erreur LoadMovements: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            }
        }

        private string GetRaisonDisplay(string raison)
        {
            return raison switch
            {
                "reception_achat" => "Réception - Achat",
                "vente" => "Vente",
                "ajustement" => "Ajustement inventaire",
                "retour_entree" => "Retour entrée",
                "retour_sortie" => "Retour sortie",
                "manuel" => "Ajustement manuel",
                _ => raison
            };
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentStocksMainView(this);
            if (parent != null)
            {
                // Utiliser la méthode publique pour naviguer vers la liste des produits
                parent.NavigateToProductsList();
            }
        }

        private StocksMainView FindParentStocksMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is StocksMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as StocksMainView;
        }
    }

    // Classe pour l'affichage des mouvements
    public class MovementDisplay
    {
        public DateTime DateMouvement { get; set; }
        public string TypeMouvement { get; set; }
        public string TypeDisplay { get; set; }
        public Brush TypeColor { get; set; }
        public int Quantite { get; set; }
        public string QuantiteDisplay { get; set; }
        public string Raison { get; set; }
        public string RaisonDisplay { get; set; }
        public string NoteUtilisateur { get; set; }
        public string NomEmploye { get; set; }
    }
}

