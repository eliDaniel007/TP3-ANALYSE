using System;
using System.ComponentModel;

namespace PGI.Models
{
    /// <summary>
    /// Classe pour représenter un article dans le panier
    /// </summary>
    public class PanierItem : INotifyPropertyChanged
    {
        private int _quantite;
        private decimal _prix;

        public int ProduitId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public decimal Prix 
        { 
            get => _prix;
            set
            {
                _prix = value;
                OnPropertyChanged(nameof(Prix));
                OnPropertyChanged(nameof(SousTotal));
            }
        }
        
        public int Quantite 
        { 
            get => _quantite;
            set
            {
                _quantite = value;
                OnPropertyChanged(nameof(Quantite));
                OnPropertyChanged(nameof(SousTotal));
            }
        }
        
        public int StockDisponible { get; set; }
        public string Categorie { get; set; } = string.Empty;
        
        public decimal SousTotal => Prix * Quantite;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


