using System;
using System.Collections.Generic;

namespace PGI.Models
{
    public class Facture
    {
        public int Id { get; set; }
        public string NumeroFacture { get; set; } = string.Empty; // Format: FAC-2025-0001
        public DateTime DateFacture { get; set; } = DateTime.Now;
        public DateTime DateEcheance { get; set; }
        public int ClientId { get; set; }
        public int? EmployeId { get; set; } // Vendeur
        
        // Montants
        public decimal SousTotal { get; set; }
        public decimal MontantTPS { get; set; }
        public decimal MontantTVQ { get; set; }
        public decimal MontantTotal { get; set; }
        public decimal MontantPaye { get; set; }
        public decimal MontantDu { get; set; }
        
        // Statuts
        public string StatutPaiement { get; set; } = "Impayée"; // Impayée, Partielle, Payée
        public string Statut { get; set; } = "Active"; // Active, Annulée
        
        // Informations additionnelles
        public string? NoteInterne { get; set; }
        public string? ConditionsPaiement { get; set; } = "Net 30 jours";
        
        // Propriétés calculées (pas dans la BDD)
        public string NomClient { get; set; } = string.Empty;
        public string CourrielClient { get; set; } = string.Empty;
        public string NomVendeur { get; set; } = string.Empty;
        public List<LigneFacture> Lignes { get; set; } = new List<LigneFacture>();
    }
}

