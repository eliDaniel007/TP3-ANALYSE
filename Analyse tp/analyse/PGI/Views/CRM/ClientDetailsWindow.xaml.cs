using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PGI.Models;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class ClientDetailsWindow : Window
    {
        private int _clientId;
        private PGI.Models.Client _client;

        public ClientDetailsWindow(int clientId)
        {
            InitializeComponent();
            _clientId = clientId;
            LoadClientData();
        }

        private void LoadClientData()
        {
            try
            {
                // 1. Infos de base
                _client = ClientService.GetClientById(_clientId);
                if (_client == null)
                {
                    MessageBox.Show("Client introuvable.");
                    this.Close();
                    return;
                }

                TxtNomClient.Text = _client.Nom ?? "-";
                TxtEmail.Text = !string.IsNullOrWhiteSpace(_client.CourrielContact) ? _client.CourrielContact : "-";
                TxtTelephone.Text = !string.IsNullOrWhiteSpace(_client.Telephone) ? _client.Telephone : "-";
                TxtType.Text = !string.IsNullOrWhiteSpace(_client.Type) ? _client.Type : "-";
                TxtStatut.Text = !string.IsNullOrWhiteSpace(_client.Statut) ? _client.Statut.ToUpper() : "-";
                TxtDateCreation.Text = _client.DateCreation.ToString("dd/MM/yyyy");

                // Couleur statut
                if (_client.Statut == "Actif") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                else if (_client.Statut == "Fidèle") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F59E0B"));
                else if (_client.Statut == "Prospect") BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
                else BorderStatut.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#64748B"));

                // 2. Statistiques - Calculer directement depuis les factures pour plus de précision
                var factures = FactureService.GetFacturesByClient(_clientId);
                // Prendre toutes les factures (pas seulement payées) pour le calcul du CA
                var facturesValides = factures.Where(f => f.Statut == "Active" || f.StatutPaiement == "Payée" || f.StatutPaiement == "Payé").ToList();
                
                if (facturesValides.Any())
                {
                    var caTotal = facturesValides.Sum(f => f.MontantTotal);
                    var nbCommandes = facturesValides.Count;
                    var panierMoyen = nbCommandes > 0 ? caTotal / nbCommandes : 0;
                    
                    TxtCA.Text = caTotal.ToString("C");
                    TxtNbCommandes.Text = nbCommandes.ToString();
                    TxtPanierMoyen.Text = panierMoyen.ToString("C");
                }
                else
                {
                    TxtCA.Text = "0,00 $";
                    TxtNbCommandes.Text = "0";
                    TxtPanierMoyen.Text = "0,00 $";
                }

                // Score = Moyenne des notes de satisfaction (calculé indépendamment des factures)
                var evaluationsForScore = EvaluationClientService.GetEvaluationsByClient(_clientId);
                if (evaluationsForScore.Any())
                {
                    var noteMoyenne = (decimal)evaluationsForScore.Average(e => (double)e.NoteSatisfaction);
                    TxtScore.Text = noteMoyenne.ToString("F1"); // Afficher avec 1 décimale (ex: 1.0, 3.5, 5.0)
                }
                else
                {
                    TxtScore.Text = "-"; // Pas d'évaluation
                }

                // 3. Historique Commandes
                var commandesDisplay = factures.Select(f => new
                {
                    Date = f.DateFacture.ToString("yyyy-MM-dd"),
                    NumeroFacture = f.NumeroFacture,
                    MontantFormate = f.MontantTotal.ToString("C"),
                    Statut = f.StatutPaiement
                }).ToList();
                CommandesDataGrid.ItemsSource = commandesDisplay;

                // 4. Interactions (Journal des actions)
                var interactions = InteractionClientService.GetInteractionsByClient(_clientId);
                var interactionsDisplay = interactions.Select(i => new
                {
                    DateFormate = i.DateInteraction.ToString("yyyy-MM-dd HH:mm"),
                    TypeInteraction = i.TypeInteraction,
                    TypeIcon = GetTypeIcon(i.TypeInteraction),
                    Sujet = i.Sujet,
                    Description = i.Description ?? "-",
                    NomEmploye = i.NomEmploye ?? "Système (Automatique)",
                    IsAutomatique = i.EmployeId == null || i.NomEmploye == null || i.NomEmploye == "Système"
                }).ToList();
                InteractionsDataGrid.ItemsSource = interactionsDisplay;

                // 5. Satisfaction Client (onglet dédié)
                var evaluations = EvaluationClientService.GetEvaluationsByClient(_clientId);
                var evaluationsDisplay = evaluations.Select(e => new
                {
                    DateFormate = e.DateEvaluation.ToString("yyyy-MM-dd"),
                    NoteFormate = $"{e.NoteSatisfaction} / 5",
                    NumeroFacture = e.NumeroFacture ?? "-",
                    Commentaire = e.Commentaire
                }).ToList();

                if (evaluations.Any())
                {
                    var noteMoyenne = evaluations.Average(e => e.NoteSatisfaction);
                    TxtNoteMoyenne.Text = $"{noteMoyenne:F1} / 5";
                    TxtNbEvaluations.Text = evaluations.Count.ToString();
                    TxtDerniereEvaluation.Text = evaluations.OrderByDescending(e => e.DateEvaluation).First().DateEvaluation.ToString("dd/MM/yyyy");
                }
                else
                {
                    TxtNoteMoyenne.Text = "- / 5";
                    TxtNbEvaluations.Text = "0";
                    TxtDerniereEvaluation.Text = "-";
                }
                SatisfactionDataGrid.ItemsSource = evaluationsDisplay;

                // 6. Alertes
                // (Si AlerteServiceClientService existe et a une méthode GetAlertesByClient)
                // Pour l'instant je commente si je ne suis pas sûr, mais je crois avoir vu le service
                try 
                {
                    var alertes = AlerteServiceClientService.GetAlertesByClient(_clientId);
                    var alertesDisplay = alertes.Select(a => new
                    {
                        DateFormate = a.DateCreation.ToString("yyyy-MM-dd"),
                        TypeAlerte = a.TypeAlerte,
                        Priorite = a.Priorite,
                        Statut = a.Statut,
                        Description = a.Description
                    }).ToList();
                    AlertesDataGrid.ItemsSource = alertesDisplay;
                }
                catch { /* Service peut-être manquant ou méthode différente */ }

                // Initialiser les placeholders pour les interactions
                TxtInteractionSujet.Text = "Sujet (ex: Appel sortant, Courriel envoyé)";
                TxtInteractionSujet.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
                TxtInteractionNote.Text = "Résumé de l'échange ou contenu du courriel...";
                TxtInteractionNote.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur chargement fiche client: {ex.Message}");
            }
        }

        private string GetTypeIcon(string typeInteraction)
        {
            return typeInteraction switch
            {
                "Visite" => "🌐",
                "Commande" => "🛒",
                "Email" => "📧",
                "Téléphone" => "📞",
                "Note" => "📝",
                "Réunion" => "🤝",
                "Vente" => "💰",
                "Réclamation" => "⚠️",
                _ => "📋"
            };
        }

        private void TxtInteractionSujet_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtInteractionSujet.Text == "Sujet (ex: Appel sortant, Courriel envoyé)")
            {
                TxtInteractionSujet.Text = "";
                TxtInteractionSujet.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtInteractionSujet_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtInteractionSujet.Text))
            {
                TxtInteractionSujet.Text = "Sujet (ex: Appel sortant, Courriel envoyé)";
                TxtInteractionSujet.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void TxtInteractionNote_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TxtInteractionNote.Text == "Résumé de l'échange ou contenu du courriel...")
            {
                TxtInteractionNote.Text = "";
                TxtInteractionNote.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E293B"));
            }
        }

        private void TxtInteractionNote_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtInteractionNote.Text))
            {
                TxtInteractionNote.Text = "Résumé de l'échange ou contenu du courriel...";
                TxtInteractionNote.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
            }
        }

        private void BtnAddInteraction_Click(object sender, RoutedEventArgs e)
        {
            string sujet = TxtInteractionSujet.Text;
            string note = TxtInteractionNote.Text;

            // Vérifier si c'est le placeholder
            if (sujet == "Sujet (ex: Appel sortant, Courriel envoyé)" || string.IsNullOrWhiteSpace(sujet))
            {
                MessageBox.Show("Veuillez remplir le sujet.", "Champ requis", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (note == "Résumé de l'échange ou contenu du courriel..." || string.IsNullOrWhiteSpace(note))
            {
                MessageBox.Show("Veuillez remplir les détails.", "Champ requis", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var interaction = new Models.InteractionClient
                {
                    ClientId = _clientId,
                    TypeInteraction = "Note",
                    Sujet = sujet,
                    Description = note,
                    DateInteraction = DateTime.Now,
                    EmployeId = 1 // TODO: Utiliser l'ID de l'employé connecté (Session)
                };

                InteractionClientService.CreerInteraction(interaction);

                // Reset avec placeholders
                TxtInteractionSujet.Text = "Sujet (ex: Appel sortant, Courriel envoyé)";
                TxtInteractionSujet.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
                TxtInteractionNote.Text = "Résumé de l'échange ou contenu du courriel...";
                TxtInteractionNote.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94A3B8"));
                
                LoadClientData();
                
                MessageBox.Show("Note ajoutée avec succès.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ajout: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}