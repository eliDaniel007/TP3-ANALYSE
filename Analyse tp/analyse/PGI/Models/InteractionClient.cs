using System;

namespace PGI.Models
{
    public class InteractionClient
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int? EmployeId { get; set; }
        public string TypeInteraction { get; set; } = string.Empty; // Email, Téléphone, Réunion, Vente, Note, Réclamation
        public string Sujet { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateInteraction { get; set; } = DateTime.Now;
        public string? ResultatAction { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.Now;
        
        // Propriétés calculées/navigation (pas dans la BDD)
        public string NomClient { get; set; } = string.Empty;
        public string NomEmploye { get; set; } = string.Empty;
    }
}

