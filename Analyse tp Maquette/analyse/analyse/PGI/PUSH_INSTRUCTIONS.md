# 📤 Instructions Push GitHub - PGI Nordik Adventures

## 🎯 Méthode Simple (Recommandée)

### Ouvrir Git Bash (pas PowerShell !)

1. **Clic droit** dans le dossier `C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi`
2. Sélectionner **"Git Bash Here"**
3. Copier-coller les commandes ci-dessous

---

## 📋 Commandes à Exécuter

```bash
# 1. Configuration (à faire une seule fois)
git config user.name "elida"
git config user.email "votre-email@exemple.com"

# 2. Ajouter tous les fichiers
git add .

# 3. Commit avec message détaillé
git commit -m "feat: Authentification complete avec MySQL

- Authentification employees et clients avec BDD MySQL
- Separation des acces: Employes -> PGI / Clients -> Site achat
- Validation email client (doit contenir 'client')
- Bouton afficher/cacher mot de passe
- Scripts SQL corriges avec valeurs ENUM
- Correction nullabilite C# (Models, Services)
- Configuration DatabaseHelper avec mot de passe MySQL
- 4 employes de test + 5 clients de test
- Documentation complete

Identifiants test:
- admin@nordikadventures.com / admin123
- jean.client@test.com / client123"

# 4. Push vers GitHub
git push origin main

# 5. Vérifier
git status
```

---

## 🔍 Si Erreur "failed to push"

### Cas 1 : Branche "master" au lieu de "main"
```bash
git push origin master
```

### Cas 2 : Besoin de créer la branche
```bash
git branch -M main
git push -u origin main
```

### Cas 3 : Repository pas configuré
```bash
git remote -v
# Si vide, ajouter le remote:
git remote add origin https://github.com/VOTRE-USERNAME/VOTRE-REPO.git
git push -u origin main
```

---

## 📊 Fichiers qui Seront Poussés

### Nouveaux Fichiers (Authentification)
```
✅ Services/EmployeService.cs
✅ Services/ClientService.cs
✅ Models/Employe.cs
✅ SQL_Schema_Auth.sql
✅ SQL_Schema_Auth_Safe.sql
✅ AUTHENTIFICATION.md
✅ INSTRUCTIONS_BDD.md
✅ VALEURS_ENUM.md
✅ GUIDE_RAPIDE_SQL.md
✅ RECAPITULATIF_AUTHENTIFICATION.md
✅ README_AUTHENTIFICATION.md
✅ .editorconfig
```

### Fichiers Modifiés
```
✅ Models/Client.cs
✅ Models/Fournisseur.cs
✅ Models/Produit.cs
✅ Models/Categorie.cs
✅ Models/MouvementStock.cs
✅ LoginWindow.xaml.cs
✅ RegisterWindow.xaml.cs
✅ Helpers/DatabaseHelper.cs
✅ Services/ProduitService.cs
✅ Views/Stocks/StocksDashboardView.xaml
✅ DEVELOPPEMENT_PROGRES.md
```

---

## 🎯 Vérifier le Push

### Sur GitHub
1. Aller sur votre repo GitHub
2. Vérifier que les nouveaux fichiers sont présents
3. Lire le commit message

### En local
```bash
git log --oneline -5
```

---

## 🆘 Besoin d'Aide ?

### Voir l'état actuel
```bash
git status
```

### Voir les fichiers modifiés
```bash
git diff
```

### Voir l'historique
```bash
git log --oneline -10
```

### Annuler le dernier commit (si erreur)
```bash
git reset --soft HEAD~1
```

---

## ✅ Checklist Avant Push

- [ ] Tous les fichiers compilent sans erreur (F6)
- [ ] L'application se lance correctement (F5)
- [ ] La connexion MySQL fonctionne
- [ ] Les tests d'authentification passent
- [ ] Le fichier `DatabaseHelper.cs` a le bon mot de passe
- [ ] Les scripts SQL sont corrigés
- [ ] La documentation est à jour

---

## 📝 Message de Commit Détaillé

Si vous voulez un message encore plus détaillé :

```bash
git commit -m "feat: Authentification complete avec MySQL - Employes et Clients

## Fonctionnalites Implementees

### Authentification
- Connexion employes avec email (sans 'client')
- Connexion clients avec email (contenant 'client')
- Inscription clients avec validation email
- Bouton afficher/cacher mot de passe (oeil)
- Redirection intelligente: Employes -> PGI / Clients -> Site achat

### Base de Donnees
- Ajout colonne mot_de_passe dans employes
- Ajout colonne mot_de_passe dans clients
- Scripts SQL avec valeurs ENUM correctes:
  * departement: Administration, Ventes, Comptabilite, Logistique, RH, IT
  * role_systeme: Admin, Gestionnaire, Employe Ventes, Comptable, Employe
- Ajout colonnes salaire_annuel et date_embauche
- 4 employes de test + 5 clients de test

### Corrections Techniques
- Nullabilite C# 8.0 (Models avec string.Empty)
- Gestion valeurs null (Services avec ?. et ??)
- DatabaseHelper cast DBNull.Value
- XAML StocksDashboardView (ajout x:Name)
- Typo StockReserve -> StockReservee
- Configuration mot de passe MySQL: 'password'

### Services
- EmployeService: Authentification employes
- ClientService: Authentification + inscription clients
- ProduitService: Correction StockReservee
- CategorieService: CRUD categories
- FournisseurService: CRUD fournisseurs

### Documentation
- AUTHENTIFICATION.md: Guide complet
- INSTRUCTIONS_BDD.md: Installation MySQL
- VALEURS_ENUM.md: Reference ENUM
- GUIDE_RAPIDE_SQL.md: Guide SQL
- README_AUTHENTIFICATION.md: README mis a jour

### Identifiants Test

Employes:
- admin@nordikadventures.com / admin123 (Admin)
- gestionnaire@nordikadventures.com / gestionnaire123 (Gestionnaire)
- employe@nordikadventures.com / employe123 (Employe Ventes)
- comptable@nordikadventures.com / comptable123 (Comptable)

Clients:
- jean.client@test.com / client123
- marie.client@test.com / client123
- pierre.client@entreprise.com / client123
- client.sophie@gmail.com / client123
- contact@nordikclient.com / client123

## Tests Realises
- Connexion employe: OK
- Connexion client: OK
- Inscription client: OK
- Validation email 'client': OK
- Bouton afficher/cacher mdp: OK
- Redirection PGI/Site: OK
- Connexion MySQL: OK
- KPIs Stocks Dashboard: OK

## Prochaines Etapes
1. Completer Module Stocks (formulaire produit, mouvements)
2. Developper Module Finances
3. Developper Module CRM
4. Developper Site achat Clients"
```

---

**Utilisez Git Bash pour éviter les problèmes PowerShell ! 🚀**

