using System;
using System.Collections.Generic;
using System.Data;
using PGI.Helpers;

namespace PGI.Services
{
    public class DashboardService
    {
        // --- DASHBOARD GLOBAL ---

        public static decimal GetChiffreAffairesTotal()
        {
            string query = "SELECT COALESCE(SUM(montant_total), 0) FROM factures WHERE statut_paiement = 'Payée'";
            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query));
        }

        public static int GetNombreVentes()
        {
            string query = "SELECT COUNT(*) FROM commandes_vente";
            return Convert.ToInt32(DatabaseHelper.ExecuteScalar(query));
        }

        public static double GetSatisfactionMoyenne()
        {
            string query = "SELECT COALESCE(AVG(note_satisfaction), 0) FROM evaluations_clients";
            return Convert.ToDouble(DatabaseHelper.ExecuteScalar(query));
        }

        public static decimal GetValeurTotaleStock()
        {
            // Somme (Quantité * Coût)
            // Il faut joindre produits et niveaux_stock
            string query = @"
                SELECT COALESCE(SUM(p.cout * ns.qte_disponible), 0)
                FROM produits p
                JOIN niveaux_stock ns ON p.id = ns.produit_id";
            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query));
        }

        public static DataTable GetAlertesReapprovisionnement()
        {
            // Produits dont qte <= seuil
            string query = @"
                SELECT p.id, p.sku, p.nom, ns.qte_disponible, p.seuil_reapprovisionnement, p.cout, p.fournisseur_id
                FROM produits p
                JOIN niveaux_stock ns ON p.id = ns.produit_id
                WHERE ns.qte_disponible <= p.seuil_reapprovisionnement";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetCommandesATraiter()
        {
            // Commandes non terminées
            string query = @"
                SELECT cv.numero_commande, c.nom AS client, cv.statut
                FROM commandes_vente cv
                JOIN clients c ON cv.client_id = c.id
                WHERE cv.statut NOT IN ('Expédiée', 'Fermée', 'Annulée', 'Livrée')
                ORDER BY cv.date_commande DESC
                LIMIT 10";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static DataTable GetAlertesServiceClient()
        {
            // Notes <= 2
            string query = @"
                SELECT ec.date_evaluation, c.nom AS client, ec.note_satisfaction, ec.commentaire
                FROM evaluations_clients ec
                JOIN clients c ON ec.client_id = c.id
                WHERE ec.note_satisfaction <= 2
                ORDER BY ec.date_evaluation DESC
                LIMIT 10";
            return DatabaseHelper.ExecuteQuery(query);
        }

        // --- DASHBOARD FINANCIER ---

        public static decimal GetVentesTotales(DateTime debut, DateTime fin)
        {
            // Ventes = Toutes les factures non annulées dans la période (peu importe le statut de paiement)
            // Utiliser DATE() pour comparer seulement les dates sans l'heure
            string query = @"
                SELECT COALESCE(SUM(montant_total), 0) 
                FROM factures 
                WHERE DATE(date_facture) BETWEEN DATE(@debut) AND DATE(@fin)
                AND statut != 'Annulée'";
            
            var parameters = new Dictionary<string, object>
            {
                { "@debut", debut.ToString("yyyy-MM-dd") },
                { "@fin", fin.ToString("yyyy-MM-dd") }
            };
            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        public static decimal GetDepensesExploitation(DateTime debut, DateTime fin)
        {
            // Dépenses = Achats Fournisseurs + Dépenses générales
            var parameters = new Dictionary<string, object>
            {
                { "@debut", debut },
                { "@fin", fin }
            };
            
            // 1. Achats fournisseurs (commandes reçues)
            string queryAchats = @"
                SELECT COALESCE(SUM(montant_total), 0) 
                FROM commandes_fournisseurs 
                WHERE DATE(date_commande) BETWEEN DATE(@debut) AND DATE(@fin)
                AND statut != 'Annulée'";
            
            decimal achats = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(queryAchats, parameters));
            
            // 2. Dépenses générales (table depenses)
            string queryDepenses = @"
                SELECT COALESCE(SUM(montant), 0) 
                FROM depenses 
                WHERE DATE(date_depense) BETWEEN DATE(@debut) AND DATE(@fin)";
            
            decimal depensesGenerales = Convert.ToDecimal(DatabaseHelper.ExecuteScalar(queryDepenses, parameters));
            
            return achats + depensesGenerales;
        }

        public static decimal GetCoutMarchandisesVendues(DateTime debut, DateTime fin)
        {
            // Coût des marchandises vendues = Coût des produits vendus dans toutes les factures non annulées
            string query = @"
                SELECT COALESCE(SUM(lf.quantite * p.cout), 0)
                FROM lignes_factures lf
                JOIN factures f ON lf.facture_id = f.id
                JOIN produits p ON lf.produit_id = p.id
                WHERE DATE(f.date_facture) BETWEEN DATE(@debut) AND DATE(@fin)
                AND f.statut != 'Annulée'";
            
            var parameters = new Dictionary<string, object>
            {
                { "@debut", debut.ToString("yyyy-MM-dd") },
                { "@fin", fin.ToString("yyyy-MM-dd") }
            };
            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query, parameters));
        }

        public static DataTable GetFacturesEnAttente()
        {
            string query = @"
                SELECT 
                    f.numero_facture AS numero_facture,
                    c.nom AS client,
                    f.montant_du AS montant_du
                FROM factures f
                JOIN clients c ON f.client_id = c.id
                WHERE f.montant_du > 0 AND f.statut != 'Annulée'
                ORDER BY f.date_echeance ASC
                LIMIT 10";
            return DatabaseHelper.ExecuteQuery(query);
        }

        public static decimal GetTaxesAPayer()
        {
            // Cumul TPS + TVQ des factures non annulées
            // Idéalement on devrait soustraire les taxes payées sur achats (CMI), mais restons simple : Taxes Collectées
            string query = @"
                SELECT COALESCE(SUM(montant_tps + montant_tvq), 0)
                FROM factures
                WHERE statut != 'Annulée'";
            return Convert.ToDecimal(DatabaseHelper.ExecuteScalar(query));
        }
        
        public static Dictionary<string, decimal> GetEvolutionVentes(int mois)
        {
            // Récupérer les ventes des X derniers mois
            var result = new Dictionary<string, decimal>();
            
            string query = @"
                SELECT DATE_FORMAT(date_facture, '%Y-%m') as mois, SUM(montant_total) as total
                FROM factures
                WHERE date_facture >= DATE_SUB(CURDATE(), INTERVAL @mois MONTH)
                GROUP BY mois
                ORDER BY mois";
                
            var parameters = new Dictionary<string, object>
            {
                { "@mois", mois }
            };
            
            var dt = DatabaseHelper.ExecuteQuery(query, parameters);
            foreach(DataRow row in dt.Rows)
            {
                result.Add(row["mois"].ToString(), Convert.ToDecimal(row["total"]));
            }
            
            return result;
        }

        public static DataTable GetDernieresTransactions()
        {
            // Extrait journal comptable : Ventes (Factures) + Achats (Commandes Fournisseurs) + Dépenses
            string query = @"
                SELECT 
                    'Vente' as type, 
                    f.date_facture as date_transaction, 
                    f.montant_total as montant, 
                    'Crédit' as sens
                FROM factures f
                WHERE f.statut != 'Annulée'
                UNION ALL
                SELECT 
                    'Achat' as type, 
                    cf.date_commande as date_transaction, 
                    cf.montant_total as montant, 
                    'Débit' as sens
                FROM commandes_fournisseurs cf
                WHERE cf.statut != 'Annulée'
                UNION ALL
                SELECT 
                    'Dépense' as type, 
                    d.date_depense as date_transaction, 
                    d.montant as montant, 
                    'Débit' as sens
                FROM depenses d
                ORDER BY date_transaction DESC
                LIMIT 10";
            
            return DatabaseHelper.ExecuteQuery(query);
        }
    }
}

