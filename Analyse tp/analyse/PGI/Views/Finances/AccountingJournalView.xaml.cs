using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.Finances
{
    public partial class AccountingJournalView : UserControl
    {
        public AccountingJournalView()
        {
            InitializeComponent();
            // Initialiser avec une période plus large (année en cours) pour avoir des données
            TxtDateDebut.SelectedDate = new DateTime(DateTime.Now.Year, 1, 1);
            TxtDateFin.SelectedDate = DateTime.Now;
            LoadJournalData();
        }

        private void LoadJournalData()
        {
            try
            {
                DateTime dateDebut = TxtDateDebut.SelectedDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime dateFin = TxtDateFin.SelectedDate ?? DateTime.Now;

                var entries = JournalComptableService.GetJournalEntries(dateDebut, dateFin);
                JournalDataGrid.ItemsSource = entries;

                var (totalDebit, totalCredit) = JournalComptableService.CalculateTotals(entries);
                TxtTotalDebit.Text = $"{totalDebit:C}";
                TxtTotalCredit.Text = $"{totalCredit:C}";

                // Vérifier l'équilibre
                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    // Afficher un avertissement si déséquilibré (tolérance de 0.01$ pour arrondis)
                    TxtTotalDebit.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626"));
                    TxtTotalCredit.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#DC2626"));
                }
                else
                {
                    TxtTotalDebit.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#10B981"));
                    TxtTotalCredit.Foreground = new System.Windows.Media.SolidColorBrush(
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#10B981"));
                }
            }
            catch (Exception ex)
            {
                // Afficher l'erreur dans la console plutôt qu'un pop-up
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement du journal : {ex.Message}");
                // Afficher un message dans l'interface
                JournalDataGrid.ItemsSource = new List<JournalEntryDisplay>();
                TxtTotalDebit.Text = "0,00 $";
                TxtTotalCredit.Text = "0,00 $";
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadJournalData();
        }
    }
}

