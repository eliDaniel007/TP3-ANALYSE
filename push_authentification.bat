@echo off
echo ========================================
echo   Push - Authentification Employe/Client
echo ========================================
echo.

echo [1/4] Ajout des fichiers...
git add .

echo.
echo [2/4] Creation du commit avec identifiants de test...
git commit -m "feat: Authentification separee employes et clients

🔐 SYSTEME D'AUTHENTIFICATION DUAL

MODIFICATIONS:
- RegisterWindow: Formulaire simplifie pour clients uniquement
- LoginWindow: Distinction employe vs client avec redirection
- ClientShoppingWindow: Nouvelle fenetre site d'achat pour clients

IDENTIFIANTS DE TEST - EMPLOYES (Acces PGI):
===========================================
Username: admin          | Mot de passe: admin123          | Role: Administrateur
Username: gestionnaire   | Mot de passe: gestionnaire123  | Role: Gestionnaire
Username: employe        | Mot de passe: employe123        | Role: Employe Ventes
Username: comptable      | Mot de passe: comptable123      | Role: Comptable

IDENTIFIANTS DE TEST - CLIENTS (Acces Site d'achat):
===================================================
Email: client1@test.com | Mot de passe: client123 | Nom: Jean Dupont
Email: client2@test.com | Mot de passe: client123 | Nom: Marie Martin
Email: client3@test.com | Mot de passe: client123 | Nom: Pierre Tremblay

FICHIERS CREES:
- ClientShoppingWindow.xaml / .cs
- SQL_Utilisateurs_Test.sql
- IDENTIFIANTS_TEST.md
- MODIFICATIONS_AUTHENTIFICATION.md

FICHIERS MODIFIES:
- RegisterWindow.xaml / .cs
- LoginWindow.xaml.cs"

echo.
echo [3/4] Verification du remote...
git remote -v

echo.
echo [4/4] Push vers GitHub...
git push -u origin main

echo.
echo ========================================
echo   Push termine avec succes !
echo   Verifiez sur: https://github.com/eliDaniel007/TP3-ANALYSE
echo ========================================
pause

