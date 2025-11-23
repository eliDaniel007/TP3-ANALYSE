using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PGI.Services;
using PGI.Models;

namespace PGI.Views.CRM
{
    public partial class ClientDetailsWindow : Window
    {
        private int clientId;

        public ClientDetailsWindow(int clientId)
        {
            InitializeComponent();
            this.clientId = clientId;
            LoadClientDetails();
        }

        private void LoadClientDetails()
        {
            try
            {
                // Charger les informations du client
                var client = ClientService.GetClientById(clientId);
                if (client == null)
                {
                    MessageBox.Show("Client introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // En-tête
                TxtNomClient.Text = client.Nom;
                TxtStatut.Text = client.Statut.ToUpper();
                TxtType.Text = client.Type;
                
                // Couleur du statut
                BorderStatut.Background = client.Statut switch
                {
                    "Actif" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981")),
                    "Fidèle" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B")),
                    "Prospect" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#669BBC")),
                    "Inactif" => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6B7280")),
                    _ => Brushes.Gray
                };

                // Informations de contact
                TxtEmail.Text = client.CourrielContact;
                TxtTelephone.Text = client.Telephone ?? "N/A";
                TxtDateCreation.Text = client.DateCreation.ToString("yyyy-MM-dd");

                // Statistiques
                var stats = ClientStatistiquesService.GetStatistiquesClient(clientId);
                if (stats != null)
                {
                    TxtCA.Text = $"{stats.ChiffreAffairesTotal:N2} $";
                    TxtNbCommandes.Text = stats.NombreCommandes.ToString();
                    TxtPanierMoyen.Text = $"{stats.PanierMoyen:N2} $";
                    TxtScore.Text = $"{stats.ScoreComposite:N1}";
                }

                // Charger l'historique des commandes (factures)
                LoadCommandes();
                
                // Charger les interactions
                LoadInteractions();
                
                // Charger les évaluations
                LoadEvaluations();
                
                // Charger les alertes
                LoadAlertes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors du chargement des détails du client:\n{ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void LoadCommandes()
        {
            try
            {
                var factures = FactureService.GetFacturesByClient(clientId);
                
                var commandesDisplay = factures.Select(f => new CommandeDisplay
                {
                    Date = f.DateFacture.ToString("yyyy-MM-dd"),
                    NumeroFacture = f.NumeroFacture,
                    MontantFormate = $"{f.MontantTotal:N2} $",
                    Statut = f.StatutPaiement
                }).ToList();
                
                CommandesDataGrid.ItemsSource = commandesDisplay;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des commandes:\n{ex.Message}");
            }
        }

        private void LoadInteractions()
        {
            try
            {
                var interactions = InteractionClientService.GetInteractionsByClient(clientId);
                
                var interactionsDisplay = interactions.Select(i => new InteractionDisplay
                {
                    DateFormate = i.DateInteraction.ToString("yyyy-MM-dd HH:mm"),
                    TypeInteraction = i.TypeInteraction,
                    Sujet = i.Sujet,
                    NomEmploye = i.NomEmploye
                }).ToList();
                
                InteractionsDataGrid.ItemsSource = interactionsDisplay;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des interactions:\n{ex.Message}");
            }
        }

        private void LoadEvaluations()
        {
            try
            {
                var evaluations = EvaluationClientService.GetEvaluationsByClient(clientId);
                
                var evaluationsDisplay = evaluations.Select(e => new EvaluationDisplay
                {
                    DateFormate = e.DateEvaluation.ToString("yyyy-MM-dd"),
                    NoteFormate = $"{e.NoteSatisfaction} / 5 ⭐",
                    NumeroFacture = e.NumeroFacture ?? "N/A",
                    Commentaire = e.Commentaire ?? "Aucun commentaire"
                }).ToList();
                
                EvaluationsDataGrid.ItemsSource = evaluationsDisplay;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des évaluations:\n{ex.Message}");
            }
        }

        private void LoadAlertes()
        {
            try
            {
                var alertes = AlerteServiceClientService.GetAlertesByClient(clientId);
                
                var alertesDisplay = alertes.Select(a => new AlerteDisplay
                {
                    DateFormate = a.DateCreation.ToString("yyyy-MM-dd"),
                    TypeAlerte = a.TypeAlerte,
                    Priorite = a.Priorite,
                    Statut = a.Statut,
                    Description = a.Description
                }).ToList();
                
                AlertesDataGrid.ItemsSource = alertesDisplay;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des alertes:\n{ex.Message}");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Classes d'affichage
        private class CommandeDisplay
        {
            public string Date { get; set; } = string.Empty;
            public string NumeroFacture { get; set; } = string.Empty;
            public string MontantFormate { get; set; } = string.Empty;
            public string Statut { get; set; } = string.Empty;
        }

        private class InteractionDisplay
        {
            public string DateFormate { get; set; } = string.Empty;
            public string TypeInteraction { get; set; } = string.Empty;
            public string Sujet { get; set; } = string.Empty;
            public string NomEmploye { get; set; } = string.Empty;
        }

        private class EvaluationDisplay
        {
            public string DateFormate { get; set; } = string.Empty;
            public string NoteFormate { get; set; } = string.Empty;
            public string NumeroFacture { get; set; } = string.Empty;
            public string Commentaire { get; set; } = string.Empty;
        }

        private class AlerteDisplay
        {
            public string DateFormate { get; set; } = string.Empty;
            public string TypeAlerte { get; set; } = string.Empty;
            public string Priorite { get; set; } = string.Empty;
            public string Statut { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}

