using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace PGI.Views.Stocks
{
    public partial class MovementsHistoryView : UserControl
    {
        public MovementsHistoryView()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var movements = new List<Movement>
            {
                new Movement { DateTime = DateTime.Now.AddMinutes(-15).ToString("dd/MM/yyyy HH:mm"), Type = "IN", TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), Produit = "Veste d'hiver Nordique", Quantite = "+25", Motif = "Réception - Achat #2045", Utilisateur = "Admin" },
                new Movement { DateTime = DateTime.Now.AddHours(-1).ToString("dd/MM/yyyy HH:mm"), Type = "OUT", TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")), Produit = "Bottes de randonnée", Quantite = "-2", Motif = "Vente - Facture #3421", Utilisateur = "Jean Tremblay" },
                new Movement { DateTime = DateTime.Now.AddHours(-3).ToString("dd/MM/yyyy HH:mm"), Type = "IN", TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), Produit = "Sac à dos 45L", Quantite = "+5", Motif = "Ajustement inventaire", Utilisateur = "Admin" },
                new Movement { DateTime = DateTime.Now.AddHours(-5).ToString("dd/MM/yyyy HH:mm"), Type = "OUT", TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444")), Produit = "Gants thermiques", Quantite = "-3", Motif = "Vente - Facture #3420", Utilisateur = "Marie Dubois" },
                new Movement { DateTime = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy HH:mm"), Type = "IN", TypeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")), Produit = "Tente 4 saisons", Quantite = "+10", Motif = "Réception - Achat #2044", Utilisateur = "Admin" },
            };

            MovementsDataGrid.ItemsSource = movements;
        }
    }

    public class Movement
    {
        public string DateTime { get; set; }
        public string Type { get; set; }
        public Brush TypeColor { get; set; }
        public string Produit { get; set; }
        public string Quantite { get; set; }
        public string Motif { get; set; }
        public string Utilisateur { get; set; }
    }
}

