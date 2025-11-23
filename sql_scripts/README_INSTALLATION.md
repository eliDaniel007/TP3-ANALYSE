# 📋 GUIDE D'INSTALLATION - BASE DE DONNÉES NORDIKADVENTURES ERP

## 🎯 Fichier SQL Complet

**Fichier unique :** `SQL_COMPLET_NordikAdventuresERP.sql`

Ce fichier contient **TOUT** le système ERP en un seul script :
- ✅ Création de la base de données
- ✅ Toutes les tables (22 tables)
- ✅ Module Stocks & Inventaire
- ✅ Module Finances & Facturation
- ✅ Module CRM
- ✅ Module RH (Employés & Paies)
- ✅ Vues calculées
- ✅ Triggers et automatisations
- ✅ Procédures stockées
- ✅ Fonctions
- ✅ Données initiales

---

## 🚀 INSTALLATION RAPIDE

### Option 1 : Ligne de commande MySQL

```bash
# Se connecter à MySQL
mysql -u root -p

# Exécuter le script complet
source sql_scripts/SQL_COMPLET_NordikAdventuresERP.sql

# OU en une seule ligne
mysql -u root -p < sql_scripts/SQL_COMPLET_NordikAdventuresERP.sql
```

### Option 2 : MySQL Workbench

1. Ouvrir MySQL Workbench
2. Se connecter à votre serveur MySQL
3. Menu **File** → **Open SQL Script**
4. Sélectionner `SQL_COMPLET_NordikAdventuresERP.sql`
5. Cliquer sur l'icône ⚡ (Execute) ou appuyer sur **Ctrl+Shift+Enter**
6. Attendre la fin de l'exécution (environ 1-2 secondes)

### Option 3 : phpMyAdmin

1. Se connecter à phpMyAdmin
2. Cliquer sur l'onglet **SQL**
3. Copier-coller le contenu de `SQL_COMPLET_NordikAdventuresERP.sql`
4. Cliquer sur **Exécuter**

---

## ✅ VÉRIFICATION DE L'INSTALLATION

### 1. Vérifier que la base de données existe

```sql
SHOW DATABASES LIKE 'NordikAdventuresERP';
```

Résultat attendu : 1 ligne avec `NordikAdventuresERP`

### 2. Vérifier le nombre de tables

```sql
USE NordikAdventuresERP;
SHOW TABLES;
```

Résultat attendu : **22 tables**

```
+-------------------------------------+
| Tables_in_NordikAdventuresERP      |
+-------------------------------------+
| alertes_service_client             |
| campagnes_marketing                |
| categories                         |
| clients                            |
| commandes_fournisseurs             |
| employes                           |
| evaluations_clients                |
| factures                           |
| fournisseurs                       |
| interactions_clients               |
| lignes_commandes_fournisseurs      |
| lignes_factures                    |
| mouvements_stock                   |
| niveaux_stock                      |
| paiements                          |
| paies                              |
| parametres_taxes                   |
| produits                           |
| vue_statistiques_clients           |
+-------------------------------------+
```

### 3. Vérifier les triggers

```sql
SHOW TRIGGERS;
```

Résultat attendu : **2 triggers**
- `trg_evaluation_alerte_satisfaction`
- `trg_facture_update_statut_client`

### 4. Vérifier les procédures stockées

```sql
SHOW PROCEDURE STATUS WHERE Db = 'NordikAdventuresERP';
```

Résultat attendu : **4 procédures**
- `sp_generer_numero_facture`
- `sp_generer_numero_commande`
- `sp_marquer_clients_inactifs`
- `sp_cloture_campagne`

### 5. Vérifier les données initiales

```sql
-- Catégories
SELECT COUNT(*) AS nb_categories FROM categories;
-- Résultat attendu: 4

-- Fournisseurs
SELECT COUNT(*) AS nb_fournisseurs FROM fournisseurs;
-- Résultat attendu: 2

-- Employés
SELECT COUNT(*) AS nb_employes FROM employes;
-- Résultat attendu: 2

-- Clients
SELECT COUNT(*) AS nb_clients FROM clients;
-- Résultat attendu: 2

-- Taxes
SELECT * FROM parametres_taxes;
-- Résultat attendu: TPS (5%) et TVQ (9.975%)
```

---

## 📊 STRUCTURE DE LA BASE DE DONNÉES

### Modules inclus

| Module | Tables | Description |
|--------|--------|-------------|
| **RH** | 2 | employes, paies |
| **Stocks** | 5 | categories, fournisseurs, produits, niveaux_stock, mouvements_stock |
| **Clients** | 1 | clients |
| **Finances** | 6 | parametres_taxes, factures, lignes_factures, paiements, commandes_fournisseurs, lignes_commandes_fournisseurs |
| **CRM** | 4 | interactions_clients, evaluations_clients, campagnes_marketing, alertes_service_client |
| **Vues** | 1 | vue_statistiques_clients |

**Total : 19 tables + 1 vue = 20 objets**

---

## ⚙️ AUTOMATISATIONS ACTIVES

Dès l'installation, les automatisations suivantes sont **actives** :

### 1. Changement automatique de statut client
- **Prospect → Actif** : Après la 1ère commande
- **Actif → Fidèle** : Après >5 commandes OU >3000$ CA
- Trigger : `trg_facture_update_statut_client`

### 2. Interaction automatique lors de vente
- Chaque nouvelle facture crée une interaction CRM
- Type : "Vente"
- Trigger : `trg_facture_update_statut_client`

### 3. Alerte automatique satisfaction faible
- Note ≤ 2 → Création automatique d'une alerte
- Priorité : Urgente (note=1) ou Haute (note=2)
- Trigger : `trg_evaluation_alerte_satisfaction`

### 4. Calcul automatique des KPIs
- Vue : `vue_statistiques_clients`
- Calcul en temps réel du score composite
- Mise à jour automatique

---

## 🔧 CONFIGURATION DE L'APPLICATION C#

### Connexion à la base de données

Dans `DatabaseHelper.cs`, vérifier/modifier la chaîne de connexion :

```csharp
private static string connectionString = 
    "Server=localhost;Database=NordikAdventuresERP;User ID=root;Password=VOTRE_MOT_DE_PASSE;";
```

**Remplacer :**
- `localhost` par votre serveur MySQL (si différent)
- `root` par votre nom d'utilisateur MySQL
- `VOTRE_MOT_DE_PASSE` par votre mot de passe MySQL

---

## 🧪 TESTS APRÈS INSTALLATION

### Test 1 : Créer un client

```sql
INSERT INTO clients (type, nom, courriel_contact, telephone, statut) VALUES
('Particulier', 'Test Client', 'test@client.com', '514-555-9999', 'Prospect');
```

### Test 2 : Créer une facture (devrait changer le statut du client)

```sql
-- Récupérer l'ID du client test
SET @client_id = (SELECT id FROM clients WHERE courriel_contact = 'test@client.com');

-- Créer une facture
CALL sp_generer_numero_facture(@numero);
INSERT INTO factures (numero_facture, client_id, date_echeance, sous_total, montant_total) VALUES
(@numero, @client_id, DATE_ADD(NOW(), INTERVAL 30 DAY), 100.00, 100.00);

-- Vérifier que le statut a changé
SELECT statut FROM clients WHERE id = @client_id;
-- Résultat attendu: 'Actif' (au lieu de 'Prospect')
```

### Test 3 : Créer une évaluation faible (devrait créer une alerte)

```sql
INSERT INTO evaluations_clients (client_id, note_satisfaction, commentaire) VALUES
(@client_id, 2, 'Service très lent');

-- Vérifier qu'une alerte a été créée
SELECT * FROM alertes_service_client WHERE client_id = @client_id;
-- Résultat attendu: 1 alerte avec priorité 'Haute'
```

---

## 🗑️ DÉSINSTALLATION (SI NÉCESSAIRE)

Pour supprimer complètement la base de données :

```sql
DROP DATABASE IF EXISTS NordikAdventuresERP;
```

⚠️ **ATTENTION** : Cette commande supprime **TOUTES** les données de manière **IRRÉVERSIBLE** !

---

## 📁 AUTRES FICHIERS SQL (OPTIONNELS)

Les fichiers SQL individuels sont toujours disponibles si vous souhaitez installer les modules séparément :

| Fichier | Description |
|---------|-------------|
| `NordikAdventuresERP_Schema_FR.sql` | Schéma principal (tables de base) |
| `SQL_Module_Finances.sql` | Module Finances uniquement |
| `SQL_Module_CRM.sql` | Module CRM uniquement |
| `SQL_Schema_Auth_Safe.sql` | Authentification sécurisée |
| `SQL_Produits_NordikAdventures.sql` | Produits d'exemple |

**Recommandation :** Utiliser `SQL_COMPLET_NordikAdventuresERP.sql` pour une installation complète en une seule fois.

---

## 🆘 DÉPANNAGE

### Erreur : "Access denied for user 'root'@'localhost'"
**Solution :** Vérifier le mot de passe MySQL ou créer un nouvel utilisateur :

```sql
CREATE USER 'erp_user'@'localhost' IDENTIFIED BY 'votre_mot_de_passe';
GRANT ALL PRIVILEGES ON NordikAdventuresERP.* TO 'erp_user'@'localhost';
FLUSH PRIVILEGES;
```

### Erreur : "Cannot load from mysql.proc"
**Solution :** Mettre à jour MySQL :

```bash
mysql_upgrade -u root -p
```

### Erreur : "Duplicate entry for key 'PRIMARY'"
**Solution :** La base existe déjà. Supprimer d'abord :

```sql
DROP DATABASE IF EXISTS NordikAdventuresERP;
```

Puis réexécuter le script.

---

## 📞 SUPPORT

Pour toute question ou problème :
1. Consulter `MODULE_CRM_DOCUMENTATION.md`
2. Consulter `MODULE_FINANCES_DOCUMENTATION.md`
3. Vérifier les logs MySQL

---

**Date de création :** 2025-01-28  
**Version :** 1.0  
**Système :** NordikAdventures ERP  
**Compatibilité :** MySQL 8.0+

