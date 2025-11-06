using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.CRM
{
    public partial class CampaignsListView : UserControl
    {
        public CampaignsListView()
        {
            InitializeComponent();
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
                parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return parent as CRMMainView;
        }
    }
}

