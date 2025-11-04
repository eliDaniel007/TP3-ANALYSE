# 🔐 Identifiants de Test - PGI Nordik Adventures

## Mode Maquette (Sans Base de Données)

Le système utilise actuellement des identifiants de test **codés en dur** pour les démonstrations et maquettes.

---

## 👥 Comptes Utilisateurs de Test

### 1️⃣ **Administrateur**
- **Nom d'utilisateur :** `admin`
- **Mot de passe :** `admin`
- **Rôle :** Admin
- **Accès :** ✅ Tous les modules (Stocks, Finances, CRM)

### 2️⃣ **Gestionnaire**
- **Nom d'utilisateur :** `gestionnaire`
- **Mot de passe :** `gestionnaire`
- **Rôle :** Gestionnaire
- **Accès :** ✅ Tous les modules (Stocks, Finances, CRM)

### 3️⃣ **Employé**
- **Nom d'utilisateur :** `employe`
- **Mot de passe :** `employe`
- **Rôle :** Employe
- **Accès :** 
  - ✅ MODULE 2: Finances & Facturation
  - ✅ MODULE 3: CRM
  - ❌ MODULE 1: Stocks & Produits (réservé au gestionnaire)

### 4️⃣ **Comptable**
- **Nom d'utilisateur :** `comptable`
- **Mot de passe :** `comptable`
- **Rôle :** Comptable
- **Accès :** 
  - ✅ MODULE 2: Finances & Facturation
  - ❌ MODULE 1: Stocks & Produits (réservé au gestionnaire)
  - ❌ MODULE 3: CRM (réservé au gestionnaire et employé)

---

## 🎯 Règles d'Accès par Rôle

⚠️ **MODE MAQUETTE : Tous les utilisateurs ont accès à TOUS les modules**

| Rôle | MODULE 1 (Stocks) | MODULE 2 (Finances) | MODULE 3 (CRM) |
|------|-------------------|---------------------|----------------|
| **Admin** | ✅ Oui | ✅ Oui | ✅ Oui |
| **Gestionnaire** | ✅ Oui | ✅ Oui | ✅ Oui |
| **Employé** | ✅ Oui | ✅ Oui | ✅ Oui |
| **Comptable** | ✅ Oui | ✅ Oui | ✅ Oui |

> 📝 **Note :** En mode maquette, la gestion des droits d'accès est désactivée. Tous les comptes peuvent accéder à tous les modules. Les restrictions seront implémentées lors du développement du backend.

---

## 🚀 Comment Tester

### Étape 1 : Lancer l'application
```powershell
cd analyse/analyse/PGI
dotnet run
```

### Étape 2 : Page de connexion
1. Entrez un des identifiants ci-dessus
2. Cliquez sur **"Se connecter"**

### Étape 3 : Sélection du module
Après la connexion, vous arrivez sur une page avec **3 boutons** :

```
┌──────────────────┐   ┌──────────────────┐   ┌──────────────────┐
│  MODULE 1        │   │  MODULE 2        │   │  MODULE 3        │
│  📦              │   │  💰              │   │  👥              │
│  Stocks &        │   │  Finances &      │   │  Gestion         │
│  Produits        │   │  Facturation     │   │  Relation Client │
│                  │   │                  │   │                  │
│  Gestionnaire    │   │  Accessible      │   │  Gestionnaire &  │
│  uniquement      │   │  à tous          │   │  Employé         │
└──────────────────┘   └──────────────────┘   └──────────────────┘
```

- Les boutons **grisés** ou **masqués** = Accès refusé pour votre rôle
- Les boutons **visibles et colorés** = Accès autorisé

### Étape 4 : Accéder au module
Cliquez sur le bouton du module désiré → Vous accédez au module complet

---

## 📝 Exemples de Tests

### Test 1 : Gestionnaire (Accès Complet)
```
Identifiant: gestionnaire / gestionnaire
→ Voit les 3 modules
→ Peut accéder à Stocks, Finances et CRM
```

### Test 2 : Employé (Accès Limité)
```
Identifiant: employe / employe
→ Voit uniquement Finances et CRM
→ Le module Stocks est masqué
```

### Test 3 : Comptable (Finances Uniquement)
```
Identifiant: comptable / comptable
→ Voit uniquement le module Finances
→ Les modules Stocks et CRM sont masqués
```

---

## 🔄 Déconnexion

Sur la page de sélection des modules, cliquez sur le bouton **"🔓 Se déconnecter"** en bas de page pour revenir à l'écran de connexion.

---

## ⚠️ Important

- **Ces identifiants sont uniquement pour les maquettes**
- **Aucune base de données n'est utilisée** (données codées en dur)
- **En production, utilisez une vraie base de données avec mots de passe hachés**
- Le bouton "Créer un compte" (inscription) n'est **pas fonctionnel** en mode maquette

---

## 📅 Date de Création
**Version:** Maquette v1.0  
**Date:** Novembre 2025  
**Mode:** Sans base de données (Identifiants en dur)

