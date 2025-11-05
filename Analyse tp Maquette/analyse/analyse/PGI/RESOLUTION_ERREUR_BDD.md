# 🔧 Résolution de l'erreur "categorie_id does not belong to table"

## 🎯 **Action Immédiate (5 minutes)**

L'application **fonctionne maintenant avec des données d'exemple** (sans BDD).

Pour utiliser la **vraie base de données MySQL**, suivez ces étapes :

---

## ✅ **Étape 1 : Vérifier MySQL**

### 1.1 Ouvrir MySQL Workbench

- Lancer **MySQL Workbench**
- Se connecter au serveur local (root / votre_mot_de_passe)

### 1.2 Vérifier si la base existe

```sql
SHOW DATABASES LIKE 'NordikAdventuresERP';
```

**Résultat attendu :**
- Si la base existe : `1 row returned`
- Si elle n'existe pas : `0 rows returned`

---

## ✅ **Étape 2 : Installer le Schéma Principal**

### 2.1 Ouvrir le Fichier SQL

Dans MySQL Workbench :

1. **File** > **Open SQL Script...**
2. Naviguer vers :
   ```
   C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\NordikAdventuresERP_Schema_FR.sql
   ```
3. Cliquer sur **Ouvrir**

### 2.2 Exécuter le Script Complet

1. **Cliquer sur l'icône ⚡ (Execute)** (ou `Ctrl+Shift+Enter`)
2. Attendre 1-2 minutes (le script crée 20+ tables)
3. Vérifier qu'il n'y a **pas d'erreurs en rouge**

### 2.3 Vérifier la Structure

```sql
USE NordikAdventuresERP;

-- Vérifier que produits existe
SHOW TABLES LIKE 'produits';

-- Vérifier la structure de produits
DESCRIBE produits;
```

**Résultat attendu :**
Vous devriez voir une colonne `categorie_id` de type `int` :
```
Field         | Type    | Null | Key | Default | Extra
categorie_id  | int     | NO   | MUL | NULL    |
```

---

## ✅ **Étape 3 : Installer les Données de Test**

### 3.1 Authentification (Employés + Clients)

1. **File** > **Open SQL Script...**
2. Ouvrir :
   ```
   Analyse tp Maquette/analyse/analyse/PGI/SQL_Schema_Auth_Safe.sql
   ```
3. **Exécuter (⚡)**

**Résultat attendu :**
```
Message: Colonne mot_de_passe ajoutée...
Message: Employés de test insérés/mis à jour.
Message: Clients de test insérés/mis à jour.
```

### 3.2 Produits (30 produits Nordik Adventures)

1. **File** > **Open SQL Script...**
2. Ouvrir :
   ```
   Analyse tp Maquette/analyse/analyse/PGI/SQL_Produits_NordikAdventures.sql
   ```
3. **Exécuter (⚡)**

**Résultat attendu :**
```
✅ 30 produits insérés avec succès !
```

---

## ✅ **Étape 4 : Vérifier les Données**

```sql
USE NordikAdventuresERP;

-- Vérifier les catégories
SELECT COUNT(*) AS 'Catégories' FROM categories;

-- Vérifier les fournisseurs
SELECT COUNT(*) AS 'Fournisseurs' FROM fournisseurs;

-- Vérifier les produits
SELECT COUNT(*) AS 'Produits' FROM produits;

-- Vérifier les employés
SELECT COUNT(*) AS 'Employés' FROM employes WHERE mot_de_passe IS NOT NULL;

-- Vérifier les clients
SELECT COUNT(*) AS 'Clients' FROM clients WHERE mot_de_passe IS NOT NULL;
```

**Résultats attendus :**
- Catégories : 5-10
- Fournisseurs : 5-10
- Produits : 30
- Employés : 4
- Clients : 5

---

## ✅ **Étape 5 : Relancer l'Application**

1. **Fermer l'application** (si elle est ouverte)
2. Dans Visual Studio : **F5** (Debug)
3. Se connecter avec :
   - **Employé** : `admin@nordikadventures.com` / `admin123`
   - **Client** : `jean.client@test.com` / `client123`

---

## 🎯 **Mode de Fonctionnement Actuel**

### Avec BDD MySQL Installée ✅
- Affiche les **vraies données** de la base
- Permet **ajout, modification, suppression**
- Calculs **temps réel**

### Sans BDD (Données d'exemple) 📊
- Affiche des **données fictives**
- Navigation **fonctionnelle**
- **Aucune persistance** (reset à chaque lancement)

---

## 🔍 **Diagnostic Rapide**

### Comment savoir si la BDD est connectée ?

Dans l'application :
1. Aller dans **Stocks** > **Produits**
2. Si vous voyez **30 produits** avec les vrais noms (Veste Everest, etc.) → **BDD OK ✅**
3. Si vous voyez **3 produits** génériques → **Données d'exemple 📊**

---

## ❓ **Problèmes Courants**

### Erreur : "Access denied for user 'root'@'localhost'"

**Solution :**
Modifier le mot de passe dans `DatabaseHelper.cs` :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

### Erreur : "Unknown database 'NordikAdventuresERP'"

**Solution :**
Le schéma principal n'a pas été exécuté. Retour à l'**Étape 2**.

### Erreur : "Column 'categorie_id' does not belong to table"

**Solution :**
La table `produits` est mal créée. Retour à l'**Étape 2**.

---

## 📝 **Récapitulatif**

| Étape | Fichier SQL | Durée | Description |
|-------|-------------|-------|-------------|
| 1 | - | 30s | Vérifier MySQL |
| 2 | `NordikAdventuresERP_Schema_FR.sql` | 2 min | Créer les tables |
| 3.1 | `SQL_Schema_Auth_Safe.sql` | 30s | Ajouter authentification |
| 3.2 | `SQL_Produits_NordikAdventures.sql` | 30s | Ajouter 30 produits |
| 4 | - | 30s | Vérifier les données |
| 5 | - | 10s | Tester l'application |

**Total : ~5 minutes** ⏱️

---

## 🎉 **Après Installation**

Vous pourrez :

- ✅ Se connecter avec 4 employés + 5 clients
- ✅ Voir 30 produits réels
- ✅ Ajouter/modifier/supprimer des données
- ✅ Voir les calculs temps réel (valeur stock, marges, etc.)
- ✅ Tester toutes les fonctionnalités du PGI

---

**🚀 Commencez par l'Étape 2 (installer le schéma) et tout fonctionnera !**

