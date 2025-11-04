# 🔐 Guide d'Authentification - PGI Nordik Adventures

## ✅ Système d'Authentification Implémenté

Votre PGI dispose maintenant d'un système d'authentification complet et sécurisé !

---

## 📋 Installation de la Base de Données

### Étape 1 : Ajouter les Tables d'Authentification

Exécutez le script SQL suivant dans MySQL Workbench :

```sql
-- Fichier: SQL_Authentification.sql
```

Ce script crée :
- ✅ Table `utilisateurs` (comptes utilisateurs)
- ✅ Table `sessions` (sessions actives)
- ✅ Table `log_connexions` (historique de connexions)
- ✅ Trigger de mise à jour de dernière connexion

### Étape 2 : Vérifier les Données de Test

4 comptes de test sont automatiquement créés :

| Nom d'utilisateur | Mot de passe | Rôle | Accès |
|-------------------|--------------|------|-------|
| `admin` | `admin123` | Admin | 📦 Stocks + 💰 Finances + 👥 CRM |
| `gestionnaire` | `gestionnaire123` | Gestionnaire | 📦 Stocks + 💰 Finances + 👥 CRM |
| `employe` | `employe123` | Employé | 📦 Stocks + 👥 CRM |
| `comptable` | `comptable123` | Comptable | 💰 Finances |

---

## 🚀 Utilisation

### 1. Démarrage de l'Application

L'application démarre maintenant avec la **fenêtre de connexion** au lieu du tableau de bord.

```
Application démarre → LoginWindow.xaml
```

### 2. Connexion avec un Compte Existant

#### Écran de Connexion

![Connexion](Design moderne avec gradient lavande/rose)

**Champs** :
- Nom d'utilisateur
- Mot de passe

**Actions** :
- 🔐 **Se connecter** : Authentifie l'utilisateur
- 📝 **S'inscrire** : Ouvre la fenêtre d'inscription
- ✕ **Fermer** : Quitte l'application

#### Test Rapide

```
Nom d'utilisateur: admin
Mot de passe: admin123
→ Cliquez sur "🔐 Se connecter"
```

Vous serez redirigé vers le **tableau de bord principal** !

### 3. Création d'un Nouveau Compte

#### Écran d'Inscription

![Inscription](Design moderne avec gradient rose/lavande inversé)

**Champs requis** :
- Nom complet *
- Nom d'utilisateur * (min 3 caractères)
- Adresse email * (format valide)
- Mot de passe * (min 6 caractères)
- Confirmer le mot de passe *

**Actions** :
- ✨ **Créer mon compte** : Enregistre le nouveau compte
- 🔐 **Se connecter** : Retour à la connexion
- ✕ **Fermer** : Quitte l'application

#### Validation Automatique

- ✅ Vérification de l'unicité du nom d'utilisateur
- ✅ Vérification de l'unicité de l'email
- ✅ Validation du format email
- ✅ Vérification de la longueur du mot de passe
- ✅ Confirmation du mot de passe

#### Compte par Défaut

Les nouveaux comptes ont :
- **Rôle** : Employé
- **Accès** : Stocks + CRM (pas Finances)
- **Statut** : Actif

---

## 🔒 Sécurité

### Hash des Mots de Passe

Les mots de passe sont **hashés avec SHA-256** avant stockage :

```csharp
SHA256("admin123") → "240be518fabd...a7e07dbd1"
```

❌ Les mots de passe en clair ne sont **jamais stockés** en base !

### Validation des Statuts

Seuls les comptes avec `statut = 'Actif'` peuvent se connecter.

Statuts possibles :
- ✅ **Actif** : Peut se connecter
- ⏸️ **Inactif** : Ne peut pas se connecter
- 🚫 **Suspendu** : Ne peut pas se connecter

### Log des Connexions

Toutes les tentatives de connexion sont enregistrées dans `log_connexions` :

```sql
SELECT * FROM log_connexions ORDER BY date_tentative DESC LIMIT 10;
```

Contient :
- Utilisateur (ID + nom)
- Date et heure
- Succès (true/false)
- Adresse IP
- Message (raison d'échec)

---

## 👤 Gestion des Rôles

### Rôles Disponibles

| Rôle | Description | Accès par défaut |
|------|-------------|------------------|
| **Admin** | Administrateur système | Tous les modules |
| **Gestionnaire** | Manager | Tous les modules |
| **Employe** | Employé standard | Stocks + CRM |
| **Comptable** | Comptable | Finances uniquement |

### Permissions par Module

Les permissions sont stockées dans la session :

```csharp
App.Current.Properties["AccesStocks"]    // bool
App.Current.Properties["AccesFinances"]  // bool
App.Current.Properties["AccesCrm"]       // bool
```

### Exemple : Restreindre l'Accès

Dans `MainWindow.xaml.cs`, vous pouvez cacher des onglets :

```csharp
public MainWindow()
{
    InitializeComponent();
    
    // Récupérer les permissions
    bool accesFinances = (bool)App.Current.Properties["AccesFinances"];
    
    // Cacher l'onglet Finances si pas d'accès
    if (!accesFinances)
    {
        // Logique pour cacher le TabItem Finances
    }
}
```

---

## 📊 Sessions Utilisateur

### Informations Stockées

Après une connexion réussie :

```csharp
App.Current.Properties["UserId"]        // int
App.Current.Properties["Username"]      // string
App.Current.Properties["NomComplet"]    // string
App.Current.Properties["Role"]          // string
App.Current.Properties["AccesStocks"]   // bool
App.Current.Properties["AccesFinances"] // bool
App.Current.Properties["AccesCrm"]      // bool
```

### Afficher l'Utilisateur Connecté

Dans n'importe quelle fenêtre :

```csharp
string nomComplet = App.Current.Properties["NomComplet"].ToString();
MessageBox.Show($"Bienvenue, {nomComplet} !");
```

### Déconnexion

Pour ajouter une fonctionnalité de déconnexion :

```csharp
private void Deconnexion()
{
    // Effacer la session
    App.Current.Properties.Clear();
    
    // Retourner à la connexion
    LoginWindow loginWindow = new LoginWindow();
    loginWindow.Show();
    this.Close();
}
```

---

## 🛠️ Configuration

### Modifier la Connexion MySQL

Dans `LoginWindow.xaml.cs` et `RegisterWindow.xaml.cs` :

```csharp
private string connectionString = "Server=localhost;Database=NordikAdventuresERP;Uid=root;Pwd=;";
```

**Paramètres à ajuster** :
- `Server` : localhost (ou IP du serveur)
- `Database` : NordikAdventuresERP
- `Uid` : root (votre utilisateur MySQL)
- `Pwd` : (votre mot de passe MySQL)

---

## 📁 Fichiers Créés

### Fichiers XAML
```
✓ LoginWindow.xaml              → Interface de connexion
✓ RegisterWindow.xaml           → Interface d'inscription
```

### Fichiers C#
```
✓ LoginWindow.xaml.cs           → Logique de connexion
✓ RegisterWindow.xaml.cs        → Logique d'inscription
```

### Fichiers SQL
```
✓ SQL_Authentification.sql      → Création des tables
```

### Modifications
```
✓ App.xaml                      → StartupUri changé vers LoginWindow
```

---

## 🎨 Design de l'Interface

### Fenêtre de Connexion

- **Gradient** : Lavande → Rose pâle → Rose (#CDB4DB → #FFC8DD → #FFAFCC)
- **Panneau gauche** : Logo + description
- **Panneau droit** : Formulaire de connexion
- **Boutons** : Bleu pastel (#A2D2FF)
- **Ombres** : DropShadow pour profondeur
- **Sans bordure** : WindowStyle="None" avec coins arrondis

### Fenêtre d'Inscription

- **Gradient inversé** : Rose → Rose pâle → Lavande
- **Design cohérent** avec la connexion
- **Scrollable** : Pour accueillir tous les champs
- **Validation en temps réel** : Messages d'erreur colorés

---

## ✨ Fonctionnalités Implémentées

### Connexion
- ✅ Validation des champs
- ✅ Vérification des credentials (hash SHA-256)
- ✅ Vérification du statut du compte
- ✅ Log des tentatives (succès/échec)
- ✅ Stockage de la session utilisateur
- ✅ Redirection vers le tableau de bord
- ✅ Gestion des erreurs

### Inscription
- ✅ Validation de tous les champs
- ✅ Vérification unicité (username + email)
- ✅ Validation format email (regex)
- ✅ Vérification longueur mot de passe (min 6)
- ✅ Confirmation du mot de passe
- ✅ Hash sécurisé du mot de passe
- ✅ Attribution automatique du rôle "Employé"
- ✅ Redirection vers la connexion après succès

### Sécurité
- ✅ Mots de passe hashés (SHA-256)
- ✅ Pas de stockage en clair
- ✅ Log complet des connexions
- ✅ Gestion des statuts de compte
- ✅ Validation côté client et serveur

---

## 🔍 Dépannage

### Erreur : "Erreur de connexion à la base de données"

**Cause** : MySQL n'est pas démarré ou mauvaise chaîne de connexion

**Solution** :
1. Vérifiez que MySQL est démarré
2. Vérifiez les credentials dans `connectionString`
3. Testez la connexion dans MySQL Workbench

### Erreur : "Table 'utilisateurs' doesn't exist"

**Cause** : Le script SQL n'a pas été exécuté

**Solution** :
1. Ouvrez MySQL Workbench
2. Exécutez `SQL_Authentification.sql`
3. Vérifiez : `SHOW TABLES;`

### La fenêtre ne s'affiche pas

**Cause** : Erreur de compilation

**Solution** :
1. Vérifiez dans `Error List` de Visual Studio
2. Assurez-vous que tous les fichiers sont inclus dans le projet
3. Rebuild Solution (Ctrl+Shift+B)

### "Identifiants incorrects" même avec le bon mot de passe

**Cause** : Différence dans le hash

**Solution** :
Vérifiez le hash en base :
```sql
SELECT mot_de_passe FROM utilisateurs WHERE nom_utilisateur = 'admin';
```

Comparez avec le hash de "admin123" :
```
240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9
```

---

## 📌 Prochaines Étapes

### Améliorations Possibles

1. **Récupération de mot de passe**
   - Envoi d'email avec lien de réinitialisation
   - Code de vérification temporaire

2. **Double authentification (2FA)**
   - Code envoyé par email/SMS
   - Application d'authentification

3. **Gestion des sessions**
   - Expiration automatique après X minutes
   - "Se souvenir de moi" (token persistant)

4. **Gestion des utilisateurs (Admin)**
   - Interface pour créer/modifier/supprimer des comptes
   - Attribution/révocation de permissions
   - Activation/désactivation de comptes

5. **Historique de connexion visible**
   - Page affichant les dernières connexions
   - Détection d'activité suspecte

---

## 🎓 Pour le TP#2

### Points à Souligner

✅ **Sécurité** : Mots de passe hashés, validation complète
✅ **Interface moderne** : Design cohérent avec le reste du PGI
✅ **Gestion des rôles** : 4 rôles avec permissions différentes
✅ **Traçabilité** : Log complet des connexions
✅ **Ergonomie** : Fenêtres modernes sans bordure, coins arrondis
✅ **Validation** : Vérifications côté client et serveur

### Captures d'Écran à Inclure

1. ✅ Fenêtre de connexion (avec compte test visible)
2. ✅ Fenêtre d'inscription (formulaire complet)
3. ✅ Message d'erreur (identifiants incorrects)
4. ✅ Redirection vers le tableau de bord après connexion
5. ✅ Table `log_connexions` dans MySQL (historique)

---

## 📞 Support

En cas de problème :
1. Vérifiez la connexion MySQL
2. Consultez les logs dans `log_connexions`
3. Vérifiez que les tables sont créées
4. Testez avec le compte `admin/admin123`

---

**🎉 Votre système d'authentification est prêt !**

*Guide créé le 1er novembre 2025*  
*PGI Nordik Adventures - Système de Gestion Intégré*

