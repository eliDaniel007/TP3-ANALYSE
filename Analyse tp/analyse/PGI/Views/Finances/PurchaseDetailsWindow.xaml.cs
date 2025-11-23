using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class PurchaseDetailsWindow : Window
    {
        private CommandeFournisseur commande;

        public PurchaseDetailsWindow(CommandeFournisseur commande)
        {
            InitializeComponent();
            this.commande = commande;
            LoadDetails();
        }

        private void LoadDetails()
        {
            // Charger les détails complets depuis la base de données
            commande = CommandeFournisseurService.GetCommandeById(commande.Id)
                ?? throw new Exception("Commande introuvable");

            // En-tête
            TxtNumeroCommande.Text = $"Commande {commande.NumeroCommande}";
            TxtStatut.Text = commande.Statut.ToUpper();
            
            // Couleur du statut
            BorderStatut.Background = commande.Statut switch
            {
                "En attente" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),
                "Partiellement reçue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6")),
                "Reçue" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                "Annulée" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                _ => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"))
            };

            // Informations
            TxtFournisseur.Text = commande.NomFournisseur;
            TxtDateCommande.Text = commande.DateCommande.ToString("yyyy-MM-dd HH:mm");
            TxtDateLivraison.Text = commande.DateLivraisonPrevue?.ToString("yyyy-MM-dd") ?? "Non spécifiée";
            TxtEmploye.Text = commande.NomEmploye;

            // Produits
            var lignesDisplay = commande.Lignes.Select(l => new LigneDisplay
            {
                NomProduit = l.NomProduit,
                SKU = l.SKU,
                QuantiteCommandee = l.QuantiteCommandee,
                QuantiteRecue = l.QuantiteRecue,
                PrixUnitaireFormate = $"{l.PrixUnitaire:N2} $",
                MontantLigneFormate = $"{l.MontantLigne:N2} $"
            }).ToList();

            ProductsDataGrid.ItemsSource = lignesDisplay;

            // Totaux
            TxtSousTotal.Text = $"{commande.SousTotal:N2} $";
            TxtTPS.Text = $"{commande.MontantTPS:N2} $";
            TxtTVQ.Text = $"{commande.MontantTVQ:N2} $";
            TxtTotal.Text = $"{commande.MontantTotal:N2} $";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private class LigneDisplay
        {
            public string NomProduit { get; set; } = string.Empty;
            public string SKU { get; set; } = string.Empty;
            public int QuantiteCommandee { get; set; }
            public int QuantiteRecue { get; set; }
            public string PrixUnitaireFormate { get; set; } = string.Empty;
            public string MontantLigneFormate { get; set; } = string.Empty;
        }
    }
}

