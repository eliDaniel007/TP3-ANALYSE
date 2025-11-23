# 🔐 Identifiants de test - SQLite + EF Core

## 📋 Comptes créés automatiquement

Au premier lancement, la base de données SQLite est créée et les comptes suivants sont automatiquement ajoutés.

---

## 👨‍💼 Employé (Accès PGI)

### Compte Administrateur

**Email:** `admin@nordikadventures.ca`  
**Mot de passe:** `admin123`  
**Nom:** Sophie Tremblay  
**Rôle:** Administrateur  
**Accès:** Tous les modules du PGI

**Modules disponibles:**
- 📊 Dashboard
- 📦 Produits & Stocks
- 💰 Finances & Comptabilité
- 👥 CRM (Gestion clients)
- ⚙️ Paramètres

---

## 🛒 Clients (Accès site d'achat)

### Client 1 - Jean Tremblay

**Email:** `jean.client@email.com`  
**Mot de passe:** `client123`  
**Type:** Particulier  
**Téléphone:** 514-555-0101  
**Accès:** Site d'achat en ligne

---

### Client 2 - Club de Plein Air Montréal

**Email:** `contact.client@clubpleinair.ca`  
**Mot de passe:** `club123`  
**Type:** Entreprise  
**Téléphone:** 514-555-0102  
**Accès:** Site d'achat en ligne

---

### Client 3 - Marie Dupont

**Email:** `marie.client@email.com`  
**Mot de passe:** `marie123`  
**Type:** Particulier  
**Téléphone:** 438-555-0103  
**Accès:** Site d'achat en ligne

---

## 📝 Inscription de nouveaux clients

### Créer un nouveau compte client

1. Cliquez sur **"Créer un compte"** sur la page de connexion
2. Remplissez le formulaire :
   - **Nom complet** (obligatoire)
   - **Email** (obligatoire, doit contenir "client")
   - **Téléphone** (optionnel)
   - **Mot de passe** (minimum 6 caractères)
   - **Confirmation du mot de passe**

3. Cliquez sur **"Créer mon compte"**
4. Connectez-vous avec vos nouveaux identifiants

### ⚠️ Important : Règles pour l'email

- **Employés** : Email avec domaine `@nordikadventures.ca` ou sans "client"
- **Clients** : Email doit contenir le mot **"client"**

**Exemples valides pour clients :**
- ✅ `jean.client@email.com`
- ✅ `client.dupont@gmail.com`
- ✅ `contact.client@entreprise.ca`

**Exemples invalides pour clients :**
- ❌ `jean.dupont@email.com` (manque "client")
- ❌ `contact@entreprise.ca` (manque "client")

---

## 🔧 Réinitialiser la base de données

Pour repartir avec les comptes par défaut :

1. Fermez l'application
2. Supprimez le fichier de base de données :
   ```
   %APPDATA%\PGI_NordikAdventures\NordikAdventuresERP.db
   ```
3. Relancez l'application
4. La base de données sera recréée avec tous les comptes de test

---

## 🎯 Test de connexion

### Scénario 1 : Connexion Employé
```
Email: admin@nordikadventures.ca
MDP: admin123
→ Accès au PGI complet
```

### Scénario 2 : Connexion Client
```
Email: jean.client@email.com
MDP: client123
→ Accès au site d'achat
```

### Scénario 3 : Inscription nouveau client
```
1. Cliquer sur "Créer un compte"
2. Remplir le formulaire
3. Email DOIT contenir "client"
4. Se connecter avec les nouveaux identifiants
```

---

## 📊 Données incluses

Outre les comptes utilisateurs, la base de données initiale contient :

- ✅ **8 catégories** de produits
- ✅ **5 fournisseurs** canadiens
- ✅ **10 produits** d'exemple (tentes, sacs, vêtements, etc.)
- ✅ **Niveaux de stock** initiaux
- ✅ **5 mouvements** de stock historiques
- ✅ **1 employé** administrateur
- ✅ **3 clients** de test

---

## 🔒 Sécurité

⚠️ **ATTENTION:** Ces mots de passe sont pour le DÉVELOPPEMENT seulement !

Pour la production, vous devriez :
- Implémenter le hashage des mots de passe (BCrypt, SHA256, etc.)
- Ajouter une politique de mots de passe forts
- Implémenter l'authentification à deux facteurs
- Limiter les tentatives de connexion

---

**Date de création:** 2025-11-15  
**Version:** SQLite + Entity Framework Core 8.0

