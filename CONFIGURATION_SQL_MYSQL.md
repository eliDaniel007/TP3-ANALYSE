# 🗄️ Configuration SQL et MySQL - Approche ADO.NET

## 📋 Architecture du projet

Ce projet utilise une **approche SQL directe** avec **MySql.Data** (ADO.NET), **PAS Entity Framework Core**.

---

## ✅ Configuration actuelle

### 📦 Package NuGet installé

- **MySql.Data** (v9.1.0) - Connecteur MySQL pour .NET

### 🔌 Configuration de connexion

**Fichier:** `Analyse tp/analyse/PGI/Helpers/DatabaseHelper.cs`

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=password;";
```

### ⚙️ Modification du mot de passe

1. Ouvrez `Analyse tp/analyse/PGI/Helpers/DatabaseHelper.cs`
2. Modifiez la ligne avec votre mot de passe MySQL :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_MOT_DE_PASSE;";
```

Exemples :
- `Pwd=root;` - si votre mot de passe est "root"
- `Pwd=admin;` - si votre mot de passe est "admin"  
- `Pwd=;` - si vous n'avez pas de mot de passe

---

## 🗄️ Configuration de la base de données

### 1️⃣ Créer la base de données

Exécutez les scripts SQL dans l'ordre :

#### Script 1: Schéma de la base de données
```
sql_scripts/NordikAdventuresERP_Schema_FR.sql
```
Ce script crée :
- La base de données `NordikAdventuresERP`
- Toutes les tables (produits, categories, fournisseurs, etc.)
- Les contraintes de clés étrangères
- Les index

#### Script 2: Données des produits
```
sql_scripts/SQL_Produits_NordikAdventures.sql
```
Ce script insère :
- Les catégories
- Les fournisseurs
- Les produits avec leurs détails
- Les niveaux de stock

### 2️⃣ Exécution des scripts

**Option A: Via MySQL Workbench**
1. Ouvrez MySQL Workbench
2. Connectez-vous à votre serveur MySQL
3. File → Open SQL Script
4. Sélectionnez le script et exécutez-le (⚡ bouton Execute)

**Option B: Via ligne de commande**
```bash
mysql -u root -p < sql_scripts/NordikAdventuresERP_Schema_FR.sql
mysql -u root -p < sql_scripts/SQL_Produits_NordikAdventures.sql
```

---

## 📊 Structure des modèles

Les modèles sont de simples classes C# **sans annotations Entity Framework**.

### Exemple : Produit.cs

```csharp
public class Produit
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int CategorieId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public decimal Prix { get; set; }
    // ... autres propriétés
    
    // Propriétés calculées (remplies manuellement via JOIN SQL)
    public string NomCategorie { get; set; } = string.Empty;
    public string NomFournisseur { get; set; } = string.Empty;
}
```

---

## 🔧 Services - Utilisation de MySql.Data

Les services utilisent `MySqlConnection` et `MySqlCommand` pour exécuter des requêtes SQL.

### Exemple : ProduitService.cs

```csharp
public static List<Produit> GetAllProduits()
{
    var produits = new List<Produit>();
    
    using (var conn = DatabaseHelper.GetConnection())
    {
        conn.Open();
        string query = @"
            SELECT p.*, 
                   c.nom AS categorie_nom,
                   f.nom AS fournisseur_nom
            FROM produits p
            LEFT JOIN categories c ON p.categorie_id = c.id
            LEFT JOIN fournisseurs f ON p.fournisseur_id = f.id";
        
        using (var cmd = new MySqlCommand(query, conn))
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                produits.Add(new Produit
                {
                    Id = reader.GetInt32("id"),
                    SKU = reader.GetString("sku"),
                    Nom = reader.GetString("nom"),
                    // ...
                    NomCategorie = reader.GetString("categorie_nom"),
                    NomFournisseur = reader.GetString("fournisseur_nom")
                });
            }
        }
    }
    
    return produits;
}
```

---

## 🚀 Compilation du projet

### Via le script batch

Double-cliquez sur `build_project.bat`

### Via ligne de commande

```batch
cd /d "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\Analyse tp\analyse\PGI"
dotnet clean
dotnet restore
dotnet build
```

### Via Visual Studio

1. Ouvrez la solution `PGI.sln`
2. Build → Rebuild Solution

---

## ✅ Checklist de configuration

- [ ] MySQL est installé et démarré
- [ ] Les scripts SQL ont été exécutés
- [ ] La base de données `NordikAdventuresERP` existe
- [ ] Le mot de passe MySQL est configuré dans `DatabaseHelper.cs`
- [ ] Le package `MySql.Data` est installé
- [ ] Le projet compile sans erreur

---

## ⚠️ Fichiers supprimés

Les fichiers suivants ont été supprimés car ils étaient liés à Entity Framework Core :

- ❌ `Data/AppDbContext.cs`
- ❌ `Data/DbContextFactory.cs`
- ❌ `Data/DataSeeder.cs`
- ❌ `Data/DatabaseFixer.cs`
- ❌ `Data/DatabaseReseeder.cs`
- ❌ `DiagnosticProduits.cs`
- ❌ `TestConnexion.cs`

---

## 🆘 Dépannage

### Erreur: "Unable to connect to any of the specified MySQL hosts"

- ✅ Vérifiez que MySQL est démarré
- ✅ Vérifiez le port (défaut: 3306)
- ✅ Vérifiez l'utilisateur et le mot de passe

### Erreur: "Access denied for user 'root'@'localhost'"

- ✅ Vérifiez le mot de passe dans `DatabaseHelper.cs`
- ✅ Testez la connexion dans MySQL Workbench

### Erreur: "Unknown database 'NordikAdventuresERP'"

- ✅ Exécutez le script `NordikAdventuresERP_Schema_FR.sql`
- ✅ Vérifiez que la base existe: `SHOW DATABASES;`

---

**Date de configuration:** 23 novembre 2025  
**Type de connexion:** ADO.NET avec MySql.Data (v9.1.0)  
**Base de données:** MySQL 8.0+

