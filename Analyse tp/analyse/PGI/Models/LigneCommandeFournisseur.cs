using System;

namespace PGI.Models
{
    public class LigneCommandeFournisseur
    {
        public int Id { get; set; }
        public int CommandeId { get; set; }
        public int ProduitId { get; set; }
        
        // Détails
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int QuantiteCommandee { get; set; }
        public int QuantiteRecue { get; set; } = 0;
        public decimal PrixUnitaire { get; set; }
        public decimal MontantLigne { get; set; }
        
        // Propriétés calculées (pas dans la BDD)
        public string NomProduit { get; set; } = string.Empty;
    }
}

