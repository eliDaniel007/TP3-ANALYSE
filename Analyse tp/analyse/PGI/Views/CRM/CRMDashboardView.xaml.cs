using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class CRMDashboardView : UserControl
    {
        public CRMDashboardView()
        {
            InitializeComponent();
            LoadKPIs();
            LoadAlertes();
        }

        private void LoadKPIs()
        {
            try
            {
                var kpis = ClientStatistiquesService.GetKPIsGlobaux();
                
                // Mettre à jour les TextBlocks du XAML avec les vrais KPIs
                // Note: Les noms des TextBlock doivent être ajoutés dans le XAML avec x:Name
                // Pour l'instant, les valeurs sont hardcodées dans le XAML
                
                // TODO: Ajouter x:Name dans le XAML et mettre à jour ici
                // Exemple:
                // TxtTotalClientsActifs.Text = kpis["NombreClientsActifs"].ToString();
                // TxtTauxFidelisation.Text = $"{kpis["TauxFidelisation"]:N1} %";
                // etc.
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des KPIs:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void LoadAlertes()
        {
            try
            {
                // Charger les alertes ouvertes
                var alertes = AlerteServiceClientService.GetAllAlertes("Ouverte");
                
                // TODO: Mettre à jour le DataGrid des alertes avec les vraies données
                // Pour cela, il faut ajouter x:Name au DataGrid dans le XAML
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des alertes:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
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
}

