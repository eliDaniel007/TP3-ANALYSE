using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using PGI.Helpers;
using Microsoft.Win32;

namespace PGI.Services
{
    public class RapportService
    {
        /// <summary>
        /// Générer un rapport de taxes (TPS/TVQ) pour une période
        /// </summary>
        public static DataTable GetRapportTaxes(DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            DATE(f.date_facture) as date_facture,
                            f.numero_facture,
                            COALESCE(c.nom, 'Client inconnu') as client,
                            f.sous_total,
                            f.montant_tps,
                            f.montant_tvq,
                            f.montant_total
                        FROM factures f
                        LEFT JOIN clients c ON f.client_id = c.id
                        WHERE DATE(f.date_facture) BETWEEN @dateDebut AND @dateFin
                        AND f.statut != 'Annulée'
                        ORDER BY f.date_facture";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut.Date);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin.Date);
                        
                        var adapter = new MySqlDataAdapter(cmd);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la génération du rapport de taxes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Générer un rapport des ventes pour une période
        /// </summary>
        public static DataTable GetRapportVentes(DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            DATE(f.date_facture) as date_facture,
                            f.numero_facture,
                            COALESCE(c.nom, 'Client inconnu') as client,
                            f.sous_total,
                            f.montant_tps,
                            f.montant_tvq,
                            f.montant_total,
                            f.statut_paiement,
                            f.statut
                        FROM factures f
                        LEFT JOIN clients c ON f.client_id = c.id
                        WHERE DATE(f.date_facture) BETWEEN @dateDebut AND @dateFin
                        AND f.statut != 'Annulée'
                        ORDER BY f.date_facture";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dateDebut", dateDebut.Date);
                        cmd.Parameters.AddWithValue("@dateFin", dateFin.Date);
                        
                        var adapter = new MySqlDataAdapter(cmd);
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la génération du rapport des ventes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Exporter un DataTable en CSV
        /// </summary>
        public static void ExportToCSV(DataTable dataTable, string filePath, string title = "")
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                throw new Exception("Aucune donnée à exporter.");
            }

            var sb = new StringBuilder();

            // En-tête du rapport
            if (!string.IsNullOrEmpty(title))
            {
                sb.AppendLine(title);
                sb.AppendLine();
            }

            // En-têtes des colonnes
            var headers = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            sb.AppendLine(string.Join(",", headers));

            // Données
            foreach (DataRow row in dataTable.Rows)
            {
                var values = row.ItemArray.Select(v => 
                {
                    if (v == null || v == DBNull.Value)
                        return "";
                    
                    string str = v.ToString() ?? "";
                    // Échapper les guillemets et les virgules
                    if (str.Contains(",") || str.Contains("\"") || str.Contains("\n"))
                    {
                        str = str.Replace("\"", "\"\"");
                        return $"\"{str}\"";
                    }
                    return str;
                });
                sb.AppendLine(string.Join(",", values));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Exporter un DataTable en PDF (simplifié - génère un fichier texte formaté)
        /// Pour une vraie génération PDF, il faudrait utiliser une bibliothèque comme iTextSharp
        /// </summary>
        public static void ExportToPDF(DataTable dataTable, string filePath, string title = "")
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                throw new Exception("Aucune donnée à exporter.");
            }

            var sb = new StringBuilder();

            // En-tête du rapport
            if (!string.IsNullOrEmpty(title))
            {
                sb.AppendLine("=".PadRight(80, '='));
                string[] titleLines = title.Split('\n');
                foreach (var line in titleLines)
                {
                    sb.AppendLine(line.PadLeft((80 + line.Length) / 2));
                }
                sb.AppendLine("=".PadRight(80, '='));
                sb.AppendLine();
            }

            // En-têtes des colonnes
            var headers = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
            int colWidth = Math.Max(15, 100 / headers.Length);
            sb.AppendLine(string.Join(" | ", headers.Select(h => (h ?? "").PadRight(colWidth))));
            sb.AppendLine("-".PadRight(Math.Min(120, colWidth * headers.Length + (headers.Length - 1) * 3), '-'));

            // Données
            foreach (DataRow row in dataTable.Rows)
            {
                var values = row.ItemArray.Select(v => 
                {
                    string str = (v == null || v == DBNull.Value) ? "" : (v.ToString() ?? "");
                    if (str.Length > colWidth)
                        str = str.Substring(0, colWidth - 3) + "...";
                    return str.PadRight(colWidth);
                });
                sb.AppendLine(string.Join(" | ", values));
            }

            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Ouvrir une boîte de dialogue pour sauvegarder un fichier
        /// </summary>
        public static string? GetSaveFilePath(string defaultFileName, string filter)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    FileName = defaultFileName,
                    Filter = filter,
                    FilterIndex = 1,
                    DefaultExt = filter.Contains("CSV") ? "csv" : "txt"
                };

                bool? result = saveDialog.ShowDialog();
                if (result == true && !string.IsNullOrEmpty(saveDialog.FileName))
                {
                    return saveDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ouverture de la boîte de dialogue : {ex.Message}", ex);
            }

            return null;
        }
    }
}

