using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class ExpensesView : UserControl
    {
        public ExpensesView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var depenses = DepenseService.GetAllDepenses();
                var displayList = new List<ExpenseDisplay>();

                foreach (var d in depenses)
                {
                    var display = new ExpenseDisplay
                    {
                        Id = d.Id,
                        Date = d.DateDepense.ToString("yyyy-MM-dd"),
                        Description = d.Description,
                        Categorie = d.Categorie,
                        Montant = d.Montant.ToString("C"),
                        Statut = d.StatutPaiement
                    };

                    if (d.StatutPaiement == "Payée")
                        display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    else
                        display.StatutColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));

                    displayList.Add(display);
                }

                ExpensesDataGrid.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement dépenses: {ex.Message}");
            }
        }

        private void BtnAddExpense_Click(object sender, RoutedEventArgs e)
        {
            // Simulation d'ajout rapide pour ce MVP
            // Idéalement, ouvrir une fenêtre modale
            var result = MessageBox.Show("Ajouter une dépense de test (Loyer - 2500$) ?", "Test", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var depense = new Depense
                    {
                        Description = "Loyer mensuel",
                        Categorie = "Loyer",
                        Montant = 2500.00m,
                        DateDepense = DateTime.Now,
                        StatutPaiement = "En attente"
                    };
                    DepenseService.AjouterDepense(depense);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur ajout: {ex.Message}");
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Édition à implémenter");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Suppression à implémenter");
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
    }
}
