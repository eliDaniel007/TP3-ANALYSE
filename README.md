# NordikAdventuresERP - PGI (Progiciel de Gestion Intégré)

## 📋 Description

Système de gestion intégré (PGI/ERP) développé en **WPF (.NET)** avec une base de données **MySQL 8.0+**.

**Modules disponibles :**
- ✅ **Module Stocks & Produits** : Gestion de l'inventaire, produits, fournisseurs, catégories
- ✅ **Module Finances & Facturation** : Ventes, achats, factures, paiements, comptabilité
- ✅ **Module CRM** : Gestion des clients, interactions, campagnes marketing

---

## 🗄️ Base de Données

### Schéma MySQL
- **Fichier principal** : `NordikAdventuresERP_Schema_FR.sql`
- **22 tables** organisées en 3 modules
- **34 Foreign Keys** pour l'intégrité référentielle
- **Vues, Triggers, Fonctions et Procédures stockées** inclus

### Structure simplifiée
- Tables `departements`, `taxes`, `emplacements_stock` supprimées (intégrées comme ENUM ou colonnes)
- Table `lignes_reception` supprimée (intégrée dans `receptions_marchandises`)
- Optimisation pour réduire la complexité du schéma

---

## 🚀 Installation

### Prérequis
- **.NET 8.0 SDK** ou supérieur
- **MySQL 8.0+** ou **MySQL Workbench**
- **Visual Studio 2022** ou **JetBrains Rider** (recommandé)

### Étapes

1. **Cloner le dépôt**
   ```bash
   git clone https://github.com/eliDaniel007/TP3-ANALYSE.git
   cd TP3-ANALYSE
   ```

2. **Créer la base de données MySQL**
   ```bash
   mysql -u root -p < NordikAdventuresERP_Schema_FR.sql
   ```

3. **Compiler l'application WPF**
   ```bash
   cd "Analyse tp Maquette/analyse/analyse"
   dotnet restore
   dotnet build
   ```

4. **Lancer l'application**
   ```bash
   dotnet run --project PGI/PGI.csproj
   ```

---

## 📁 Structure du Projet

```
TP3-ANALYSE/
├── NordikAdventuresERP_Schema_FR.sql    # Schéma MySQL complet
├── schema 2.0.mwb                       # Modèle MySQL Workbench
├── README.md                            # Ce fichier
│
└── Analyse tp Maquette/
    └── analyse/
        └── analyse/
            ├── PGI.sln                  # Solution Visual Studio
            └── PGI/
                ├── Views/
                │   ├── Stocks/          # Module Stocks
                │   ├── Finances/        # Module Finances
                │   └── CRM/             # Module CRM
                ├── MainWindow.xaml      # Fenêtre principale
                ├── LoginWindow.xaml      # Authentification
                └── ModuleSelectionWindow.xaml
```

---

## 📚 Documentation

### Fichiers de documentation disponibles :
- `LISTE_RELATIONS_FK.md` : Liste complète des 34 Foreign Keys
- `STRUCTURE_FINALE.md` : Vue d'ensemble du schéma final
- `PRIORITE_TABLES.md` : Priorisation des tables pour développement
- `ENUMS_REFERENCE.md` : Liste de tous les ENUMs utilisés
- `SIMPLIFICATIONS_FINALES.md` : Historique des simplifications

---

## 🎯 Fonctionnalités Principales

### Module Stocks
- ✅ Gestion des produits (CRUD)
- ✅ Gestion des fournisseurs
- ✅ Gestion des catégories
- ✅ Suivi des mouvements de stock
- ✅ Tableau de bord avec KPIs
- ✅ Calcul de la valorisation de l'inventaire

### Module Finances
- ✅ Gestion des ventes et factures
- ✅ Gestion des achats fournisseurs
- ✅ Enregistrement des paiements
- ✅ Remboursements
- ✅ Journal comptable
- ✅ Rapports (TPS/TVQ, ventes)
- ✅ Paramètres fiscaux (TPS 5%, TVQ 9.975%)

### Module CRM
- ✅ Gestion des clients
- ✅ Historique des interactions
- ✅ Campagnes marketing
- ✅ Scores de fidélisation
- ✅ Alertes CRM

---

## 🔧 Technologies Utilisées

- **Frontend** : WPF (.NET 8.0), XAML, C#
- **Backend** : MySQL 8.0+
- **Architecture** : MVVM (Model-View-ViewModel)
- **IDE** : Visual Studio 2022 / JetBrains Rider

---

## 👤 Auteur

**eliDaniel007** - TP3 Analyse - INF27523

---

## 📝 Licence

Ce projet est développé dans le cadre d'un travail académique.

---

## 🔗 Liens Utiles

- **Repository GitHub** : https://github.com/eliDaniel007/TP3-ANALYSE
- **MySQL Documentation** : https://dev.mysql.com/doc/
- **WPF Documentation** : https://docs.microsoft.com/en-us/dotnet/desktop/wpf/

---

**Dernière mise à jour** : Janvier 2025

