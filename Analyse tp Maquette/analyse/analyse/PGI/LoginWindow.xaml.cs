using System;
using System.Collections.Generic;
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
                // === AUTHENTIFICATION SIMPLIFIÉE ===
                // Mode développement : vérification basique avec données de test
                
                // Vérifier si c'est un employé (identifiants de test)
                if (IsEmployee(username, password))
                {
                    // C'est un employé → Rediriger vers le PGI
                    string role = GetEmployeeRole(username);
                    ModuleSelectionWindow moduleWindow = new ModuleSelectionWindow(username, role);
                    moduleWindow.Show();
                    this.Close();
                    return;
                }
                
                // Vérifier si c'est un client (identifiants de test)
                if (IsClient(username, password))
                {
                    // C'est un client → Rediriger vers le site d'achat
                    string clientName = GetClientName(username);
                    string clientEmail = username; // Email utilisé comme username pour les clients
                    ClientShoppingWindow shoppingWindow = new ClientShoppingWindow(clientName, clientEmail);
                    shoppingWindow.Show();
                    this.Close();
                    return;
                }
                
                // Aucun utilisateur trouvé
                ShowError("Identifiants incorrects. Veuillez réessayer.");
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

        // Vérifier si c'est un employé (données de test)
        private bool IsEmployee(string username, string password)
        {
            // Employés de test
            var employees = new Dictionary<string, string>
            {
                { "admin", "admin123" },
                { "gestionnaire", "gestionnaire123" },
                { "employe", "employe123" },
                { "comptable", "comptable123" }
            };
            
            return employees.ContainsKey(username.ToLower()) && 
                   employees[username.ToLower()] == password;
        }

        // Obtenir le rôle d'un employé
        private string GetEmployeeRole(string username)
        {
            var roles = new Dictionary<string, string>
            {
                { "admin", "Administrateur" },
                { "gestionnaire", "Gestionnaire" },
                { "employe", "Employé" },
                { "comptable", "Comptable" }
            };
            
            return roles.ContainsKey(username.ToLower()) 
                ? roles[username.ToLower()] 
                : "Employé";
        }

        // Vérifier si c'est un client (données de test)
        private bool IsClient(string username, string password)
        {
            // Clients de test (email comme username)
            var clients = new Dictionary<string, (string password, string name)>
            {
                { "client1@test.com", ("client123", "Jean Dupont") },
                { "client2@test.com", ("client123", "Marie Martin") },
                { "client3@test.com", ("client123", "Pierre Tremblay") }
            };
            
            return clients.ContainsKey(username.ToLower()) && 
                   clients[username.ToLower()].password == password;
        }

        // Obtenir le nom d'un client
        private string GetClientName(string username)
        {
            var clients = new Dictionary<string, string>
            {
                { "client1@test.com", "Jean Dupont" },
                { "client2@test.com", "Marie Martin" },
                { "client3@test.com", "Pierre Tremblay" }
            };
            
            return clients.ContainsKey(username.ToLower()) 
                ? clients[username.ToLower()] 
                : username;
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

