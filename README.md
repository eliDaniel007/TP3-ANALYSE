# 🏔️ Nordik Adventures ERP

**Système de Gestion Intégrée (PGI/ERP) pour Nordik Adventures**  
Application WPF .NET 8.0 avec base de données MySQL

---

## 📋 Table des Matières

- [À propos](#-à-propos)
- [Fonctionnalités](#-fonctionnalités)
- [Prérequis](#-prérequis)
- [Installation](#-installation)
- [Configuration](#-configuration)
- [Identifiants de Test](#-identifiants-de-test)
- [Structure du Projet](#-structure-du-projet)
- [Technologies Utilisées](#-technologies-utilisées)
- [Captures d'Écran](#-captures-décran)
- [Contribuer](#-contribuer)
- [Licence](#-licence)

---

## 🎯 À propos

**Nordik Adventures ERP** est une application de gestion complète développée en WPF (Windows Presentation Foundation) et C# pour gérer les opérations d'une entreprise d'équipements de plein air.

L'application propose :
- ✅ **Double système d'authentification** (Employés et Clients)
- ✅ **Gestion complète des stocks** (30 produits pré-chargés)
- ✅ **Gestion des catégories et fournisseurs**
- ✅ **Tableau de bord avec KPIs en temps réel**
- ✅ **Recherche, ajout, modification, suppression** de produits
- ✅ **Calculs automatiques** (valeur stock, marges brutes, etc.)
- ✅ **Interface moderne et intuitive**

---

## 🚀 Fonctionnalités

### 🔐 Authentification
- **Employés** : Accès au PGI (système de gestion)
- **Clients** : Accès au site d'achat (en développement)
- Validation par email (clients doivent avoir "client" dans l'email)
- Bouton afficher/cacher le mot de passe

### 📦 Gestion des Stocks
- **Produits** : Liste complète avec recherche, filtrage, CRUD
- **Catégories** : Vêtements, Chaussures, Équipement, Accessoires, Camping
- **Fournisseurs** : Mountain Gear, AventureX, NordicSupply, GlobalOutdoor, Expedition Pro
- **Mouvements** : Historique des entrées/sorties de stock

### 📊 Tableau de Bord
- Nombre de produits actifs
- Nombre de fournisseurs
- Valeur totale du stock
- Marge brute globale
- Graphiques et statistiques

### 🛠️ Modules (Maquettes)
- **Stocks** : Module complet et fonctionnel
- **Finances** : Maquette visuelle
- **CRM** : Maquette visuelle
- **Achats** : Maquette visuelle
- **Ventes** : Maquette visuelle

---

## 📦 Prérequis

Avant d'installer l'application, assurez-vous d'avoir :

| Logiciel | Version | Téléchargement |
|----------|---------|----------------|
| **Windows** | 10/11 | - |
| **.NET SDK** | 8.0+ | [Télécharger](https://dotnet.microsoft.com/download) |
| **Visual Studio** | 2022+ | [Télécharger](https://visualstudio.microsoft.com/) |
| **MySQL Server** | 8.0+ | [Télécharger](https://dev.mysql.com/downloads/mysql/) |
| **MySQL Workbench** | 8.0+ | [Télécharger](https://dev.mysql.com/downloads/workbench/) |

---

## ⚙️ Installation

### 1️⃣ Cloner le Projet

```bash
git clone https://github.com/eliDaniel007/TP3-ANALYSE.git
cd TP3-ANALYSE
```

### 2️⃣ Installer MySQL

1. **Télécharger et installer MySQL Community Server**
   - URL : https://dev.mysql.com/downloads/mysql/
   - Suivre les instructions d'installation
   - Définir un mot de passe **root** (notez-le !)

2. **Télécharger et installer MySQL Workbench**
   - URL : https://dev.mysql.com/downloads/workbench/
   - Lancer MySQL Workbench
   - Se connecter au serveur local (root + votre mot de passe)

### 3️⃣ Créer la Base de Données

**Dans MySQL Workbench, exécuter les 3 scripts SQL dans cet ordre :**

#### Script 1 : Schéma Principal (OBLIGATOIRE)
```sql
-- File > Open SQL Script > NordikAdventuresERP_Schema_FR.sql
-- Puis cliquer sur ⚡ Execute
```
**Résultat attendu :** 20+ tables créées

#### Script 2 : Authentification (OBLIGATOIRE)
```sql
-- File > Open SQL Script > Analyse tp Maquette/analyse/analyse/PGI/SQL_Schema_Auth_Safe.sql
-- Puis cliquer sur ⚡ Execute
```
**Résultat attendu :** 4 employés + 5 clients ajoutés

#### Script 3 : Produits (RECOMMANDÉ)
```sql
-- File > Open SQL Script > Analyse tp Maquette/analyse/analyse/PGI/SQL_Produits_NordikAdventures.sql
-- Puis cliquer sur ⚡ Execute
```
**Résultat attendu :** 30 produits + catégories + fournisseurs ajoutés

### 4️⃣ Ouvrir le Projet dans Visual Studio

1. Ouvrir **Visual Studio 2022**
2. **File > Open > Project/Solution**
3. Sélectionner : `Analyse tp Maquette/analyse/analyse/PGI.sln`
4. Attendre le chargement des packages NuGet

---

## 🔧 Configuration

### Configurer le Mot de Passe MySQL

1. Ouvrir le fichier : **`Helpers/DatabaseHelper.cs`**
2. Ligne 13, modifier :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

3. Remplacer `VOTRE_MOT_DE_PASSE` par votre mot de passe MySQL root
4. Sauvegarder le fichier (Ctrl+S)

### Lancer l'Application

1. Appuyer sur **F5** (ou cliquer sur le bouton ▶️ Debug)
2. L'application se lance
3. Se connecter avec les identifiants de test

---

## 🔑 Identifiants de Test

### 👨‍💼 Employés (Accès PGI)

| Nom | Email | Mot de passe | Rôle |
|-----|-------|--------------|------|
| Admin Tremblay | `admin@nordikadventures.com` | `admin123` | Administrateur |
| Gestionnaire Roy | `gestionnaire@nordikadventures.com` | `gestionnaire123` | Gestionnaire Stocks |
| Employé Bouchard | `employe@nordikadventures.com` | `employe123` | Employé Ventes |
| Comptable Martin | `comptable@nordikadventures.com` | `comptable123` | Comptable |

### 👥 Clients (Accès Site d'Achat)

| Nom | Email | Mot de passe |
|-----|-------|--------------|
| Jean Client | `jean.client@test.com` | `client123` |
| Marie Client | `marie.client@test.com` | `client123` |
| Pierre Client | `pierre.client@entreprise.com` | `client123` |
| Sophie Client | `client.sophie@gmail.com` | `client123` |
| Nordik Client | `contact@nordikclient.com` | `client123` |

---

## 📁 Structure du Projet

```
TP3-ANALYSE/
│
├── 📄 README.md                    # Documentation principale
├── 📄 .gitignore                   # Fichiers à ignorer par Git
├── 📄 reorganiser.bat              # Script de réorganisation
│
├── 📂 docs/                        # 📚 Documentation
│   ├── README.md                   # Index de la documentation
│   ├── NETTOYAGE_EFFECTUE.md       # Récapitulatif du nettoyage
│   ├── PUSH_GITHUB_INSTRUCTIONS.md # Instructions Git/GitHub
│   └── COMMIT_FINAL.txt            # Message de commit détaillé
│
├── 📂 sql_scripts/                 # 🗄️ Scripts SQL
│   ├── README.md                   # Guide d'installation SQL
│   ├── NordikAdventuresERP_Schema_FR.sql      # Schéma complet (20+ tables)
│   ├── SQL_Schema_Auth_Safe.sql    # Authentification (4 employés + 5 clients)
│   └── SQL_Produits_NordikAdventures.sql      # 30 produits + catégories + fournisseurs
│
├── 📂 scripts/                     # 📜 Scripts batch
│   ├── README.md                   # Guide des scripts
│   ├── push_produits.bat           # Push automatique vers GitHub
│   └── push_to_github.bat          # Push alternatif
│
├── 📂 assets/                      # 🎨 Ressources
│   ├── README.md                   # Guide des assets
│   ├── iiiooo.png                  # Logo/Image
│   ├── NordikAdventures - Liste des produits PGI.xlsx  # Liste des produits
│   └── schema 2.0.mwb              # Modèle MySQL Workbench
│
└── 📂 Analyse tp Maquette/         # 💻 Code source
    └── analyse/
        └── analyse/
            ├── 📄 PGI.sln          # Solution Visual Studio
            │
            └── PGI/
                ├── 📂 Helpers/     # Classes utilitaires
                │   └── DatabaseHelper.cs
                │
                ├── 📂 Models/      # Modèles de données
                │   ├── Produit.cs
                │   ├── Categorie.cs
                │   ├── Fournisseur.cs
                │   ├── Client.cs
                │   ├── Employe.cs
                │   └── MouvementStock.cs
                │
                ├── 📂 Services/    # Logique métier
                │   ├── ProduitService.cs
                │   ├── CategorieService.cs
                │   ├── FournisseurService.cs
                │   ├── ClientService.cs
                │   └── EmployeService.cs
                │
                ├── 📂 Views/       # Interfaces utilisateur
                │   ├── Dashboard/
                │   ├── Stocks/     # Module Stocks (complet)
                │   │   ├── ProductsListView.xaml
                │   │   ├── StocksDashboardView.xaml
                │   │   ├── CategoriesView.xaml
                │   │   ├── SuppliersView.xaml
                │   │   └── MovementsHistoryView.xaml
                │   ├── Finances/   # Maquettes
                │   ├── CRM/        # Maquettes
                │   └── Settings/
                │
                ├── 📄 LoginWindow.xaml
                ├── 📄 RegisterWindow.xaml
                ├── 📄 ModuleSelectionWindow.xaml
                ├── 📄 ClientShoppingWindow.xaml
                │
                ├── 📄 GUIDE_DEMARRAGE_RAPIDE.md
                ├── 📄 GUIDE_RAPIDE_SQL.md
                ├── 📄 IDENTIFIANTS_TEST.md
                ├── 📄 INSTALL_BDD_ETAPE_PAR_ETAPE.md
                ├── 📄 README_INSTALLATION_MYSQL.md
                ├── 📄 RESOLUTION_ERREUR_BDD.md
                └── 📄 VALEURS_ENUM.md
```

---

## 🛠️ Technologies Utilisées

### Frontend
- **WPF** (Windows Presentation Foundation)
- **XAML** (Extensible Application Markup Language)
- **C# 12.0** (.NET 8.0)

### Backend
- **C# 12.0**
- **ADO.NET** (accès base de données)
- **MySql.Data** (connecteur MySQL)

### Base de Données
- **MySQL 8.0+**
- **Procédures stockées**
- **Vues SQL**
- **Triggers**
- **Fonctions SQL**

### Outils de Développement
- **Visual Studio 2022**
- **MySQL Workbench 8.0**
- **Git / GitHub**

---

## 📸 Captures d'Écran

### 🔐 Fenêtre de Connexion
Interface de connexion avec validation d'email et bouton afficher/cacher le mot de passe.

### 📊 Tableau de Bord Stocks
Vue d'ensemble avec KPIs : produits actifs, fournisseurs, valeur stock, marge brute.

### 📦 Liste des Produits
DataGrid avec 30 produits, recherche, filtrage, et actions (modifier, supprimer).

### 🏷️ Gestion des Catégories
Interface de gestion des 5 catégories de produits.

### 🏢 Gestion des Fournisseurs
Liste des fournisseurs avec coordonnées et délais de livraison.

---

## 📚 Documentation Complète

La documentation est organisée dans des dossiers dédiés :

### 📂 Documentation Générale ([docs/](docs/))
| Document | Description |
|----------|-------------|
| [docs/README.md](docs/README.md) | Index de la documentation |
| [docs/NETTOYAGE_EFFECTUE.md](docs/NETTOYAGE_EFFECTUE.md) | Récapitulatif du nettoyage (33 fichiers supprimés) |
| [docs/PUSH_GITHUB_INSTRUCTIONS.md](docs/PUSH_GITHUB_INSTRUCTIONS.md) | Instructions Git/GitHub |

### 🗄️ Scripts SQL ([sql_scripts/](sql_scripts/))
| Document | Description |
|----------|-------------|
| [sql_scripts/README.md](sql_scripts/README.md) | Guide d'installation SQL complet |
| [sql_scripts/NordikAdventuresERP_Schema_FR.sql](sql_scripts/NordikAdventuresERP_Schema_FR.sql) | Schéma MySQL (20+ tables) |
| [sql_scripts/SQL_Schema_Auth_Safe.sql](sql_scripts/SQL_Schema_Auth_Safe.sql) | Authentification |
| [sql_scripts/SQL_Produits_NordikAdventures.sql](sql_scripts/SQL_Produits_NordikAdventures.sql) | 30 produits |

### 📜 Scripts ([scripts/](scripts/))
| Document | Description |
|----------|-------------|
| [scripts/README.md](scripts/README.md) | Guide des scripts batch |
| [scripts/push_produits.bat](scripts/push_produits.bat) | Push automatique vers GitHub |
| [scripts/push_to_github.bat](scripts/push_to_github.bat) | Push alternatif |

### 🎨 Assets ([assets/](assets/))
| Document | Description |
|----------|-------------|
| [assets/README.md](assets/README.md) | Guide des ressources |
| [assets/schema 2.0.mwb](assets/schema%202.0.mwb) | Modèle MySQL Workbench |
| [assets/NordikAdventures - Liste des produits PGI.xlsx](assets/NordikAdventures%20-%20Liste%20des%20produits%20PGI.xlsx) | Liste Excel des produits |

### 📖 Guides d'Installation (dans PGI/)
| Document | Description |
|----------|-------------|
| **GUIDE_DEMARRAGE_RAPIDE.md** | Guide pour démarrer rapidement |
| **GUIDE_RAPIDE_SQL.md** | Installation et configuration MySQL |
| **IDENTIFIANTS_TEST.md** | Liste complète des identifiants |
| **INSTALL_BDD_ETAPE_PAR_ETAPE.md** | Installation MySQL détaillée |
| **README_INSTALLATION_MYSQL.md** | Instructions complètes pour MySQL |
| **RESOLUTION_ERREUR_BDD.md** | Dépannage des erreurs courantes |
| **VALEURS_ENUM.md** | Valeurs ENUM de la base de données |

---

## 🤝 Contribuer

### Workflow Git

1. **Fork** le projet
2. Créer une branche : `git checkout -b feature/ma-fonctionnalite`
3. Commit : `git commit -m "Ajout de ma fonctionnalité"`
4. Push : `git push origin feature/ma-fonctionnalite`
5. Ouvrir une **Pull Request**

### Standards de Code

- **Indentation** : 4 espaces
- **Langue** : Commentaires en français, noms de variables en anglais
- **Conventions C#** : PascalCase pour classes/méthodes, camelCase pour variables
- **XAML** : PascalCase pour `x:Name`

---

## 🆘 Problèmes Courants

### ❌ "Access denied for user 'root'@'localhost'"
**Solution :** Vérifier le mot de passe dans `DatabaseHelper.cs`

### ❌ "Unknown database 'NordikAdventuresERP'"
**Solution :** Exécuter le script `NordikAdventuresERP_Schema_FR.sql`

### ❌ "Column 'categorie_id' does not belong to table"
**Solution :** Le schéma est incomplet. Supprimer et recréer la BDD :
```sql
DROP DATABASE IF EXISTS NordikAdventuresERP;
```
Puis réexécuter les 3 scripts SQL.

### ❌ L'application affiche 3 produits (données d'exemple)
**Solutions :**
1. Vérifier le mot de passe MySQL dans `DatabaseHelper.cs`
2. Vérifier que les produits existent : `SELECT COUNT(*) FROM produits;` (doit retourner 30)
3. Vérifier que MySQL tourne sur le port 3306

---

## 📝 Notes Importantes

- ⚠️ **Les mots de passe ne sont PAS hashés** (pour simplifier les tests)
- ⚠️ **Utiliser uniquement pour le développement/apprentissage**
- ⚠️ **Ne PAS déployer en production sans sécuriser les mots de passe**
- ✅ **Fallback sur données d'exemple si MySQL n'est pas disponible**

---

## 📄 Licence

Ce projet est développé dans un cadre académique.  
Libre d'utilisation pour l'apprentissage et les tests.

---

## 👤 Auteur

**eliDaniel007**  
GitHub : [github.com/eliDaniel007](https://github.com/eliDaniel007)

---

## 🎯 Prochaines Étapes

- [ ] Implémenter le module CRM complet
- [ ] Implémenter le module Finances complet
- [ ] Ajouter le panier d'achat pour les clients
- [ ] Sécuriser les mots de passe (hashing bcrypt)
- [ ] Ajouter des rapports PDF
- [ ] Intégration d'une API REST
- [ ] Migration vers .NET MAUI (multiplateforme)

---

**🚀 Bon développement avec Nordik Adventures ERP !**

Pour toute question, ouvrir une **issue** sur GitHub.
