# 🎨 Assets - Nordik Adventures ERP

Ce dossier contient les fichiers médias et ressources du projet.

---

## 📋 Fichiers Disponibles

| Fichier | Type | Description |
|---------|------|-------------|
| **iiiooo.png** | Image | Logo ou image du projet |
| **NordikAdventures - Liste des produits PGI.xlsx** | Excel | Liste originale des 30 produits |
| **schema 2.0.mwb** | MySQL Workbench | Modèle de la base de données (fichier source) |

---

## 🖼️ Images

### iiiooo.png
- **Type :** Image PNG
- **Utilisation :** Logo ou illustration du projet
- **Format :** PNG

---

## 📊 Fichiers Excel

### NordikAdventures - Liste des produits PGI.xlsx
- **Type :** Classeur Excel
- **Contenu :** Liste des 30 produits avec :
  - SKU (code produit)
  - Nom du produit
  - Catégorie
  - Fournisseur
  - Prix de vente
  - Coût d'achat
  - Stock disponible
  - Seuil de réapprovisionnement
  - Description

**Utilisation :**
- Source de données pour générer le script SQL `SQL_Produits_NordikAdventures.sql`
- Référence pour ajouter de nouveaux produits
- Documentation des produits existants

---

## 🗄️ Fichiers MySQL Workbench

### schema 2.0.mwb
- **Type :** Modèle MySQL Workbench
- **Contenu :** Schéma visuel de la base de données
  - 20+ tables
  - Relations (Foreign Keys)
  - Index
  - Contraintes

**Utilisation :**
- Visualiser le schéma de la base de données
- Modifier le schéma graphiquement
- Générer le script SQL (`NordikAdventuresERP_Schema_FR.sql`)

**Ouvrir avec :**
- MySQL Workbench 8.0+
- Double-cliquer sur le fichier (si MySQL Workbench est installé)

**Actions possibles :**
1. **Visualiser** : Voir les tables et relations
2. **Modifier** : Ajouter/supprimer des tables ou colonnes
3. **Forward Engineer** : Générer un nouveau script SQL
   - Menu : `Database > Forward Engineer...`
   - Sélectionner les options
   - Générer le script SQL

---

## 📁 Structure du Schéma

### Tables Principales
- **Produits** : `produits`, `categories`, `fournisseurs`
- **Clients** : `clients`, `commandes_clients`, `details_commandes_clients`
- **Employés** : `employes`
- **Stocks** : `niveaux_stock`, `mouvements_stock`
- **Achats** : `achats_fournisseurs`, `details_achats_fournisseurs`
- **Finances** : `factures_clients`, `paiements_clients`, `paiements_fournisseurs`

### Relations (Foreign Keys)
- `produits.categorie_id` → `categories.id`
- `produits.fournisseur_id` → `fournisseurs.id`
- `commandes_clients.client_id` → `clients.id`
- `details_commandes_clients.produit_id` → `produits.id`
- etc.

---

## 🎯 Ajouter de Nouveaux Assets

### Images
1. Placer l'image dans ce dossier
2. Formats recommandés : PNG, JPG, SVG
3. Nommer clairement : `logo.png`, `screenshot-dashboard.png`, etc.

### Fichiers Excel
1. Utiliser le template `NordikAdventures - Liste des produits PGI.xlsx`
2. Ajouter les nouvelles données
3. Sauvegarder avec un nom descriptif

### Modèles MySQL
1. Ouvrir `schema 2.0.mwb` dans MySQL Workbench
2. Modifier le schéma
3. Sauvegarder (Ctrl+S)
4. Générer le nouveau script SQL via `Forward Engineer`

---

## 📚 Captures d'Écran (À Ajouter)

Pour améliorer la documentation, vous pouvez ajouter des captures d'écran :

### Captures Suggérées
- `login-window.png` - Fenêtre de connexion
- `dashboard-stocks.png` - Tableau de bord Stocks
- `products-list.png` - Liste des produits
- `categories-view.png` - Gestion des catégories
- `suppliers-view.png` - Gestion des fournisseurs
- `module-selection.png` - Sélection des modules

### Utilisation
1. Capturer l'écran (Windows + Shift + S)
2. Sauvegarder dans ce dossier
3. Ajouter dans le README principal :
   ```markdown
   ![Dashboard Stocks](assets/dashboard-stocks.png)
   ```

---

## 🔧 Outils Recommandés

### Pour Images
- **Paint.NET** : Édition d'images (gratuit)
- **GIMP** : Alternative à Photoshop (gratuit)
- **Figma** : Design d'interfaces (gratuit)

### Pour Excel
- **Microsoft Excel** : Édition de tableaux
- **LibreOffice Calc** : Alternative gratuite

### Pour MySQL
- **MySQL Workbench 8.0+** : Modélisation de bases de données

---

## 📝 Notes

- **Taille des fichiers** : Éviter les fichiers > 10 MB pour Git
- **Images optimisées** : Compresser les images avant de commit
- **Formats ouverts** : Privilégier PNG, SVG pour les images
- **Backups** : Sauvegarder le fichier `.mwb` avant modification

---

**Retour au README principal : [../README.md](../README.md)**

