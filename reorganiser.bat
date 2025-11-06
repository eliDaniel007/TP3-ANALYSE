@echo off
echo ========================================
echo Reorganisation de la structure du projet
echo ========================================
echo.

cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"

echo [1/7] Creation des dossiers...
if not exist "docs" mkdir "docs"
if not exist "sql_scripts" mkdir "sql_scripts"
if not exist "scripts" mkdir "scripts"
if not exist "assets" mkdir "assets"

echo.
echo [2/7] Deplacement des fichiers de documentation...
if exist "NETTOYAGE_EFFECTUE.md" move "NETTOYAGE_EFFECTUE.md" "docs\"
if exist "PUSH_GITHUB_INSTRUCTIONS.md" move "PUSH_GITHUB_INSTRUCTIONS.md" "docs\"
if exist "COMMIT_FINAL.txt" move "COMMIT_FINAL.txt" "docs\"

echo.
echo [3/7] Deplacement des scripts SQL...
if exist "NordikAdventuresERP_Schema_FR.sql" move "NordikAdventuresERP_Schema_FR.sql" "sql_scripts\"
if exist "Analyse tp Maquette\analyse\analyse\PGI\SQL_Produits_NordikAdventures.sql" move "Analyse tp Maquette\analyse\analyse\PGI\SQL_Produits_NordikAdventures.sql" "sql_scripts\"
if exist "Analyse tp Maquette\analyse\analyse\PGI\SQL_Schema_Auth_Safe.sql" move "Analyse tp Maquette\analyse\analyse\PGI\SQL_Schema_Auth_Safe.sql" "sql_scripts\"

echo.
echo [4/7] Deplacement des scripts batch...
if exist "push_produits.bat" move "push_produits.bat" "scripts\"
if exist "push_to_github.bat" move "push_to_github.bat" "scripts\"

echo.
echo [5/7] Deplacement des fichiers divers...
if exist "iiiooo.png" move "iiiooo.png" "assets\"
if exist "NordikAdventures - Liste des produits PGI.xlsx" move "NordikAdventures - Liste des produits PGI.xlsx" "assets\"
if exist "schema 2.0.mwb" move "schema 2.0.mwb" "assets\"

echo.
echo [6/7] Mise a jour du README.md...
echo Ajout des liens vers les nouveaux dossiers...

echo.
echo [7/7] Reorganisation terminee !
echo.
echo ========================================
echo Structure finale :
echo ========================================
echo docs\               - Documentation (3 fichiers + README)
echo sql_scripts\        - Scripts SQL (3 fichiers + README)
echo scripts\            - Scripts batch (3 fichiers + README)
echo assets\             - Images et fichiers divers (3 fichiers + README)
echo Analyse tp Maquette\ - Code source PGI
echo README.md           - Guide principal (mis a jour)
echo .gitignore          - Exclusions Git
echo ========================================
echo.
echo IMPORTANT : Ne pas oublier de :
echo 1. Mettre a jour les chemins dans les scripts batch
echo 2. Mettre a jour les liens dans le README principal
echo 3. Git add + commit + push pour sauvegarder
echo.
echo Appuyez sur une touche pour continuer...
pause >nul

