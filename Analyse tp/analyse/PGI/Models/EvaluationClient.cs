using System;

namespace PGI.Models
{
    public class EvaluationClient
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? FactureId { get; set; }
        public int NoteSatisfaction { get; set; } // 1 à 5
        public string? Commentaire { get; set; }
        public DateTime DateEvaluation { get; set; } = DateTime.Now;
        public bool AlerteGeneree { get; set; } = false; // true si note ≤ 2
        
        // Propriétés calculées/navigation
        public string NomClient { get; set; } = string.Empty;
        public string? NumeroFacture { get; set; }
    }
}

