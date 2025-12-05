using System;
using System.Linq;
using System.Windows.Controls;
using PGI.Services;

namespace PGI.Views.CRM
{
    public partial class SatisfactionView : UserControl
    {
        public SatisfactionView()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var evaluations = EvaluationClientService.GetAllEvaluations(50);
                
                // Calculer la moyenne
                double moyenne = evaluations.Count > 0 ? evaluations.Average(e => e.NoteSatisfaction) : 0;
                TxtMoyenne.Text = moyenne.ToString("N1");

                // Afficher la liste
                var displayList = evaluations.Select(e => new
                {
                    DateFormate = e.DateEvaluation.ToString("yyyy-MM-dd"),
                    NomClient = e.NomClient,
                    NoteEtoiles = new string('★', e.NoteSatisfaction) + new string('☆', 5 - e.NoteSatisfaction),
                    Commentaire = e.Commentaire,
                    NumeroFacture = e.NumeroFacture ?? "-"
                }).ToList();

                EvaluationsDataGrid.ItemsSource = displayList;
            }
            catch (Exception ex)
            {
                // En prod: Logger
                Console.WriteLine(ex.Message);
            }
        }
    }
}
