# 🚀 Guide d'Installation et d'Exécution - PGI Nordik Adventures

## 📋 Prérequis

### Logiciels Requis

1. **Visual Studio 2022** (ou version ultérieure)
   - Workload : `.NET Desktop Development`
   - SDK : `.NET 8.0` ou supérieur

2. **MySQL Server 8.0+**
   - MySQL Workbench (recommandé)
   - Accès administrateur

3. **Git** (optionnel, pour le versioning)

### Configuration Système Minimale

- **OS** : Windows 10/11 (build 10.0.26200 ou supérieur)
- **RAM** : 4 GB minimum (8 GB recommandé)
- **Espace disque** : 500 MB pour l'application + 1 GB pour la base de données
- **Résolution** : 1000x650 minimum (1920x1080 recommandé)

## 🗄️ Installation de la Base de Données

### Étape 1 : Créer la Base de Données

1. Ouvrez **MySQL Workbench**
2. Connectez-vous à votre serveur MySQL local
3. Ouvrez le fichier : `NordikAdventuresERP_Schema_FR.sql`
4. Exécutez le script complet (⚡ Execute)

```sql
-- Le script va automatiquement :
-- ✓ Créer la base de données NordikAdventuresERP
-- ✓ Créer toutes les tables (20+ tables)
-- ✓ Installer les vues, triggers, fonctions et procédures stockées
```

### Étape 2 : Insérer les Données de Test (Optionnel)

1. Ouvrez le fichier : `Donnees_Test_NordikAdventuresERP.sql`
2. Exécutez le script pour charger des données d'exemple

## 💻 Compilation de l'Application

### Méthode 1 : Visual Studio (Recommandé)

1. **Ouvrir le projet**
   ```
   Double-cliquez sur : analyse\analyse\PGI.sln
   ```

2. **Restaurer les packages NuGet**
   - Visual Studio le fait automatiquement
   - Ou : `Tools > NuGet Package Manager > Restore NuGet Packages`

3. **Configuration de Build**
   ```
   Configuration : Debug ou Release
   Platform : Any CPU
   ```

4. **Compiler**
   ```
   Build > Build Solution (Ctrl+Shift+B)
   ```

5. **Exécuter**
   ```
   Debug > Start Debugging (F5)
   Ou
   Debug > Start Without Debugging (Ctrl+F5)
   ```

### Méthode 2 : Ligne de Commande

```powershell
# Naviguer vers le dossier du projet
cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\analyse\analyse\PGI"

# Restaurer les dépendances
dotnet restore

# Compiler le projet
dotnet build --configuration Release

# Exécuter l'application
dotnet run --configuration Release
```

## 🔧 Configuration de la Connexion MySQL

### Modifier la Chaîne de Connexion

Si vous devez modifier les paramètres de connexion MySQL, éditez le fichier `App.config` ou `appsettings.json` :

```xml
<connectionStrings>
  <add name="NordikAdventuresERP" 
       connectionString="Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VotreMotDePasse;" 
       providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

**Paramètres à ajuster** :
- `Server` : adresse du serveur MySQL (localhost par défaut)
- `Database` : nom de la base (NordikAdventuresERP)
- `Uid` : nom d'utilisateur MySQL (root par défaut)
- `Pwd` : mot de passe MySQL

## 📂 Structure des Dossiers

```
PGI/
├── MainWindow.xaml              # Interface principale (NOUVELLE VERSION)
├── MainWindow.xaml.cs           # Code-behind
├── App.xaml                     # Configuration application
├── App.xaml.cs                  
├── PGI.csproj                   # Fichier de projet
├── bin/
│   └── Debug/net8.0-windows/    # Fichiers compilés
│       └── PGI.exe              # Exécutable
├── obj/                         # Fichiers temporaires
├── *Window.xaml                 # Fenêtres secondaires
├── AMELIORATIONS_INTERFACE.md   # Documentation des améliorations
└── README_INSTALLATION.md       # Ce fichier
```

## 🎨 Fonctionnalités de l'Interface

### Module 1 : Stocks & Produits 📦
- Gestion des produits (CRUD)
- Suivi des niveaux de stock
- Alertes de réapprovisionnement
- Gestion des fournisseurs
- Historique des mouvements

### Module 2 : Finances & Facturation 💰
- Création de ventes/commandes
- Génération de factures (TPS/TVQ)
- Gestion des paiements
- Achats fournisseurs
- Journal comptable
- États financiers

### Module 3 : CRM 👥
- Fiches clients complètes
- Historique des interactions
- Rapports de ventes par client
- Gestion de la satisfaction
- KPI de fidélisation

### Fonctions Transversales
- 🏠 **Tableau de bord** avec 6 KPI en temps réel
- 📊 **Rapports** (ventes, taxes, inventaire, clients)
- ⚙️ **Paramètres** (fiscalité, droits d'accès)

## 🐛 Résolution de Problèmes

### Erreur : "MySQL.Data not found"

```powershell
# Installer le package MySQL via NuGet
Install-Package MySql.Data -Version 8.0.33
```

### Erreur : "Cannot connect to MySQL server"

1. Vérifiez que MySQL est démarré :
   ```powershell
   # Windows Services
   services.msc
   # Chercher "MySQL" et démarrer le service
   ```

2. Testez la connexion via MySQL Workbench

3. Vérifiez les credentials dans `App.config`

### Erreur : "Window initialization exception"

- Vérifiez que tous les fichiers `.xaml` sont bien présents
- Nettoyez et recompilez :
  ```
  Build > Clean Solution
  Build > Rebuild Solution
  ```

### L'interface ne s'affiche pas correctement

- Vérifiez la résolution de votre écran (minimum 1000x650)
- Essayez de maximiser la fenêtre
- Redémarrez l'application

## 📝 Notes de Développement

### Thème de Couleurs

L'application utilise la palette pastel Coolors :
- Lavande (#CDB4DB) → Module Stocks
- Rose Pâle (#FFC8DD) → Module Finances
- Rose (#FFAFCC) → Module CRM
- Bleu Clair (#BDE0FE) → Rapports
- Bleu Pastel (#A2D2FF) → Boutons

### Architecture

- **Pattern** : MVVM (Model-View-ViewModel)
- **UI Framework** : WPF avec XAML
- **Database** : MySQL 8.0+ avec InnoDB
- **Language** : C# (.NET 8.0)

## 📞 Support

Pour toute question ou problème :

1. Consultez le fichier `AMELIORATIONS_INTERFACE.md`
2. Vérifiez les erreurs dans la console de Visual Studio
3. Consultez les logs MySQL dans `MySQL Workbench`

## ✅ Checklist de Démarrage

- [ ] MySQL Server installé et démarré
- [ ] Base de données créée avec le script SQL
- [ ] Visual Studio 2022 installé
- [ ] .NET 8.0 SDK installé
- [ ] Projet compilé sans erreur
- [ ] Connexion MySQL configurée
- [ ] Application lancée avec succès
- [ ] Interface affichée correctement

## 🎓 Pour le Rendu du TP#2

### Fichiers à Inclure

1. ✅ `NordikAdventuresERP_Schema_FR.sql` (Base de données)
2. ✅ `Donnees_Test_NordikAdventuresERP.sql` (Données de test)
3. ✅ Dossier complet `PGI/` (Code source)
4. ✅ `AMELIORATIONS_INTERFACE.md` (Documentation)
5. ✅ Captures d'écran de l'interface
6. ✅ Diagrammes UML (contexte, cas d'utilisation, etc.)
7. ✅ Rapport PDF final

### Démonstration

L'application démontre :
- ✅ Les 3 modules bien séparés visuellement
- ✅ L'intégration entre les modules
- ✅ Les règles d'affaires implémentées
- ✅ Une interface moderne et professionnelle
- ✅ La conformité avec le cahier des charges

---

**🎉 Bon travail et bonne chance pour votre TP#2 !**

*Interface modernisée le 1er novembre 2025*

