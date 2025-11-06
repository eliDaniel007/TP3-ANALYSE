using System;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.CRM
{
    public partial class CRMMainView : UserControl
    {
        private Button activeSubNavButton;

        public CRMMainView()
        {
            InitializeComponent();
            // Charger le tableau de bord par défaut
            NavigateToDashboard();
        }

        // Navigation vers le tableau de bord
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            SetActiveSubNavButton(BtnDashboard);
            LoadView("Views/CRM/CRMDashboardView.xaml");
        }

        // Navigation vers Clients
        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnClients);
            LoadView("Views/CRM/ClientsListView.xaml");
        }

        // Navigation vers Campagnes
        private void BtnCampaigns_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnCampaigns);
            LoadView("Views/CRM/CampaignsListView.xaml");
        }

        // Navigation vers le formulaire client
        public void NavigateToClientForm()
        {
            SetActiveSubNavButton(BtnClients);
            LoadView("Views/CRM/ClientFormView.xaml");
        }

        // Navigation vers le formulaire campagne
        public void NavigateToCampaignForm()
        {
            SetActiveSubNavButton(BtnCampaigns);
            LoadView("Views/CRM/CampaignFormView.xaml");
        }

        // Méthodes utilitaires
        private void SetActiveSubNavButton(Button button)
        {
            // Réinitialiser le style de l'ancien bouton actif
            if (activeSubNavButton != null)
            {
                activeSubNavButton.Style = (Style)FindResource("SubNavButtonStyle");
            }

            // Appliquer le style actif au nouveau bouton
            button.Style = (Style)FindResource("ActiveSubNavButtonStyle");
            activeSubNavButton = button;
        }

        private void LoadView(string viewPath)
        {
            try
            {
                var uri = new Uri(viewPath, UriKind.Relative);
                var control = Application.LoadComponent(uri) as UserControl;
                SubContentArea.Content = control;
            }
            catch (Exception ex)
            {
                ShowTemporaryMessage($"Erreur lors du chargement de la vue : {ex.Message}");
            }
        }

        private void ShowTemporaryMessage(string message)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                FontSize = 18,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#64748B")
                ),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(50)
            };

            SubContentArea.Content = textBlock;
        }
    }
}

