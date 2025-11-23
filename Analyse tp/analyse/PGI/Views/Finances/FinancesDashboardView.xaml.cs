using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class FinancesDashboardView : UserControl
    {
        public FinancesDashboardView()
        {
            InitializeComponent();
            LoadData();
        }

        public void LoadData()
        {
            try
            {
                LoadKPIs();
                LoadUnpaidInvoices();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement du tableau de bord:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void LoadKPIs()
        {
            // KPI 1: Ventes des 30 derniers jours
            var factures = FactureService.GetAllFactures();
            var date30jAgo = DateTime.Now.AddDays(-30);
            
            var ventesLast30 = factures
                .Where(f => f.DateFacture >= date30jAgo && f.Statut != "Annulée")
                .Sum(f => f.MontantTotal);
            
            TxtVentes30j.Text = $"{ventesLast30:N2} $";

            // KPI 2: Profit Net (basé sur les marges des produits vendus)
            // Pour simplifier, on calcule: (Prix de vente - Coût) des produits vendus
            var profitEstime = factures
                .Where(f => f.DateFacture >= date30jAgo && f.Statut != "Annulée")
                .SelectMany(f => f.Lignes)
                .Sum(l => (l.PrixUnitaire - (l.PrixUnitaire * 0.6m)) * l.Quantite); // Estimation 40% de marge
            
            TxtProfit30j.Text = $"{profitEstime:N2} $";
            
            if (ventesLast30 > 0)
            {
                var margeNette = (profitEstime / ventesLast30) * 100;
                TxtMargeNette.Text = $"Marge nette : {margeNette:N1}%";
            }
            else
            {
                TxtMargeNette.Text = "Marge nette : 0%";
            }

            // KPI 3: Factures en retard
            var facturesEnRetard = factures
                .Where(f => f.DateEcheance < DateTime.Now && 
                           f.StatutPaiement != "Payée" && 
                           f.Statut != "Annulée")
                .ToList();
            
            TxtFacturesRetard.Text = facturesEnRetard.Count.ToString();
            
            var montantRetard = facturesEnRetard.Sum(f => f.MontantDu);
            TxtMontantRetard.Text = $"Montant dû : {montantRetard:N2} $";

            // KPI 4: Commandes fournisseurs en attente
            var commandesEnAttente = CommandeFournisseurService.GetAllCommandes()
                .Count(c => c.Statut == "En attente" || c.Statut == "Partiellement reçue");
            
            TxtCommandesAttente.Text = commandesEnAttente.ToString();
        }

        private void LoadUnpaidInvoices()
        {
            var factures = FactureService.GetAllFactures();
            var facturesEnRetardPlus30j = factures
                .Where(f => f.DateEcheance < DateTime.Now.AddDays(-30) && 
                           f.StatutPaiement != "Payée" && 
                           f.Statut != "Annulée")
                .OrderBy(f => f.DateEcheance)
                .Take(10) // Top 10
                .Select(f => new UnpaidInvoice
                {
                    NumeroFacture = f.NumeroFacture,
                    Client = f.NomClient,
                    MontantDu = $"{f.MontantDu:N2} $",
                    JoursRetard = ((int)(DateTime.Now - f.DateEcheance).TotalDays).ToString()
                })
                .ToList();
            
            UnpaidInvoicesDataGrid.ItemsSource = facturesEnRetardPlus30j;
        }

        private void BtnViewUnpaidInvoices_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers la liste des ventes avec filtre "En retard"
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.BtnSales.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            }
        }

        private void BtnNewSale_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire de nouvelle vente
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToSaleForm();
            }
        }

        private void BtnNewPurchase_Click(object sender, RoutedEventArgs e)
        {
            // Naviguer vers le formulaire de nouvelle commande fournisseur
            var parent = FindParentFinancesMainView(this);
            if (parent != null)
            {
                parent.NavigateToPurchaseForm();
            }
        }

        private void BtnRecordPayment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Fonctionnalité 'Enregistrer un paiement' à implémenter.\n\n" +
                "Cette fonction ouvrira un pop-up pour enregistrer un paiement sur une facture existante.",
                "Information",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnTaxSettings_Click(object sender, RoutedEventArgs e)
        {
            var taxSettingsWindow = new TaxSettingsWindow();
            taxSettingsWindow.ShowDialog();
        }

        private FinancesMainView? FindParentFinancesMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is FinancesMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as FinancesMainView;
        }
    }

    // Classe modèle pour les factures impayées
    public class UnpaidInvoice
    {
        public string NumeroFacture { get; set; } = string.Empty;
        public string Client { get; set; } = string.Empty;
        public string MontantDu { get; set; } = string.Empty;
        public string JoursRetard { get; set; } = string.Empty;
    }
}

