# 🔐 Système d'Authentification - PGI Nordik Adventures

## 📋 Vue d'ensemble

Le système d'authentification permet de **séparer l'accès entre les employés et les clients** :

- **Employés** → Accès au **PGI** (système de gestion)
- **Clients** → Accès au **Site d'achat** (boutique en ligne)

---

## 🎯 Règles de connexion

### ✅ Connexion Employé
- **Condition** : L'email **NE CONTIENT PAS** le mot "client"
- **Redirection** : `ModuleSelectionWindow` (choix du module PGI)
- **Exemples d'emails valides** :
  - `admin@nordikadventures.com`
  - `gestionnaire@nordikadventures.com`
  - `employe@nordikadventures.com`
  - `comptable@nordikadventures.com`

### ✅ Connexion Client
- **Condition** : L'email **CONTIENT** le mot "client"
- **Redirection** : `ClientShoppingWindow` (site d'achat)
- **Exemples d'emails valides** :
  - `jean.client@test.com`
  - `marie.client@email.com`
  - `client.sophie@gmail.com`
  - `contact@nordikclient.com`

---

## 🔑 Identifiants de test

### 👨‍💼 Employés (Accès PGI)

| Rôle | Email | Mot de passe |
|------|-------|--------------|
| **Administrateur** | `admin@nordikadventures.com` | `admin123` |
| **Gestionnaire** | `gestionnaire@nordikadventures.com` | `gestionnaire123` |
| **Employé Ventes** | `employe@nordikadventures.com` | `employe123` |
| **Comptable** | `comptable@nordikadventures.com` | `comptable123` |

### 👤 Clients (Accès Site d'achat)

| Nom | Email | Mot de passe |
|-----|-------|--------------|
| Jean Dupont | `jean.client@test.com` | `client123` |
| Marie Martin | `marie.client@test.com` | `client123` |
| Pierre Tremblay | `pierre.client@entreprise.com` | `client123` |
| Sophie Lavoie | `client.sophie@gmail.com` | `client123` |
| Nordik Sports Inc. | `contact@nordikclient.com` | `client123` |

---

## 📝 Inscription des clients

### Règles d'inscription
1. **Email obligatoire** : L'email **DOIT contenir** le mot "client"
2. **Validation** : Si l'email ne contient pas "client", l'inscription est refusée
3. **Enregistrement automatique** : Le client est ajouté à la table `clients` dans la BDD
4. **Mot de passe** : Minimum 6 caractères (stocké en clair, pas de hashage)

### Exemple d'inscription valide
```
Nom : Jean Tremblay
Email : jean.client@gmail.com  ✅ (contient "client")
Téléphone : 514-555-1234 (optionnel)
Mot de passe : monmotdepasse
```

### Exemple d'inscription invalide
```
Nom : Jean Tremblay
Email : jean@gmail.com  ❌ (ne contient PAS "client")
→ Erreur : "L'adresse email doit contenir le mot 'client'"
```

---

## 🛠️ Fonctionnalités implémentées

### LoginWindow
- ✅ Authentification avec base de données MySQL
- ✅ Détection automatique Employé vs Client (via email)
- ✅ Redirection intelligente selon le type d'utilisateur
- ✅ Bouton 👁️ pour afficher/cacher le mot de passe
- ✅ Gestion des erreurs avec messages clairs

### RegisterWindow
- ✅ Inscription des clients dans la BDD
- ✅ Validation de l'email (doit contenir "client")
- ✅ Vérification de doublon (email déjà utilisé)
- ✅ Bouton 👁️ pour afficher/cacher les mots de passe
- ✅ Redirection vers LoginWindow après inscription réussie

---

## 💾 Structure de la base de données

### Table `employes`
```sql
ALTER TABLE employes 
ADD COLUMN mot_de_passe VARCHAR(255) DEFAULT NULL;
```

### Table `clients`
```sql
ALTER TABLE clients 
ADD COLUMN mot_de_passe VARCHAR(255) DEFAULT NULL;
```

---

## 🚀 Installation et configuration

### Étape 1 : Exécuter le script SQL
```sql
-- Dans MySQL Workbench ou ligne de commande
source SQL_Schema_Auth.sql;
```

### Étape 2 : Vérifier la connexion MySQL
Ouvrir `DatabaseHelper.cs` et vérifier la chaîne de connexion :
```csharp
private static string connectionString = 
    "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=;";
```

### Étape 3 : Tester l'authentification
1. Lancer l'application
2. Tenter de se connecter avec un compte employé
3. Vérifier la redirection vers `ModuleSelectionWindow`
4. Tenter de se connecter avec un compte client
5. Vérifier la redirection vers `ClientShoppingWindow`

---

## 📊 Architecture des Services

### EmployeService.cs
```csharp
public static (bool success, string nom, string prenom, string role) 
    Authenticate(string email, string password)
```
- Authentifie un employé
- Retourne : succès, nom, prénom, rôle système

### ClientService.cs
```csharp
public static (bool success, string nom, int clientId) 
    Authenticate(string email, string password)
```
- Authentifie un client
- Retourne : succès, nom, ID client

```csharp
public static (bool success, string message, int clientId) 
    Register(string nom, string email, string telephone, string password)
```
- Inscrit un nouveau client
- Vérifie que l'email contient "client"
- Retourne : succès, message, ID client

```csharp
public static bool IsClientEmail(string email)
```
- Vérifie si un email contient "client"
- Utilisé pour distinguer employés et clients

---

## 🔒 Sécurité

### ⚠️ Important
- **Mots de passe en clair** : Pour simplifier le développement, les mots de passe sont stockés en clair (pas de hashage).
- **À faire en production** : Utiliser `BCrypt.Net` ou `SHA256` pour hasher les mots de passe.

### Amélioration recommandée (pour production)
```csharp
// Hashage avec BCrypt
using BCrypt.Net;

string hashedPassword = BCrypt.HashPassword(password);
bool isValid = BCrypt.Verify(password, hashedPassword);
```

---

## 📝 Notes de développement

### Flux d'authentification
```
LoginWindow
    ↓
ClientService.IsClientEmail(email) ?
    ↓ OUI (contient "client")
    ClientService.Authenticate(email, password)
        ↓ Succès
        ClientShoppingWindow (Site d'achat)
    
    ↓ NON (ne contient pas "client")
    EmployeService.Authenticate(email, password)
        ↓ Succès
        ModuleSelectionWindow (PGI)
```

### Flux d'inscription
```
RegisterWindow
    ↓
Validation de l'email (doit contenir "client")
    ↓
ClientService.Register(nom, email, telephone, password)
    ↓ Succès
    LoginWindow (connexion)
```

---

## ✅ Tests à effectuer

- [ ] Connexion employé avec `admin@nordikadventures.com` / `admin123`
- [ ] Connexion client avec `jean.client@test.com` / `client123`
- [ ] Inscription nouveau client avec email contenant "client"
- [ ] Inscription refusée si email ne contient pas "client"
- [ ] Bouton 👁️ pour afficher/cacher les mots de passe
- [ ] Message d'erreur si identifiants incorrects
- [ ] Redirection correcte selon le type d'utilisateur

---

## 🐛 Dépannage

### Erreur : "Impossible de se connecter à MySQL"
→ Vérifier que MySQL est démarré et que la BDD existe

### Erreur : "Unknown column 'mot_de_passe'"
→ Exécuter le script `SQL_Schema_Auth.sql`

### Erreur : "L'email doit contenir 'client'"
→ C'est normal ! L'inscription est réservée aux clients uniquement

---

## 📦 Fichiers créés/modifiés

### Services
- ✅ `Services/EmployeService.cs`
- ✅ `Services/ClientService.cs`

### Fenêtres
- ✅ `LoginWindow.xaml.cs` (mis à jour)
- ✅ `RegisterWindow.xaml.cs` (mis à jour)

### Base de données
- ✅ `SQL_Schema_Auth.sql`

### Documentation
- ✅ `AUTHENTIFICATION.md` (ce fichier)

---

**Développement terminé ! ✨**

