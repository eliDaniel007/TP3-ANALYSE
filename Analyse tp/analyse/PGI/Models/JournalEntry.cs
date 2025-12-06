using System;

namespace PGI.Models
{
    /// <summary>
    /// Modèle pour une écriture comptable dans le journal général
    /// </summary>
    public class JournalEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Compte { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Reference { get; set; } = string.Empty; // Numéro de facture, commande, etc.
        public string TypeTransaction { get; set; } = string.Empty; // Vente, Achat, Salaire, Dépense
    }
}

