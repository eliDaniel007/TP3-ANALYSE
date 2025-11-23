# 🏔️ NordikAdventures ERP

> Système de gestion intégré (ERP) complet pour NordikAdventures - Spécialiste en équipement de plein air

![Version](https://img.shields.io/badge/version-2.0-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![MySQL](https://img.shields.io/badge/MySQL-8.0+-orange)
![WPF](https://img.shields.io/badge/WPF-Windows-blue)
![Status](https://img.shields.io/badge/status-Production%20Ready-green)

---

## 📋 Table des matières

- [À propos](#à-propos)
- [Fonctionnalités](#fonctionnalités)
- [Technologies](#technologies)
- [Installation rapide](#installation-rapide)
- [Modules](#modules)
- [Documentation](#documentation)
- [Architecture](#architecture)
- [Captures d'écran](#captures-décran)
- [Contribution](#contribution)

---

## 🎯 À propos

**NordikAdventures ERP** est un système de gestion d'entreprise complet développé en C# WPF avec MySQL, conçu spécifiquement pour une entreprise de vente d'équipement de plein air.

### ✨ Points forts

- 🔄 **Gestion complète des stocks** avec suivi des mouvements
- 💰 **Module financier** avec facturation automatisée
- 👥 **CRM avancé** avec scoring et automatisations
- 📊 **Tableaux de bord** avec KPIs en temps réel
- 🔒 **Authentification** multi-rôles (Admin, Employé, Client)
- ⚡ **Automatisations** via triggers SQL
- 🎨 **Interface moderne** en WPF avec design épuré

---

## 🚀 Fonctionnalités

### Module Stocks & Inventaire
- ✅ Gestion des produits, catégories et fournisseurs
- ✅ Suivi des niveaux de stock en temps réel
- ✅ Historique complet des mouvements (entrées/sorties)
- ✅ Alertes automatiques de réapprovisionnement
- ✅ Calcul automatique des marges bénéficiaires

### Module Finances & Facturation
- ✅ Création de factures avec taxes (TPS/TVQ)
- ✅ Gestion des paiements multiples
- ✅ Commandes fournisseurs avec réception
- ✅ Mise à jour automatique du stock
- ✅ Journal comptable automatisé
- ✅ Rapports financiers (ventes, profits, top produits)

### Module CRM (Gestion Clients)
- ✅ Fiche client complète avec historique
- ✅ **Scoring automatique** des clients
- ✅ **Changement automatique de statut** (Prospect → Actif → Fidèle)
- ✅ Évaluations de satisfaction (1-5 étoiles)
- ✅ **Alertes automatiques** si satisfaction faible
- ✅ Campagnes marketing avec suivi
- ✅ Statistiques et KPIs par client
- ✅ Interactions traçables

### Automatisations CRM 🤖
- **Prospect → Actif** après 1ère commande
- **Actif → Fidèle** après >5 commandes ou >3000$ CA
- **Alerte automatique** si note ≤ 2/5
- **Interaction automatique** lors de chaque vente
- **Email de bienvenue** pour nouveaux clients
- **Détection d'inactivité** (12 mois sans achat)

### Module Dashboard
- ✅ Vue d'ensemble des KPIs
- ✅ Statistiques de ventes
- ✅ État des stocks critiques
- ✅ Alertes et notifications

### Module Shopping (Client)
- ✅ Catalogue de produits avec recherche
- ✅ Panier d'achat
- ✅ Historique des commandes
- ✅ Profil client

---

## 🛠️ Technologies

### Backend
- **C# .NET 8.0** - Framework principal
- **ADO.NET** - Accès aux données direct
- **MySQL 8.0+** - Base de données
- **MySql.Data** - Connecteur MySQL

### Frontend
- **WPF (Windows Presentation Foundation)** - Interface graphique
- **XAML** - Langage de balisage
- **MVVM Pattern** - Architecture

### Base de données
- **MySQL 8.0+**
- **Triggers** - Automatisations
- **Stored Procedures** - Logique métier
- **Views** - Calculs en temps réel
- **Functions** - Validation et règles

---

## ⚡ Installation rapide

### Prérequis
- Windows 10/11
- .NET 8.0 SDK
- MySQL 8.0+ (ou MariaDB 10.5+)
- Visual Studio 2022 (recommandé) ou VS Code

### Étape 1 : Cloner le projet

```bash
git clone https://github.com/votre-utilisateur/nordikadventures-erp.git
cd nordikadventures-erp
```

### Étape 2 : Installer la base de données

**Option A - Ligne de commande :**
```bash
mysql -u root -p < sql_scripts/SQL_COMPLET_NordikAdventuresERP.sql
```

**Option B - MySQL Workbench :**
1. Ouvrir MySQL Workbench
2. File → Open SQL Script
3. Sélectionner `sql_scripts/SQL_COMPLET_NordikAdventuresERP.sql`
4. Exécuter (⚡)

### Étape 3 : Configurer la connexion

Modifier `Helpers/DatabaseHelper.cs` :

```csharp
private static string connectionString = 
    "Server=localhost;Database=NordikAdventuresERP;User ID=root;Password=VOTRE_MOT_DE_PASSE;";
```

### Étape 4 : Compiler et lancer

```bash
cd "Analyse tp/analyse/PGI"
dotnet restore
dotnet build
dotnet run
```

**OU** ouvrir `Analyse tp/analyse/PGI.sln` dans Visual Studio et appuyer sur F5.

---

## 📦 Modules

| Module | Description | Statut |
|--------|-------------|--------|
| **Stocks** | Gestion inventaire, produits, fournisseurs | ✅ Complet |
| **Finances** | Facturation, paiements, commandes | ✅ Complet |
| **CRM** | Gestion clients, scoring, automatisations | ✅ Complet |
| **Dashboard** | Tableaux de bord et KPIs | ✅ Complet |
| **Shopping** | Interface client (catalogue, panier) | ✅ Complet |
| **RH** | Employés et paies | 🔄 Données initiales |

---

## 📚 Documentation

### Guides d'installation
- 📄 [`sql_scripts/README_INSTALLATION.md`](sql_scripts/README_INSTALLATION.md) - Installation complète BDD
- 📄 [`Analyse tp/analyse/PGI/GUIDE_DEMARRAGE_RAPIDE.md`](Analyse%20tp/analyse/PGI/GUIDE_DEMARRAGE_RAPIDE.md) - Démarrage rapide
- 📄 [`CONFIGURATION_SQL_MYSQL.md`](CONFIGURATION_SQL_MYSQL.md) - Configuration MySQL
- 📄 [`Analyse tp/analyse/PGI/README_INSTALLATION_MYSQL.md`](Analyse%20tp/analyse/PGI/README_INSTALLATION_MYSQL.md) - Installation MySQL

### Documentation modules
- 📄 [`MODULE_CRM_DOCUMENTATION.md`](MODULE_CRM_DOCUMENTATION.md) - Documentation complète CRM
- 📄 [`MODULE_FINANCES_DOCUMENTATION.md`](MODULE_FINANCES_DOCUMENTATION.md) - Documentation complète Finances
- 📄 [`GUIDE_RAPIDE_FINANCES.md`](GUIDE_RAPIDE_FINANCES.md) - Guide rapide Finances

### Référence technique
- 📄 [`IDENTIFIANTS_TEST.md`](IDENTIFIANTS_TEST.md) - Comptes de test
- 📄 [`Analyse tp/analyse/PGI/VALEURS_ENUM.md`](Analyse%20tp/analyse/PGI/VALEURS_ENUM.md) - Valeurs ENUM SQL

---

## 🏗️ Architecture

### Structure du projet

```
NordikAdventures ERP/
│
├── 📁 Analyse tp/analyse/PGI/          # Application C# WPF
│   ├── Helpers/                        # DatabaseHelper
│   ├── Models/                         # 18 modèles de données
│   ├── Services/                       # 16 services métier
│   ├── Views/                          # Vues XAML
│   │   ├── CRM/                        # Module CRM (14 fichiers)
│   │   ├── Finances/                   # Module Finances (30 fichiers)
│   │   ├── Stocks/                     # Module Stocks (20 fichiers)
│   │   ├── Dashboard/                  # Tableau de bord
│   │   ├── Shopping/                   # Interface client
│   │   └── Settings/                   # Paramètres
│   ├── App.xaml                        # Application principale
│   ├── MainWindow.xaml                 # Fenêtre principale
│   └── PGI.csproj                      # Fichier projet
│
├── 📁 sql_scripts/                     # Scripts SQL
│   ├── SQL_COMPLET_NordikAdventuresERP.sql  ⭐ FICHIER UNIQUE
│   └── README_INSTALLATION.md
│
├── 📁 docs/                            # Documentation projet
├── 📁 assets/                          # Ressources (images, schémas)
├── 📁 scripts/                         # Scripts utilitaires
└── README.md                           # Ce fichier
```

### Base de données (22 tables)

**Module RH :** `employes`, `paies`  
**Module Stocks :** `categories`, `fournisseurs`, `produits`, `niveaux_stock`, `mouvements_stock`  
**Module Clients :** `clients`  
**Module Finances :** `parametres_taxes`, `factures`, `lignes_factures`, `paiements`, `commandes_fournisseurs`, `lignes_commandes_fournisseurs`  
**Module CRM :** `interactions_clients`, `evaluations_clients`, `campagnes_marketing`, `alertes_service_client`

**+ 1 Vue :** `vue_statistiques_clients` (KPIs calculés)

---

## 🖼️ Captures d'écran

### Authentification
Connexion sécurisée avec 3 types de comptes : Admin, Employé, Client

### Dashboard
Vue d'ensemble avec KPIs en temps réel et graphiques

### Module Stocks
Gestion complète des produits, catégories, fournisseurs et mouvements

### Module Finances
Facturation, paiements, commandes fournisseurs, journal comptable

### Module CRM
Fiche client complète, scoring, interactions, alertes automatiques

### Shopping Client
Catalogue de produits avec panier et historique de commandes

---

## 🔐 Identifiants de test

### Administrateur
- **Email :** `admin@nordikadventures.com`
- **Mot de passe :** `Admin123`

### Employé
- **Email :** `employe@nordikadventures.com`
- **Mot de passe :** `Employe123`

### Client
- **Email :** `jean.tremblay@client.com`
- **Mot de passe :** `Client123`

📄 Voir [`IDENTIFIANTS_TEST.md`](IDENTIFIANTS_TEST.md) pour la liste complète

---

## 🎯 Cas d'utilisation

### Pour un commerce de plein air
- Gestion de l'inventaire (vêtements, équipements, accessoires)
- Facturation avec taxes canadiennes (TPS/TVQ)
- Fidélisation automatique des clients
- Suivi des commandes fournisseurs
- Rapports financiers

### Pour un projet académique
- Démontre l'architecture 3-tiers
- Utilisation de triggers et procédures stockées
- Automatisations métier
- Interface utilisateur professionnelle
- Documentation complète

---

## 🤝 Contribution

Ce projet est un système ERP complet et fonctionnel. Les contributions sont les bienvenues !

### Comment contribuer
1. Fork le projet
2. Créer une branche (`git checkout -b feature/AmazingFeature`)
3. Commit les changements (`git commit -m 'Add AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une Pull Request

---

## 📄 Licence

Ce projet est développé dans un cadre académique.

---

## 👨‍💻 Auteur

**Projet académique** - INF27523  
**Institution :** [Votre institution]  
**Date :** Janvier 2025

---

## 🙏 Remerciements

- MySQL pour la base de données robuste
- Microsoft pour .NET et WPF
- La communauté open source

---

## 📞 Support

Pour toute question ou problème :
1. Consulter la [documentation](#documentation)
2. Vérifier les [identifiants de test](IDENTIFIANTS_TEST.md)
3. Lire le [guide d'installation](sql_scripts/README_INSTALLATION.md)

---

## 🚀 Roadmap future (optionnel)

- [ ] Export PDF des factures
- [ ] Notifications push
- [ ] API REST pour mobile
- [ ] Dashboard analytique avancé
- [ ] Gestion multi-devises
- [ ] Intégration paiement en ligne

---

**⭐ Si ce projet vous a été utile, n'hésitez pas à laisser une étoile !**

---

<p align="center">
  Made with ❤️ for NordikAdventures
</p>
