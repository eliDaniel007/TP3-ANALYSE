# 📚 Index de la Documentation - PGI Nordik Adventures

## 🎯 Navigation Rapide

Ce document sert d'index pour accéder rapidement à toute la documentation du projet.

---

## 📄 Documents Principaux

### 🎨 1. Améliorations de l'Interface
**Fichier** : [`PGI/AMELIORATIONS_INTERFACE.md`](PGI/AMELIORATIONS_INTERFACE.md)

**Contenu** :
- ✅ Liste détaillée des modifications apportées
- ✅ Palette de couleurs implémentée (#CDB4DB, #FFC8DD, #FFAFCC, #BDE0FE, #A2D2FF)
- ✅ Description des 3 modules séparés
- ✅ Effets visuels et design moderne
- ✅ Avantages pour l'utilisateur et le développement

**À consulter pour** : Comprendre toutes les améliorations visuelles

---

### 🚀 2. Guide d'Installation
**Fichier** : [`PGI/README_INSTALLATION.md`](PGI/README_INSTALLATION.md)

**Contenu** :
- ✅ Prérequis système (Visual Studio, MySQL, .NET 8.0)
- ✅ Installation de la base de données
- ✅ Compilation de l'application (VS + CLI)
- ✅ Configuration de la connexion MySQL
- ✅ Résolution de problèmes courants
- ✅ Checklist de démarrage

**À consulter pour** : Installer et exécuter l'application

---

### 🖼️ 3. Aperçu Visuel
**Fichier** : [`PGI/APERCU_INTERFACE.md`](PGI/APERCU_INTERFACE.md)

**Contenu** :
- ✅ Représentations ASCII de tous les écrans
- ✅ Tableau de bord avec les 6 KPI
- ✅ Module Stocks (Lavande)
- ✅ Module Finances (Rose Pâle)
- ✅ Module CRM (Rose)
- ✅ Rapports et Paramètres
- ✅ Légende des couleurs et dimensions

**À consulter pour** : Visualiser l'interface avant compilation

---

### 📋 4. Résumé Final
**Fichier** : [`PGI/RESUME_FINAL.md`](PGI/RESUME_FINAL.md)

**Contenu** :
- ✅ Travail complété (résumé exécutif)
- ✅ Fichiers créés/modifiés
- ✅ Objectifs atteints
- ✅ Conformité avec le TP#2
- ✅ Statistiques du projet
- ✅ Conseils pour la présentation
- ✅ Checklist pour la remise

**À consulter pour** : Vue d'ensemble du projet terminé

---

### 📑 5. Ce Document (Index)
**Fichier** : [`INDEX_DOCUMENTATION.md`](INDEX_DOCUMENTATION.md)

**Contenu** :
- ✅ Navigation vers tous les documents
- ✅ Structure du projet
- ✅ Liens rapides

**À consulter pour** : Trouver rapidement un document

---

## 🗂️ Structure Complète du Projet

```
📁 cette fois ci j'ai reussi/
│
├── 📁 analyse/
│   ├── 📁 analyse/
│   │   ├── 📁 PGI/                              ← CODE SOURCE
│   │   │   ├── 📄 MainWindow.xaml               ← INTERFACE MODERNISÉE ✨
│   │   │   ├── 📄 MainWindow.xaml.cs
│   │   │   ├── 📄 App.xaml
│   │   │   ├── 📄 PGI.csproj
│   │   │   ├── 📄 AMELIORATIONS_INTERFACE.md    ← DOC 1
│   │   │   ├── 📄 README_INSTALLATION.md        ← DOC 2
│   │   │   ├── 📄 APERCU_INTERFACE.md           ← DOC 3
│   │   │   ├── 📄 RESUME_FINAL.md               ← DOC 4
│   │   │   └── 📁 bin/Debug/net8.0-windows/
│   │   │       └── 📄 PGI.exe                   ← EXÉCUTABLE
│   │   ├── 📄 PGI.sln                           ← SOLUTION VISUAL STUDIO
│   │   └── 📄 INDEX_DOCUMENTATION.md            ← CE FICHIER
│   └── 📄 analyse.zip
│
├── 📄 NordikAdventuresERP_Schema_FR.sql         ← BASE DE DONNÉES
├── 📄 Donnees_Test_NordikAdventuresERP.sql      ← DONNÉES DE TEST
├── 📄 RESUME_MODIFICATIONS.md
└── 🖼️ iii.png
```

---

## 🎯 Liens Rapides par Besoin

### Je veux comprendre les modifications
👉 Consultez : [`AMELIORATIONS_INTERFACE.md`](PGI/AMELIORATIONS_INTERFACE.md)

### Je veux installer l'application
👉 Consultez : [`README_INSTALLATION.md`](PGI/README_INSTALLATION.md)

### Je veux voir l'interface
👉 Consultez : [`APERCU_INTERFACE.md`](PGI/APERCU_INTERFACE.md)

### Je veux un résumé global
👉 Consultez : [`RESUME_FINAL.md`](PGI/RESUME_FINAL.md)

### Je veux exécuter l'application
👉 Ouvrez : `PGI.sln` avec Visual Studio 2022

### Je veux créer la base de données
👉 Exécutez : `NordikAdventuresERP_Schema_FR.sql` dans MySQL

---

## 📦 Fichiers du Code Source (PGI/)

### Fichiers Principaux XAML
```
✓ MainWindow.xaml                    → Interface principale (MODERNISÉE)
✓ AchatsFournisseursWindow.xaml      → Fenêtre achats fournisseurs
✓ EtatFinancierWindow.xaml           → Fenêtre état financier
✓ FournisseursWindow.xaml            → Fenêtre gestion fournisseurs
✓ GraphiquesWindow.xaml              → Fenêtre graphiques
✓ HistoriqueMouvementsWindow.xaml    → Fenêtre historique stock
✓ InteractionsWindow.xaml            → Fenêtre interactions clients
✓ JournalComptableWindow.xaml        → Fenêtre journal comptable
✓ RapportVentesWindow.xaml           → Fenêtre rapport ventes
✓ App.xaml                           → Configuration application
```

### Fichiers Code-Behind C#
```
✓ MainWindow.xaml.cs                 → Logique interface principale
✓ [Autres fichiers .xaml.cs]         → Logique des fenêtres secondaires
✓ App.xaml.cs                        → Point d'entrée application
```

### Fichiers Projet
```
✓ PGI.csproj                         → Fichier de projet .NET
✓ PGI.sln                            → Solution Visual Studio
```

### Documentation
```
✓ AMELIORATIONS_INTERFACE.md         → Liste des modifications
✓ README_INSTALLATION.md             → Guide d'installation
✓ APERCU_INTERFACE.md                → Aperçu visuel ASCII
✓ RESUME_FINAL.md                    → Résumé exécutif
```

---

## 🗄️ Fichiers de Base de Données

### Schéma Principal
**Fichier** : `NordikAdventuresERP_Schema_FR.sql`  
**Taille** : 1077 lignes  
**Contenu** :
- 20+ tables (produits, stocks, clients, factures, etc.)
- Vues (v_ventes_par_client, v_valorisation_stock, etc.)
- Fonctions stockées (fn_obtenir_taux_taxe_courant, etc.)
- Procédures (sp_recalculer_totaux_facture, etc.)
- Triggers (trg_valider_prix_produit, etc.)

### Données de Test
**Fichier** : `Donnees_Test_NordikAdventuresERP.sql`  
**Contenu** :
- Données d'exemple pour toutes les tables
- Catégories, produits, fournisseurs
- Clients, commandes, factures
- Transactions de test

---

## 🎨 Palette de Couleurs du Projet

| Nom | Hex Code | RGB | Utilisation |
|-----|----------|-----|-------------|
| Lavande | `#CDB4DB` | rgb(205, 180, 219) | Module Stocks |
| Rose Pâle | `#FFC8DD` | rgb(255, 200, 221) | Module Finances |
| Rose | `#FFAFCC` | rgb(255, 175, 204) | Module CRM |
| Bleu Clair | `#BDE0FE` | rgb(189, 224, 254) | Rapports |
| Bleu Pastel | `#A2D2FF` | rgb(162, 210, 255) | Boutons |

**Source** : https://coolors.co/palette/cdb4db-ffc8dd-ffafcc-bde0fe-a2d2ff

---

## 📊 Modules du Système

### 📦 Module 1 : Stocks & Produits (Lavande)
- Gestion des produits (CRUD)
- Suivi des niveaux de stock
- Alertes de réapprovisionnement
- Gestion des fournisseurs
- Historique des mouvements
- Calcul de la marge brute
- Valorisation du stock

### 💰 Module 2 : Finances & Facturation (Rose Pâle)
- Création de ventes/commandes
- Génération de factures
- Calcul automatique TPS/TVQ (5% + 9.975%)
- Gestion des paiements
- Achats fournisseurs
- Journal comptable
- États financiers (ventes, dépenses, profit)

### 👥 Module 3 : CRM (Rose)
- Fiches clients complètes
- Historique des interactions
- Rapports de ventes par client
- Gestion de la satisfaction (1-5 étoiles)
- KPI de fidélisation
- Statuts clients (Prospect, Actif, Fidèle)
- Alertes clients à risque

### 🔄 Fonctions Transversales
- 🏠 Tableau de bord avec 6 KPI
- 📊 Rapports multiples (ventes, taxes, inventaire, clients)
- ⚙️ Paramètres système (fiscalité, droits d'accès)
- 📥 Export CSV/PDF
- 🔐 Authentification et contrôle d'accès

---

## 🛠️ Technologies Utilisées

### Frontend
- **Framework** : WPF (Windows Presentation Foundation)
- **Langage** : XAML pour l'interface
- **Version** : .NET 8.0

### Backend
- **Langage** : C# 12.0
- **Pattern** : MVVM (Model-View-ViewModel)
- **Data Binding** : Two-way binding

### Base de Données
- **SGBD** : MySQL 8.0+
- **Moteur** : InnoDB
- **Encodage** : utf8mb4_unicode_ci

### Outils de Développement
- **IDE** : Visual Studio 2022
- **Versioning** : Git (optionnel)
- **DB Tool** : MySQL Workbench

---

## 📞 Support et Ressources

### Documentation Officielle
- [WPF Documentation](https://docs.microsoft.com/fr-fr/dotnet/desktop/wpf/)
- [MySQL 8.0 Reference](https://dev.mysql.com/doc/refman/8.0/en/)
- [C# Programming Guide](https://docs.microsoft.com/fr-fr/dotnet/csharp/)

### Aide Locale
Consultez les fichiers de documentation dans le dossier `PGI/` :
1. `README_INSTALLATION.md` pour les problèmes d'installation
2. `AMELIORATIONS_INTERFACE.md` pour comprendre l'interface
3. `RESUME_FINAL.md` pour les questions générales

---

## ✅ Checklist de Vérification

Avant la remise du TP#2, vérifiez :

### Code
- [ ] Application compile sans erreur
- [ ] Toutes les fenêtres s'ouvrent correctement
- [ ] L'interface affiche les bonnes couleurs
- [ ] Les 3 modules sont distincts visuellement

### Base de Données
- [ ] Script SQL s'exécute sans erreur
- [ ] Toutes les tables sont créées
- [ ] Les données de test sont chargées
- [ ] La connexion depuis l'app fonctionne

### Documentation
- [ ] Tous les fichiers .md sont présents
- [ ] Les captures d'écran sont prises
- [ ] Le rapport PDF est rédigé
- [ ] Les diagrammes UML sont complétés

### Présentation
- [ ] Démonstration préparée (8-10 min)
- [ ] Points clés identifiés
- [ ] Réponses aux questions potentielles préparées

---

## 🎓 Conformité TP#2 INF23307

### Exigences Satisfaites

| Exigence | État | Localisation |
|----------|------|--------------|
| Module Stocks | ✅ | `MainWindow.xaml` - Tab "MODULE 1" |
| Module Finances | ✅ | `MainWindow.xaml` - Tab "MODULE 2" |
| Module CRM | ✅ | `MainWindow.xaml` - Tab "MODULE 3" |
| Intégration | ✅ | Triggers et logique métier |
| Interface | ✅ | Complètement modernisée |
| Base de données | ✅ | `NordikAdventuresERP_Schema_FR.sql` |
| Documentation | ✅ | 4 fichiers .md complets |

### Pondération : 20% du cours
**Date limite** : Lundi 3 novembre 2025 à 19h00  
**Format** : PDF + Code source + BD

---

## 🎉 Félicitations !

Vous disposez maintenant de :
- ✅ Une interface **magnifique** et **moderne**
- ✅ Une palette de couleurs **harmonieuse**
- ✅ Des modules **visuellement séparés**
- ✅ Une **documentation complète**
- ✅ Un projet **prêt pour la remise**

**Bon succès pour votre TP#2 ! 🚀**

---

*Documentation créée le 1er novembre 2025*  
*Projet : PGI Nordik Adventures*  
*Cours : INF23307 - Session Automne 2025*

