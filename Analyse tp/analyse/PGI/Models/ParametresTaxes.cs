using System;

namespace PGI.Models
{
    public class ParametresTaxes
    {
        public int Id { get; set; }
        public string NomTaxe { get; set; } = string.Empty; // TPS, TVQ
        public decimal Taux { get; set; } // 0.05 pour 5%, 0.09975 pour 9.975%
        public bool Actif { get; set; } = true;
        public DateTime DateEffective { get; set; } = DateTime.Now;
        public string? Description { get; set; }
    }
}

