using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.CRM
{
    public partial class ClientsListView : UserControl
    {
        public ClientsListView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var clients = new List<Client>
            {
                new Client { Nom = "Jean Dupont", Email = "jean.dupont@email.com", Telephone = "(514) 555-1234", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), ChiffreAffaires = "12 450 $", DerniereInteraction = "2025-01-27" },
                new Client { Nom = "Marie Tremblay", Email = "marie.t@email.com", Telephone = "(514) 555-5678", Statut = "Fidèle", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")), ChiffreAffaires = "28 900 $", DerniereInteraction = "2025-01-25" },
                new Client { Nom = "Tech Solutions Inc.", Email = "info@techsolutions.com", Telephone = "(514) 555-9012", Statut = "Actif", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), ChiffreAffaires = "45 200 $", DerniereInteraction = "2025-01-26" },
                new Client { Nom = "Pierre Gagnon", Email = "p.gagnon@email.com", Telephone = "(514) 555-3456", Statut = "Prospect", StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#669BBC")), ChiffreAffaires = "0 $", DerniereInteraction = "2025-01-28" },
            };

            ClientsDataGrid.ItemsSource = clients;
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
            var client = button?.DataContext as Client;
            
            if (client != null)
            {
                MessageBox.Show($"Ouvrir la fiche client : {client.Nom}", "Édition", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var client = button?.DataContext as Client;
            
            if (client != null)
            {
                MessageBox.Show($"Désactiver le client : {client.Nom}", "Désactivation", MessageBoxButton.OK, MessageBoxImage.Information);
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

    public class Client
    {
        public string Nom { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public string ChiffreAffaires { get; set; } = string.Empty;
        public string DerniereInteraction { get; set; } = string.Empty;
    }
}

