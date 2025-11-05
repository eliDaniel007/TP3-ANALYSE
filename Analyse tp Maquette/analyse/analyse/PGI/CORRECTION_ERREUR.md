# 🔧 Correction Erreur "Column 'categorie_id' does not belong to table"

## ❌ **Problème**

L'erreur indique que la colonne `categorie_id` n'existe pas dans la table `produits`.

**Cause** : Le schéma principal n'a pas été exécuté correctement, ou la table a une structure incomplète.

---

## ✅ **Solution Simple (Recommandée)**

### Étape 1 : Réexécuter le Schéma Complet

Dans MySQL Workbench :

1. **File** > **Open SQL Script**
2. Sélectionner **`NordikAdventuresERP_Schema_FR.sql`**
3. **Cliquer sur ⚡ Execute**
4. Attendre la fin (peut prendre 1-2 minutes)

⚠️ **Note** : Cela va **recréer la base de données** (DROP DATABASE IF EXISTS)

---

## ✅ **Solution Alternative (Si vous avez déjà des données)**

Si vous avez déjà des données à conserver, utilisez le script de correction :

### Étape 1 : Exécuter le Script de Correction

1. **File** > **Open SQL Script**
2. Sélectionner **`FIX_Structure_Tables.sql`**
3. **Cliquer sur ⚡ Execute**

### Étape 2 : Vérifier la Structure

```sql
USE NordikAdventuresERP;

-- Vérifier la structure de produits
DESCRIBE produits;

-- Vous devriez voir categorie_id dans la liste
```

---

## 📋 **Ordre Correct d'Exécution des Scripts**

### 1️⃣ **Schéma Principal** (OBLIGATOIRE en premier)
```
NordikAdventuresERP_Schema_FR.sql
```
Crée :
- La base de données
- Les tables (categories, fournisseurs, produits, etc.)
- Les contraintes

### 2️⃣ **Authentification** (Optionnel)
```
SQL_Schema_Auth_Safe.sql
```
Ajoute :
- Colonnes mot_de_passe
- 4 employés de test
- 5 clients de test

### 3️⃣ **Produits** (Optionnel)
```
SQL_Produits_NordikAdventures.sql
```
Ajoute :
- 30 produits Nordik Adventures
- Catégories et fournisseurs
- Niveaux de stock

---

## 🔍 **Vérification Rapide**

Après avoir exécuté le schéma principal, vérifiez :

```sql
USE NordikAdventuresERP;

-- 1. Vérifier que la base existe
SHOW DATABASES LIKE 'NordikAdventuresERP';

-- 2. Vérifier que la table produits existe
SHOW TABLES LIKE 'produits';

-- 3. Vérifier la structure de produits
DESCRIBE produits;

-- 4. Vérifier que categorie_id est présent
SHOW COLUMNS FROM produits LIKE 'categorie_id';
```

**Résultat attendu** :
```
Field         | Type    | Null | Key | Default | Extra
categorie_id  | int     | NO   | MUL | NULL    |
```

---

## 🎯 **Procédure Complète (Recommandée)**

### Si vous voulez tout réinstaller proprement :

```sql
-- 1. Supprimer l'ancienne base (si elle existe)
DROP DATABASE IF EXISTS NordikAdventuresERP;

-- 2. Créer la base
CREATE DATABASE NordikAdventuresERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- 3. Sélectionner la base
USE NordikAdventuresERP;
```

Puis exécuter dans l'ordre :
1. **`NordikAdventuresERP_Schema_FR.sql`** (tout le contenu)
2. **`SQL_Schema_Auth_Safe.sql`** (authentification)
3. **`SQL_Produits_NordikAdventures.sql`** (produits)

---

## 🆘 **Si l'Erreur Persiste**

### Vérifier les Foreign Keys

```sql
-- Désactiver temporairement les contraintes
SET FOREIGN_KEY_CHECKS = 0;

-- Vérifier si categories existe
SELECT COUNT(*) FROM categories;

-- Vérifier si fournisseurs existe
SELECT COUNT(*) FROM fournisseurs;

-- Réactiver les contraintes
SET FOREIGN_KEY_CHECKS = 1;
```

### Vérifier les Permissions

```sql
-- Vérifier vos permissions
SHOW GRANTS FOR CURRENT_USER();
```

---

## 📝 **Résumé de la Solution**

1. ✅ **Exécuter** `NordikAdventuresERP_Schema_FR.sql` (recrée tout)
2. ✅ **Vérifier** avec `DESCRIBE produits;`
3. ✅ **Exécuter** `SQL_Schema_Auth_Safe.sql` (authentification)
4. ✅ **Exécuter** `SQL_Produits_NordikAdventures.sql` (produits)
5. ✅ **Tester** l'application

---

## 🎓 **Pourquoi Cette Erreur ?**

L'erreur `Column 'categorie_id' does not belong to table` se produit quand :

1. La table `produits` existe mais **sans la colonne** `categorie_id`
2. La table `produits` **n'existe pas du tout**
3. Le schéma a été **partiellement exécuté** (erreur à mi-chemin)

**Solution** : Toujours exécuter le schéma complet en entier !

---

**Exécutez `NordikAdventuresERP_Schema_FR.sql` en entier et tout fonctionnera ! 🎯**

