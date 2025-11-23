# 🧹 PLAN DE NETTOYAGE ET RÉORGANISATION

## 📋 FICHIERS À SUPPRIMER

### ❌ Racine du projet (fichiers obsolètes)
- `MIGRATION_EF_CORE.md` - On n'utilise pas Entity Framework Core
- `NETTOYAGE_COMPLET.md` - Ancien fichier de nettoyage
- `STRUCTURE_FINALE.md` - Obsolète
- `INTEGRATION_COMPLETE_FINANCES.md` - Doublon avec MODULE_FINANCES_DOCUMENTATION.md
- `INTEGRATION_MODULE_FINANCES.md` - Doublon
- `reorganiser.bat` - Script temporaire
- `tatus --untracked-files=all` - Fichier d'erreur

### ❌ sql_scripts/ (fichiers SQL individuels - remplacés par SQL_COMPLET)
- `NordikAdventuresERP_Schema_FR.sql` - Inclus dans SQL_COMPLET
- `SQL_Module_CRM.sql` - Inclus dans SQL_COMPLET
- `SQL_Module_Finances.sql` - Inclus dans SQL_COMPLET
- `SQL_Schema_Auth_Safe.sql` - Inclus dans SQL_COMPLET
- `SQL_Produits_NordikAdventures.sql` - Obsolète (données d'exemple)
- `README.md` - Doublon avec README_INSTALLATION.md

### ❌ Analyse tp/analyse/PGI/ (anciennes fenêtres obsolètes)
- `AchatsFournisseursWindow.xaml` + `.cs` - Remplacé par module Finances
- `EtatFinancierWindow.xaml` + `.cs` - Remplacé par module Finances
- `FournisseursWindow.xaml` + `.cs` - Remplacé par module Stocks
- `GraphiquesWindow.xaml` + `.cs` - Remplacé par Dashboard
- `HistoriqueMouvementsWindow.xaml` + `.cs` - Remplacé par module Stocks
- `InteractionsWindow.xaml` + `.cs` - Remplacé par module CRM
- `JournalComptableWindow.xaml` + `.cs` - Remplacé par module Finances
- `RapportVentesWindow.xaml` + `.cs` - Remplacé par module Finances

### ❌ Analyse tp/analyse/PGI/ (fichiers temporaires/debug)
- `RESOLUTION_ERREUR_BDD.md` - Fichier de debug temporaire
- `DeleteDatabase.bat` - Script dangereux
- `INSTALL_BDD_ETAPE_PAR_ETAPE.md` - Remplacé par sql_scripts/README_INSTALLATION.md
- `GUIDE_RAPIDE_SQL.md` - Doublon avec CONFIGURATION_SQL_MYSQL.md
- `Data/` - Dossier vide (était pour EF Core)

---

## ✅ FICHIERS À CONSERVER

### 📄 Documentation principale (racine)
- `README.md` - Documentation principale du projet
- `MODULE_CRM_DOCUMENTATION.md` - Documentation du module CRM
- `MODULE_FINANCES_DOCUMENTATION.md` - Documentation du module Finances
- `GUIDE_RAPIDE_FINANCES.md` - Guide rapide Finances
- `CONFIGURATION_SQL_MYSQL.md` - Configuration MySQL
- `IDENTIFIANTS_TEST.md` - Identifiants de test

### 📁 Documentation organisée (docs/)
- `docs/COMMIT_FINAL.txt`
- `docs/NETTOYAGE_EFFECTUE.md`
- `docs/PUSH_GITHUB_INSTRUCTIONS.md`
- `docs/README.md`

### 🗄️ Scripts SQL (sql_scripts/)
- `SQL_COMPLET_NordikAdventuresERP.sql` ⭐ - LE SEUL fichier SQL nécessaire
- `README_INSTALLATION.md` - Guide d'installation complet

### 🏗️ Code source (Analyse tp/analyse/PGI/)
- `App.xaml` + `.cs`
- `AssemblyInfo.cs`
- `PGI.csproj`
- `LoginWindow.xaml` + `.cs`
- `RegisterWindow.xaml` + `.cs`
- `MainWindow.xaml` + `.cs`
- `ModuleSelectionWindow.xaml` + `.cs`
- `ClientShoppingWindow.xaml` + `.cs`
- `README_INSTALLATION_MYSQL.md`
- `GUIDE_DEMARRAGE_RAPIDE.md`
- `IDENTIFIANTS_TEST.md`
- `VALEURS_ENUM.md`

### 📂 Dossiers à conserver
- `Helpers/` (DatabaseHelper.cs)
- `Models/` (tous les modèles)
- `Services/` (tous les services)
- `Views/` (CRM, Dashboard, Finances, Settings, Shopping, Stocks)
- `bin/` (fichiers compilés)
- `obj/` (fichiers temporaires de build)

---

## 📁 RÉORGANISATION PROPOSÉE

```
NordikAdventures ERP/
│
├── 📄 README.md (Principal)
├── 📄 CONFIGURATION_SQL_MYSQL.md
├── 📄 IDENTIFIANTS_TEST.md
│
├── 📁 Analyse tp/
│   └── analyse/
│       ├── PGI.sln
│       └── PGI/
│           ├── App.xaml + .cs
│           ├── MainWindow.xaml + .cs
│           ├── ModuleSelectionWindow.xaml + .cs
│           ├── LoginWindow.xaml + .cs
│           ├── RegisterWindow.xaml + .cs
│           ├── ClientShoppingWindow.xaml + .cs
│           ├── PGI.csproj
│           ├── 📄 GUIDE_DEMARRAGE_RAPIDE.md
│           ├── 📄 IDENTIFIANTS_TEST.md
│           ├── 📄 VALEURS_ENUM.md
│           ├── 📁 Helpers/
│           ├── 📁 Models/
│           ├── 📁 Services/
│           └── 📁 Views/
│               ├── CRM/
│               ├── Dashboard/
│               ├── Finances/
│               ├── Settings/
│               ├── Shopping/
│               └── Stocks/
│
├── 📁 sql_scripts/
│   ├── ⭐ SQL_COMPLET_NordikAdventuresERP.sql
│   └── 📄 README_INSTALLATION.md
│
├── 📁 docs/
│   ├── 📄 MODULE_CRM_DOCUMENTATION.md (DÉPLACER ICI)
│   ├── 📄 MODULE_FINANCES_DOCUMENTATION.md (DÉPLACER ICI)
│   ├── 📄 GUIDE_RAPIDE_FINANCES.md (DÉPLACER ICI)
│   ├── COMMIT_FINAL.txt
│   ├── NETTOYAGE_EFFECTUE.md
│   ├── PUSH_GITHUB_INSTRUCTIONS.md
│   └── README.md
│
├── 📁 assets/
│   ├── iiiooo.png
│   ├── NordikAdventures - Liste des produits PGI.xlsx
│   ├── schema 2.0.mwb
│   └── README.md
│
└── 📁 scripts/
    ├── build_project.bat
    ├── push_produits.bat
    ├── push_to_github.bat
    └── README.md
```

---

## 🎯 ACTIONS À EFFECTUER

1. ✅ **Supprimer les fichiers obsolètes** (voir liste ci-dessus)
2. ✅ **Déplacer les documentations** vers `docs/`
3. ✅ **Nettoyer sql_scripts/** (garder seulement 2 fichiers)
4. ✅ **Supprimer les anciennes fenêtres** remplacées par les modules
5. ✅ **Créer un README.md principal** mis à jour

---

## 📊 RÉSULTAT ATTENDU

### Avant le nettoyage
- ~50+ fichiers à la racine
- SQL dispersés (7 fichiers)
- Doublons de documentation
- Anciennes fenêtres obsolètes

### Après le nettoyage
- ~6 fichiers à la racine (essentiels)
- SQL unifié (1 fichier + 1 guide)
- Documentation organisée dans `docs/`
- Code source propre et modulaire

---

**Gain :**
- ✅ Structure claire et professionnelle
- ✅ Moins de confusion
- ✅ Facilité de navigation
- ✅ Prêt pour GitHub/partage

