# 💰 Module Ventes et Finances - Documentation Complète

## 📋 Table des matières
1. [Vue d'ensemble](#vue-densemble)
2. [Architecture](#architecture)
3. [Fonctionnalités implémentées](#fonctionnalités-implémentées)
4. [Validations et Contraintes](#validations-et-contraintes)
5. [Installation et Configuration](#installation-et-configuration)
6. [Utilisation](#utilisation)

---

## 🎯 Vue d'ensemble

Le **Module Ventes et Finances** est un système complet de gestion des factures, paiements, commandes fournisseurs et rapports financiers conforme aux exigences PGI.

### Caractéristiques principales

✅ **Gestion complète des ventes**
- Création de factures avec validation du stock
- Calcul automatique des taxes (TPS/TVQ)
- Numérotation unique et séquentielle
- Gestion des paiements partiels et complets

✅ **Gestion des achats fournisseurs**
- Commandes fournisseurs avec suivi
- Réception partielle ou complète
- Mise à jour automatique du stock

✅ **Rapports financiers**
- Profit brut et profit net
- Top clients et produits
- Rapports périodiques automatiques

✅ **Conformité comptable**
- Taux de taxes paramétrables
- Pas de suppression de factures (annulation uniquement)
- Traçabilité complète

---

## 🏗️ Architecture

### Structure des fichiers

```
PGI/
├── Models/
│   ├── Facture.cs
│   ├── LigneFacture.cs
│   ├── Paiement.cs
│   ├── ParametresTaxes.cs
│   ├── CommandeFournisseur.cs
│   ├── LigneCommandeFournisseur.cs
│   └── RapportFinancier.cs
├── Services/
│   ├── FactureService.cs
│   ├── PaiementService.cs
│   ├── TaxesService.cs
│   ├── CommandeFournisseurService.cs
│   └── RapportFinancierService.cs
└── Views/Finances/
    ├── FinancesMainView.xaml
    ├── FacturesListView.xaml
    ├── FactureFormView.xaml
    ├── PaiementsView.xaml
    └── RapportsView.xaml
```

### Base de données

```
Tables créées:
├── factures
├── lignes_factures
├── paiements
├── parametres_taxes
├── commandes_fournisseurs
├── lignes_commandes_fournisseurs
└── charges_operationnelles
```

---

## ✨ Fonctionnalités implémentées

### 1. Gestion des Factures

#### Création de facture
```csharp
// Générer un numéro unique
string numeroFacture = FactureService.GenererNumeroFacture();
// Format: FAC-2025-0001

// Créer la facture avec lignes
var facture = new Facture { /* ... */ };
var lignes = new List<LigneFacture> { /* ... */ };
int factureId = FactureService.CreerFacture(facture, lignes);
```

**Validations automatiques:**
- ✅ Vérification que le client est actif
- ✅ Vérification des factures en retard du client
- ✅ Vérification du stock disponible pour tous les produits
- ✅ Calcul automatique des taxes (TPS 5% + TVQ 9.975%)
- ✅ Mise à jour automatique du stock
- ✅ Création des mouvements de stock

#### Annulation de facture
```csharp
// Seules les factures impayées peuvent être annulées
bool success = FactureService.AnnulerFacture(factureId, "Motif d'annulation");
// Remet automatiquement les produits en stock
```

### 2. Gestion des Paiements

#### Enregistrer un paiement
```csharp
var paiement = new Paiement
{
    FactureId = 1,
    Montant = 150.00m,
    ModePaiement = "Carte", // Comptant, Carte, Chèque, Virement
    NumeroReference = "REF-12345",
    EmployeId = employeId
};

int paiementId = PaiementService.EnregistrerPaiement(paiement);
```

**Validations automatiques:**
- ✅ Le montant ne peut pas dépasser le montant dû
- ✅ Impossible de payer une facture annulée
- ✅ Mise à jour automatique du statut de paiement (Impayée → Partielle → Payée)

### 3. Taxes Paramétrables

```csharp
// Obtenir les taux actuels
decimal tauxTPS = TaxesService.GetTauxTPS(); // 0.05 (5%)
decimal tauxTVQ = TaxesService.GetTauxTVQ(); // 0.09975 (9.975%)

// Calculer les taxes sur un montant
var (tps, tvq, total) = TaxesService.CalculerTaxes(100.00m);
// tps = 5.00, tvq = 9.98, total = 114.98

// Mettre à jour un taux (gestionnaire uniquement)
bool success = TaxesService.UpdateTaxe(taxeId, 0.06m); // Nouveau taux 6%
```

### 4. Commandes Fournisseurs

#### Créer une commande
```csharp
string numeroCommande = CommandeFournisseurService.GenererNumeroCommande();
// Format: CMD-2025-0001

var commande = new CommandeFournisseur { /* ... */ };
var lignes = new List<LigneCommandeFournisseur> { /* ... */ };
int commandeId = CommandeFournisseurService.CreerCommande(commande, lignes);
```

#### Recevoir une commande
```csharp
// Réception partielle ou complète
var quantitesRecues = new Dictionary<int, int>
{
    { ligneId1, 50 },  // 50 unités reçues pour cette ligne
    { ligneId2, 100 }  // 100 unités reçues pour cette ligne
};

bool success = CommandeFournisseurService.RecevoirCommande(
    commandeId, 
    quantitesRecues, 
    employeId
);
// Met à jour automatiquement le stock et crée les mouvements
```

### 5. Rapports Financiers

#### Générer un rapport
```csharp
// Rapport pour une période spécifique
var rapport = RapportFinancierService.GenererRapport(
    new DateTime(2025, 1, 1),
    new DateTime(2025, 1, 31)
);

Console.WriteLine($"Ventes: {rapport.TotalVentes:C}");
Console.WriteLine($"Profit brut: {rapport.ProfitBrut:C}");
Console.WriteLine($"Profit net: {rapport.ProfitNet:C}");
Console.WriteLine($"Marge: {rapport.MargeProfit}%");

// Rapport mois en cours
var rapportMois = RapportFinancierService.GenererRapportMoisEnCours();

// Rapport année en cours
var rapportAnnee = RapportFinancierService.GenererRapportAnneeEnCours();
```

#### Top clients et produits
```csharp
// Top 5 clients par revenus
var topClients = RapportFinancierService.GetTop5Clients(dateDebut, dateFin);
foreach (var (nom, ventes, nbFactures) in topClients)
{
    Console.WriteLine($"{nom}: {ventes:C} ({nbFactures} factures)");
}

// Top 5 produits par revenus
var topProduits = RapportFinancierService.GetTop5Produits(dateDebut, dateFin);
foreach (var (nom, ventes, quantite) in topProduits)
{
    Console.WriteLine($"{nom}: {ventes:C} ({quantite} unités)");
}
```

---

## 🔒 Validations et Contraintes

### Validation des Ventes

| Règle | Implémentation |
|-------|----------------|
| **Stock disponible** | Vérification avant création de facture |
| **Client actif** | Refus si statut = "Inactif" |
| **Factures en retard** | Refus si le client a des impayés |
| **Calcul automatique** | TPS + TVQ calculées, non modifiables |
| **Numéro unique** | Séquentiel par année (procédure stockée) |

### Validation des Paiements

| Règle | Implémentation |
|-------|----------------|
| **Montant ≤ Montant dû** | Vérification stricte |
| **Mise à jour automatique** | Trigger SQL met à jour le statut |
| **Pas de paiement sur facture annulée** | Validation dans le service |

### Contraintes Comptables

| Règle | Implémentation |
|-------|----------------|
| **Pas de suppression** | Seule l'annulation est possible |
| **Traçabilité** | Tous les mouvements enregistrés |
| **Taxes paramétrables** | Table `parametres_taxes` |
| **Débit = Crédit** | À implémenter si comptabilité en partie double |

### Contraintes Commandes Fournisseurs

| Règle | Implémentation |
|-------|----------------|
| **Fermeture après réception complète** | Statut "Reçue" automatique |
| **Réception partielle** | Statut "Partiellement reçue" |
| **Mise à jour stock** | Automatique lors de la réception |

---

## 🚀 Installation et Configuration

### 1. Exécuter le script SQL

```bash
mysql -u root -p < sql_scripts/SQL_Module_Finances.sql
```

Ce script crée:
- ✅ 7 tables
- ✅ 2 vues SQL
- ✅ 4 triggers automatiques
- ✅ 2 procédures stockées
- ✅ Données de taxes par défaut (TPS 5%, TVQ 9.975%)

### 2. Vérifier la configuration

```sql
-- Vérifier les tables
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'NordikAdventuresERP' 
AND table_name IN ('factures', 'paiements', 'parametres_taxes');

-- Vérifier les taux de taxes
SELECT * FROM parametres_taxes;
```

### 3. Compiler le projet

```bash
dotnet build
```

---

## 📊 Utilisation

### Scénario complet: Créer une vente

```csharp
// 1. Générer le numéro de facture
string numero = FactureService.GenererNumeroFacture();

// 2. Préparer la facture
var facture = new Facture
{
    NumeroFacture = numero,
    DateFacture = DateTime.Now,
    DateEcheance = DateTime.Now.AddDays(30),
    ClientId = 1,
    EmployeId = 1,
    ConditionsPaiement = "Net 30 jours"
};

// 3. Préparer les lignes (avec produits déjà chargés)
var lignes = new List<LigneFacture>
{
    new LigneFacture
    {
        ProduitId = 1,
        SKU = "TENT-001",
        Description = "Tente 4 saisons",
        Quantite = 2,
        PrixUnitaire = 299.99m
    },
    new LigneFacture
    {
        ProduitId = 2,
        SKU = "BAG-001",
        Description = "Sac de couchage -20°C",
        Quantite = 2,
        PrixUnitaire = 149.99m
    }
};

// 4. Créer la facture (avec toutes les validations)
try
{
    int factureId = FactureService.CreerFacture(facture, lignes);
    MessageBox.Show($"Facture {numero} créée avec succès!", "Succès");
    
    // Le stock a été automatiquement mis à jour
    // Les mouvements de stock ont été créés
    // Les taxes ont été calculées
}
catch (Exception ex)
{
    MessageBox.Show($"Erreur: {ex.Message}", "Erreur");
}
```

### Scénario: Enregistrer un paiement

```csharp
// Le client paie 500$ sur une facture de 1000$
var paiement = new Paiement
{
    FactureId = factureId,
    Montant = 500.00m,
    ModePaiement = "Carte",
    NumeroReference = "VISA-****1234",
    Note = "Paiement partiel",
    EmployeId = employeId
};

try
{
    PaiementService.EnregistrerPaiement(paiement);
    // Statut de la facture passe automatiquement à "Partielle"
    // montant_paye = 500, montant_du = 500
}
catch (Exception ex)
{
    MessageBox.Show($"Erreur: {ex.Message}", "Erreur");
}
```

---

## 📈 Calculs Financiers

### Formules implémentées

```
Sous-total = Σ (Quantité × Prix unitaire)
TPS = Sous-total × 0.05
TVQ = Sous-total × 0.09975
Total = Sous-total + TPS + TVQ

Profit Brut = Total Ventes - Coût des Produits Vendus
Profit Net = Profit Brut - Charges Opérationnelles
Marge de Profit = (Profit Net / Total Ventes) × 100
```

### Exemple de calcul

```
Article 1: 2 × 299.99$ = 599.98$
Article 2: 2 × 149.99$ = 299.98$
─────────────────────────────────
Sous-total:              899.96$
TPS (5%):                 45.00$
TVQ (9.975%):             89.75$
─────────────────────────────────
TOTAL:                 1,034.71$
```

---

## 🔧 Personnalisation

### Modifier les taux de taxes

```sql
-- Via SQL
UPDATE parametres_taxes 
SET taux = 0.06 
WHERE nom_taxe = 'TPS';

-- Via le service C#
TaxesService.UpdateTaxe(taxeId, 0.06m);
```

### Ajouter une nouvelle taxe

```sql
INSERT INTO parametres_taxes (nom_taxe, taux, actif, description)
VALUES ('PST', 0.07, TRUE, 'Provincial Sales Tax');
```

---

## ✅ Checklist de conformité

- [x] Vente uniquement si stock suffisant
- [x] Total calculé automatiquement (non modifiable)
- [x] Numéro de facture unique et séquentiel
- [x] Paiement ne peut pas dépasser montant dû
- [x] Commande fermée après réception complète
- [x] TPS (5%) et TVQ (9.975%) calculées automatiquement
- [x] Profit brut et net calculés
- [x] Enregistrement de vente met à jour stock immédiatement
- [x] Paiement met à jour statut de facture
- [x] Réception commande augmente le stock
- [x] Facture impayée ne peut pas être supprimée
- [x] Taux de taxes paramétrables (pas en dur)
- [x] Refus vente client inactif ou en retard

---

## 📞 Support

Pour toute question ou problème:
1. Consultez les logs de la console
2. Vérifiez les contraintes SQL
3. Testez avec des données d'exemple

---

**Version:** 1.0  
**Date:** 23 novembre 2025  
**Base de données:** MySQL 8.0+  
**Framework:** .NET 8.0 + WPF

