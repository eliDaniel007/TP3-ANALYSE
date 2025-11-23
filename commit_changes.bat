@echo off
echo ========================================
echo   COMMIT DES CHANGEMENTS
echo ========================================
echo.

echo [1/3] Verification de l'etat Git...
git status
echo.

echo [2/3] Ajout de tous les fichiers modifies...
git add .
echo.

echo [3/3] Creation du commit...
git commit -m "feat: Nettoyage complet du projet et correction erreur XAML

- Suppression de 26 fichiers obsolètes (SQL redondants, anciennes fenêtres, docs doublons)
- SQL unifié en 1 seul fichier (SQL_COMPLET_NordikAdventuresERP.sql)
- Création de documentation professionnelle (README.md, guides, rapports)
- Correction erreur XML dans ClientDetailsWindow.xaml (balise Grid/StackPanel)
- Structure projet réorganisée et optimisée
- Prêt pour production et GitHub"

echo.
echo ========================================
echo   COMMIT TERMINE AVEC SUCCES !
echo ========================================
echo.
git log -1 --oneline
echo.
pause

