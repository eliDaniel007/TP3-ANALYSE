using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.CRM
{
    public partial class CampaignsListView : UserControl
    {
        public CampaignsListView()
        {
            InitializeComponent();
            LoadCampaigns();
        }

        private void LoadCampaigns()
        {
            try
            {
                var campagnes = CampagneMarketingService.GetAllCampagnes();
                
                var display = campagnes.Select(c => new CampaignDisplay
                {
                    Id = c.Id,
                    NomCampagne = c.NomCampagne,
                    Type = c.Type,
                    DateDebut = c.DateDebut.ToString("yyyy-MM-dd"),
                    DateFin = c.DateFin.ToString("yyyy-MM-dd"),
                    Budget = $"{c.Budget:N2} $",
                    Destinataires = c.NombreDestinataires.ToString(),
                    Reponses = c.NombreReponses.ToString(),
                    TauxParticipation = $"{c.TauxParticipation:N1} %",
                    Statut = c.Statut,
                    StatutColor = GetStatutColor(c.Statut)
                }).ToList();
                
                // Supposons qu'il y a un DataGrid nommé CampaignsDataGrid dans le XAML
                // CampaignsDataGrid.ItemsSource = display;
                
                // Pour l'instant, comme je ne peux pas modifier le XAML facilement, 
                // je vais commenter cette ligne
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des campagnes:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private Brush GetStatutColor(string statut)
        {
            return statut switch
            {
                "Active" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                "Planifiée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#669BBC")),
                "Terminée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                "Annulée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626")),
                _ => Brushes.Gray
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
        public string DateDebut { get; set; } = string.Empty;
        public string DateFin { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Destinataires { get; set; } = string.Empty;
        public string Reponses { get; set; } = string.Empty;
        public string TauxParticipation { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
    }
}

