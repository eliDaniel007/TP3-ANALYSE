# ⚡ Guide Rapide - Module Finances

## 🚀 Installation en 3 étapes

### Étape 1: Base de données (2 minutes)

```bash
# Ouvrir MySQL et exécuter
mysql -u root -p < "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\sql_scripts\SQL_Module_Finances.sql"
```

✅ Cela crée 7 tables + triggers + procédures stockées

### Étape 2: Ajouter la référence Visual Basic (1 minute)

Ajoutez au fichier `PGI.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="MySql.Data" Version="9.1.0" />
  <PackageReference Include="Microsoft.VisualBasic" Version="10.3.0" />
</ItemGroup>
```

### Étape 3: Compiler (1 minute)

```bash
cd "Analyse tp\analyse\PGI"
dotnet restore
dotnet build
```

---

## ✅ Test rapide

### Test 1: Voir les factures

1. Lancez l'application
2. Menu **Finances** → **Ventes/Factures**
3. Vous devriez voir une liste (vide si aucune facture)

### Test 2: Créer une facture via code

Ajoutez ce bouton de test temporaire n'importe où:

```csharp
private void BtnTestFacture_Click(object sender, RoutedEventArgs e)
{
    try
    {
        // Vérifier qu'un client et un produit existent
        var clients = ClientService.GetAllClients();
        var produits = ProduitService.GetAllProduits();
        
        if (clients.Count == 0 || produits.Count == 0)
        {
            MessageBox.Show("Créez d'abord un client et un produit!");
            return;
        }
        
        var client = clients[0];
        var produit = produits[0];
        
        // Générer le numéro
        string numero = FactureService.GenererNumeroFacture();
        
        // Créer la facture
        var facture = new Facture
        {
            NumeroFacture = numero,
            DateFacture = DateTime.Now,
            DateEcheance = DateTime.Now.AddDays(30),
            ClientId = client.Id,
            EmployeId = null
        };
        
        var lignes = new List<LigneFacture>
        {
            new LigneFacture
            {
                ProduitId = produit.Id,
                SKU = produit.SKU,
                Description = produit.Nom,
                Quantite = 1,
                PrixUnitaire = produit.Prix
            }
        };
        
        int id = FactureService.CreerFacture(facture, lignes);
        MessageBox.Show($"✅ Facture {numero} créée! ID: {id}");
    }
    catch (Exception ex)
    {
        MessageBox.Show($"❌ Erreur: {ex.Message}");
    }
}
```

---

## 🎯 Fonctionnalités disponibles maintenant

### ✅ Liste des factures
- Voir toutes les factures de la BDD
- Filtrer par statut (Payée, Impayée, En retard, etc.)
- Couleurs par statut

### ✅ Paiements
- Enregistrer un paiement
- Validation automatique (montant ≤ montant dû)
- Mise à jour du statut automatique

### ✅ Annulation
- Annuler une facture impayée
- Saisir un motif
- Remise en stock automatique

### ✅ Backend complet
- `FactureService` - toutes les opérations sur factures
- `PaiementService` - gestion des paiements
- `TaxesService` - calcul TPS/TVQ
- `CommandeFournisseurService` - commandes fournisseurs
- `RapportFinancierService` - rapports financiers

---

## ⏳ À intégrer prochainement

1. **Formulaire de création de facture** (interface graphique)
2. **Commandes fournisseurs** (interface graphique)
3. **Rapports** (interface graphique)
4. **Tableau de bord** (KPIs + graphiques)

---

## 🐛 Dépannage

### Erreur: "Table factures doesn't exist"
➡️ Exécutez le script SQL

### Erreur: "Microsoft.VisualBasic not found"
➡️ Ajoutez le package NuGet dans PGI.csproj

### Erreur: "Client introuvable"
➡️ Créez au moins un client dans le module CRM

### Erreur: "Stock insuffisant"
➡️ C'est normal ! La validation fonctionne. Ajoutez du stock au produit.

---

## 📚 Documentation complète

- `MODULE_FINANCES_DOCUMENTATION.md` - Documentation détaillée
- `INTEGRATION_MODULE_FINANCES.md` - État d'avancement
- `sql_scripts/SQL_Module_Finances.sql` - Script de base de données

---

**Prêt à utiliser! 🎉**

