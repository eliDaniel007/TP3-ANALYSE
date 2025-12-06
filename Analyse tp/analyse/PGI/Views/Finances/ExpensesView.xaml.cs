using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class ExpensesView : UserControl
    {
        private List<ExpenseDisplay> allExpenses;
        private const string searchPlaceholder = "Rechercher par description ou catégorie...";

        public ExpensesView()
        {
            InitializeComponent();
            InitializePlaceholder();
            LoadData();
        }

        private void InitializePlaceholder()
        {
            TxtSearch.Text = searchPlaceholder;
            TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == searchPlaceholder)
            {
                TxtSearch.Text = "";
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                TxtSearch.Text = searchPlaceholder;
                TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (TxtSearch.Text != searchPlaceholder)
            {
                ApplyFilters();
            }
        }

        private void CmbCategorieFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            TxtSearch.Text = searchPlaceholder;
            TxtSearch.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            CmbCategorieFilter.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (allExpenses == null) return;

            var filtered = allExpenses.AsEnumerable();

            // Filtre par recherche
            var searchText = TxtSearch.Text?.Trim();
            if (!string.IsNullOrWhiteSpace(searchText) && searchText != searchPlaceholder)
            {
                filtered = filtered.Where(e =>
                    e.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    e.Categorie.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                );
            }

            // Filtre par catégorie
            var selectedCategorie = (CmbCategorieFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedCategorie) && selectedCategorie != "Toutes les catégories")
            {
                filtered = filtered.Where(e => e.Categorie == selectedCategorie);
            }

            ExpensesDataGrid.ItemsSource = filtered.ToList();
        }

        private void LoadData()
        {
            try
            {
                var depenses = DepenseService.GetAllDepenses();
                allExpenses = new List<ExpenseDisplay>();

                foreach (var d in depenses)
                {
                    var display = new ExpenseDisplay
                    {
                        Id = d.Id,
                        Date = d.DateDepense.ToString("yyyy-MM-dd"),
                        Description = d.Description,
                        Categorie = d.Categorie,
                        Montant = d.Montant.ToString("C"),
                        Statut = d.StatutPaiement,
                        Source = d.Source
                    };

                    if (d.StatutPaiement == "Payée")
                        display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    else
                        display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));

                    allExpenses.Add(display);
                }

                ExpensesDataGrid.ItemsSource = allExpenses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement dépenses: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            var expenseWindow = new ExpenseFormWindow();
            if (expenseWindow.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var expense = button?.DataContext as ExpenseDisplay;
            
            if (expense != null)
            {
                // Vérifier que c'est une dépense générale (pas une commande fournisseur)
                if (expense.Source == "Stock")
                {
                    MessageBox.Show(
                        "Cette dépense provient d'une commande fournisseur et ne peut pas être modifiée ici.",
                        "Modification impossible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                try
                {
                    var expenseWindow = new ExpenseFormWindow(expense.Id);
                    if (expenseWindow.ShowDialog() == true)
                    {
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de l'ouverture du formulaire : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var expense = button?.DataContext as ExpenseDisplay;
            
            if (expense != null)
            {
                // Vérifier que c'est une dépense générale (pas une commande fournisseur)
                if (expense.Source == "Stock")
                {
                    MessageBox.Show(
                        "Cette dépense provient d'une commande fournisseur et ne peut pas être supprimée ici.",
                        "Suppression impossible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                var result = MessageBox.Show(
                    $"Voulez-vous vraiment supprimer cette dépense ?\n\n" +
                    $"Description : {expense.Description}\n" +
                    $"Montant : {expense.Montant}",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        DepenseService.SupprimerDepense(expense.Id);
                        MessageBox.Show("✅ Dépense supprimée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erreur lors de la suppression : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }

    public class ExpenseDisplay
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Categorie { get; set; } = string.Empty;
        public string Montant { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        public Brush StatutColor { get; set; } = Brushes.Gray;
        public string Source { get; set; } = "Generale"; // Pour distinguer les dépenses générales des commandes fournisseurs
    }
}
