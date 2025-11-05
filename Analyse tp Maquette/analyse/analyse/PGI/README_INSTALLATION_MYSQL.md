# 📦 Installation MySQL pour Nordik Adventures ERP

## ⚠️ IMPORTANT pour les Collaborateurs

L'application est maintenant connectée à une **base de données MySQL**. Pour utiliser l'application avec les 30 produits et toutes les fonctionnalités, vous devez installer MySQL et exécuter les scripts SQL fournis.

---

## 🚀 Installation Rapide (5 minutes)

### Étape 1 : Installer MySQL

1. Télécharger MySQL Community Server : https://dev.mysql.com/downloads/mysql/
2. Installer avec les options par défaut
3. Définir un mot de passe root (notez-le !)

### Étape 2 : Installer MySQL Workbench

1. Télécharger : https://dev.mysql.com/downloads/workbench/
2. Installer et lancer MySQL Workbench
3. Se connecter au serveur local (root + votre mot de passe)

### Étape 3 : Exécuter les Scripts SQL

**Dans MySQL Workbench, exécuter dans cet ordre :**

#### 3.1 Schéma Principal (OBLIGATOIRE)
```
File > Open SQL Script > NordikAdventuresERP_Schema_FR.sql
Cliquer sur ⚡ Execute
Attendre 1-2 minutes
```

**Résultat attendu :** 20+ tables créées

#### 3.2 Authentification (OBLIGATOIRE)
```
File > Open SQL Script > Analyse tp Maquette/analyse/analyse/PGI/SQL_Schema_Auth_Safe.sql
Cliquer sur ⚡ Execute
```

**Résultat attendu :** 4 employés + 5 clients ajoutés

#### 3.3 Produits (RECOMMANDÉ)
```
File > Open SQL Script > Analyse tp Maquette/analyse/analyse/PGI/SQL_Produits_NordikAdventures.sql
Cliquer sur ⚡ Execute
```

**Résultat attendu :** 30 produits + catégories + fournisseurs ajoutés

### Étape 4 : Configurer le Mot de Passe dans l'Application

1. Ouvrir **`Helpers/DatabaseHelper.cs`**
2. Ligne 13, modifier :
   ```csharp
   private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
   ```
3. Remplacer `VOTRE_MOT_DE_PASSE` par votre mot de passe MySQL root
4. Sauvegarder (Ctrl+S)

### Étape 5 : Lancer l'Application

1. Dans Visual Studio : **F5** (Debug)
2. Se connecter avec :
   - **Employé** : `admin@nordikadventures.com` / `admin123`
   - **Client** : `jean.client@test.com` / `client123`
3. Aller dans **Stocks** > **Produits**
4. Vous devriez voir **30 produits** !

---

## ✅ Vérification Rapide

### Test 1 : Vérifier que la BDD existe
```sql
SHOW DATABASES LIKE 'NordikAdventuresERP';
```
**Attendu :** 1 ligne

### Test 2 : Vérifier les produits
```sql
USE NordikAdventuresERP;
SELECT COUNT(*) FROM produits;
```
**Attendu :** 30

### Test 3 : Vérifier les employés
```sql
SELECT nom, prenom, courriel FROM employes WHERE mot_de_passe IS NOT NULL;
```
**Attendu :** 4 employés

---

## 🎯 Mode Sans MySQL (Fallback)

Si vous **ne voulez pas installer MySQL**, l'application fonctionne quand même avec des **données d'exemple** (3 produits fictifs). Mais vous n'aurez pas :
- ❌ Les 30 vrais produits
- ❌ La persistance des données
- ❌ Les fonctionnalités d'ajout/modification/suppression

---

## 📋 Identifiants de Test

### Employés (accès PGI)
| Email | Mot de passe | Rôle |
|-------|--------------|------|
| admin@nordikadventures.com | admin123 | Admin |
| gestionnaire@nordikadventures.com | gestionnaire123 | Gestionnaire |
| employe@nordikadventures.com | employe123 | Employé Ventes |
| comptable@nordikadventures.com | comptable123 | Comptable |

### Clients (accès site d'achat)
| Email | Mot de passe |
|-------|--------------|
| jean.client@test.com | client123 |
| marie.client@test.com | client123 |
| pierre.client@entreprise.com | client123 |
| client.sophie@gmail.com | client123 |
| contact@nordikclient.com | client123 |

---

## 🆘 Problèmes Courants

### Erreur : "Access denied for user 'root'@'localhost'"
**Solution :** Mauvais mot de passe dans `DatabaseHelper.cs` (Étape 4)

### Erreur : "Unknown database 'NordikAdventuresERP'"
**Solution :** Le schéma n'a pas été exécuté (retour à Étape 3.1)

### Erreur : "Column 'categorie_id' does not belong to table"
**Solution :** Le schéma a été partiellement exécuté. Supprimer et recréer :
```sql
DROP DATABASE IF EXISTS NordikAdventuresERP;
```
Puis retour à Étape 3.1

### L'application affiche encore 3 produits (données d'exemple)
**Solutions :**
1. Vérifier le mot de passe dans `DatabaseHelper.cs`
2. Vérifier que les produits existent : `SELECT COUNT(*) FROM produits;`
3. Vérifier la connexion MySQL (port 3306)

---

## 📁 Fichiers SQL Fournis

| Fichier | Description | Obligatoire |
|---------|-------------|-------------|
| `NordikAdventuresERP_Schema_FR.sql` | Schéma complet (tables, contraintes, vues, procédures) | ✅ OUI |
| `SQL_Schema_Auth_Safe.sql` | Employés + Clients de test | ✅ OUI |
| `SQL_Produits_NordikAdventures.sql` | 30 produits + catégories + fournisseurs | ⭐ RECOMMANDÉ |

---

## 🎉 Après Installation

Vous pourrez :
- ✅ Se connecter avec 4 employés différents (rôles différents)
- ✅ Se connecter avec 5 clients (site d'achat)
- ✅ Voir 30 produits réels dans le module Stocks
- ✅ Ajouter, modifier, supprimer des produits
- ✅ Voir les calculs en temps réel (valeur stock, marges)
- ✅ Rechercher des produits
- ✅ Gérer les catégories et fournisseurs

---

## 💡 Aide Supplémentaire

Consultez les fichiers :
- `INSTALL_BDD_ETAPE_PAR_ETAPE.md` (guide détaillé)
- `RESOLUTION_ERREUR_BDD.md` (dépannage)
- `GUIDE_RAPIDE_SQL.md` (installation MySQL)

---

**🚀 Bon développement avec Nordik Adventures ERP !**

