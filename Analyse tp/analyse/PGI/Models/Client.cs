using System;

namespace PGI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Type { get; set; } = "Particulier";
        public string Nom { get; set; } = string.Empty;
        public string CourrielContact { get; set; } = string.Empty;
        public string? Telephone { get; set; }
        public string Statut { get; set; } = "Prospect";
        public DateTime DateCreation { get; set; }
    }
}

