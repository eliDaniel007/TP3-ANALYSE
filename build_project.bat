@echo off
cd /d "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi\Analyse tp\analyse\PGI"

echo ========================================
echo    COMPILATION DU PROJET - SQL/MySQL
echo ========================================
echo.

echo [INFO] Configuration: MySql.Data (ADO.NET) - Pas d'Entity Framework
echo.

echo [1/3] Nettoyage des fichiers de compilation...
dotnet clean
echo.

echo [2/3] Restauration des packages NuGet (MySql.Data uniquement)...
dotnet restore
echo.

echo [3/3] Compilation du projet...
dotnet build
echo.

if %ERRORLEVEL% EQU 0 (
    echo ========================================
    echo    COMPILATION REUSSIE!
    echo ========================================
    echo.
    echo Package installe: MySql.Data v9.1.0
    echo Fichier de configuration: Helpers/DatabaseHelper.cs
    echo.
    echo N'oubliez pas de configurer votre mot de passe MySQL!
    echo.
) else (
    echo ========================================
    echo    ERREUR DE COMPILATION
    echo ========================================
    echo.
    echo Consultez les erreurs ci-dessus pour plus de details.
    echo.
)

pause

