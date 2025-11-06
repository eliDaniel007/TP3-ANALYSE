using System.Windows;

namespace PGI
{
    public partial class ModuleSelectionWindow : Window
    {
        private string userName;
        private string userRole;

        public ModuleSelectionWindow(string username, string role)
        {
            InitializeComponent();
            this.userName = username;
            this.userRole = role;
            
            // Afficher le nom d'utilisateur et rôle
            WelcomeUserText.Text = $"Connecté en tant que: {username} ({role})";
            
            // Gérer les accès selon le rôle
            ConfigureAccessByRole();
        }

        private void ConfigureAccessByRole()
        {
            // === MAQUETTE : Tout le monde a accès à TOUS les modules ===
            BtnStocksModule.Visibility = Visibility.Visible;
            BtnFinancesModule.Visibility = Visibility.Visible;
            BtnCRMModule.Visibility = Visibility.Visible;
        }

        private void BtnStocksModule_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir MainWindow avec le module Stocks
            MainWindow mainWindow = new MainWindow(userName, userRole, "stocks");
            mainWindow.Show();
            this.Close();
        }

        private void BtnFinancesModule_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir MainWindow avec le module Finances
            MainWindow mainWindow = new MainWindow(userName, userRole, "finances");
            mainWindow.Show();
            this.Close();
        }

        private void BtnCRMModule_Click(object sender, RoutedEventArgs e)
        {
            // Ouvrir MainWindow avec le module CRM
            MainWindow mainWindow = new MainWindow(userName, userRole, "crm");
            mainWindow.Show();
            this.Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la fenêtre de connexion
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

