@echo off
echo ========================================
echo Push vers GitHub - Produits et MySQL
echo ========================================
echo.

cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"

echo [1/4] Ajout des fichiers modifies...
git add .

echo.
echo [2/4] Commit avec message detaille...
git commit -F "COMMIT_FINAL.txt"

echo.
echo [3/4] Push vers GitHub...
git push origin main

echo.
echo [4/4] Verification...
git status

echo.
echo ========================================
echo Push termine avec succes !
echo ========================================
pause

