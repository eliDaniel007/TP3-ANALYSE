# 🗄️ Scripts SQL - Nordik Adventures ERP

Ce dossier contient tous les scripts SQL nécessaires pour installer et configurer la base de données MySQL.

---

## 📋 Scripts Disponibles

| Ordre | Fichier | Description | Obligatoire |
|-------|---------|-------------|-------------|
| **1** | `NordikAdventuresERP_Schema_FR.sql` | Schéma complet de la base de données (20+ tables) | ✅ OUI |
| **2** | `SQL_Schema_Auth_Safe.sql` | Utilisateurs de test (4 employés + 5 clients) | ✅ OUI |
| **3** | `SQL_Produits_NordikAdventures.sql` | 30 produits + catégories + fournisseurs | ⭐ RECOMMANDÉ |

---

## 🚀 Installation (Ordre Important)

### ✅ Étape 1 : Schéma Principal (OBLIGATOIRE)

**Fichier :** `NordikAdventuresERP_Schema_FR.sql`

**Contenu :**
- 20+ tables (produits, clients, employés, ventes, stocks, etc.)
- Vues SQL (calculs KPIs)
- Procédures stockées
- Triggers
- Contraintes et index

**Exécution dans MySQL Workbench :**
```sql
-- File > Open SQL Script > NordikAdventuresERP_Schema_FR.sql
-- Cliquer sur ⚡ Execute
-- Attendre 1-2 minutes
```

**Résultat attendu :** Message `20+ tables created successfully`

---

### ✅ Étape 2 : Authentification (OBLIGATOIRE)

**Fichier :** `SQL_Schema_Auth_Safe.sql`

**Contenu :**
- Ajout de la colonne `mot_de_passe` (si elle n'existe pas)
- 4 employés de test avec mots de passe
- 5 clients de test avec mots de passe

**Exécution dans MySQL Workbench :**
```sql
-- File > Open SQL Script > SQL_Schema_Auth_Safe.sql
-- Cliquer sur ⚡ Execute
```

**Résultat attendu :** `9 rows affected` (4 employés + 5 clients)

**Identifiants créés :**

**Employés :**
- `admin@nordikadventures.com` / `admin123` (Administrateur)
- `gestionnaire@nordikadventures.com` / `gestionnaire123` (Gestionnaire Stocks)
- `employe@nordikadventures.com` / `employe123` (Employé Ventes)
- `comptable@nordikadventures.com` / `comptable123` (Comptable)

**Clients :**
- `jean.client@test.com` / `client123`
- `marie.client@test.com` / `client123`
- `pierre.client@entreprise.com` / `client123`
- `client.sophie@gmail.com` / `client123`
- `contact@nordikclient.com` / `client123`

---

### ⭐ Étape 3 : Produits (RECOMMANDÉ)

**Fichier :** `SQL_Produits_NordikAdventures.sql`

**Contenu :**
- 5 catégories (Vêtements, Chaussures, Équipement, Accessoires, Camping)
- 5 fournisseurs (Mountain Gear, AventureX, NordicSupply, GlobalOutdoor, Expedition Pro)
- 30 produits réels avec :
  - SKU, nom, description
  - Prix de vente et coût d'achat
  - Stock disponible et réservé
  - Seuils de réapprovisionnement
  - Catégorie et fournisseur

**Exécution dans MySQL Workbench :**
```sql
-- File > Open SQL Script > SQL_Produits_NordikAdventures.sql
-- Cliquer sur ⚡ Execute
```

**Résultat attendu :** `40+ rows affected` (5 catégories + 5 fournisseurs + 30 produits)

---

## ✅ Vérification de l'Installation

### Test 1 : Vérifier que la BDD existe
```sql
SHOW DATABASES LIKE 'NordikAdventuresERP';
```
**Attendu :** 1 ligne

### Test 2 : Vérifier les tables
```sql
USE NordikAdventuresERP;
SHOW TABLES;
```
**Attendu :** 20+ tables

### Test 3 : Vérifier les produits
```sql
SELECT COUNT(*) FROM produits;
```
**Attendu :** 30

### Test 4 : Vérifier les employés
```sql
SELECT nom, prenom, courriel FROM employes WHERE mot_de_passe IS NOT NULL;
```
**Attendu :** 4 employés

### Test 5 : Vérifier les clients
```sql
SELECT nom, prenom, courriel FROM clients WHERE mot_de_passe IS NOT NULL;
```
**Attendu :** 5 clients

### Test 6 : Vérifier les catégories
```sql
SELECT COUNT(*) FROM categories;
```
**Attendu :** 5

### Test 7 : Vérifier les fournisseurs
```sql
SELECT COUNT(*) FROM fournisseurs;
```
**Attendu :** 5

---

## 🔧 Configuration de l'Application

Après avoir exécuté les scripts, configurer le mot de passe MySQL dans l'application :

**Fichier :** `Analyse tp Maquette/analyse/analyse/PGI/Helpers/DatabaseHelper.cs`

**Ligne 13 :**
```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

Remplacer `VOTRE_MOT_DE_PASSE` par votre mot de passe MySQL root.

---

## 🆘 Problèmes Courants

### Erreur : "Table 'produits' already exists"
**Cause :** Le script a déjà été exécuté.

**Solution :** Sauter ce script ou supprimer la base de données et recommencer :
```sql
DROP DATABASE IF EXISTS NordikAdventuresERP;
```
Puis réexécuter les 3 scripts dans l'ordre.

### Erreur : "Column 'mot_de_passe' already exists"
**Cause :** La colonne a déjà été ajoutée.

**Solution :** Le script `SQL_Schema_Auth_Safe.sql` gère cette situation automatiquement. Continuer avec les `INSERT` statements.

### Erreur : "Data truncated for column 'departement'"
**Cause :** Valeur ENUM invalide.

**Solution :** Consulter `../Analyse tp Maquette/analyse/analyse/PGI/VALEURS_ENUM.md` pour les valeurs ENUM valides.

### Erreur : "Duplicate entry 'EMP-001' for key 'PRIMARY'"
**Cause :** Les employés existent déjà.

**Solution :** Le script utilise `ON DUPLICATE KEY UPDATE`, donc cette erreur ne devrait pas se produire. Si elle persiste, supprimer et recréer la base de données.

---

## 📊 Contenu des Produits

Les 30 produits incluent :
- **Vêtements** : Vestes, pantalons, pulls (10 produits)
- **Chaussures** : Bottes, chaussures de randonnée (6 produits)
- **Équipement** : Tentes, sacs à dos, sacs de couchage (8 produits)
- **Accessoires** : Gants, bonnets, lunettes, bâtons (4 produits)
- **Camping** : Lampes, réchauds, gourdes (2 produits)

**Exemples :**
- Veste Everest Pro (399,99 $)
- Bottes Grand Froid (-40°C) (299,99 $)
- Tente 4 Saisons Alpine (899,99 $)
- Sac à Dos 65L Expédition (279,99 $)

---

## 📁 Structure de la Base de Données

### Tables Principales
- `produits` - Produits en stock
- `categories` - Catégories de produits
- `fournisseurs` - Fournisseurs
- `clients` - Clients
- `employes` - Employés
- `commandes_clients` - Commandes clients
- `achats_fournisseurs` - Achats fournisseurs
- `mouvements_stock` - Historique des mouvements
- `niveaux_stock` - Niveaux de stock par entrepôt

### Vues SQL
- `vue_stock_global` - Stock global par produit
- `vue_valeur_stock` - Valeur totale du stock
- `vue_produits_critique` - Produits sous le seuil

### Procédures Stockées
- `sp_ajuster_stock()` - Ajuster le stock d'un produit
- `sp_calculer_marge()` - Calculer la marge brute

---

## 🎯 Prochaines Étapes

1. ✅ Exécuter les 3 scripts SQL
2. ✅ Vérifier l'installation (7 tests ci-dessus)
3. ✅ Configurer le mot de passe dans `DatabaseHelper.cs`
4. ✅ Lancer l'application (F5)
5. ✅ Se connecter avec les identifiants de test
6. ✅ Voir les 30 produits dans le module Stocks !

---

**Retour au README principal : [../README.md](../README.md)**

**Documentation complète : [../docs/](../docs/)**

