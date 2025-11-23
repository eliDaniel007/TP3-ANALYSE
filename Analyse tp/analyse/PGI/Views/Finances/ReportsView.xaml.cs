using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.Finances
{
    public partial class ReportsView : UserControl
    {
        private RapportFinancier? rapportActuel;

        public ReportsView()
        {
            InitializeComponent();
            
            // Charger le rapport du mois en cours par défaut
            LoadRapportMoisEnCours();
        }

        private void LoadRapportMoisEnCours()
        {
            try
            {
                rapportActuel = RapportFinancierService.GenererRapportMoisEnCours();
                AfficherRapport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement du rapport:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void AfficherRapport()
        {
            if (rapportActuel == null) return;

            // Afficher les KPIs (seulement ceux qui existent dans le XAML)
            try
            {
                // TxtTotalVentes.Text = $"{rapportActuel.TotalVentes:N2} $";
                // TxtNombreFactures.Text = rapportActuel.NombreFactures.ToString();
                // TxtTotalTPS.Text = $"{rapportActuel.TotalTPS:N2} $";
                // TxtTotalTVQ.Text = $"{rapportActuel.TotalTVQ:N2} $";
                TxtProfitBrut.Text = $"{rapportActuel.ProfitBrut:N2} $";
                TxtProfitNet.Text = $"{rapportActuel.ProfitNet:N2} $";
                // TxtMargeProfit.Text = $"{rapportActuel.MargeProfit:N2} %";
                // TxtTotalImpaye.Text = $"{rapportActuel.TotalImpaye:N2} $";
                // TxtPaiementsRecus.Text = $"{rapportActuel.TotalPaiementsRecus:N2} $";
            }
            catch
            {
                // Les contrôles n'existent peut-être pas tous dans le XAML
            }

            // Charger les top clients et produits
            LoadTopClientsEtProduits();
        }

        private void LoadTopClientsEtProduits()
        {
            if (rapportActuel == null) return;

            try
            {
                // Top 5 clients
                var topClients = RapportFinancierService.GetTop5Clients(
                    rapportActuel.DateDebut,
                    rapportActuel.DateFin
                );

                // TODO: Ajouter TopClientsPanel dans le XAML
                // TopClientsPanel.Children.Clear();
                // foreach (var (nom, ventes, nbFactures) in topClients)
                // {
                //     var textBlock = new TextBlock
                //     {
                //         Text = $"• {nom}: {ventes:N2} $ ({nbFactures} factures)",
                //         FontSize = 14,
                //         Margin = new Thickness(0, 5, 0, 5),
                //         Foreground = new System.Windows.Media.SolidColorBrush(
                //             (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E293B")
                //         )
                //     };
                //     TopClientsPanel.Children.Add(textBlock);
                // }

                // Top 5 produits
                var topProduits = RapportFinancierService.GetTop5Produits(
                    rapportActuel.DateDebut,
                    rapportActuel.DateFin
                );

                // TODO: Ajouter TopProduitsPanel dans le XAML
                // TopProduitsPanel.Children.Clear();
                // foreach (var (nom, ventes, quantite) in topProduits)
                // {
                //     var textBlock = new TextBlock
                //     {
                //         Text = $"• {nom}: {ventes:N2} $ ({quantite} unités)",
                //         FontSize = 14,
                //         Margin = new Thickness(0, 5, 0, 5),
                //         Foreground = new System.Windows.Media.SolidColorBrush(
                //             (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#1E293B")
                //         )
                //     };
                //     TopProduitsPanel.Children.Add(textBlock);
                // }
            }
            catch (Exception ex)
            {
                // Erreur lors du chargement des tops - ignorée pour l'instant
                Console.WriteLine($"Erreur tops: {ex.Message}");
            }
        }

        private void CmbPeriod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPeriod == null) return;

            var selectedItem = CmbPeriod.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            string periode = selectedItem.Content.ToString() ?? "";

            try
            {
                DateTime dateDebut, dateFin;

                switch (periode)
                {
                    case "Mois en cours":
                        rapportActuel = RapportFinancierService.GenererRapportMoisEnCours();
                        break;

                    case "Année en cours":
                        rapportActuel = RapportFinancierService.GenererRapportAnneeEnCours();
                        break;

                    case "Personnalisé":
                        // Demander les dates
                        if (DateTime.TryParse(TxtTaxDateDebut.Text, out dateDebut) &&
                            DateTime.TryParse(TxtTaxDateFin.Text, out dateFin))
                        {
                            rapportActuel = RapportFinancierService.GenererRapport(dateDebut, dateFin);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Veuillez entrer des dates valides (format: AAAA-MM-JJ)",
                                "Validation",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning
                            );
                            return;
                        }
                        break;

                    default:
                        LoadRapportMoisEnCours();
                        return;
                }

                AfficherRapport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du changement de période:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void BtnGenerateTaxPDF_Click(object sender, RoutedEventArgs e)
        {
            if (rapportActuel == null)
            {
                MessageBox.Show(
                    "Aucun rapport à exporter. Veuillez d'abord générer un rapport.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            MessageBox.Show(
                "📄 Génération du rapport de taxes en PDF...\n\n" +
                $"Période : {rapportActuel.DateDebut:yyyy-MM-dd} → {rapportActuel.DateFin:yyyy-MM-dd}\n" +
                $"TPS collectée : {rapportActuel.TotalTPS:N2} $\n" +
                $"TVQ collectée : {rapportActuel.TotalTVQ:N2} $\n\n" +
                "Fonctionnalité d'export PDF à implémenter.\n" +
                "Le fichier sera sauvegardé dans Documents/Rapports/",
                "Rapport de Taxes (PDF)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateTaxCSV_Click(object sender, RoutedEventArgs e)
        {
            if (rapportActuel == null)
            {
                MessageBox.Show(
                    "Aucun rapport à exporter. Veuillez d'abord générer un rapport.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            MessageBox.Show(
                "📊 Génération du rapport de taxes en CSV...\n\n" +
                $"Période : {rapportActuel.DateDebut:yyyy-MM-dd} → {rapportActuel.DateFin:yyyy-MM-dd}\n\n" +
                "Fonctionnalité d'export CSV à implémenter.",
                "Rapport de Taxes (CSV)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateSalesPDF_Click(object sender, RoutedEventArgs e)
        {
            if (rapportActuel == null)
            {
                MessageBox.Show(
                    "Aucun rapport à exporter. Veuillez d'abord générer un rapport.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            MessageBox.Show(
                "📄 Génération du rapport des ventes en PDF...\n\n" +
                $"Période : {rapportActuel.DateDebut:yyyy-MM-dd} → {rapportActuel.DateFin:yyyy-MM-dd}\n" +
                $"Total ventes : {rapportActuel.TotalVentes:N2} $\n" +
                $"Nombre de factures : {rapportActuel.NombreFactures}\n" +
                $"Profit net : {rapportActuel.ProfitNet:N2} $\n\n" +
                "Fonctionnalité d'export PDF à implémenter.",
                "Rapport des Ventes (PDF)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        private void BtnGenerateSalesCSV_Click(object sender, RoutedEventArgs e)
        {
            if (rapportActuel == null)
            {
                MessageBox.Show(
                    "Aucun rapport à exporter. Veuillez d'abord générer un rapport.",
                    "Information",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                return;
            }

            MessageBox.Show(
                "📊 Génération du rapport des ventes en CSV...\n\n" +
                $"Période : {rapportActuel.DateDebut:yyyy-MM-dd} → {rapportActuel.DateFin:yyyy-MM-dd}\n\n" +
                "Fonctionnalité d'export CSV à implémenter.",
                "Rapport des Ventes (CSV)",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }
    }
}

