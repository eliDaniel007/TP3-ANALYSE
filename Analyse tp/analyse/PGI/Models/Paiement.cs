using System;

namespace PGI.Models
{
    public class Paiement
    {
        public int Id { get; set; }
        public int FactureId { get; set; }
        public DateTime DatePaiement { get; set; } = DateTime.Now;
        public decimal Montant { get; set; }
        public string ModePaiement { get; set; } = "Comptant"; // Comptant, Carte, Chèque, Virement
        public string? NumeroReference { get; set; } // Numéro de chèque, référence de transaction
        public string? Note { get; set; }
        public int? EmployeId { get; set; } // Qui a enregistré le paiement
        
        // Propriétés calculées (pas dans la BDD)
        public string NumeroFacture { get; set; } = string.Empty;
        public string NomClient { get; set; } = string.Empty;
        public string NomEmploye { get; set; } = string.Empty;
    }
}

