using System;

namespace PGI.Models
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Statut { get; set; } = "Actif";
        public DateTime DateCreation { get; set; }
    }
}
