using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class CampaignDetailsWindow : Window
    {
        private int _campagneId;
        private CampagneMarketing _campagne;

        public CampaignDetailsWindow(int campagneId)
        {
            InitializeComponent();
            _campagneId = campagneId;
            LoadCampagneData();
        }

        private void LoadCampagneData()
        {
            try
            {
                _campagne = CampagneMarketingService.GetCampagneById(_campagneId);
                if (_campagne == null)
                {
                    MessageBox.Show("Campagne introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // En-tête
                TxtNomCampagne.Text = _campagne.NomCampagne;
                TxtType.Text = _campagne.Type;
                TxtStatut.Text = _campagne.Statut.ToUpper();
                
                // Couleur statut
                if (_campagne.Statut == "Active") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                else if (_campagne.Statut == "Terminée") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                else if (_campagne.Statut == "Annulée") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF4444"));
                else BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B")); // Planifiée

                // KPIs
                TxtBudget.Text = _campagne.Budget.ToString("C");
                TxtDestinataires.Text = _campagne.NombreDestinataires.ToString();
                TxtReponses.Text = _campagne.NombreReponses.ToString();
                TxtTauxReponse.Text = _campagne.TauxParticipation.ToString("P2");

                // Informations
                TxtDescription.Text = !string.IsNullOrWhiteSpace(_campagne.Description) ? _campagne.Description : "Aucune description";
                TxtDateDebut.Text = _campagne.DateDebut.ToString("dd/MM/yyyy");
                TxtDateFin.Text = _campagne.DateFin.ToString("dd/MM/yyyy");
                TxtDateCreation.Text = _campagne.DateCreation.ToString("dd/MM/yyyy");

                // Statistiques détaillées
                if (_campagne.Statut == "Terminée")
                {
                    var joursDuree = (_campagne.DateFin - _campagne.DateDebut).Days + 1;
                    var coutParDestinataire = _campagne.NombreDestinataires > 0 
                        ? _campagne.Budget / _campagne.NombreDestinataires 
                        : 0;
                    
                    TxtStatistiques.Text = $"Durée de la campagne : {joursDuree} jour(s)\n" +
                                          $"Coût par destinataire : {coutParDestinataire:C}\n" +
                                          $"Budget total : {_campagne.Budget:C}\n" +
                                          $"Taux de réponse : {_campagne.TauxParticipation:P2}\n" +
                                          $"Nombre de réponses : {_campagne.NombreReponses} / {_campagne.NombreDestinataires}";
                }
                else
                {
                    var joursRestants = (_campagne.DateFin - DateTime.Now).Days;
                    if (joursRestants < 0) joursRestants = 0;
                    
                    TxtStatistiques.Text = $"Campagne en cours...\n" +
                                          $"Jours restants : {joursRestants}\n" +
                                          $"Réponses reçues : {_campagne.NombreReponses} / {_campagne.NombreDestinataires}\n" +
                                          $"Taux de réponse actuel : {_campagne.TauxParticipation:P2}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de la campagne : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

