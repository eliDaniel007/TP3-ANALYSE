using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using PGI.Helpers;

namespace PGI.Views.Stocks
{
    public partial class StocksDashboardView : UserControl
    {
        public StocksDashboardView()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                LoadKPIs();
                LoadAlertes();
                LoadRecentMovements();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du tableau de bord : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadSampleData();
            }
        }

        private void LoadKPIs()
        {
            try
            {
                // KPI 1 : Valeur totale du stock
                string queryValeur = @"
                    SELECT COALESCE(SUM(ns.qte_disponible * p.cout), 0) AS valeur_stock
                    FROM niveaux_stock ns
                    INNER JOIN produits p ON ns.produit_id = p.id
                    WHERE p.statut = 'Actif'";
                
                var dtValeur = DatabaseHelper.ExecuteQuery(queryValeur);
                if (dtValeur.Rows.Count > 0)
                {
                    decimal valeurStock = Convert.ToDecimal(dtValeur.Rows[0]["valeur_stock"]);
                    TxtInventoryValue.Text = $"{valeurStock:N0} $";
                }

                // KPI 2 : Nombre de produits actifs
                string queryProduits = "SELECT COUNT(*) as total FROM produits WHERE statut = 'Actif'";
                var dtProduits = DatabaseHelper.ExecuteQuery(queryProduits);
                if (dtProduits.Rows.Count > 0)
                {
                    int totalProduits = Convert.ToInt32(dtProduits.Rows[0]["total"]);
                    TxtActiveProducts.Text = totalProduits.ToString();
                }

                // KPI 3 : Nombre de fournisseurs actifs
                string queryFournisseurs = "SELECT COUNT(*) as total FROM fournisseurs WHERE statut = 'Actif'";
                var dtFournisseurs = DatabaseHelper.ExecuteQuery(queryFournisseurs);
                if (dtFournisseurs.Rows.Count > 0)
                {
                    int totalFournisseurs = Convert.ToInt32(dtFournisseurs.Rows[0]["total"]);
                    TxtSuppliers.Text = totalFournisseurs.ToString();
                }

                // KPI 4 : Marge brute moyenne
                string queryMarge = @"
                    SELECT COALESCE(AVG(marge_brute), 0) as marge_moyenne
                    FROM produits
                    WHERE statut = 'Actif'";
                
                var dtMarge = DatabaseHelper.ExecuteQuery(queryMarge);
                if (dtMarge.Rows.Count > 0)
                {
                    decimal margeMoyenne = Convert.ToDecimal(dtMarge.Rows[0]["marge_moyenne"]);
                    TxtGrossMargin.Text = $"{margeMoyenne:N1}%";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur KPIs: {ex.Message}");
                // Valeurs par défaut en cas d'erreur
                TxtInventoryValue.Text = "0 $";
                TxtActiveProducts.Text = "0";
                TxtSuppliers.Text = "0";
                TxtGrossMargin.Text = "0%";
            }
        }

        private void LoadAlertes()
        {
            try
            {
                // Utiliser la vue v_produits_a_reapprovisionner
                string query = @"
                    SELECT p.sku, p.nom, 
                           COALESCE(SUM(ns.qte_disponible), 0) as stock_actuel,
                           p.seuil_reapprovisionnement,
                           f.nom as fournisseur,
                           f.delai_livraison_jours
                    FROM produits p
                    LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
                    INNER JOIN fournisseurs f ON p.fournisseur_id = f.id
                    WHERE p.statut = 'Actif'
                    GROUP BY p.id, p.sku, p.nom, p.seuil_reapprovisionnement, f.nom, f.delai_livraison_jours
                    HAVING SUM(ns.qte_disponible) <= p.seuil_reapprovisionnement
                    ORDER BY SUM(ns.qte_disponible) ASC
                    LIMIT 10";

                var dt = DatabaseHelper.ExecuteQuery(query);
                
                // TODO: Afficher les alertes dans l'interface
                // Pour l'instant, on stocke juste le nombre
                int nbAlertes = dt.Rows.Count;
                Console.WriteLine($"{nbAlertes} produits nécessitent un réapprovisionnement");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur alertes: {ex.Message}");
            }
        }

        private void LoadRecentMovements()
        {
            try
            {
                string query = @"
                    SELECT m.date_mouvement, m.type_mouvement, m.quantite,
                           p.sku, p.nom as produit_nom
                    FROM mouvements_stock m
                    INNER JOIN produits p ON m.produit_id = p.id
                    ORDER BY m.date_mouvement DESC
                    LIMIT 10";

                var dt = DatabaseHelper.ExecuteQuery(query);
                
                // TODO: Afficher les mouvements dans l'interface
                Console.WriteLine($"{dt.Rows.Count} mouvements récents chargés");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur mouvements: {ex.Message}");
            }
        }

        private void LoadSampleData()
        {
            // Données d'exemple si la BDD n'est pas disponible
            TxtInventoryValue.Text = "487 350 $";
            TxtActiveProducts.Text = "142";
            TxtSuppliers.Text = "23";
            TxtGrossMargin.Text = "38.5%";
        }

        private void BtnCalculateInventory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Recalculer la valeur totale du stock
                string query = @"
                    SELECT COALESCE(SUM(ns.qte_disponible * p.cout), 0) AS valeur_stock,
                           COUNT(DISTINCT p.id) as nb_produits,
                           SUM(ns.qte_disponible) as quantite_totale
                    FROM niveaux_stock ns
                    INNER JOIN produits p ON ns.produit_id = p.id
                    WHERE p.statut = 'Actif'";
                
                var dt = DatabaseHelper.ExecuteQuery(query);
                
                if (dt.Rows.Count > 0)
                {
                    decimal valeurStock = Convert.ToDecimal(dt.Rows[0]["valeur_stock"]);
                    int nbProduits = Convert.ToInt32(dt.Rows[0]["nb_produits"]);
                    int quantiteTotale = Convert.ToInt32(dt.Rows[0]["quantite_totale"]);
                    
                    // Mise à jour de l'affichage
                    TxtInventoryValue.Text = $"{valeurStock:N0} $";
                    
                    MessageBox.Show(
                        $"✅ Valeur de l'inventaire recalculée avec succès !\n\n" +
                        $"Valeur totale : {valeurStock:N0} $\n" +
                        $"Nombre de produits : {nbProduits}\n" +
                        $"Quantité totale : {quantiteTotale} unités\n\n" +
                        $"Basé sur le coût d'achat de tous les produits en stock.",
                        "Calcul terminé",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );
                }
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

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
            MessageBox.Show("✅ Données actualisées avec succès !",
                "Actualisation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

