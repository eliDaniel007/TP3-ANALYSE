# 🏔️ Nordik Adventures ERP

**Système de Gestion Intégrée (PGI/ERP) pour Nordik Adventures**  
Application WPF .NET 8.0 avec base de données MySQL

---

## ✅ Ce qui est fait

### 📦 Module Produits et Stocks
- ✅ Gestion complète des produits (CRUD)
- ✅ Gestion des catégories
- ✅ Gestion des fournisseurs
- ✅ Historique des mouvements de stock
- ✅ Tableau de bord avec statistiques
- ✅ Recherche et filtrage des produits

### 🛒 Côté Client
- ✅ Interface d'achat pour les clients
- ✅ Panier d'achat
- ✅ Gestion des commandes
- ✅ Images des produits intégrées

---

## ⚠️ Ce qu'il reste à faire

- [ ] Module Finances (comptabilité, factures, paiements)
- [ ] Module CRM complet (gestion des clients, campagnes marketing)
- [ ] Module Achats (commandes fournisseurs)
- [ ] Module Ventes (gestion des ventes)
- [ ] Rapports et graphiques avancés
- [ ] Sécurisation des mots de passe (hashing)
- [ ] Tests unitaires

---

## 🚀 Avant de pouvoir utiliser le projet

### 1️⃣ Prérequis

- **Windows** 10/11
- **.NET SDK 8.0+** : [Télécharger](https://dotnet.microsoft.com/download)
- **Visual Studio 2022+** : [Télécharger](https://visualstudio.microsoft.com/)
- **MySQL Server 8.0+** : [Télécharger](https://dev.mysql.com/downloads/mysql/)
- **MySQL Workbench 8.0+** : [Télécharger](https://dev.mysql.com/downloads/workbench/)

### 2️⃣ Installation de MySQL

1. Installer MySQL Community Server
2. Définir un mot de passe pour l'utilisateur **root** (notez-le !)
3. Installer MySQL Workbench

### 3️⃣ Créer la Base de Données

**Dans MySQL Workbench :**

1. Se connecter au serveur local (root + votre mot de passe)
2. Ouvrir le fichier SQL : `sql_scripts/SQL_COMPLET_UNIFIE.sql`
3. Exécuter le script (⚡ Execute)
4. Vérifier que la base de données `NordikAdventuresERP` a été créée

### 4️⃣ Configurer la Connexion

1. Ouvrir le fichier : `Analyse tp/analyse/PGI/Helpers/DatabaseHelper.cs`
2. Ligne 13, modifier le mot de passe MySQL :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

3. Remplacer `VOTRE_MOT_DE_PASSE` par votre mot de passe MySQL root
4. Sauvegarder le fichier

### 5️⃣ Lancer l'Application

1. Ouvrir le projet dans Visual Studio : `Analyse tp/analyse/PGI.sln`
2. Appuyer sur **F5** (ou cliquer sur ▶️ Debug)
3. Se connecter avec les identifiants ci-dessous

---

## 🔑 Identifiants de Connexion

### 👨‍💼 Employé (Accès PGI)

**Email :** `admin@nordikadventures.com`  
**Mot de passe :** `Admin123`  
**Rôle :** Administrateur (accès complet)

### 🛒 Client (Accès Site d'Achat)

**Email :** `jean.tremblay@client.com`  
**Mot de passe :** `Client123`

**Autres clients disponibles :**
- `marie.client@test.com` / `Client123`
- `pierre.client@entreprise.com` / `Client123`
- `client.sophie@gmail.com` / `Client123`
- `contact@nordikclient.com` / `Client123`

---

## 📁 Structure du Projet

```
TP3-ANALYSE/
│
├── README.md                          # Documentation principale
├── build_project.bat                  # Script de compilation
│
├── sql_scripts/                       # Scripts SQL
│   └── SQL_COMPLET_UNIFIE.sql        # Script complet (schéma + données)
│
├── assets/                            # Ressources
│   └── IMAGES PRODUITS/              # Images des produits
│
└── Analyse tp/analyse/                # Code source
    └── PGI/
        ├── PGI.sln                   # Solution Visual Studio
        ├── Models/                   # Modèles de données
        ├── Services/                  # Logique métier
        ├── Views/                     # Interfaces utilisateur
        │   ├── Stocks/               # Module Stocks (complet)
        │   ├── Shopping/             # Module Client (complet)
        │   ├── Finances/             # Module Finances (en développement)
        │   └── CRM/                  # Module CRM (en développement)
        └── Helpers/
            └── DatabaseHelper.cs     # Configuration connexion MySQL
```

---

## 🛠️ Technologies Utilisées

- **WPF** (.NET 8.0)
- **C# 12.0**
- **MySQL 8.0+**
- **ADO.NET**

---

## 📝 Notes Importantes

- ⚠️ **Les mots de passe ne sont PAS hashés** (développement uniquement)
- ⚠️ **Ne PAS utiliser en production sans sécuriser les mots de passe**
- ✅ **Le fichier SQL contient toutes les données nécessaires** (schéma + données de test)

---

## 👤 Auteur

**eliDaniel007**  
GitHub : [github.com/eliDaniel007](https://github.com/eliDaniel007)

---

**🚀 Bon développement avec Nordik Adventures ERP !**
