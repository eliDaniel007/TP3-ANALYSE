# 🚀 Guide de Démarrage Rapide - PGI Nordik Adventures

## ⚡ Lancement Express

### Étape 1 : Ouvrir le terminal
```powershell
cd analyse/analyse/PGI
dotnet run
```

### Étape 2 : Se connecter
Choisissez un compte :

```
┌─────────────────────────────────────────┐
│  COMPTES DE TEST DISPONIBLES            │
├─────────────────────────────────────────┤
│  👑 admin / admin                       │
│     → Accès COMPLET (3 modules)         │
│                                         │
│  👨‍💼 gestionnaire / gestionnaire          │
│     → Accès COMPLET (3 modules)         │
│                                         │
│  👤 employe / employe                   │
│     → Finances + CRM (2 modules)        │
│                                         │
│  💼 comptable / comptable               │
│     → Finances uniquement (1 module)    │
└─────────────────────────────────────────┘
```

### Étape 3 : Choisir votre module
Après la connexion, 3 boutons s'affichent :

```
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│   MODULE 1   │  │   MODULE 2   │  │   MODULE 3   │
│     📦       │  │     💰       │  │     👥       │
│              │  │              │  │              │
│   Stocks &   │  │  Finances &  │  │  Gestion     │
│   Produits   │  │  Facturation │  │  Relation    │
│              │  │              │  │  Client      │
│              │  │              │  │              │
│ Gestionnaire │  │ Accessible   │  │ Gestionnaire │
│  uniquement  │  │   à tous     │  │ & Employé    │
└──────────────┘  └──────────────┘  └──────────────┘
```

**Cliquez sur un bouton** → Vous entrez dans le module complet !

---

## 🎯 Tests Rapides

### Test A : Gestionnaire (Tout voir)
```
1. Connexion: gestionnaire / gestionnaire
2. Résultat: Les 3 boutons sont disponibles
3. Cliquer sur n'importe quel module
```

### Test B : Employé (Limité)
```
1. Connexion: employe / employe
2. Résultat: Seulement Finances + CRM visibles
3. Le module Stocks est masqué
```

### Test C : Comptable (Finances seul)
```
1. Connexion: comptable / comptable
2. Résultat: Seulement le module Finances visible
3. Stocks et CRM masqués
```

---

## 📊 Architecture du Flux

```
    ┌──────────────────┐
    │  LoginWindow     │ ← Vous êtes ici au démarrage
    │  🔐 Connexion    │
    └────────┬─────────┘
             │
             │ [Identifiant valide]
             ▼
    ┌──────────────────┐
    │ ModuleSelection  │ ← Choisissez votre module
    │ 📦 💰 👥         │
    └────────┬─────────┘
             │
             │ [Clic sur un bouton]
             ▼
    ┌──────────────────┐
    │  MainWindow      │ ← Interface complète du module
    │  Module sélec.   │
    └──────────────────┘
```

---

## 🎨 Raccourcis Clavier

| Touche | Action |
|--------|--------|
| `Enter` | Valider la connexion |
| `Esc` | *(Non implémenté)* |

---

## 🔓 Déconnexion

Sur la page de sélection des modules :
1. Cliquez sur **"🔓 Se déconnecter"** en bas de page
2. Vous revenez à l'écran de connexion

---

## ❌ Dépannage

### Problème : "ModuleSelectionWindow introuvable"
**Solution :** Recompiler le projet
```powershell
dotnet clean
dotnet build
dotnet run
```

### Problème : "Connexion refusée"
**Solution :** Vérifiez les identifiants (voir `IDENTIFIANTS_TEST.md`)

### Problème : "Module masqué"
**Solution :** Normal ! Votre rôle n'a pas accès à ce module.
- Employé → Pas d'accès à Stocks
- Comptable → Pas d'accès à Stocks ni CRM

---

## 📚 Documentation Complète

- 🔐 **Identifiants & Accès :** `IDENTIFIANTS_TEST.md`
- 🎨 **Couleurs & Design :** `PALETTE_COULEURS.md`
- 📝 **Modifications Détaillées :** `RESUME_MODIFICATIONS.md`
- ⚡ **Ce guide :** `GUIDE_DEMARRAGE_RAPIDE.md`

---

## 💡 Astuces

### Pour Tester Tous les Rôles
1. Déconnectez-vous après chaque test
2. Reconnectez-vous avec un autre compte
3. Comparez les modules visibles

### Pour Voir l'Interface Complète
1. Connectez-vous en tant que **gestionnaire**
2. Testez les 3 modules un par un
3. Naviguez entre les onglets de chaque module

---

## 🌟 Fonctionnalités Clés

✅ **Authentification simplifiée** (4 comptes de test)  
✅ **Sélection par boutons** (interface intuitive)  
✅ **Gestion des accès** (selon le rôle)  
✅ **Design moderne** (palette professionnelle)  
✅ **Navigation fluide** (aucune latence)

---

## 📅 Prochaines Étapes (Production)

Pour transformer cette maquette en application réelle :
1. Connecter une vraie base de données MySQL
2. Implémenter le hashage des mots de passe (bcrypt)
3. Ajouter la validation des formulaires
4. Créer des sessions persistantes
5. Développer les fonctionnalités métier de chaque module

---

**Bon test ! 🎉**

*Pour toute question, consultez les autres fichiers de documentation.*

