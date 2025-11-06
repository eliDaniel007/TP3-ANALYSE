# 📋 Valeurs ENUM du Schéma - Référence Rapide

## 📌 Table: `employes`

### `departement`
```sql
ENUM('Administration', 'Ventes', 'Comptabilité', 'Logistique', 'RH', 'IT', 'Autre')
```

**Valeurs valides :**
- `Administration` - Direction, gestion
- `Ventes` - Équipe commerciale
- `Comptabilité` - Finances, comptabilité
- `Logistique` - Entrepôt, stocks, achats
- `RH` - Ressources humaines
- `IT` - Informatique, support technique
- `Autre` - Autres départements

---

### `role_systeme`
```sql
ENUM('Admin', 'Gestionnaire', 'Employé Ventes', 'Comptable', 'Employé')
```

**Valeurs valides :**
- `Admin` - Administrateur (accès complet)
- `Gestionnaire` - Gestionnaire (stocks, achats)
- `Employé Ventes` - Employé ventes (commandes, clients)
- `Comptable` - Comptable (finances, rapports)
- `Employé` - Employé standard (accès limité)

---

### `statut` (employes)
```sql
ENUM('Actif', 'Congé', 'Inactif')
```

**Valeurs valides :**
- `Actif` - Employé actif
- `Congé` - Employé en congé
- `Inactif` - Employé inactif/parti

---

## 📌 Table: `clients`

### `type`
```sql
ENUM('Particulier', 'Entreprise')
```

**Valeurs valides :**
- `Particulier` - Client particulier
- `Entreprise` - Client entreprise

---

### `statut` (clients)
```sql
ENUM('Actif', 'Inactif', 'Prospect', 'Fidèle')
```

**Valeurs valides :**
- `Actif` - Client actif
- `Inactif` - Client inactif
- `Prospect` - Client potentiel
- `Fidèle` - Client fidèle

---

## 📌 Table: `produits`

### `statut` (produits)
```sql
ENUM('Actif', 'Inactif')
```

**Valeurs valides :**
- `Actif` - Produit actif
- `Inactif` - Produit inactif

---

## 📌 Table: `niveaux_stock`

### `emplacement`
```sql
VARCHAR(100) NOT NULL DEFAULT 'Entrepôt Principal'
```

**Exemples de valeurs :**
- `Entrepôt Principal`
- `Magasin Centre-Ville`
- `Magasin Rive-Sud`
- `Entrepôt Réserve`

---

## 📌 Table: `mouvements_stock`

### `type_mouvement`
```sql
ENUM('ENTREE', 'SORTIE')
```

**Valeurs valides :**
- `ENTREE` - Entrée en stock
- `SORTIE` - Sortie de stock

---

### `raison`
```sql
ENUM('reception_achat', 'vente', 'ajustement', 'retour_entree', 'retour_sortie', 'manuel')
```

**Valeurs valides :**
- `reception_achat` - Réception d'un achat fournisseur
- `vente` - Vente à un client
- `ajustement` - Ajustement manuel
- `retour_entree` - Retour client (entrée)
- `retour_sortie` - Retour fournisseur (sortie)
- `manuel` - Mouvement manuel

---

## 📌 Table: `commandes_vente`

### `statut` (commandes_vente)
```sql
ENUM('Brouillon', 'Confirmée', 'Facturée', 'Annulée')
```

**Valeurs valides :**
- `Brouillon` - Commande en cours de création
- `Confirmée` - Commande confirmée
- `Facturée` - Commande facturée
- `Annulée` - Commande annulée

---

## 📌 Table: `factures`

### `statut` (factures)
```sql
ENUM('Brouillon', 'Émise', 'Payée', 'PayéePartiellement', 'Annulée')
```

**Valeurs valides :**
- `Brouillon` - Facture en brouillon
- `Émise` - Facture émise
- `Payée` - Facture payée
- `PayéePartiellement` - Facture partiellement payée
- `Annulée` - Facture annulée

---

## 📌 Table: `paiements`

### `methode`
```sql
ENUM('Carte', 'Interac', 'Comptant', 'VirementBancaire', 'Autre')
```

**Valeurs valides :**
- `Carte` - Carte de crédit/débit
- `Interac` - Interac
- `Comptant` - Argent comptant
- `VirementBancaire` - Virement bancaire
- `Autre` - Autre méthode

---

## 📌 Table: `achats`

### `statut` (achats)
```sql
ENUM('Brouillon', 'Commandé', 'Reçu', 'Annulé')
```

**Valeurs valides :**
- `Brouillon` - Commande en brouillon
- `Commandé` - Commande envoyée au fournisseur
- `Reçu` - Marchandise reçue
- `Annulé` - Commande annulée

---

## 📌 Table: `depenses`

### `categorie`
```sql
ENUM('Salaire', 'Loyer', 'Électricité', 'Internet', 'Marketing', 'Autre')
```

**Valeurs valides :**
- `Salaire` - Salaires et paies
- `Loyer` - Loyer des locaux
- `Électricité` - Factures d'électricité
- `Internet` - Services internet
- `Marketing` - Dépenses marketing
- `Autre` - Autres dépenses

---

### `statut` (depenses)
```sql
ENUM('En attente', 'Payée', 'Annulée')
```

**Valeurs valides :**
- `En attente` - Dépense en attente de paiement
- `Payée` - Dépense payée
- `Annulée` - Dépense annulée

---

## 📌 Table: `paies`

### `periode_type`
```sql
ENUM('Hebdomadaire', 'Bimensuelle', 'Mensuelle')
```

**Valeurs valides :**
- `Hebdomadaire` - Paie hebdomadaire
- `Bimensuelle` - Paie aux 2 semaines
- `Mensuelle` - Paie mensuelle

---

### `statut` (paies)
```sql
ENUM('Brouillon', 'Validée', 'Payée', 'Annulée')
```

**Valeurs valides :**
- `Brouillon` - Paie en brouillon
- `Validée` - Paie validée
- `Payée` - Paie payée
- `Annulée` - Paie annulée

---

## 📌 Table: `interactions_clients`

### `canal`
```sql
ENUM('courriel', 'appel', 'rencontre', 'systeme', 'autre')
```

**Valeurs valides :**
- `courriel` - Interaction par courriel
- `appel` - Appel téléphonique
- `rencontre` - Rencontre en personne
- `systeme` - Interaction système (automatique)
- `autre` - Autre canal

---

## 🔧 Utilisation dans les INSERT

### ✅ Correct
```sql
INSERT INTO employes (departement, role_systeme, statut, ...)
VALUES ('Administration', 'Admin', 'Actif', ...);
```

### ❌ Incorrect (valeurs invalides)
```sql
INSERT INTO employes (departement, role_systeme, statut, ...)
VALUES ('Direction', 'Administrateur', 'Actif', ...);
-- Erreur: 'Direction' et 'Administrateur' ne sont pas des valeurs ENUM valides
```

---

## 📝 Notes Importantes

1. **Sensibilité à la casse** : Les valeurs ENUM sont **sensibles à la casse**
   - ✅ `'Admin'` (correct)
   - ❌ `'admin'` (incorrect)
   - ❌ `'ADMIN'` (incorrect)

2. **Valeur par défaut** : Si une colonne ENUM a une valeur par défaut, elle sera utilisée si aucune valeur n'est fournie

3. **Modification des ENUM** : Pour ajouter/supprimer des valeurs, il faut modifier la structure de la table avec `ALTER TABLE`

---

**Référence complète des valeurs ENUM du schéma NordikAdventuresERP** ✅

