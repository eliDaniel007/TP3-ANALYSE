# 🚀 Guide Rapide - Installation SQL

## ⚠️ Erreur "IF NOT EXISTS" corrigée

MySQL ne supporte pas `IF NOT EXISTS` pour les colonnes. J'ai créé **2 versions** du script :

---

## 📁 Fichiers Disponibles

### 1️⃣ `SQL_Schema_Auth.sql` (VERSION SIMPLE)
- ✅ Plus simple et direct
- ⚠️ Erreur si la colonne existe déjà (vous pouvez l'ignorer)
- **Recommandé pour première installation**

### 2️⃣ `SQL_Schema_Auth_Safe.sql` (VERSION SAFE)
- ✅ Vérifie l'existence des colonnes avant de les ajouter
- ✅ Aucune erreur même si déjà exécuté
- ✅ Affiche des messages de confirmation
- **Recommandé si vous réexécutez le script**

---

## 🎯 Méthode 1 : Version Simple (Recommandée)

### Étape 1 : Ouvrir MySQL Workbench
1. Se connecter au serveur local
2. Sélectionner la base de données `NordikAdventuresERP`

### Étape 2 : Exécuter le script
1. File > Open SQL Script
2. Sélectionner `SQL_Schema_Auth.sql`
3. Cliquer sur ⚡ (Execute)

### Étape 3 : En cas d'erreur "Duplicate column name"
**C'est normal !** Cela signifie que la colonne existe déjà.

**Solutions :**
- **Option A** : Ignorer l'erreur et continuer (les INSERT fonctionneront quand même)
- **Option B** : Commenter les lignes `ALTER TABLE` et réexécuter

---

## 🛡️ Méthode 2 : Version Safe (Sans erreur)

### Étape 1 : Ouvrir MySQL Workbench
1. Se connecter au serveur local
2. Sélectionner la base de données `NordikAdventuresERP`

### Étape 2 : Exécuter le script safe
1. File > Open SQL Script
2. Sélectionner `SQL_Schema_Auth_Safe.sql`
3. Cliquer sur ⚡ (Execute)

### Résultat
```
✅ Colonne mot_de_passe ajoutée à employes
✅ Colonne mot_de_passe ajoutée à clients
```

Ou si déjà existantes :
```
⚠️ Colonne mot_de_passe existe déjà dans employes
⚠️ Colonne mot_de_passe existe déjà dans clients
```

---

## 🖥️ Méthode 3 : Ligne de Commande

### Version Simple
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth.sql
```

### Version Safe
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth_Safe.sql
```

---

## ✅ Vérification Rapide

### Vérifier les colonnes
```sql
USE NordikAdventuresERP;

-- Vérifier employes
SHOW COLUMNS FROM employes LIKE 'mot_de_passe';

-- Vérifier clients
SHOW COLUMNS FROM clients LIKE 'mot_de_passe';
```

### Vérifier les données
```sql
-- Compter les employés
SELECT COUNT(*) AS nb_employes FROM employes WHERE mot_de_passe IS NOT NULL;

-- Compter les clients
SELECT COUNT(*) AS nb_clients FROM clients WHERE mot_de_passe IS NOT NULL;
```

**Résultat attendu :**
- `nb_employes` : **4** (Admin, Gestionnaire, Employé, Comptable)
- `nb_clients` : **5** (Jean, Marie, Pierre, Sophie, Nordik Sports)

---

## 🐛 Dépannage

### Erreur : "Duplicate column name 'mot_de_passe'"
**Cause** : La colonne existe déjà

**Solution** : Utiliser `SQL_Schema_Auth_Safe.sql` ou ignorer l'erreur

---

### Erreur : "Unknown column 'role_systeme'"
**Cause** : La table `employes` n'a pas été créée

**Solution** : Exécuter d'abord `NordikAdventuresERP_Schema_FR.sql`

---

### Erreur : "Table 'employes' doesn't exist"
**Cause** : Le schéma principal n'a pas été créé

**Solution** :
```bash
# Exécuter d'abord le schéma principal
mysql -u root -p < NordikAdventuresERP_Schema_FR.sql

# Puis le script d'authentification
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth.sql
```

---

## 📝 Ordre d'Exécution

### ✅ Ordre Correct
1. `NordikAdventuresERP_Schema_FR.sql` (Créer la BDD et les tables)
2. `SQL_Schema_Auth.sql` ou `SQL_Schema_Auth_Safe.sql` (Ajouter l'authentification)

### ❌ Ne PAS faire
- Exécuter `SQL_Schema_Auth.sql` avant `NordikAdventuresERP_Schema_FR.sql`

---

## 🎯 Test Rapide

### Dans MySQL Workbench
```sql
USE NordikAdventuresERP;

-- Tester l'authentification d'un employé
SELECT CONCAT(prenom, ' ', nom) AS nom, role_systeme 
FROM employes 
WHERE courriel = 'admin@nordikadventures.com' AND mot_de_passe = 'admin123';

-- Résultat attendu : Admin Tremblay | Administrateur

-- Tester l'authentification d'un client
SELECT nom, type 
FROM clients 
WHERE courriel_contact = 'jean.client@test.com' AND mot_de_passe = 'client123';

-- Résultat attendu : Jean Dupont | Particulier
```

---

## 🔄 Réexécuter le Script

Si vous devez réexécuter le script (par exemple, pour réinitialiser les mots de passe) :

### Option 1 : Utiliser la version Safe
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth_Safe.sql
```
✅ Aucune erreur, même si déjà exécuté

### Option 2 : Supprimer les colonnes d'abord
```sql
-- Supprimer les colonnes
ALTER TABLE employes DROP COLUMN mot_de_passe;
ALTER TABLE clients DROP COLUMN mot_de_passe;

-- Puis réexécuter SQL_Schema_Auth.sql
```

---

## 📚 Fichiers de Référence

- `SQL_Schema_Auth.sql` - Version simple
- `SQL_Schema_Auth_Safe.sql` - Version safe (recommandée)
- `INSTRUCTIONS_BDD.md` - Guide complet
- `AUTHENTIFICATION.md` - Documentation de l'authentification

---

## ✅ Checklist

- [ ] Base de données `NordikAdventuresERP` créée
- [ ] Script `NordikAdventuresERP_Schema_FR.sql` exécuté
- [ ] Script `SQL_Schema_Auth.sql` ou `SQL_Schema_Auth_Safe.sql` exécuté
- [ ] Colonnes `mot_de_passe` ajoutées (vérifiées avec `SHOW COLUMNS`)
- [ ] 4 employés de test insérés
- [ ] 5 clients de test insérés
- [ ] Test d'authentification réussi
- [ ] Application C# peut se connecter à MySQL

---

**Vous êtes prêt ! 🎉**

Lancez maintenant l'application WPF et testez la connexion avec :
- Employé : `admin@nordikadventures.com` / `admin123`
- Client : `jean.client@test.com` / `client123`

