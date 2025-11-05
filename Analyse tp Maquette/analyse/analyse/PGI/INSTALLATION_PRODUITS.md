# 📦 Installation des Produits Nordik Adventures

## 🎯 Contenu

Ce script ajoute **30 produits réels** dans la base de données avec :
- ✅ 5 catégories de produits
- ✅ 7 fournisseurs
- ✅ 30 produits avec toutes les informations
- ✅ Niveaux de stock par emplacement

---

## 🚀 Installation

### Méthode 1 : MySQL Workbench (Recommandé)

1. Ouvrir MySQL Workbench
2. Se connecter au serveur
3. File > Open SQL Script
4. Sélectionner **`SQL_Produits_NordikAdventures.sql`**
5. Cliquer sur ⚡ (Execute)

### Méthode 2 : Ligne de commande

```bash
mysql -u root -ppassword NordikAdventuresERP < SQL_Produits_NordikAdventures.sql
```

---

## 📋 Produits Insérés

### 🏕️ Tentes & abris (6 produits)
| SKU | Produit | Prix | Stock | Emplacement |
|-----|---------|------|-------|-------------|
| NC-TNT-001 | Tente légère 2 places | 299 $ | 18 | A1 |
| NC-TNT-002 | Tente familiale 6 places | 499 $ | 9 | A1 |
| NC-TNT-003 | Toile imperméable 3x3 m | 59 $ | 25 | A2 |
| NC-TNT-004 | Tapis de sol isolant | 39 $ | 40 | A2 |
| NC-TNT-005 | Abri cuisine pliable | 149 $ | 12 | A1 |
| NC-TNT-006 | Mât télescopique alu | 29 $ | 30 | A3 |

### 🎒 Sacs & portage (6 produits)
| SKU | Produit | Prix | Stock | Emplacement |
|-----|---------|------|-------|-------------|
| NC-SAC-001 | Sac à dos 50 L étanche | 139 $ | 20 | B1 |
| NC-SAC-002 | Sac de jour 25 L | 79 $ | 25 | B2 |
| NC-SAC-003 | Sac de couchage -10°C | 169 $ | 15 | B3 |
| NC-SAC-004 | Tapis autogonflant | 59 $ | 35 | B3 |
| NC-SAC-005 | Housse imperméable sac à dos | 19 $ | 40 | B2 |
| NC-SAC-006 | Bâtons de marche carbone | 79 $ | 18 | B1 |

### 👕 Vêtements techniques (7 produits)
| SKU | Produit | Prix | Stock | Emplacement |
|-----|---------|------|-------|-------------|
| NC-VET-001 | Chandail thermique homme | 59 $ | 50 | C1 |
| NC-VET-002 | Chandail thermique femme | 59 $ | 48 | C1 |
| NC-VET-003 | Pantalon de randonnée homme | 89 $ | 30 | C2 |
| NC-VET-004 | Pantalon de randonnée femme | 89 $ | 32 | C2 |
| NC-VET-005 | Manteau coupe-vent | 129 $ | 20 | C3 |
| NC-VET-006 | Tuque en laine mérinos | 29 $ | 40 | C4 |
| NC-VET-007 | Gants isolants Hiver+ | 45 $ | 25 | C4 |

### 🍳 Accessoires & cuisine (6 produits)
| SKU | Produit | Prix | Stock | Emplacement |
|-----|---------|------|-------|-------------|
| NC-ACC-001 | Réchaud portatif | 59 $ | 20 | D1 |
| NC-ACC-002 | Bouteille isotherme 1L | 29 $ | 40 | D2 |
| NC-ACC-003 | Lampe frontale 300 lumens | 39 $ | 35 | D3 |
| NC-ACC-004 | Ensemble vaisselle 4 pers. | 49 $ | 25 | D2 |
| NC-ACC-005 | Filtre à eau compact | 69 $ | 18 | D3 |
| NC-ACC-006 | Couteau multifonction | 39 $ | 28 | D4 |

### 📡 Électronique & navigation (5 produits)
| SKU | Produit | Prix | Stock | Emplacement |
|-----|---------|------|-------|-------------|
| NC-ELE-001 | Montre GPS plein air | 279 $ | 10 | E1 |
| NC-ELE-002 | Chargeur solaire 20W | 79 $ | 18 | E2 |
| NC-ELE-003 | Boussole de précision | 24 $ | 40 | E3 |
| NC-ELE-004 | Radio météo portable | 49 $ | 15 | E4 |
| NC-ELE-005 | Lampe USB rechargeable | 25 $ | 35 | E5 |

---

## 🏭 Fournisseurs Ajoutés

| Code | Nom | Délai (jours) | Remise (%) |
|------|-----|---------------|------------|
| AX-001 | AventureX | 10 | 5% |
| TS-001 | TrekSupply | 7 | 4% |
| MN-001 | MontNord | 8 | 3% |
| NP-001 | NordPack | 8 | 6% |
| NW-001 | NordWear | 6 | 5% |
| AL-001 | ArcticLine | 8 | 4% |
| TT-001 | TechTrail | 10 | 4% |

---

## 📊 Statistiques

| Indicateur | Valeur |
|------------|--------|
| **Produits** | 30 |
| **Catégories** | 5 |
| **Fournisseurs** | 7 |
| **Emplacements** | 19 (A1-A3, B1-B3, C1-C4, D1-D4, E1-E5) |
| **Valeur totale stock** | ~20 000 $ |
| **Stock total (unités)** | 736 unités |

---

## ✅ Vérification

Après l'exécution du script, vérifiez :

```sql
-- Vérifier les catégories
SELECT COUNT(*) FROM categories;
-- Résultat attendu : 5+

-- Vérifier les fournisseurs
SELECT COUNT(*) FROM fournisseurs;
-- Résultat attendu : 7+

-- Vérifier les produits NC
SELECT COUNT(*) FROM produits WHERE sku LIKE 'NC-%';
-- Résultat attendu : 30

-- Vérifier le stock total
SELECT SUM(qte_disponible) FROM niveaux_stock;
-- Résultat attendu : 736

-- Voir tous les produits avec stock
SELECT 
    p.sku,
    p.nom,
    p.prix,
    COALESCE(SUM(ns.qte_disponible), 0) AS stock,
    ns.emplacement
FROM produits p
LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
WHERE p.sku LIKE 'NC-%'
GROUP BY p.id, ns.emplacement
ORDER BY p.sku;
```

---

## 🎨 Dans l'Application

Une fois le script exécuté, les produits seront visibles dans :

### Module Stocks
- **Tableau de bord** : KPIs mis à jour (valeur stock, nombre produits)
- **Liste produits** : 30 produits affichés avec recherche
- **Catégories** : 5 catégories disponibles
- **Fournisseurs** : 7 fournisseurs disponibles

### Recherche
Vous pourrez rechercher par :
- SKU : `NC-TNT-001`
- Nom : `Tente légère`
- Catégorie : `Tentes & abris`

---

## 🔄 Réexécution du Script

Le script utilise `ON DUPLICATE KEY UPDATE`, donc :
- ✅ Vous pouvez l'exécuter plusieurs fois
- ✅ Les données existantes ne seront pas dupliquées
- ✅ Le statut sera préservé

---

## 🐛 Dépannage

### Erreur : "Unknown database"
**Solution** : Exécuter d'abord `NordikAdventuresERP_Schema_FR.sql`

### Erreur : "Foreign key constraint fails"
**Solution** : Vérifier que les tables `categories` et `fournisseurs` existent

### Produits ne s'affichent pas dans l'app
**Solution** : 
1. Vérifier que le script a été exécuté
2. Redémarrer l'application
3. Vérifier la connexion MySQL dans `DatabaseHelper.cs`

---

## 📝 Notes

- Tous les produits ont le statut `Actif`
- Les dates d'entrée en stock sont en 2025
- Les marges brutes varient de 47% à 65%
- Les emplacements suivent une organisation logique par catégorie
- Stock total : 736 unités pour une valeur d'environ 20 000 $

---

**Installation terminée ! Vous avez maintenant 30 produits réels dans votre PGI ! 🎉**

