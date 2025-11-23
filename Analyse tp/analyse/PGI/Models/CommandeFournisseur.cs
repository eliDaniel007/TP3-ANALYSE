using System;
using System.Collections.Generic;

namespace PGI.Models
{
    public class CommandeFournisseur
    {
        public int Id { get; set; }
        public string NumeroCommande { get; set; } = string.Empty; // Format: CMD-2025-0001
        public DateTime DateCommande { get; set; } = DateTime.Now;
        public DateTime? DateLivraisonPrevue { get; set; }
        public DateTime? DateReception { get; set; }
        public int FournisseurId { get; set; }
        public int? EmployeId { get; set; } // Qui a créé la commande
        
        // Montants
        public decimal SousTotal { get; set; }
        public decimal MontantTPS { get; set; }
        public decimal MontantTVQ { get; set; }
        public decimal MontantTotal { get; set; }
        
        // Statuts
        public string Statut { get; set; } = "En attente"; // En attente, Partiellement reçue, Reçue, Annulée
        
        // Informations additionnelles
        public string? NoteInterne { get; set; }
        
        // Propriétés calculées (pas dans la BDD)
        public string NomFournisseur { get; set; } = string.Empty;
        public string NomEmploye { get; set; } = string.Empty;
        public List<LigneCommandeFournisseur> Lignes { get; set; } = new List<LigneCommandeFournisseur>();
    }
}

