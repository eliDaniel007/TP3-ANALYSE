using System;

namespace PGI.Models
{
    public class Fournisseur
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string CourrielContact { get; set; } = string.Empty;
        public int DelaiLivraisonJours { get; set; } = 7;
        public decimal PourcentageEscompte { get; set; } = 0;
        public string Statut { get; set; } = "Actif";
        public DateTime DateCreation { get; set; }
    }
}
