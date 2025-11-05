# 📋 Instructions Base de Données - Authentification

## 🎯 Objectif
Configurer l'authentification pour les employés et les clients dans le PGI Nordik Adventures.

---

## ⚙️ Étape 1 : Vérifier MySQL

### 1.1 Vérifier que MySQL est installé et démarré
```bash
# Windows
net start MySQL80

# Ou via XAMPP Control Panel
# Démarrer Apache + MySQL
```

### 1.2 Vérifier la connexion
```bash
mysql -u root -p
# Entrer le mot de passe (ou laisser vide si pas de mot de passe)
```

---

## 📦 Étape 2 : Créer la base de données (si ce n'est pas déjà fait)

### Option A : Via MySQL Workbench
1. Ouvrir MySQL Workbench
2. Se connecter au serveur local
3. File > Open SQL Script
4. Sélectionner `NordikAdventuresERP_Schema_FR.sql`
5. Cliquer sur ⚡ (Execute)

### Option B : Via ligne de commande
```bash
mysql -u root -p < NordikAdventuresERP_Schema_FR.sql
```

---

## 🔐 Étape 3 : Ajouter l'authentification

### Exécuter le script d'authentification

#### Via MySQL Workbench
1. File > Open SQL Script
2. Sélectionner `SQL_Schema_Auth.sql`
3. Cliquer sur ⚡ (Execute)

#### Via ligne de commande
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth.sql
```

---

## ✅ Étape 4 : Vérifier l'installation

### 4.1 Vérifier les colonnes ajoutées
```sql
USE NordikAdventuresERP;

-- Vérifier la colonne mot_de_passe dans employes
DESCRIBE employes;

-- Vérifier la colonne mot_de_passe dans clients
DESCRIBE clients;
```

### 4.2 Vérifier les données de test
```sql
-- Afficher les employés
SELECT matricule, CONCAT(prenom, ' ', nom) AS nom, courriel, role_systeme 
FROM employes 
WHERE mot_de_passe IS NOT NULL;

-- Afficher les clients
SELECT id, nom, courriel_contact, type, statut 
FROM clients 
WHERE mot_de_passe IS NOT NULL;
```

---

## 🔧 Étape 5 : Configurer la connexion dans le projet

### 5.1 Ouvrir `DatabaseHelper.cs`
Chemin : `Analyse tp Maquette/analyse/analyse/PGI/Helpers/DatabaseHelper.cs`

### 5.2 Modifier la chaîne de connexion si nécessaire
```csharp
private static string connectionString = 
    "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

**Exemples courants :**
- **Pas de mot de passe** : `Pwd=;`
- **Mot de passe "root"** : `Pwd=root;`
- **XAMPP par défaut** : `Pwd=;` (vide)

---

## 🧪 Étape 6 : Tester l'authentification

### 6.1 Lancer l'application
```bash
# Dans Visual Studio
F5 (Démarrer avec débogage)
```

### 6.2 Tester la connexion Employé
```
Email: admin@nordikadventures.com
Mot de passe: admin123
```
→ Devrait rediriger vers `ModuleSelectionWindow`

### 6.3 Tester la connexion Client
```
Email: jean.client@test.com
Mot de passe: client123
```
→ Devrait rediriger vers `ClientShoppingWindow`

### 6.4 Tester l'inscription Client
1. Cliquer sur "S'inscrire"
2. Remplir le formulaire avec un email contenant "client"
   - Nom : `Test Client`
   - Email : `test.client@exemple.com`
   - Mot de passe : `test123`
3. Cliquer sur "Créer mon compte"
4. Se connecter avec ces identifiants

---

## 🐛 Dépannage

### Erreur : "Unable to connect to any of the specified MySQL hosts"
**Cause** : MySQL n'est pas démarré ou mauvais serveur

**Solution** :
1. Vérifier que MySQL est démarré
2. Vérifier `Server=localhost` dans `DatabaseHelper.cs`

---

### Erreur : "Access denied for user 'root'@'localhost'"
**Cause** : Mauvais mot de passe MySQL

**Solution** :
1. Vérifier le mot de passe dans `DatabaseHelper.cs`
2. Essayer avec `Pwd=;` (vide)
3. Ou `Pwd=root;`

---

### Erreur : "Unknown database 'NordikAdventuresERP'"
**Cause** : La base de données n'existe pas

**Solution** :
```sql
CREATE DATABASE NordikAdventuresERP;
-- Puis exécuter NordikAdventuresERP_Schema_FR.sql
```

---

### Erreur : "Unknown column 'mot_de_passe' in 'field list'"
**Cause** : Le script `SQL_Schema_Auth.sql` n'a pas été exécuté

**Solution** :
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth.sql
```

---

### Erreur : "Duplicate entry 'admin@nordikadventures.com' for key 'courriel'"
**Cause** : Les données de test existent déjà (normal)

**Solution** : C'est normal ! Le script utilise `ON DUPLICATE KEY UPDATE` pour mettre à jour les mots de passe existants.

---

## 📝 Résumé des commandes SQL

### Vérifier tout fonctionne
```sql
USE NordikAdventuresERP;

-- 1. Vérifier les colonnes
SHOW COLUMNS FROM employes LIKE 'mot_de_passe';
SHOW COLUMNS FROM clients LIKE 'mot_de_passe';

-- 2. Compter les employés de test
SELECT COUNT(*) AS nb_employes FROM employes WHERE mot_de_passe IS NOT NULL;

-- 3. Compter les clients de test
SELECT COUNT(*) AS nb_clients FROM clients WHERE mot_de_passe IS NOT NULL;

-- 4. Tester l'authentification d'un employé
SELECT CONCAT(prenom, ' ', nom) AS nom, role_systeme 
FROM employes 
WHERE courriel = 'admin@nordikadventures.com' AND mot_de_passe = 'admin123';

-- 5. Tester l'authentification d'un client
SELECT nom, type 
FROM clients 
WHERE courriel_contact = 'jean.client@test.com' AND mot_de_passe = 'client123';
```

---

## 🎯 Checklist finale

Avant de tester l'application, vérifier :

- [ ] MySQL est démarré
- [ ] Base de données `NordikAdventuresERP` existe
- [ ] Script `NordikAdventuresERP_Schema_FR.sql` exécuté
- [ ] Script `SQL_Schema_Auth.sql` exécuté
- [ ] Colonnes `mot_de_passe` ajoutées aux tables `employes` et `clients`
- [ ] Données de test insérées (4 employés + 5 clients minimum)
- [ ] Chaîne de connexion dans `DatabaseHelper.cs` correcte
- [ ] Application se lance sans erreur
- [ ] Connexion employé fonctionne
- [ ] Connexion client fonctionne
- [ ] Inscription client fonctionne

---

## 📧 Support

En cas de problème :
1. Vérifier les logs MySQL
2. Vérifier la console de Visual Studio (erreurs C#)
3. Relire ce fichier d'instructions

---

**Installation terminée ! ✨**

