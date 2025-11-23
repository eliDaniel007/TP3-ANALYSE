using System;

namespace PGI.Models
{
    /// <summary>
    /// Modèle pour les rapports financiers périodiques
    /// </summary>
    public class RapportFinancier
    {
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        
        // Ventes
        public decimal TotalVentes { get; set; }
        public int NombreFactures { get; set; }
        public decimal TotalTPS { get; set; }
        public decimal TotalTVQ { get; set; }
        
        // Achats
        public decimal TotalAchats { get; set; }
        public int NombreCommandes { get; set; }
        
        // Coûts
        public decimal CoutProduitsVendus { get; set; }
        public decimal ChargesOperationnelles { get; set; }
        
        // Profits
        public decimal ProfitBrut { get; set; } // TotalVentes - CoutProduitsVendus
        public decimal ProfitNet { get; set; }  // ProfitBrut - ChargesOperationnelles
        public decimal MargeProfit { get; set; } // (ProfitNet / TotalVentes) * 100
        
        // Paiements
        public decimal TotalPaiementsRecus { get; set; }
        public decimal TotalImpaye { get; set; }
    }
}

