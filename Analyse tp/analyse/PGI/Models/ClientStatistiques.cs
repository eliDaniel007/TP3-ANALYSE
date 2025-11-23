using System;

namespace PGI.Models
{
    /// <summary>
    /// Statistiques et KPIs d'un client
    /// </summary>
    public class ClientStatistiques
    {
        public int ClientId { get; set; }
        public string NomClient { get; set; } = string.Empty;
        public string Statut { get; set; } = string.Empty;
        
        // Statistiques d'achats
        public int NombreCommandes { get; set; }
        public decimal ChiffreAffairesTotal { get; set; }
        public decimal PanierMoyen { get; set; }
        public DateTime? DateDerniereCommande { get; set; }
        public int JoursSanActivite { get; set; }
        
        // Score et fidélisation
        public decimal ScoreComposite { get; set; } // Calculé : (CA / 1000) + (NbCommandes * 2) + (NoteMoyenne * 10)
        public decimal NoteSatisfactionMoyenne { get; set; }
        public int NombreInteractions { get; set; }
        public int NombreReclamations { get; set; }
        
        // Paiement
        public decimal MontantImpaye { get; set; }
        public bool RetardPaiement { get; set; }
    }
}

