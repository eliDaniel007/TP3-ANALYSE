using System;

namespace PGI.Models
{
    public class AlerteServiceClient
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? EvaluationId { get; set; }
        public string TypeAlerte { get; set; } = string.Empty; // Satisfaction faible, Retard paiement, Inactivité
        public string Description { get; set; } = string.Empty;
        public string Priorite { get; set; } = "Moyenne"; // Basse, Moyenne, Haute, Urgente
        public string Statut { get; set; } = "Ouverte"; // Ouverte, En cours, Résolue, Fermée
        public int? EmployeAssigneId { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public DateTime? DateResolution { get; set; }
        public string? NoteResolution { get; set; }
        
        // Propriétés calculées/navigation
        public string NomClient { get; set; } = string.Empty;
        public string? NomEmployeAssigne { get; set; }
    }
}

