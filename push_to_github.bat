@echo off
echo ========================================
echo   NordikAdventuresERP - Push to GitHub
echo ========================================
echo.

echo [1/6] Initialisation du depot Git...
git init
if %errorlevel% neq 0 (
    echo ERREUR: Git n'est pas installe ou n'est pas dans le PATH
    pause
    exit /b 1
)

echo.
echo [2/6] Ajout du fichier .gitignore...
git add .gitignore

echo.
echo [3/6] Ajout de tous les fichiers du projet...
git add .

echo.
echo [4/6] Creation du commit initial...
git commit -m "Initial commit - NordikAdventuresERP (PGI complet: WPF + MySQL)"

echo.
echo [5/6] Ajout du remote GitHub...
git remote add origin https://github.com/eliDaniel007/TP3-ANALYSE.git

echo.
echo [6/6] Poussee vers GitHub...
git branch -M main
git push -u origin main

echo.
echo ========================================
echo   Projet pousse sur GitHub avec succes !
echo   Verifiez sur: https://github.com/eliDaniel007/TP3-ANALYSE
echo ========================================
pause

