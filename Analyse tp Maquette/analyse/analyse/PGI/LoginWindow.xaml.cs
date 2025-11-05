using System;
using System.Windows;
using System.Windows.Input;
using PGI.Services;

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
            string email = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : PasswordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Veuillez remplir tous les champs.");
                return;
            }

            // Désactiver le bouton pendant la connexion
            LoginButton.IsEnabled = false;
            LoginButton.Content = "⏳ Connexion...";

            try
            {
                // === AUTHENTIFICATION AVEC BASE DE DONNÉES ===
                
                // Vérifier si l'email contient "client" → C'est un CLIENT
                if (ClientService.IsClientEmail(email))
                {
                    var (success, nom, clientId) = ClientService.Authenticate(email, password);
                    
                    if (success)
                    {
                        // C'est un client → Rediriger vers le site d'achat
                        ClientShoppingWindow shoppingWindow = new ClientShoppingWindow(nom, email);
                        shoppingWindow.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        ShowError("❌ Identifiants incorrects. Vérifiez votre email et mot de passe.");
                    }
                }
                else
                {
                    // Sinon → C'est un EMPLOYÉ
                    var (success, nom, prenom, role) = EmployeService.Authenticate(email, password);
                    
                    if (success)
                    {
                        // C'est un employé → Rediriger vers le PGI
                        string fullName = $"{prenom} {nom}";
                        ModuleSelectionWindow moduleWindow = new ModuleSelectionWindow(fullName, role);
                        moduleWindow.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        ShowError("❌ Identifiants incorrects. Vérifiez votre email et mot de passe.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError($"❌ Erreur de connexion : {ex.Message}");
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

