using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PGI.Services;

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

            // Validation des champs obligatoires
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                ShowMessage("Veuillez remplir tous les champs obligatoires (Nom, Email, Mot de passe).", false);
                return;
            }

            // Validation email basique
            if (!email.Contains("@") || !email.Contains("."))
            {
                ShowMessage("Veuillez entrer une adresse email valide.", false);
                return;
            }

            // Vérifier que l'email contient "client"
            if (!email.ToLower().Contains("client"))
            {
                ShowMessage("❌ L'adresse email doit contenir le mot 'client'.\nExemple : jean.client@email.com", false);
                return;
            }

            // Validation mot de passe
            if (password != confirmPassword)
            {
                ShowMessage("Les mots de passe ne correspondent pas.", false);
                return;
            }

            if (password.Length < 6)
            {
                ShowMessage("Le mot de passe doit contenir au moins 6 caractères.", false);
                return;
            }

            // Désactiver le bouton pendant l'inscription
            RegisterButton.IsEnabled = false;
            RegisterButton.Content = "⏳ Inscription en cours...";

            try
            {
                // === ENREGISTREMENT DANS LA BASE DE DONNÉES ===
                var (success, message, clientId) = ClientService.Register(nom, email, telephone, password);

                if (success)
                {
                    MessageBox.Show(
                        $"✅ Inscription réussie !\n\n" +
                        $"Nom : {nom}\n" +
                        $"Email : {email}\n" +
                        $"Téléphone : {(string.IsNullOrWhiteSpace(telephone) ? "Non renseigné" : telephone)}\n\n" +
                        $"Vous pouvez maintenant vous connecter pour accéder au site d'achat.",
                        "Inscription réussie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Retour à la page de connexion
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowMessage(message, false);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"❌ Erreur lors de l'inscription : {ex.Message}", false);
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Content = "✨ Créer mon compte";
            }
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
                PasswordBox.Visibility = Visibility.Collapsed;
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
                ConfirmPasswordBox.Visibility = Visibility.Collapsed;
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

        private void ShowMessage(string message, bool isSuccess)
        {
            Message.Text = message;
            Message.Foreground = isSuccess 
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")) 
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
            Message.Visibility = Visibility.Visible;
        }
    }
}
