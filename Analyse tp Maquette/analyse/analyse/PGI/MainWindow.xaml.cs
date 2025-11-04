using System.Windows;
using System.Windows.Controls;

namespace PGI
{
    public partial class MainWindow : Window
    {
        private string userName;
        private string userRole;
        private Button activeMenuButton;

        public MainWindow(string username, string role, string initialModule = null)
        {
            InitializeComponent();
            this.userName = username;
            this.userRole = role;

            // Afficher les informations utilisateur
            TxtUserName.Text = username;
            TxtUserRole.Text = role;

            // Charger le module initial si spécifié, sinon tableau de bord
            if (!string.IsNullOrEmpty(initialModule))
            {
                HideOtherModules(initialModule);
                NavigateToInitialModule(initialModule);
            }
            else
            {
                NavigateToDashboard();
            }
        }

        private void HideOtherModules(string activeModule)
        {
            // Masquer tous les boutons de modules sauf celui actif et le tableau de bord
            BtnDashboard.Visibility = Visibility.Visible; // Toujours visible pour revenir
            BtnStocks.Visibility = Visibility.Collapsed;
            BtnFinances.Visibility = Visibility.Collapsed;
            BtnCRM.Visibility = Visibility.Collapsed;

            // Afficher uniquement le module actif
            switch (activeModule.ToLower())
            {
                case "stocks":
                    BtnStocks.Visibility = Visibility.Visible;
                    break;
                case "finances":
                    BtnFinances.Visibility = Visibility.Visible;
                    break;
                case "crm":
                    BtnCRM.Visibility = Visibility.Visible;
                    break;
            }
        }

        // Navigation vers le tableau de bord
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            SetActiveButton(BtnDashboard);
            TxtModuleTitle.Text = "Tableau de bord";
            TxtModuleSubtitle.Text = "Vue d'ensemble de votre entreprise";
            
            // Charger DashboardView
            LoadView("Views/Dashboard/DashboardView.xaml");
        }

        private void NavigateToInitialModule(string module)
        {
            switch (module.ToLower())
            {
                case "stocks":
                    BtnStocks.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
                case "finances":
                    BtnFinances.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
                case "crm":
                    BtnCRM.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
                    break;
                default:
                    NavigateToDashboard();
                    break;
            }
        }

        // Navigation vers Stocks
        private void BtnStocks_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnStocks);
            TxtModuleTitle.Text = "📦 Stocks & Produits";
            TxtModuleSubtitle.Text = "Gestion des produits, fournisseurs et inventaire";
            
            // Charger StocksMainView
            LoadView("Views/Stocks/StocksMainView.xaml");
        }

        // Navigation vers Finances
        private void BtnFinances_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnFinances);
            TxtModuleTitle.Text = "💰 Finances & Ventes";
            TxtModuleSubtitle.Text = "Facturation, paiements et comptabilité";
            
            // Charger FinancesMainView
            LoadView("Views/Finances/FinancesMainView.xaml");
        }

        // Navigation vers CRM
        private void BtnCRM_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnCRM);
            TxtModuleTitle.Text = "👥 Clients (CRM)";
            TxtModuleSubtitle.Text = "Gestion de la relation client";
            
            // Charger CRMMainView
            LoadView("Views/CRM/CRMMainView.xaml");
        }

        // Navigation vers Paramètres
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            SetActiveButton(BtnSettings);
            TxtModuleTitle.Text = "⚙️ Paramètres";
            TxtModuleSubtitle.Text = "Configuration du système";
            
            // TODO: Charger SettingsView
            ShowTemporaryMessage("Module Paramètres en cours de développement...");
        }

        // Quitter le module
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Voulez-vous quitter ce module ?",
                "Quitter",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                ModuleSelectionWindow moduleWindow = new ModuleSelectionWindow(userName, userRole);
                moduleWindow.Show();
                this.Close();
            }
        }

        // Gestion de la recherche globale
        private void TxtGlobalSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtGlobalSearch.Text == "Rechercher...")
            {
                TxtGlobalSearch.Text = "";
                TxtGlobalSearch.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E293B")
                );
            }
        }

        private void TxtGlobalSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtGlobalSearch.Text))
            {
                TxtGlobalSearch.Text = "Rechercher...";
                TxtGlobalSearch.Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#94A3B8")
                );
            }
        }

        // Méthodes utilitaires
        private void SetActiveButton(Button button)
        {
            // Réinitialiser le style de l'ancien bouton actif
            if (activeMenuButton != null)
            {
                activeMenuButton.Style = (Style)FindResource("MenuButtonStyle");
            }

            // Appliquer le style actif au nouveau bouton
            button.Style = (Style)FindResource("ActiveMenuButtonStyle");
            activeMenuButton = button;
        }

        private void LoadView(string viewPath)
        {
            try
            {
                // Charger le UserControl depuis le chemin XAML
                var uri = new System.Uri(viewPath, System.UriKind.Relative);
                var control = System.Windows.Application.LoadComponent(uri) as UserControl;
                MainContentArea.Content = control;
            }
            catch (System.Exception ex)
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

            MainContentArea.Content = textBlock;
        }
    }
}
