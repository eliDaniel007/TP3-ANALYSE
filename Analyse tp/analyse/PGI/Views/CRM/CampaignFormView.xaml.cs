using System;
using System.Windows;
using System.Windows.Controls;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class CampaignFormView : UserControl
    {
        private int? _campagneId = null; // null pour nouvelle campagne, ID pour édition

        public CampaignFormView()
        {
            InitializeComponent();
            DpDateDebut.SelectedDate = DateTime.Now;
            DpDateFin.SelectedDate = DateTime.Now.AddDays(30);
        }

        public CampaignFormView(int campagneId) : this()
        {
            _campagneId = campagneId;
            LoadCampagneData();
        }

        private void LoadCampagneData()
        {
            try
            {
                var campagne = CampagneMarketingService.GetCampagneById(_campagneId.Value);
                if (campagne == null)
                {
                    MessageBox.Show("Campagne introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                TxtTitle.Text = $"📢 Modifier la Campagne : {campagne.NomCampagne}";
                TxtNomCampagne.Text = campagne.NomCampagne;
                TxtDescription.Text = campagne.Description;
                DpDateDebut.SelectedDate = campagne.DateDebut;
                DpDateFin.SelectedDate = campagne.DateFin;
                TxtBudget.Text = campagne.Budget.ToString("F2");
                TxtNbDestinataires.Text = campagne.NombreDestinataires.ToString();

                // Sélectionner le type dans la ComboBox
                foreach (ComboBoxItem item in CmbType.Items)
                {
                    if (item.Content.ToString() == campagne.Type)
                    {
                        CmbType.SelectedItem = item;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la campagne : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(TxtNomCampagne.Text))
            {
                MessageBox.Show("Le nom de la campagne est requis.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpDateDebut.SelectedDate == null || DpDateFin.SelectedDate == null)
            {
                MessageBox.Show("Les dates de début et de fin sont requises.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpDateFin.SelectedDate < DpDateDebut.SelectedDate)
            {
                MessageBox.Show("La date de fin doit être postérieure ou égale à la date de début.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtBudget.Text, out decimal budget) || budget < 0)
            {
                MessageBox.Show("Le budget doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtNbDestinataires.Text, out int nbDestinataires) || nbDestinataires <= 0)
            {
                MessageBox.Show("Le nombre de destinataires doit être un nombre positif.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var campagne = new CampagneMarketing
                {
                    NomCampagne = TxtNomCampagne.Text.Trim(),
                    Type = (CmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Email",
                    Description = TxtDescription.Text.Trim(),
                    DateDebut = DpDateDebut.SelectedDate.Value,
                    DateFin = DpDateFin.SelectedDate.Value,
                    Budget = budget,
                    NombreDestinataires = nbDestinataires,
                    Statut = "Planifiée"
                };

                if (_campagneId.HasValue)
                {
                    // Mise à jour
                    campagne.Id = _campagneId.Value;
                    CampagneMarketingService.UpdateCampagne(campagne);
                    MessageBox.Show("Campagne mise à jour avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Création
                    CampagneMarketingService.CreerCampagne(campagne);
                    MessageBox.Show("Campagne créée avec succès !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Retourner à la liste
                var parent = FindParentCRMMainView(this);
                if (parent != null)
                {
                    parent.NavigateToCampaigns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            var parent = FindParentCRMMainView(this);
            if (parent != null)
            {
                parent.NavigateToCampaigns();
            }
        }

        private CRMMainView? FindParentCRMMainView(DependencyObject child)
        {
            DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(child);
            while (parent != null && !(parent is CRMMainView))
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }
            return parent as CRMMainView;
        }
    }
}

