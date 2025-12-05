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
    public partial class ClientsListView : UserControl
    {
        private List<ClientDisplay> allClients;
        private string searchPlaceholder = "Rechercher par Nom, Email ou Téléphone...";

        public ClientsListView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Récupérer les clients de base
                var clients = ClientService.GetAllClients();
                
                // Récupérer les stats pour enrichir
                var stats = ClientStatistiquesService.GetAllStatistiques();
                var statsDict = stats.ToDictionary(s => s.ClientId, s => s);

                allClients = new List<ClientDisplay>();

                foreach (var c in clients)
                {
                    var display = new ClientDisplay
                    {
                        Id = c.Id,
                        Nom = c.Nom,
                        Email = c.CourrielContact,
                        Telephone = c.Telephone,
                        Statut = c.Statut,
                        Type = c.Type // J'ajoute le Type pour le filtrage
                    };

                    // Enrichir avec les stats
                    if (statsDict.ContainsKey(c.Id))
                    {
                        var s = statsDict[c.Id];
                        display.ChiffreAffaires = s.ChiffreAffairesTotal.ToString("C");
                        display.DerniereInteraction = s.DateDerniereCommande?.ToString("yyyy-MM-dd") ?? "Aucune";
                    }
                    else
                    {
                        display.ChiffreAffaires = "0,00 $";
                        display.DerniereInteraction = "Aucune";
                    }

                    // Couleur du statut
                    if (c.Statut == "Actif") display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    else if (c.Statut == "Fidèle") display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                    else if (c.Statut == "Prospect") display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                    else display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B"));

                    allClients.Add(display);
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des clients : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (allClients == null) return;

            var filtered = allClients.AsEnumerable();

            // Filtre Recherche
            string searchText = TxtSearch.Text;
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != searchPlaceholder)
            {
                searchText = searchText.ToLower();
                filtered = filtered.Where(c => 
                    (c.Nom?.ToLower().Contains(searchText) ?? false) ||
                    (c.Email?.ToLower().Contains(searchText) ?? false) ||
                    (c.Telephone?.ToLower().Contains(searchText) ?? false));
            }

            // Filtre Statut
            if (CmbStatusFilter.SelectedItem is ComboBoxItem selectedStatusItem)
            {
                string statusContent = selectedStatusItem.Content.ToString();
                if (statusContent != "Tous les statuts")
                {
                    // Extraire le texte sans l'emoji (ex: "🔵 Prospect" -> "Prospect")
                    string statusText = statusContent.Length > 2 ? statusContent.Substring(2).Trim() : statusContent;
                    filtered = filtered.Where(c => c.Statut == statusText);
                }
            }

            // Filtre Type
            if (CmbTypeFilter.SelectedItem is ComboBoxItem selectedTypeItem)
            {
                string typeContent = selectedTypeItem.Content.ToString();
                if (typeContent != "Tous les types")
                {
                    // Extraire le texte sans l'emoji (ex: "👤 Particulier" -> "Particulier")
                    string typeText = typeContent.Length > 2 ? typeContent.Substring(2).Trim() : typeContent;
                    filtered = filtered.Where(c => c.Type == typeText);
                }
            }

            ClientsDataGrid.ItemsSource = filtered.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == searchPlaceholder)
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = searchPlaceholder;
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
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
            var client = button?.DataContext as ClientDisplay;
            
            if (client != null)
            {
                var detailsWindow = new ClientDetailsWindow(client.Id);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
                LoadData();
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.DataContext as ClientDisplay;
            
            if (client != null)
            {
                var result = MessageBox.Show($"Voulez-vous vraiment désactiver le client {client.Nom} ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    // Simulation pour l'instant, mais on pourrait appeler ClientService.UpdateStatut
                    MessageBox.Show($"Client {client.Nom} désactivé (Simulation).", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ClientsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem is ClientDisplay client)
            {
                var detailsWindow = new ClientDetailsWindow(client.Id);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
                LoadData(); // Recharger pour mettre à jour les données
            }
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            // Réinitialiser la recherche
            TxtSearch.Text = searchPlaceholder;
            TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));

            // Réinitialiser les filtres
            CmbStatusFilter.SelectedIndex = 0; // "Tous les statuts"
            CmbTypeFilter.SelectedIndex = 0; // "Tous les types"

            // Appliquer les filtres (qui vont tout afficher maintenant)
            ApplyFilters();
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
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public string ChiffreAffaires { get; set; } = string.Empty;
        public string DerniereInteraction { get; set; } = string.Empty;
    }
}