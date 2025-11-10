using System;

namespace PGI.Models
{
    /// <summary>
    /// Classe pour représenter un article dans le panier
    /// </summary>
    public class PanierItem
    {
        public int ProduitId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
        public int StockDisponible { get; set; }
        public string Categorie { get; set; } = string.Empty;
        
        public decimal SousTotal => Prix * Quantite;
    }
}


