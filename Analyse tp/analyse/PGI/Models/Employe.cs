using System;

namespace PGI.Models
{
    public class Employe
    {
        public int Id { get; set; }
        public string Matricule { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Courriel { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public string Departement { get; set; } = "Autre";
        public string Poste { get; set; } = string.Empty;
        public string RoleSysteme { get; set; } = "Employé";
        public string Statut { get; set; } = "Actif";
        public DateTime DateCreation { get; set; }
    }
}

