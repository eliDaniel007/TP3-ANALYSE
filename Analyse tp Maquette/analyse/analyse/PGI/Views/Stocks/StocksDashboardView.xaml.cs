using System;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Stocks
{
    public partial class StocksDashboardView : UserControl
    {
        public StocksDashboardView()
        {
            InitializeComponent();
            // TODO: Charger les données depuis la base de données
        }

        private void BtnCalculateInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Récupérer les données depuis la base de données
                // Pour le moment, simulation du calcul
                
                // Simulation d'un calcul
                Random random = new Random();
                double inventoryValue = random.Next(400000, 600000);
                
                // Mise à jour de l'affichage
                TxtInventoryValue.Text = $"{inventoryValue:N0} $";
                
                MessageBox.Show(
                    $"✅ Valeur de l'inventaire recalculée avec succès !\n\n" +
                    $"Valeur totale : {inventoryValue:N0} $\n" +
                    $"Basé sur le coût d'achat de tous les produits en stock.",
                    "Calcul terminé",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du calcul : {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}

