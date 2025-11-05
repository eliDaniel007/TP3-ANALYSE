# 🔐 PGI Nordik Adventures - Authentification Complète

## 🎯 Dernière Mise à Jour

**✅ Authentification avec MySQL implémentée !**

L'application dispose maintenant d'un système d'authentification complet avec :
- Connexion employés (accès PGI)
- Connexion clients (accès site d'achat)
- Inscription clients
- Base de données MySQL

---

## 🚀 Installation Rapide

### 1️⃣ Prérequis
- Visual Studio 2022
- .NET 8.0
- MySQL 8.0+
- MySQL Workbench (optionnel)

### 2️⃣ Configuration MySQL

#### A. Créer la base de données
```bash
mysql -u root -p < NordikAdventuresERP_Schema_FR.sql
```

#### B. Ajouter l'authentification
```bash
mysql -u root -p NordikAdventuresERP < SQL_Schema_Auth_Safe.sql
```

#### C. Configurer le mot de passe
Le mot de passe MySQL est déjà configuré dans `DatabaseHelper.cs` : **`password`**

Si votre mot de passe est différent, modifiez la ligne 13 de `Helpers/DatabaseHelper.cs`.

### 3️⃣ Lancer l'application
```
1. Ouvrir la solution dans Visual Studio
2. Appuyer sur F5 (Start Debugging)
3. Tester la connexion !
```

---

## 🔑 Identifiants de Test

### 👨‍💼 Employés (Accès PGI)

```
Admin:
Email: admin@nordikadventures.com
Mot de passe: admin123

Gestionnaire:
Email: gestionnaire@nordikadventures.com
Mot de passe: gestionnaire123

Employé Ventes:
Email: employe@nordikadventures.com
Mot de passe: employe123

Comptable:
Email: comptable@nordikadventures.com
Mot de passe: comptable123
```

### 👤 Clients (Accès Site d'achat)

```
Jean Dupont:
Email: jean.client@test.com
Mot de passe: client123

Marie Martin:
Email: marie.client@test.com
Mot de passe: client123

Pierre Tremblay:
Email: pierre.client@entreprise.com
Mot de passe: client123

Sophie Lavoie:
Email: client.sophie@gmail.com
Mot de passe: client123

Nordik Sports Inc.:
Email: contact@nordikclient.com
Mot de passe: client123
```

---

## 📋 Fonctionnalités

### ✅ Authentification
- [x] Connexion employés avec email
- [x] Connexion clients avec email
- [x] Inscription clients (email doit contenir "client")
- [x] Bouton afficher/cacher mot de passe (👁️)
- [x] Validation des emails
- [x] Gestion des erreurs

### ✅ Redirection Intelligente
- [x] Employés → ModuleSelectionWindow (PGI)
- [x] Clients → ClientShoppingWindow (Site d'achat)
- [x] Détection automatique via email

### ✅ Base de Données
- [x] Connexion MySQL opérationnelle
- [x] Tables `employes` avec colonne `mot_de_passe`
- [x] Tables `clients` avec colonne `mot_de_passe`
- [x] 4 employés de test
- [x] 5 clients de test

### ✅ Module Stocks (En cours)
- [x] Dashboard avec KPIs réels
- [x] Liste produits avec BDD
- [x] Gestion catégories avec BDD
- [x] Gestion fournisseurs avec BDD
- [ ] Formulaire produit (à compléter)
- [ ] Mouvements de stock (à compléter)

---

## 🏗️ Architecture

```
PGI/
├── Helpers/
│   └── DatabaseHelper.cs (Connexion MySQL)
├── Models/
│   ├── Employe.cs
│   ├── Client.cs
│   ├── Produit.cs
│   ├── Categorie.cs
│   ├── Fournisseur.cs
│   └── MouvementStock.cs
├── Services/
│   ├── EmployeService.cs (Authentification employés)
│   ├── ClientService.cs (Authentification + inscription clients)
│   ├── ProduitService.cs
│   ├── CategorieService.cs
│   └── FournisseurService.cs
├── Views/
│   ├── LoginWindow.xaml (Connexion)
│   ├── RegisterWindow.xaml (Inscription)
│   ├── ModuleSelectionWindow.xaml (Choix module PGI)
│   ├── ClientShoppingWindow.xaml (Site d'achat)
│   └── Stocks/
│       ├── StocksDashboardView.xaml
│       ├── ProductsListView.xaml
│       ├── CategoriesView.xaml
│       └── SuppliersView.xaml
└── SQL/
    ├── NordikAdventuresERP_Schema_FR.sql (Schéma complet)
    ├── SQL_Schema_Auth.sql (Script authentification)
    └── SQL_Schema_Auth_Safe.sql (Script safe)
```

---

## 📚 Documentation

- **AUTHENTIFICATION.md** - Guide complet de l'authentification
- **INSTRUCTIONS_BDD.md** - Installation MySQL pas à pas
- **VALEURS_ENUM.md** - Référence des valeurs ENUM
- **GUIDE_RAPIDE_SQL.md** - Guide SQL rapide
- **RECAPITULATIF_AUTHENTIFICATION.md** - Résumé complet
- **DEVELOPPEMENT_PROGRES.md** - Suivi du développement

---

## 🧪 Tests

### Test 1 : Connexion Employé
1. Lancer l'application
2. Email : `admin@nordikadventures.com`
3. Mot de passe : `admin123`
4. ✅ Redirection vers ModuleSelectionWindow

### Test 2 : Connexion Client
1. Lancer l'application
2. Email : `jean.client@test.com`
3. Mot de passe : `client123`
4. ✅ Redirection vers ClientShoppingWindow

### Test 3 : Inscription Client
1. Cliquer sur "S'inscrire"
2. Remplir le formulaire avec un email contenant "client"
3. ✅ Enregistrement dans la BDD
4. ✅ Redirection vers LoginWindow

### Test 4 : Validation Email
1. Cliquer sur "S'inscrire"
2. Entrer un email SANS "client"
3. ✅ Message d'erreur affiché

---

## 🐛 Dépannage

### Erreur : "Access denied for user 'root'@'localhost'"
**Solution** : Modifier le mot de passe dans `DatabaseHelper.cs` ligne 13

### Erreur : "Unknown database 'NordikAdventuresERP'"
**Solution** : Exécuter `NordikAdventuresERP_Schema_FR.sql` d'abord

### Erreur : "Unknown column 'mot_de_passe'"
**Solution** : Exécuter `SQL_Schema_Auth_Safe.sql`

### Erreur : "Data truncated for column 'departement'"
**Solution** : Les scripts SQL ont été corrigés avec les bonnes valeurs ENUM

---

## 🔄 Prochaines Étapes

1. ✅ ~~Authentification MySQL~~ (Terminé)
2. 🔄 Compléter le Module Stocks
   - Formulaire produit (4 onglets)
   - Mouvements de stock
   - Alertes de réapprovisionnement
3. ⏳ Développer le Module Finances
4. ⏳ Développer le Module CRM
5. ⏳ Développer le Site d'achat Clients

---

## 👥 Contributeurs

- **Développement** : IA Assistant + Équipe de développement
- **Base de données** : MySQL 8.0
- **Framework** : .NET 8.0 WPF
- **Projet** : TP#2 - INF27523

---

## 📄 Licence

Projet éducatif - Tous droits réservés

---

**Développement en cours... 🚧**

Dernière mise à jour : Janvier 2025

