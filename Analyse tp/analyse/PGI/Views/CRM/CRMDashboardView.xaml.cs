using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.CRM
{
    public partial class CRMDashboardView : UserControl
    {
        public CRMDashboardView()
        {
            InitializeComponent();
            // TODO: Charger les données depuis la base de données
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

