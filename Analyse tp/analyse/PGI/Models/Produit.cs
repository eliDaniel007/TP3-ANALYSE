using System;

namespace PGI.Models
{
    public class Produit
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int CategorieId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cout { get; set; }
        public decimal Prix { get; set; }
        public decimal MargeBrute { get; set; }
        public int SeuilReapprovisionnement { get; set; }
        public int StockMinimum { get; set; }
        public decimal PoidsKg { get; set; }
        public int FournisseurId { get; set; }
        public string Statut { get; set; } = "Actif";
        public DateTime? DateEntreeStock { get; set; }
        public DateTime DateCreation { get; set; }

        // Propriétés calculées/navigations (pas dans la BDD)
        public string NomCategorie { get; set; } = string.Empty;
        public string NomFournisseur { get; set; } = string.Empty;
        public int StockDisponible { get; set; }
        public int StockReservee { get; set; }
    }
}
