using System;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using PGI.Models;

namespace PGI.Services
{
    public class RapportFinancierService
    {
        /// <summary>
        /// Générer un rapport financier pour une période
        /// </summary>
        public static RapportFinancier GenererRapport(DateTime dateDebut, DateTime dateFin)
        {
            var rapport = new RapportFinancier
            {
                DateDebut = dateDebut,
                DateFin = dateFin
            };

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // 1. Calcul des ventes
                    string queryVentes = @"
                        SELECT 
                            COUNT(*) AS nb_factures,
                            COALESCE(SUM(sous_total), 0) AS total_ventes,
                            COALESCE(SUM(montant_tps), 0) AS total_tps,
                            COALESCE(SUM(montant_tvq), 0) AS total_tvq
                        FROM factures
                        WHERE date_facture BETWEEN @dateDebut AND @dateFin
                        AND statut = 'Active'";
                    
                    using (var cmd = new MySqlCommand(queryVentes, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rapport.NombreFactures = reader.GetInt32("nb_factures");
                                rapport.TotalVentes = reader.GetDecimal("total_ventes");
                                rapport.TotalTPS = reader.GetDecimal("total_tps");
                                rapport.TotalTVQ = reader.GetDecimal("total_tvq");
                            }
                        }
                    }

                    // 2. Calcul des paiements reçus
                    string queryPaiements = @"
                        SELECT COALESCE(SUM(montant), 0) AS total_paiements
                        FROM paiements
                        WHERE date_paiement BETWEEN @dateDebut AND @dateFin";
                    
                    using (var cmd = new MySqlCommand(queryPaiements, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        rapport.TotalPaiementsRecus = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0m);
                    }

                    // 3. Calcul du coût des produits vendus (CPV)
                    string queryCPV = @"
                        SELECT COALESCE(SUM(lf.quantite * p.cout), 0) AS cout_produits_vendus
                        FROM lignes_factures lf
                        INNER JOIN factures f ON lf.facture_id = f.id
                        INNER JOIN produits p ON lf.produit_id = p.id
                        WHERE f.date_facture BETWEEN @dateDebut AND @dateFin
                        AND f.statut = 'Active'";
                    
                    using (var cmd = new MySqlCommand(queryCPV, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        rapport.CoutProduitsVendus = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0m);
                    }

                    // 4. Calcul des achats (commandes fournisseurs reçues)
                    string queryAchats = @"
                        SELECT 
                            COUNT(*) AS nb_commandes,
                            COALESCE(SUM(sous_total), 0) AS total_achats
                        FROM commandes_fournisseurs
                        WHERE date_reception BETWEEN @dateDebut AND @dateFin
                        AND statut = 'Reçue'";
                    
                    using (var cmd = new MySqlCommand(queryAchats, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rapport.NombreCommandes = reader.GetInt32("nb_commandes");
                                rapport.TotalAchats = reader.GetDecimal("total_achats");
                            }
                        }
                    }

                    // 5. Calcul des charges opérationnelles
                    string queryCharges = @"
                        SELECT COALESCE(SUM(montant), 0) AS total_charges
                        FROM charges_operationnelles
                        WHERE date_charge BETWEEN @dateDebut AND @dateFin
                        AND statut = 'Payée'";
                    
                    using (var cmd = new MySqlCommand(queryCharges, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        rapport.ChargesOperationnelles = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0m);
                    }

                    // 6. Calcul du montant impayé
                    string queryImpaye = @"
                        SELECT COALESCE(SUM(montant_du), 0) AS total_impaye
                        FROM factures
                        WHERE date_facture BETWEEN @dateDebut AND @dateFin
                        AND statut = 'Active'
                        AND statut_paiement IN ('Impayée', 'Partielle')";
                    
                    using (var cmd = new MySqlCommand(queryImpaye, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        rapport.TotalImpaye = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0m);
                    }
                }

                // 7. Calculs des profits
                rapport.ProfitBrut = rapport.TotalVentes - rapport.CoutProduitsVendus;
                rapport.ProfitNet = rapport.ProfitBrut - rapport.ChargesOperationnelles;
                
                // 8. Calcul de la marge de profit
                if (rapport.TotalVentes > 0)
                {
                    rapport.MargeProfit = Math.Round((rapport.ProfitNet / rapport.TotalVentes) * 100, 2);
                }
                else
                {
                    rapport.MargeProfit = 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la génération du rapport financier: {ex.Message}", ex);
            }

            return rapport;
        }

        /// <summary>
        /// Générer un rapport pour le mois en cours
        /// </summary>
        public static RapportFinancier GenererRapportMoisEnCours()
        {
            DateTime debut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fin = debut.AddMonths(1).AddDays(-1);
            return GenererRapport(debut, fin);
        }

        /// <summary>
        /// Générer un rapport pour l'année en cours
        /// </summary>
        public static RapportFinancier GenererRapportAnneeEnCours()
        {
            DateTime debut = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime fin = new DateTime(DateTime.Now.Year, 12, 31);
            return GenererRapport(debut, fin);
        }

        /// <summary>
        /// Obtenir le top 5 des clients par revenus
        /// </summary>
        public static List<(string nomClient, decimal totalVentes, int nombreFactures)> GetTop5Clients(DateTime dateDebut, DateTime dateFin)
        {
            var resultats = new List<(string, decimal, int)>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            c.nom AS nom_client,
                            COUNT(f.id) AS nb_factures,
                            COALESCE(SUM(f.montant_total), 0) AS total_ventes
                        FROM clients c
                        INNER JOIN factures f ON c.id = f.client_id
                        WHERE f.date_facture BETWEEN @dateDebut AND @dateFin
                        AND f.statut = 'Active'
                        GROUP BY c.id, c.nom
                        ORDER BY total_ventes DESC
                        LIMIT 5";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultats.Add((
                                    reader.GetString("nom_client"),
                                    reader.GetDecimal("total_ventes"),
                                    reader.GetInt32("nb_factures")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetTop5Clients: {ex.Message}");
            }

            return resultats;
        }

        /// <summary>
        /// Obtenir le top 5 des produits par revenus
        /// </summary>
        public static List<(string nomProduit, decimal totalVentes, int quantiteVendue)> GetTop5Produits(DateTime dateDebut, DateTime dateFin)
        {
            var resultats = new List<(string, decimal, int)>();

            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            p.nom AS nom_produit,
                            SUM(lf.quantite) AS qte_vendue,
                            SUM(lf.montant_ligne) AS total_ventes
                        FROM produits p
                        INNER JOIN lignes_factures lf ON p.id = lf.produit_id
                        INNER JOIN factures f ON lf.facture_id = f.id
                        WHERE f.date_facture BETWEEN @dateDebut AND @dateFin
                        AND f.statut = 'Active'
                        GROUP BY p.id, p.nom
                        ORDER BY total_ventes DESC
                        LIMIT 5";
                    
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                resultats.Add((
                                    reader.GetString("nom_produit"),
                                    reader.GetDecimal("total_ventes"),
                                    reader.GetInt32("qte_vendue")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetTop5Produits: {ex.Message}");
            }

            return resultats;
        }
    }
}

