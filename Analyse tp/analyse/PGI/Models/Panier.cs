using System;
using System.Collections.Generic;
using System.Linq;

namespace PGI.Models
{
    /// <summary>
    /// Classe pour gérer le panier d'achat du client
    /// </summary>
    public class Panier
    {
        private List<PanierItem> _items = new List<PanierItem>();

        public List<PanierItem> Items => _items;
        public int NombreArticles => _items.Sum(i => i.Quantite);
        public decimal Total => _items.Sum(i => i.SousTotal);

        /// <summary>
        /// Ajouter un produit au panier
        /// </summary>
        public void AjouterProduit(PanierItem item)
        {
            var existingItem = _items.FirstOrDefault(i => i.ProduitId == item.ProduitId);
            
            if (existingItem != null)
            {
                // Vérifier le stock disponible
                int nouvelleQuantite = existingItem.Quantite + item.Quantite;
                if (nouvelleQuantite > existingItem.StockDisponible)
                {
                    throw new Exception($"Stock insuffisant. Stock disponible : {existingItem.StockDisponible}");
                }
                existingItem.Quantite = nouvelleQuantite;
            }
            else
            {
                // Vérifier le stock disponible
                if (item.Quantite > item.StockDisponible)
                {
                    throw new Exception($"Stock insuffisant. Stock disponible : {item.StockDisponible}");
                }
                _items.Add(item);
            }
        }

        /// <summary>
        /// Modifier la quantité d'un produit
        /// </summary>
        public void ModifierQuantite(int produitId, int nouvelleQuantite)
        {
            var item = _items.FirstOrDefault(i => i.ProduitId == produitId);
            if (item != null)
            {
                if (nouvelleQuantite <= 0)
                {
                    RetirerProduit(produitId);
                }
                else if (nouvelleQuantite > item.StockDisponible)
                {
                    throw new Exception($"Stock insuffisant. Stock disponible : {item.StockDisponible}");
                }
                else
                {
                    item.Quantite = nouvelleQuantite;
                }
            }
        }

        /// <summary>
        /// Retirer un produit du panier
        /// </summary>
        public void RetirerProduit(int produitId)
        {
            _items.RemoveAll(i => i.ProduitId == produitId);
        }

        /// <summary>
        /// Vider le panier
        /// </summary>
        public void Vider()
        {
            _items.Clear();
        }

        /// <summary>
        /// Vérifier si le panier est vide
        /// </summary>
        public bool EstVide => _items.Count == 0;
    }
}


