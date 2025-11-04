using System;
using System.Windows;
using System.Windows.Input;

namespace PGI
{
    public partial class LoginWindow : Window
    {

        public LoginWindow()
        {
            InitializeComponent();
            
            // Permettre de déplacer la fenêtre
            this.MouseLeftButtonDown += (s, e) => { if (e.LeftButton == MouseButtonState.Pressed) this.DragMove(); };
            
            // Enter pour se connecter
            this.KeyDown += (s, e) => { if (e.Key == Key.Enter) LoginButton_Click(s, e); };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : PasswordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            // Désactiver le bouton pendant la connexion
            LoginButton.IsEnabled = false;
            LoginButton.Content = "⏳ Connexion...";

            try
            {
                // === AUTHENTIFICATION DÉSACTIVÉE ===
                // Tous les utilisateurs peuvent se connecter avec n'importe quels identifiants
                // Rôle par défaut : Admin (tous les accès)
                
                string role = "Administrateur";
                
                // Ouvrir la fenêtre de sélection du module
                    ModuleSelectionWindow moduleWindow = new ModuleSelectionWindow(username, role);
                    moduleWindow.Show();
                    this.Close();
            }
            catch (Exception ex)
            {
                ShowError($"Erreur: {ex.Message}");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginButton.Content = "🔐 Se connecter";
            }
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                // Afficher le mot de passe
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                TogglePasswordButton.Content = "🙈";
            }
            else
            {
                // Cacher le mot de passe
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                TogglePasswordButton.Content = "👁️";
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}

