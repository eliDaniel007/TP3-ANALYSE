using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Stocks
{
    public partial class SuppliersView : UserControl
    {
        private List<Fournisseur> fournisseurs;

        public SuppliersView()
        {
            InitializeComponent();
            LoadFournisseurs();
        }

        public void LoadFournisseurs()
        {
            try
            {
                // Charger depuis la base de données
                fournisseurs = FournisseurService.GetAllFournisseurs();
                DisplayFournisseurs();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur BDD (fournisseurs): {ex.Message}");
                LoadSampleData();
            }
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Naviguer vers le formulaire fournisseur (mode ajout)
                var parent = FindParentStocksMainView(this);
                if (parent != null)
                {
                    parent.NavigateToSupplierForm(null);
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de trouver la vue parente.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Erreur BtnAddSupplier_Click: parent est null");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du formulaire : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur BtnAddSupplier_Click: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var supplier = button?.DataContext as SupplierDisplay;
                
                if (supplier != null)
                {
                    // Naviguer vers le formulaire d'édition avec l'ID du fournisseur
                    var parent = FindParentStocksMainView(this);
                    if (parent != null)
                    {
                        parent.NavigateToSupplierForm(supplier.Id);
                    }
                    else
                    {
                        MessageBox.Show("Erreur : Impossible de trouver la vue parente.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        Console.WriteLine($"Erreur BtnEdit_Click: parent est null pour fournisseur {supplier.Id}");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de récupérer les informations du fournisseur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Erreur BtnEdit_Click: supplier est null");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture du formulaire : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur BtnEdit_Click: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                if (button == null)
                {
                    MessageBox.Show("Erreur : Bouton introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var supplier = button.DataContext as SupplierDisplay;
                
                if (supplier != null)
                {
                    var result = MessageBox.Show(
                        $"⚠️ Voulez-vous vraiment supprimer le fournisseur '{supplier.Nom}' ?\n\n" +
                        $"Code : {supplier.Code}\n" +
                        $"Email : {supplier.Email}\n\n" +
                        $"Cette action est irréversible !",
                        "Confirmer la suppression",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            bool success = FournisseurService.DeleteFournisseur(supplier.Id);
                            if (success)
                            {
                                LoadFournisseurs();
                                MessageBox.Show($"✅ Le fournisseur '{supplier.Nom}' a été supprimé avec succès.",
                                    "Suppression réussie", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"❌ Erreur lors de la suppression : {ex.Message}",
                                "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                            Console.WriteLine($"Erreur BtnDelete_Click: {ex.Message}");
                            Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Erreur : Impossible de récupérer les informations du fournisseur.\n\nLe DataContext est null.", 
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    Console.WriteLine("Erreur BtnDelete_Click: supplier est null");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Erreur inattendue : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine($"Erreur inattendue BtnDelete_Click: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void BtnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le fournisseur sélectionné
            var button = sender as Button;
            var supplier = button?.DataContext as SupplierDisplay;
            
            if (supplier != null)
            {
                try
                {
                    // Charger le fournisseur depuis la BDD pour obtenir le statut actuel
                    var fournisseur = FournisseurService.GetFournisseurById(supplier.Id);
                    if (fournisseur == null)
                    {
                        MessageBox.Show("Fournisseur introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Déterminer le nouveau statut
                    string nouveauStatut;
                    string message;
                    if (fournisseur.Statut == "Actif")
                    {
                        nouveauStatut = "Inactif";
                        message = $"⚠️ Voulez-vous vraiment désactiver ce fournisseur ?\n\n" +
                                 $"Code : {supplier.Code}\n" +
                                 $"Nom : {supplier.Nom}\n\n" +
                                 $"Le fournisseur ne sera plus disponible pour les commandes.";
                    }
                    else
                    {
                        nouveauStatut = "Actif";
                        message = $"✅ Voulez-vous réactiver ce fournisseur ?\n\n" +
                                 $"Code : {supplier.Code}\n" +
                                 $"Nom : {supplier.Nom}\n\n" +
                                 $"Le fournisseur sera à nouveau disponible pour les commandes.";
                    }

                    var result = MessageBox.Show(
                        message,
                        nouveauStatut == "Inactif" ? "Confirmer la désactivation" : "Confirmer la réactivation",
                        MessageBoxButton.YesNo,
                        nouveauStatut == "Inactif" ? MessageBoxImage.Warning : MessageBoxImage.Question
                    );

                    if (result == MessageBoxResult.Yes)
                    {
                        // Mettre à jour le statut
                        fournisseur.Statut = nouveauStatut;
                        bool success = FournisseurService.UpdateFournisseur(fournisseur);

                        if (success)
                        {
                            // Recharger la liste
                            LoadFournisseurs();

                            MessageBox.Show(
                                $"✅ Le fournisseur '{supplier.Nom}' a été {(nouveauStatut == "Inactif" ? "désactivé" : "réactivé")} avec succès.",
                                nouveauStatut == "Inactif" ? "Désactivation réussie" : "Réactivation réussie",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information
                            );
                        }
                        else
                        {
                            MessageBox.Show(
                                $"❌ Erreur lors de la {(nouveauStatut == "Inactif" ? "désactivation" : "réactivation")} du fournisseur.",
                                "Erreur",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"❌ Erreur : {ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    Console.WriteLine($"Erreur lors du changement de statut: {ex.Message}");
                }
            }
        }

        private void DisplayFournisseurs()
        {
            var displayList = fournisseurs.Select(f => new SupplierDisplay
            {
                Id = f.Id,
                Code = f.Code,
                Nom = f.Nom,
                Email = f.CourrielContact,
                Delai = $"{f.DelaiLivraisonJours} jours",
                Escompte = $"{f.PourcentageEscompte:0.#}%",
                Statut = f.Statut,
                StatutColor = f.Statut == "Actif" 
                    ? new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#10B981"))
                    : new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#94A3B8")),
                IsInactif = f.Statut != "Actif"
            }).ToList();

            SuppliersDataGrid.ItemsSource = displayList;
        }

        private void LoadSampleData()
        {
            fournisseurs = new List<Fournisseur>
            {
                new Fournisseur 
                { 
                    Id = 1, 
                    Code = "FOUR-001", 
                    Nom = "Columbia Sportswear", 
                    CourrielContact = "contact@columbia.com",
                    DelaiLivraisonJours = 7,
                    PourcentageEscompte = 2.5m,
                    Statut = "Actif"
                },
                new Fournisseur 
                { 
                    Id = 2, 
                    Code = "FOUR-002", 
                    Nom = "The North Face Distribution", 
                    CourrielContact = "sales@northface.com",
                    DelaiLivraisonJours = 10,
                    PourcentageEscompte = 3.0m,
                    Statut = "Actif"
                },
                new Fournisseur 
                { 
                    Id = 3, 
                    Code = "FOUR-003", 
                    Nom = "Patagonia Supplies", 
                    CourrielContact = "orders@patagonia.com",
                    DelaiLivraisonJours = 14,
                    PourcentageEscompte = 2.0m,
                    Statut = "Inactif"
                }
            };
            DisplayFournisseurs();
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

    public class SupplierDisplay
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Delai { get; set; } = string.Empty;
        public string Escompte { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public System.Windows.Media.Brush StatutColor { get; set; } = System.Windows.Media.Brushes.Gray;
        public bool IsInactif { get; set; }
    }
}

