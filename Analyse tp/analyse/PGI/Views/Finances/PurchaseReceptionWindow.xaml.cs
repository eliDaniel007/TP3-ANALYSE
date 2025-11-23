using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class PurchaseReceptionWindow : Window
    {
        private CommandeFournisseur commande;
        private List<ReceptionLine> receptionLines;

        public PurchaseReceptionWindow(CommandeFournisseur commande)
        {
            InitializeComponent();
            this.commande = commande;
            LoadCommandeDetails();
        }

        private void LoadCommandeDetails()
        {
            // Charger les détails de la commande avec les lignes
            commande = CommandeFournisseurService.GetCommandeById(commande.Id) 
                ?? throw new Exception("Commande introuvable");
            
            TxtNumeroCommande.Text = $"📦 Réceptionner la commande {commande.NumeroCommande}";
            TxtFournisseur.Text = $"Fournisseur : {commande.NomFournisseur}";
            
            receptionLines = commande.Lignes.Select(l => new ReceptionLine
            {
                LigneId = l.Id,
                ProduitId = l.ProduitId,
                NomProduit = l.NomProduit,
                SKU = l.SKU,
                QuantiteCommandee = l.QuantiteCommandee,
                QuantiteRecue = l.QuantiteRecue,
                QuantiteARecevoir = Math.Max(0, l.QuantiteCommandee - l.QuantiteRecue) // Par défaut, le reste
            }).ToList();
            
            ProductsDataGrid.ItemsSource = receptionLines;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validation
                var quantitesRecues = new Dictionary<int, int>();
                bool hasReception = false;

                foreach (var line in receptionLines)
                {
                    if (line.QuantiteARecevoir < 0)
                    {
                        MessageBox.Show(
                            $"La quantité à recevoir pour {line.NomProduit} ne peut pas être négative.",
                            "Validation",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    if (line.QuantiteRecue + line.QuantiteARecevoir > line.QuantiteCommandee)
                    {
                        MessageBox.Show(
                            $"La quantité totale reçue pour {line.NomProduit} ({line.QuantiteRecue + line.QuantiteARecevoir}) " +
                            $"dépasse la quantité commandée ({line.QuantiteCommandee}).",
                            "Validation",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                        );
                        return;
                    }

                    if (line.QuantiteARecevoir > 0)
                    {
                        hasReception = true;
                        quantitesRecues[line.LigneId] = line.QuantiteARecevoir;
                    }
                }

                if (!hasReception)
                {
                    MessageBox.Show(
                        "Veuillez entrer au moins une quantité à recevoir.",
                        "Validation",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }

                // Confirmation
                var result = MessageBox.Show(
                    $"Confirmer la réception de {quantitesRecues.Count} produit(s) ?\n\n" +
                    "Le stock sera automatiquement mis à jour.",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    // Appeler le service pour réceptionner
                    CommandeFournisseurService.RecevoirCommande(
                        commande.Id,
                        quantitesRecues,
                        null // TODO: employeId
                    );

                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la réception:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private class ReceptionLine : INotifyPropertyChanged
        {
            public int LigneId { get; set; }
            public int ProduitId { get; set; }
            public string NomProduit { get; set; } = string.Empty;
            public string SKU { get; set; } = string.Empty;
            public int QuantiteCommandee { get; set; }
            public int QuantiteRecue { get; set; }

            private int _quantiteARecevoir;
            public int QuantiteARecevoir
            {
                get => _quantiteARecevoir;
                set
                {
                    if (_quantiteARecevoir != value)
                    {
                        _quantiteARecevoir = value;
                        OnPropertyChanged(nameof(QuantiteARecevoir));
                    }
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

