using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.Dashboard
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                // A. Indicateurs de Performance (KPIs)
                decimal caTotal = DashboardService.GetChiffreAffairesTotal();
                int nbVentes = DashboardService.GetNombreVentes();
                double satisfaction = DashboardService.GetSatisfactionMoyenne();
                decimal valeurStock = DashboardService.GetValeurTotaleStock();

                TxtCATotal.Text = $"{caTotal:C}";
                TxtNbVentes.Text = nbVentes.ToString();
                TxtSatisfaction.Text = $"{satisfaction:N1} / 5";
                TxtValeurStock.Text = $"{valeurStock:C}";

                // B. Alertes et Actions Requises
                GridReappro.ItemsSource = DashboardService.GetAlertesReapprovisionnement().DefaultView;
                GridCommandes.ItemsSource = DashboardService.GetCommandesATraiter().DefaultView;
                GridService.ItemsSource = DashboardService.GetAlertesServiceClient().DefaultView;

                // C. Graphique Évolution des ventes
                LoadGraphData();
            }
            catch (Exception ex)
            {
                // En production, logger l'erreur. Ici on ne bloque pas l'UI.
                Console.WriteLine($"Erreur Dashboard: {ex.Message}");
            }
        }

        private void LoadGraphData()
        {
            try
            {
                // 1. Préparer les 6 derniers mois (vides par défaut)
                var dataMap = new Dictionary<string, decimal>();
                DateTime currentDate = DateTime.Now;
                
                for (int i = 5; i >= 0; i--)
                {
                    string moisKey = currentDate.AddMonths(-i).ToString("yyyy-MM");
                    dataMap[moisKey] = 0;
                }

                // 2. Récupérer les données réelles
                var evolutionVentes = DashboardService.GetEvolutionVentes(6);
                
                // 3. Fusionner
                foreach(var kvp in evolutionVentes)
                {
                    if (dataMap.ContainsKey(kvp.Key))
                    {
                        dataMap[kvp.Key] = kvp.Value;
                    }
                }

                // 4. Afficher
                // L'axe Y est fixe à 10k dans le XAML pour l'instant
                decimal axisMax = 10000; 
                
                // Si les ventes dépassent 10k, il faudrait idéalement adapter l'échelle (mise à jour future)
                // Pour l'instant, on plafonne à 10k pour ne pas sortir du graphique
                
                var graphData = dataMap.Select(kvp => new
                {
                    Mois = kvp.Key, // Format yyyy-MM
                    // Hauteur relative à l'axe fixe de 10 000 $
                    // La hauteur du graphique est de 300px dans le XAML
                    Hauteur = (double)(Math.Min(kvp.Value, axisMax) / axisMax) * 300, 
                    Tooltip = $"{kvp.Key}: {kvp.Value:C}",
                    MontantFormatte = kvp.Value > 0 ? (kvp.Value >= 1000 ? $"{kvp.Value/1000:N1}k" : $"{kvp.Value:N0}") : ""
                }).ToList();

                GraphItemsControl.ItemsSource = graphData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Graphique: {ex.Message}");
            }
        }

        private void BtnReappro_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                var button = sender as Button;
                if (button?.DataContext is System.Data.DataRowView row)
                {
                    // Validation des données
                    if (row["id"] == DBNull.Value || row["fournisseur_id"] == DBNull.Value)
                    {
                        MessageBox.Show("Données produit incomplètes (ID ou Fournisseur manquant).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    int produitId = Convert.ToInt32(row["id"]);
                    string sku = row["sku"].ToString();
                    string nom = row["nom"].ToString();
                    
                    decimal cout = 0;
                    if (row["cout"] != DBNull.Value)
                        cout = Convert.ToDecimal(row["cout"]);
                        
                    int fournisseurId = Convert.ToInt32(row["fournisseur_id"]);
                    
                    int seuil = 0;
                    if (row["seuil_reapprovisionnement"] != DBNull.Value)
                        seuil = Convert.ToInt32(row["seuil_reapprovisionnement"]);
                    
                    // Quantité à commander : Remonter au seuil + marge
                    int quantiteACommander = Math.Max(20, seuil * 2);

                    // Créer l'objet commande
                    var commande = new Models.CommandeFournisseur
                    {
                        NumeroCommande = CommandeFournisseurService.GenererNumeroCommande(),
                        DateCommande = DateTime.Now,
                        FournisseurId = fournisseurId,
                        NoteInterne = "Réapprovisionnement automatique depuis le tableau de bord"
                    };

                    // Créer la ligne de commande
                    var ligne = new Models.LigneCommandeFournisseur
                    {
                        ProduitId = produitId,
                        SKU = sku,
                        Description = nom,
                        QuantiteCommandee = quantiteACommander,
                        PrixUnitaire = cout
                    };

                    // Sauvegarder
                    int cmdId = CommandeFournisseurService.CreerCommande(commande, new List<Models.LigneCommandeFournisseur> { ligne });
                    
                    if (cmdId > 0)
                    {
                        MessageBox.Show(
                            $"✅ Commande {commande.NumeroCommande} créée avec succès !\n\n" +
                            $"Produit : {nom}\n" +
                            $"Quantité : {quantiteACommander}\n" +
                            $"Statut : Envoyée",
                            "Réapprovisionnement effectué",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                            
                        // Rafraîchir les alertes
                        LoadDashboardData();
                    }
                    else
                    {
                        MessageBox.Show("La commande n'a pas pu être créée.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                 MessageBox.Show($"Erreur détaillée : {ex.Message}\n\nStack: {ex.StackTrace}", "Erreur Critique", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
