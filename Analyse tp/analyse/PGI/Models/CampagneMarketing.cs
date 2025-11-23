using System;

namespace PGI.Models
{
    public class CampagneMarketing
    {
        public int Id { get; set; }
        public string NomCampagne { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Email, SMS, Publicité, Promo
        public string Description { get; set; } = string.Empty;
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal Budget { get; set; }
        public int NombreDestinataires { get; set; }
        public int NombreReponses { get; set; } = 0;
        public decimal TauxParticipation { get; set; } = 0; // Calculé automatiquement
        public string Statut { get; set; } = "Planifiée"; // Planifiée, Active, Terminée, Annulée
        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}

