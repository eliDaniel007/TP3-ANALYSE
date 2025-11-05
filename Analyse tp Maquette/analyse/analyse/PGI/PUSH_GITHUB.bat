@echo off
echo ========================================
echo Push vers GitHub - PGI Nordik Adventures
echo ========================================
echo.

cd /d "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"

echo [1/5] Configuration Git...
git config user.name "elida"
git config user.email "votre-email@exemple.com"

echo.
echo [2/5] Ajout des fichiers modifies...
git add .

echo.
echo [3/5] Commit des changements...
git commit -m "feat: Authentification complete avec MySQL - Employes et Clients

- Authentification employees et clients avec base de donnees MySQL
- Separation des acces: Employes -> PGI / Clients -> Site d'achat
- Validation email client (doit contenir 'client')
- Bouton afficher/cacher mot de passe (oeil)
- Scripts SQL corriges avec valeurs ENUM correctes
- Correction nullabilite C# (Models, Services, DatabaseHelper)
- Correction DatabaseHelper.cs avec mot de passe MySQL
- 4 employes de test + 5 clients de test
- Documentation complete (AUTHENTIFICATION.md, INSTRUCTIONS_BDD.md, VALEURS_ENUM.md)

Identifiants de test:
Employes:
- admin@nordikadventures.com / admin123
- gestionnaire@nordikadventures.com / gestionnaire123
- employe@nordikadventures.com / employe123
- comptable@nordikadventures.com / comptable123

Clients:
- jean.client@test.com / client123
- marie.client@test.com / client123
- pierre.client@entreprise.com / client123
- client.sophie@gmail.com / client123
- contact@nordikclient.com / client123"

echo.
echo [4/5] Push vers GitHub...
git push origin main

echo.
echo [5/5] Verification...
git status

echo.
echo ========================================
echo Push termine avec succes!
echo ========================================
echo.
pause

