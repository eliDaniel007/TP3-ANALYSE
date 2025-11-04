using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PGI
{
    public partial class RegisterWindow : Window
    {

        public RegisterWindow()
        {
            InitializeComponent();
            
            // Permettre de déplacer la fenêtre
            this.MouseLeftButtonDown += (s, e) => { if (e.LeftButton == MouseButtonState.Pressed) this.DragMove(); };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string telephone = TelephoneTextBox.Text.Trim();
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : PasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordBox.Visibility == Visibility.Visible ? ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs obligatoires (Nom, Email, Mot de passe).", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation email basique
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Le mot de passe doit contenir au moins 6 caractères.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // TODO: Enregistrer le client dans la base de données
            // Pour l'instant, simulation d'inscription réussie
            
            MessageBox.Show(
                $"✅ Inscription réussie !\n\n" +
                $"Nom : {nom}\n" +
                $"Email : {email}\n" +
                $"Téléphone : {(string.IsNullOrWhiteSpace(telephone) ? "Non renseigné" : telephone)}\n\n" +
                $"Vous pouvez maintenant vous connecter pour accéder au site d'achat.",
                "Succès",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            // Retour à la page de connexion
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Visibility == Visibility.Visible)
            {
                PasswordTextBox.Text = PasswordBox.Password;
                PasswordBox.Visibility = Visibility.Collapsed;
                PasswordTextBox.Visibility = Visibility.Visible;
                TogglePasswordButton.Content = "🙈";
            }
            else
            {
                PasswordBox.Password = PasswordTextBox.Text;
                PasswordTextBox.Visibility = Visibility.Collapsed;
                PasswordBox.Visibility = Visibility.Visible;
                TogglePasswordButton.Content = "👁️";
            }
        }

        private void ToggleConfirmPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmPasswordBox.Visibility == Visibility.Visible)
            {
                ConfirmPasswordTextBox.Text = ConfirmPasswordBox.Password;
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordTextBox.Visibility = Visibility.Visible;
                ToggleConfirmPasswordButton.Content = "🙈";
            }
            else
            {
                ConfirmPasswordBox.Password = ConfirmPasswordTextBox.Text;
                ConfirmPasswordTextBox.Visibility = Visibility.Collapsed;
                ConfirmPasswordBox.Visibility = Visibility.Visible;
                ToggleConfirmPasswordButton.Content = "👁️";
            }
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}

