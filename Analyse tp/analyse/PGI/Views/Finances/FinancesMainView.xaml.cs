using System;
using System.Windows;
using System.Windows.Controls;

namespace PGI.Views.Finances
{
    public partial class FinancesMainView : UserControl
    {
        private Button activeSubNavButton;

        public FinancesMainView()
        {
            InitializeComponent();
            // Charger le tableau de bord par défaut
            NavigateToDashboard();
        }

        // Navigation vers le tableau de bord
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            SetActiveSubNavButton(BtnDashboard);
            LoadView("Views/Finances/FinancesDashboardView.xaml");
        }

        // Navigation vers Ventes/Factures
        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnSales);
            LoadView("Views/Finances/SalesListView.xaml");
        }

        // Navigation vers Achats Fournisseur
        private void BtnPurchases_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnPurchases);
            LoadView("Views/Finances/PurchasesListView.xaml");
        }

        // Navigation vers Dépenses
        private void BtnExpenses_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnExpenses);
            LoadView("Views/Finances/ExpensesView.xaml");
        }

        // Navigation vers Journal Comptable
        private void BtnAccounting_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnAccounting);
            LoadView("Views/Finances/AccountingJournalView.xaml");
        }

        // Navigation vers Rapports
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            SetActiveSubNavButton(BtnReports);
            LoadView("Views/Finances/ReportsView.xaml");
        }

        // Navigation vers le formulaire de facture
        public void NavigateToSaleForm()
        {
            SetActiveSubNavButton(BtnSales);
            LoadView("Views/Finances/SaleFormView.xaml");
        }

        // Navigation vers le formulaire d'achat
        public void NavigateToPurchaseForm(int? commandeId = null)
        {
            SetActiveSubNavButton(BtnPurchases);
            if (commandeId.HasValue)
            {
                // Charger le formulaire avec la commande à éditer
                try
                {
                    var constructor = typeof(PurchaseFormView).GetConstructor(new[] { typeof(int) });
                    if (constructor != null)
                    {
                        var control = (PurchaseFormView)constructor.Invoke(new object[] { commandeId.Value });
                        SubContentArea.Content = control;
                    }
                    else
                    {
                        SubContentArea.Content = new TextBlock { Text = "Erreur de chargement : constructeur introuvable" };
                    }
                }
                catch (Exception ex)
                {
                    SubContentArea.Content = new TextBlock { Text = $"Erreur de chargement : {ex.Message}" };
                }
            }
            else
            {
                LoadView("Views/Finances/PurchaseFormView.xaml");
            }
        }

        // Navigation vers Rapports (méthode publique pour accès depuis le dashboard)
        public void NavigateToReports()
        {
            SetActiveSubNavButton(BtnReports);
            LoadView("Views/Finances/ReportsView.xaml");
        }

        // Méthodes utilitaires
        private void SetActiveSubNavButton(Button button)
        {
            // Réinitialiser le style de l'ancien bouton actif
            if (activeSubNavButton != null)
            {
                activeSubNavButton.Style = (Style)FindResource("SubNavButtonStyle");
            }

            // Appliquer le style actif au nouveau bouton
            button.Style = (Style)FindResource("ActiveSubNavButtonStyle");
            activeSubNavButton = button;
        }

        private void LoadView(string viewPath)
        {
            try
            {
                var uri = new Uri(viewPath, UriKind.Relative);
                var control = Application.LoadComponent(uri) as UserControl;
                SubContentArea.Content = control;
            }
            catch (Exception ex)
            {
                ShowTemporaryMessage($"Erreur lors du chargement de la vue : {ex.Message}");
            }
        }

        private void ShowTemporaryMessage(string message)
        {
            var textBlock = new TextBlock
            {
                Text = message,
                FontSize = 18,
                Foreground = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#64748B")
                ),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(50)
            };

            SubContentArea.Content = textBlock;
        }
    }
}