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

        private void LoadFournisseurs()
        {
            try
            {
                // Charger depuis la base de données
                fournisseurs = FournisseurService.GetAllFournisseurs();
                DisplayFournisseurs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des fournisseurs: {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadSampleData();
            }
        }

        private void DisplayFournisseurs()
        {
            var displaySuppliers = fournisseurs.Select(f => new SupplierDisplay
            {
                Id = f.Id,
                Code = f.Code,
                Nom = f.Nom,
                Email = f.CourrielContact,
                Delai = $"{f.DelaiLivraisonJours} jours",
                Escompte = $"{f.PourcentageEscompte}%",
                Statut = f.Statut
            }).ToList();

            SuppliersDataGrid.ItemsSource = displaySuppliers;
        }

        private void LoadSampleData()
        {
            // Données d'exemple si pas de connexion BDD
            var suppliers = new List<SupplierDisplay>
            {
                new SupplierDisplay { Id = 1, Code = "NS-001", Nom = "Nordic Supplies", Email = "contact@nordicsupplies.com", Delai = "7 jours", Escompte = "5%", Statut = "Actif" },
                new SupplierDisplay { Id = 2, Code = "AC-002", Nom = "Adventure Co.", Email = "info@adventureco.com", Delai = "10 jours", Escompte = "3%", Statut = "Actif" },
                new SupplierDisplay { Id = 3, Code = "MG-003", Nom = "Mountain Gear", Email = "sales@mountaingear.com", Delai = "5 jours", Escompte = "7%", Statut = "Actif" },
            };

            SuppliersDataGrid.ItemsSource = suppliers;
        }

        private void BtnAddSupplier_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Ouvrir une fenêtre d'ajout de fournisseur
            MessageBox.Show("Fonctionnalité d'ajout de fournisseur à implémenter",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var supplier = button?.DataContext as SupplierDisplay;
            
            if (supplier != null)
            {
                // TODO: Ouvrir une fenêtre d'édition
                MessageBox.Show($"Modification du fournisseur : {supplier.Nom}",
                    "Édition", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var supplier = button?.DataContext as SupplierDisplay;
            
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
                    }
                }
            }
        }
    }

    public class SupplierDisplay
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Delai { get; set; }
        public string Escompte { get; set; }
        public string Statut { get; set; }
    }
}

