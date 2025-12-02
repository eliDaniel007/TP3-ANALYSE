using System;
using System.Collections.Generic;

namespace PGI.Models
{
    public class Commande
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string NumeroCommande { get; set; } = string.Empty;
        public DateTime DateCommande { get; set; }
        public string Statut { get; set; } = "Brouillon"; // Brouillon, Confirmée, Facturée, Annulée
        public decimal MontantTotal { get; set; }
        public string? AdresseLivraison { get; set; }
        public string? Notes { get; set; }
        
        // Propriétés de navigation
        public string NomClient { get; set; } = string.Empty;
        public List<LigneCommande> Lignes { get; set; } = new List<LigneCommande>();
    }

    public class LigneCommande
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public int ProduitId { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal SousTotal { get; set; }
        
        // Propriétés de navigation
        public string NomProduit { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
    }
}


