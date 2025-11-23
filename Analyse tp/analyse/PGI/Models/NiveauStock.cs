namespace PGI.Models
{
    public class NiveauStock
    {
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public string Emplacement { get; set; } = "Entrepôt Principal";
        public int QteDisponible { get; set; } = 0;
        public int QteReservee { get; set; } = 0;
    }
}

