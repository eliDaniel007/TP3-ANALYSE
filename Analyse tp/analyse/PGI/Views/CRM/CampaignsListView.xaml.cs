using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class CampaignsListView : UserControl
    {
        public CampaignsListView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var campagnes = CampagneMarketingService.GetAllCampagnes();
                var displayList = campagnes.Select(c => new CampaignDisplay
                {
                    Id = c.Id,
                    NomCampagne = c.NomCampagne,
                    Type = c.Type,
                    DateDebutFormate = c.DateDebut.ToString("yyyy-MM-dd"),
                    DateFinFormate = c.DateFin.ToString("yyyy-MM-dd"),
                    BudgetFormate = c.Budget.ToString("C"),
                    NombreDestinataires = c.NombreDestinataires,
                    NombreReponses = c.NombreReponses,
                    TauxParticipationFormate = c.TauxParticipation.ToString("P2"),
                    Statut = c.Statut,
                    StatutColor = GetStatutColor(c.Statut)
                }).ToList();

                CampaignsDataGrid.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des campagnes : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private SolidColorBrush GetStatutColor(string statut)
        {
            return statut switch
            {
                "Active" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                "Terminée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                "Annulée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")) // Planifiée
            };
        }

        private void BtnNewCampaign_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentCRMMainView(this);
            if (parent != null)
            {
                parent.NavigateToCampaignForm();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var campagne = button?.DataContext as CampaignDisplay;
            
            if (campagne != null)
            {
                var parent = FindParentCRMMainView(this);
                if (parent != null)
                {
                    // Charger le formulaire avec l'ID de la campagne
                    parent.LoadCampaignForm(campagne.Id);
                }
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var campagne = button?.DataContext as CampaignDisplay;
            
            if (campagne != null)
            {
                var detailsWindow = new CampaignDetailsWindow(campagne.Id);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
                LoadData(); // Recharger pour mettre à jour les données
            }
        }

        private void BtnCloturer_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var campagne = button?.DataContext as CampaignDisplay;
            
            if (campagne != null)
            {
                var result = MessageBox.Show($"Voulez-vous vraiment clôturer la campagne {campagne.NomCampagne} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        CampagneMarketingService.CloturerCampagne(campagne.Id);
                        MessageBox.Show("Campagne clôturée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la clôture : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var campagne = button?.DataContext as CampaignDisplay;
            
            if (campagne != null)
            {
                var result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer la campagne \"{campagne.NomCampagne}\" ?\n\nCette action est irréversible.", 
                    "Confirmation de suppression", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        CampagneMarketingService.SupprimerCampagne(campagne.Id);
                        MessageBox.Show("Campagne supprimée avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private CRMMainView? FindParentCRMMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is CRMMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as CRMMainView;
        }
    }

    public class CampaignDisplay
    {
        public int Id { get; set; }
        public string NomCampagne { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string DateDebutFormate { get; set; } = string.Empty;
        public string DateFinFormate { get; set; } = string.Empty;
        public string BudgetFormate { get; set; } = string.Empty;
        public int NombreDestinataires { get; set; }
        public int NombreReponses { get; set; }
        public string TauxParticipationFormate { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
    }
}

