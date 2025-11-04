# 🔐 Modifications - Système d'Authentification

## 📅 Date : Janvier 2025

---

## 🎯 Objectif

Séparer l'authentification en **deux types d'utilisateurs** :
- **👔 Employés** : Ajoutés par l'admin, accèdent au **PGI**
- **🛒 Clients** : S'inscrivent via le site, accèdent au **site d'achat**

---

## ✅ Modifications Effectuées

### 1. **RegisterWindow.xaml** - Inscription pour clients uniquement

**Changements :**
- ✅ Supprimé les champs **Rôle** et **Département**
- ✅ Simplifié le formulaire : **Nom**, **Email**, **Téléphone**, **Mot de passe**
- ✅ Modifié les textes pour cibler les clients (site d'achat)
- ✅ Titre changé : "Créer un compte client 📝"

**Fichier modifié :**
- `RegisterWindow.xaml`

---

### 2. **RegisterWindow.xaml.cs** - Logique d'inscription client

**Changements :**
- ✅ Retiré la logique de rôle/département
- ✅ Validation simplifiée (Nom, Email, Mot de passe)
- ✅ Validation email basique (`@` et `.`)
- ✅ Validation longueur mot de passe (min 6 caractères)
- ✅ Message de confirmation adapté pour les clients

**Fichier modifié :**
- `RegisterWindow.xaml.cs`

---

### 3. **LoginWindow.xaml.cs** - Distinction employé vs client

**Changements :**
- ✅ Ajout de méthodes `IsEmployee()` et `IsClient()` pour vérifier le type d'utilisateur
- ✅ Ajout de méthodes `GetEmployeeRole()` et `GetClientName()` pour obtenir les infos
- ✅ Redirection conditionnelle :
  - **Employé** → `ModuleSelectionWindow` (PGI)
  - **Client** → `ClientShoppingWindow` (Site d'achat)
- ✅ Gestion des erreurs si identifiants incorrects

**Fichier modifié :**
- `LoginWindow.xaml.cs`

---

### 4. **ClientShoppingWindow.xaml** - Nouvelle fenêtre site d'achat

**Nouveau fichier créé :**
- ✅ Interface de site d'achat pour les clients
- ✅ Header avec nom du client et bouton déconnexion
- ✅ Contenu principal (placeholder pour développement futur)
- ✅ Design cohérent avec le reste de l'application

**Fichier créé :**
- `ClientShoppingWindow.xaml`

---

### 5. **ClientShoppingWindow.xaml.cs** - Code-behind site d'achat

**Nouveau fichier créé :**
- ✅ Constructeur acceptant `name` et `email`
- ✅ Affichage du nom du client dans le header
- ✅ Bouton déconnexion retournant à `LoginWindow`

**Fichier créé :**
- `ClientShoppingWindow.xaml.cs`

---

### 6. **SQL_Utilisateurs_Test.sql** - Données de test

**Nouveau fichier créé :**
- ✅ Script SQL pour ajouter des employés de test
- ✅ Script SQL pour ajouter des clients de test
- ✅ Documentation des identifiants de test

**Fichier créé :**
- `SQL_Utilisateurs_Test.sql`

---

### 7. **IDENTIFIANTS_TEST.md** - Documentation

**Nouveau fichier créé :**
- ✅ Liste complète des identifiants de test
- ✅ Instructions pour tester les deux types d'utilisateurs
- ✅ Diagramme de flux de navigation

**Fichier créé :**
- `IDENTIFIANTS_TEST.md`

---

## 🔐 Identifiants de Test

### 👔 Employés (PGI)

| Username | Mot de passe | Rôle |
|----------|-------------|------|
| `admin` | `admin123` | Administrateur |
| `gestionnaire` | `gestionnaire123` | Gestionnaire |
| `employe` | `employe123` | Employé Ventes |
| `comptable` | `comptable123` | Comptable |

### 🛒 Clients (Site d'achat)

| Email | Mot de passe | Nom |
|-------|-------------|-----|
| `client1@test.com` | `client123` | Jean Dupont |
| `client2@test.com` | `client123` | Marie Martin |
| `client3@test.com` | `client123` | Pierre Tremblay |

---

## 🔄 Flux de Navigation

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

## 📝 Notes Importantes

### Pour les employés :
- ✅ Ajoutés par l'admin dans la table `employes`
- ✅ Mots de passe hashés avec SHA2 (256 bits)
- ✅ Accès au PGI via `ModuleSelectionWindow`

### Pour les clients :
- ✅ S'inscrivent via `RegisterWindow`
- ⚠️ **Actuellement** : Mots de passe vérifiés dans le code C# (dictionnaire)
- ⚠️ **Pour la production** : Ajouter colonne `mot_de_passe_hash` dans `clients`
- ✅ Accès au site d'achat via `ClientShoppingWindow`

---

## 🚀 Développement Futur

### À implémenter :
1. ✅ Connexion à MySQL pour vérifier les identifiants
2. ✅ Hash des mots de passe clients dans la base de données
3. ✅ Gestion des sessions utilisateurs
4. ✅ Logs des connexions
5. ✅ Récupération de mot de passe
6. ✅ Validation email lors de l'inscription
7. ✅ Fonctionnalités du site d'achat (catalogue, panier, commandes)

---

## 📁 Fichiers Modifiés/Créés

### Modifiés :
- ✅ `RegisterWindow.xaml`
- ✅ `RegisterWindow.xaml.cs`
- ✅ `LoginWindow.xaml.cs`

### Créés :
- ✅ `ClientShoppingWindow.xaml`
- ✅ `ClientShoppingWindow.xaml.cs`
- ✅ `SQL_Utilisateurs_Test.sql`
- ✅ `IDENTIFIANTS_TEST.md`
- ✅ `MODIFICATIONS_AUTHENTIFICATION.md` (ce fichier)

---

**Dernière mise à jour** : Janvier 2025

