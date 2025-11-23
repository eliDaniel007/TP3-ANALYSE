using System;

namespace PGI.Models
{
    public class LigneFacture
    {
        public int Id { get; set; }
        public int FactureId { get; set; }
        public int ProduitId { get; set; }
        
        // Détails du produit (snapshot au moment de la vente)
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal MontantLigne { get; set; } // Quantite * PrixUnitaire
        
        // Propriétés calculées (pas dans la BDD)
        public string NomProduit { get; set; } = string.Empty;
    }
}

