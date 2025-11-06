# 🚀 Installation Base de Données - Étape par Étape

## ✅ **Étape 1 : Ouvrir MySQL Workbench**

1. Lancer **MySQL Workbench**
2. Cliquer sur votre connexion locale (généralement `Local instance MySQL80`)
3. Entrer votre mot de passe root
4. Cliquer sur **OK**

---

## ✅ **Étape 2 : Installer le Schéma Principal (2 minutes)**

### 2.1 Ouvrir le fichier SQL

1. Dans MySQL Workbench, cliquer sur **File** (en haut à gauche)
2. Cliquer sur **Open SQL Script...**
3. Naviguer vers :
   ```
   C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\NordikAdventuresERP_Schema_FR.sql
   ```
4. Cliquer sur **Ouvrir**

### 2.2 Exécuter le script

1. **Cliquer sur l'icône ⚡ (Execute)** en haut
   - Ou appuyer sur `Ctrl+Shift+Enter`
2. Attendre 1-2 minutes (le script va défiler rapidement)
3. En bas, dans **Action Output**, vérifier qu'il n'y a **pas d'erreurs en rouge**

### 2.3 Vérifier que ça a marché

Copiez et exécutez cette commande :

```sql
USE NordikAdventuresERP;
SHOW TABLES;
```

**Résultat attendu :** Vous devriez voir environ **20 tables** :
- `categories`
- `clients`
- `employes`
- `fournisseurs`
- `produits`
- `niveaux_stock`
- etc.

---

## ✅ **Étape 3 : Ajouter les Employés et Clients (30 secondes)**

### 3.1 Ouvrir le fichier

1. **File** > **Open SQL Script...**
2. Naviguer vers :
   ```
   C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\Analyse tp Maquette\analyse\analyse\PGI\SQL_Schema_Auth_Safe.sql
   ```
3. **Ouvrir**

### 3.2 Exécuter

1. **Cliquer sur ⚡ (Execute)**
2. Vérifier les messages en bas :
   ```
   Message: Colonne mot_de_passe ajoutée à la table employes.
   Message: Employés de test insérés/mis à jour.
   Message: Clients de test insérés/mis à jour.
   ```

### 3.3 Vérifier

```sql
SELECT nom, prenom, courriel FROM employes WHERE mot_de_passe IS NOT NULL;
```

**Résultat attendu :** 4 employés (Admin, Marie, Pierre, Sophie)

---

## ✅ **Étape 4 : Ajouter les 30 Produits (30 secondes)**

### 4.1 Ouvrir le fichier

1. **File** > **Open SQL Script...**
2. Naviguer vers :
   ```
   C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\Analyse tp Maquette\analyse\analyse\PGI\SQL_Produits_NordikAdventures.sql
   ```
3. **Ouvrir**

### 4.2 Exécuter

1. **Cliquer sur ⚡ (Execute)**
2. Attendre 10-20 secondes
3. En bas, vous devriez voir :
   ```
   ✅ 30 produits insérés avec succès !
   ```

### 4.3 Vérifier

```sql
SELECT COUNT(*) AS 'Nombre de produits' FROM produits;
SELECT nom, prix, cout FROM produits LIMIT 5;
```

**Résultat attendu :** 
- Nombre de produits : **30**
- Liste de 5 produits (Veste Everest Pro, Pantalon Trekking, etc.)

---

## ✅ **Étape 5 : Vérifier le Mot de Passe MySQL dans l'Application**

### 5.1 Ouvrir le fichier DatabaseHelper.cs

Dans Visual Studio :
1. **Solution Explorer** > **PGI** > **Helpers** > **DatabaseHelper.cs**
2. Trouver la ligne 12 :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=password;";
```

### 5.2 Vérifier le mot de passe

**Si votre mot de passe MySQL n'est PAS "password"**, changez-le :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=VOTRE_VRAI_MOT_DE_PASSE;";
```

Par exemple, si votre mot de passe est "root" :

```csharp
private static string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=root;";
```

### 5.3 Sauvegarder

- **Ctrl+S** pour sauvegarder

---

## ✅ **Étape 6 : Relancer l'Application**

1. **Fermer l'application** si elle est ouverte
2. Dans Visual Studio : **F5** (ou cliquer sur ▶️ Start)
3. Se connecter avec :
   - Email : `admin@nordikadventures.com`
   - Mot de passe : `admin123`
4. Cliquer sur **Module Stocks**
5. Cliquer sur **Produits**

**Résultat attendu :** Vous devriez voir **30 produits** :
- VES-001 - Veste Everest Pro
- PAN-002 - Pantalon Trekking Résistant
- BOT-003 - Bottes Grand Froid -40°C
- etc.

---

## 🔍 **Test de Connexion Rapide**

Pour vérifier que l'application peut se connecter à MySQL :

1. Dans l'application, aller dans **Stocks** > **Dashboard**
2. Cliquer sur le bouton **Recalculer** (valeur de l'inventaire)
3. Si ça affiche un montant calculé → **Connexion OK ✅**
4. Si ça affiche une erreur → **Problème de connexion ❌**

---

## ❌ **Problèmes Courants**

### Erreur : "Access denied for user 'root'@'localhost'"

**Cause :** Mauvais mot de passe dans `DatabaseHelper.cs`

**Solution :** Modifier le mot de passe à l'Étape 5

---

### Erreur : "Unknown database 'NordikAdventuresERP'"

**Cause :** Le schéma principal n'a pas été exécuté

**Solution :** Retour à l'Étape 2

---

### Erreur : "Column 'categorie_id' does not belong to table"

**Cause :** Le schéma a été partiellement exécuté

**Solution :** 
1. Supprimer la base :
   ```sql
   DROP DATABASE IF EXISTS NordikAdventuresERP;
   ```
2. Retour à l'Étape 2

---

### L'application affiche encore les données d'exemple (3 produits)

**Cause :** Mot de passe MySQL incorrect OU produits pas insérés

**Solution :**
1. Vérifier le mot de passe (Étape 5)
2. Vérifier que les produits existent :
   ```sql
   SELECT COUNT(*) FROM produits;
   ```
3. Si 0 → Retour à l'Étape 4

---

## 📋 **Checklist Complète**

- [ ] Étape 1 : MySQL Workbench ouvert
- [ ] Étape 2 : Schéma principal exécuté (`NordikAdventuresERP_Schema_FR.sql`)
- [ ] Étape 2.3 : Vérifié que les tables existent (`SHOW TABLES;`)
- [ ] Étape 3 : Authentification exécutée (`SQL_Schema_Auth_Safe.sql`)
- [ ] Étape 3.3 : Vérifié que 4 employés existent
- [ ] Étape 4 : Produits exécutés (`SQL_Produits_NordikAdventures.sql`)
- [ ] Étape 4.3 : Vérifié que 30 produits existent
- [ ] Étape 5 : Mot de passe vérifié dans `DatabaseHelper.cs`
- [ ] Étape 6 : Application relancée
- [ ] Étape 6 : 30 produits visibles dans l'application

---

## 🎯 **Durée Totale : ~5 minutes**

| Étape | Temps |
|-------|-------|
| 1. Ouvrir MySQL | 30s |
| 2. Schéma principal | 2 min |
| 3. Authentification | 30s |
| 4. Produits | 30s |
| 5. Mot de passe | 30s |
| 6. Test | 1 min |

---

## 🆘 **Besoin d'Aide ?**

Si vous avez une erreur, **postez le message d'erreur exact** et je vous aiderai !

---

**🚀 Commencez par l'Étape 1 et suivez chaque étape dans l'ordre !**

