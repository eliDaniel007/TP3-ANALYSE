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
            string prenom = PrenomTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string role = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();
            string departement = ((ComboBoxItem)DepartementComboBox.SelectedItem).Content.ToString();
            string password = PasswordBox.Visibility == Visibility.Visible ? PasswordBox.Password : PasswordTextBox.Text;
            string confirmPassword = ConfirmPasswordBox.Visibility == Visibility.Visible ? ConfirmPasswordBox.Password : ConfirmPasswordTextBox.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(prenom) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mode développement : inscription réussie automatiquement
            MessageBox.Show(
                $"✅ Inscription réussie !\n\n" +
                $"Nom complet : {prenom} {nom}\n" +
                $"Email : {email}\n" +
                $"Rôle : {role}\n" +
                $"Département : {departement}\n\n" +
                $"Vous pouvez maintenant vous connecter.",
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

