using System.Windows;

namespace PGI
{
    public partial class ClientShoppingWindow : Window
    {
        private string clientName;
        private string clientEmail;

        public ClientShoppingWindow(string name, string email)
        {
            InitializeComponent();
            this.clientName = name;
            this.clientEmail = email;
            
            WelcomeText.Text = $"Bienvenue, {name} !";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Retour à la page de connexion
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

