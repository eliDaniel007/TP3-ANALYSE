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
    public partial class ClientsListView : UserControl
    {
        private List<ClientDisplay> allClients;

        public ClientsListView()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            try
            {
                // Charger les clients avec leurs statistiques
                var statistiques = ClientStatistiquesService.GetAllStatistiques();
                
                allClients = statistiques.Select(s => new ClientDisplay
                {
                    ClientId = s.ClientId,
                    Nom = s.NomClient,
                    Email = "", // TODO: Charger depuis la table clients
                    Telephone = "", // TODO: Charger depuis la table clients
                    Statut = s.Statut,
                    StatutColor = GetStatutColor(s.Statut),
                    ChiffreAffaires = $"{s.ChiffreAffairesTotal:N2} $",
                    PanierMoyen = $"{s.PanierMoyen:N2} $",
                    NombreCommandes = s.NombreCommandes.ToString(),
                    ScoreComposite = s.ScoreComposite,
                    DerniereInteraction = s.DateDerniereCommande?.ToString("yyyy-MM-dd") ?? "Jamais"
                }).ToList();

                ClientsDataGrid.ItemsSource = allClients;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des clients:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                allClients = new List<ClientDisplay>();
                ClientsDataGrid.ItemsSource = allClients;
            }
        }

        private Brush GetStatutColor(string statut)
        {
            return statut switch
            {
                "Actif" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                "Fidèle" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),
                "Prospect" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#669BBC")),
                "Inactif" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                _ => Brushes.Gray
            };
        }

        private void BtnAddClient_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentCRMMainView(this);
            if (parent != null)
            {
                parent.NavigateToClientForm();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clientDisplay = button?.DataContext as ClientDisplay;
            
            if (clientDisplay != null)
            {
                // Ouvrir la fenêtre de détails du client
                try
                {
                    var detailsWindow = new ClientDetailsWindow(clientDisplay.ClientId);
                    detailsWindow.ShowDialog();
                    
                    // Recharger la liste après fermeture
                    LoadClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erreur lors de l'ouverture de la fiche client:\n{ex.Message}",
                        "Erreur",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var clientDisplay = button?.DataContext as ClientDisplay;
            
            if (clientDisplay != null)
            {
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir désactiver le client '{clientDisplay.Nom}' ?\n\n" +
                    "Note: Si le client a des ventes associées, il sera marqué comme 'Inactif' mais ne sera pas supprimé.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        ClientService.DesactiverClient(clientDisplay.ClientId);
                        MessageBox.Show(
                            "Client désactivé avec succès.",
                            "Succès",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                        LoadClients();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Erreur lors de la désactivation du client:\n{ex.Message}",
                            "Erreur",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        private CRMMainView? FindParentCRMMainView(DependencyObject child)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is CRMMainView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as CRMMainView;
        }
    }

    public class ClientDisplay
    {
        public int ClientId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public string ChiffreAffaires { get; set; } = string.Empty;
        public string PanierMoyen { get; set; } = string.Empty;
        public string NombreCommandes { get; set; } = string.Empty;
        public decimal ScoreComposite { get; set; }
        public string DerniereInteraction { get; set; } = string.Empty;
    }
}

