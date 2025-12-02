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
        
        // Chemin de l'image du produit (basé sur le SKU)
        // Supporte .jpg, .jpeg et .png automatiquement
        public string ImagePath 
        { 
            get 
            {
                // Mapper le SKU du produit à l'image correspondante
                var skuToImage = new Dictionary<string, string>
                {
                    { "NC-TNT-005", "a" },   // Abri cuisine pliable
                    { "NC-SAC-006", "b" },   // Bâtons de marche carbone
                    { "NC-ELE-003", "c" },   // Boussole de précision
                    { "NC-ACC-002", "d" },   // Bouteille isotherme 1L
                    { "NC-VET-002", "e" },   // Chandail thermique femme
                    { "NC-VET-001", "f" },   // Chandail thermique homme
                    { "NC-ELE-002", "g" },   // Chargeur solaire 20W
                    { "NC-ACC-006", "h" },   // Couteau multifonction
                    { "NC-ACC-004", "i" },   // Ensemble vaisselle 4 pers.
                    { "NC-ACC-005", "j" },   // Filtre à eau compact
                    { "NC-VET-007", "k" },   // Gants isolants Hiver+
                    { "NC-SAC-005", "l" },   // Housse imperméable sac à dos
                    { "NC-ACC-003", "m" },   // Lampe frontale 300 lumens
                    { "NC-ELE-005", "n" },   // Lampe USB rechargeable
                    { "NC-VET-005", "o" },   // Manteau coupe-vent
                    { "NC-TNT-006", "p" },   // Mât télescopique alu
                    { "NC-ELE-001", "q" },   // Montre GPS plein air
                    { "NC-VET-004", "r" },   // Pantalon de randonnée femme
                    { "NC-VET-003", "s" },   // Pantalon de randonnée homme
                    { "NC-ELE-004", "t" },   // Radio météo portable
                    { "NC-ACC-001", "u" },   // Réchaud portatif
                    { "NC-SAC-001", "v" },   // Sac à dos 50 L étanche
                    { "NC-SAC-003", "w" },   // Sac de couchage -10°C
                    { "NC-SAC-002", "x" },   // Sac de jour 25 L
                    { "NC-SAC-004", "y" },   // Tapis autogonflant
                    { "NC-TNT-004", "z" },   // Tapis de sol isolant
                    { "NC-TNT-002", "z1" },  // Tente familiale 6 places
                    { "NC-TNT-001", "z2" },  // Tente légère 2 places
                    { "NC-TNT-003", "z3" },  // Toile imperméable 3x3 m
                    { "NC-VET-006", "z4" }   // Tuque en laine mérinos
                };
                
                if (skuToImage.ContainsKey(SKU))
                {
                    string imageName = skuToImage[SKU];
                    // Mapper les extensions spéciales (b.png, d.png, f.jpeg)
                    string extension = ".jpg";
                    if (imageName == "b" || imageName == "d") extension = ".png";
                    if (imageName == "f") extension = ".jpeg";
                    
                    // Chemin absolu depuis le répertoire de l'application
                    string basePath = System.IO.Path.Combine(
                        System.AppDomain.CurrentDomain.BaseDirectory,
                        "assets", "IMAGES PRODUITS", $"{imageName}{extension}"
                    );
                    return basePath;
                }
                
                string defaultPath = System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    "assets", "IMAGES PRODUITS", "placeholder.jpg"
                );
                return defaultPath;
            } 
        }
    }
}
