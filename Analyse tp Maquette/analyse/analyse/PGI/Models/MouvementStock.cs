using System;

namespace PGI.Models
{
    public class MouvementStock
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public string TypeMouvement { get; set; } = "ENTREE";
        public int Quantite { get; set; }
        public string Raison { get; set; } = "manuel";
        public DateTime DateMouvement { get; set; } = DateTime.Now;
        public string? NoteUtilisateur { get; set; }
        public int? EmployeId { get; set; }

        // Propriétés de navigation
        public string? NomProduit { get; set; }
        public string? SKUProduit { get; set; }
        public string? NomEmploye { get; set; }
    }
}
