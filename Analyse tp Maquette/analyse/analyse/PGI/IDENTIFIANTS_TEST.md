# 🔐 Identifiants de Test - NordikAdventuresERP

## 📋 Vue d'ensemble

Le système distingue **deux types d'utilisateurs** :
- **👔 Employés** : Ajoutés par l'admin dans le système, accèdent au **PGI**
- **🛒 Clients** : S'inscrivent via le site, accèdent au **site d'achat**

---

## 👔 EMPLOYÉS (Accès au PGI)

Les employés sont ajoutés dans le système par l'administrateur. Ils utilisent leur **nom d'utilisateur** (matricule ou email) pour se connecter.

### Comptes de test disponibles :

| Username | Mot de passe | Rôle | Accès |
|----------|-------------|------|-------|
| `admin` | `admin123` | Administrateur | Tous les modules |
| `gestionnaire` | `gestionnaire123` | Gestionnaire | Tous les modules |
| `employe` | `employe123` | Employé Ventes | Modules selon permissions |
| `comptable` | `comptable123` | Comptable | Module Finances principalement |

### Comment tester :

1. Ouvrir l'application
2. Entrer un **username** et **mot de passe** d'employé
3. ✅ **Résultat** : Redirection vers `ModuleSelectionWindow` (choix du module PGI)

---

## 🛒 CLIENTS (Accès au site d'achat)

Les clients s'inscrivent via la page d'inscription (`RegisterWindow`). Ils utilisent leur **email** comme identifiant.

### Comptes de test disponibles :

| Email | Mot de passe | Nom | Type |
|-------|-------------|-----|------|
| `client1@test.com` | `client123` | Jean Dupont | Particulier |
| `client2@test.com` | `client123` | Marie Martin | Particulier |
| `client3@test.com` | `client123` | Pierre Tremblay | Particulier |

### Comment tester :

1. **Option 1 : Utiliser un compte existant**
   - Ouvrir l'application
   - Entrer un **email client** et **mot de passe**
   - ✅ **Résultat** : Redirection vers `ClientShoppingWindow` (site d'achat)

2. **Option 2 : Créer un nouveau compte**
   - Ouvrir l'application
   - Cliquer sur "S'inscrire"
   - Remplir le formulaire (Nom, Email, Téléphone, Mot de passe)
   - ✅ **Résultat** : Compte créé, retour à la page de connexion

---

## 🔄 Flux de navigation

```
┌─────────────────┐
│  LOGIN WINDOW   │
└────────┬────────┘
         │
         ├─ Employé → ModuleSelectionWindow → MainWindow (PGI)
         │
         └─ Client → ClientShoppingWindow (Site d'achat)
```

---

## 📝 Notes importantes

### Pour les employés :
- ✅ Les employés sont ajoutés dans la table `employes` par l'admin
- ✅ Leurs mots de passe sont hashés avec SHA2 (256 bits)
- ✅ Ils accèdent au PGI via `ModuleSelectionWindow`

### Pour les clients :
- ✅ Les clients s'inscrivent via `RegisterWindow`
- ⚠️ **Actuellement** : Les mots de passe clients sont vérifiés dans le code C# (dictionnaire)
- ⚠️ **Pour la production** : Ajouter une colonne `mot_de_passe_hash` dans la table `clients`
- ✅ Ils accèdent au site d'achat via `ClientShoppingWindow`

---

## 🗄️ Base de données

### Tables utilisées :
- `employes` : Employés avec accès au PGI
- `clients` : Clients avec accès au site d'achat

### Scripts SQL :
- `SQL_Authentification.sql` : Structure des tables (si nécessaire)
- `SQL_Utilisateurs_Test.sql` : Données de test (employés + clients)

---

## 🚀 Développement futur

### À implémenter :
1. ✅ Connexion à la base de données MySQL pour vérifier les identifiants
2. ✅ Hash des mots de passe clients dans la table `clients`
3. ✅ Gestion des sessions utilisateurs
4. ✅ Logs des connexions
5. ✅ Récupération de mot de passe
6. ✅ Validation email lors de l'inscription

---

**Dernière mise à jour** : Janvier 2025
