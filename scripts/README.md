# 📜 Scripts - Nordik Adventures ERP

Ce dossier contient les scripts batch pour automatiser les tâches répétitives.

---

## 📋 Scripts Disponibles

| Script | Description | Utilisation |
|--------|-------------|-------------|
| **push_produits.bat** | Push automatique vers GitHub avec message détaillé | Double-cliquer |
| **push_to_github.bat** | Push alternatif vers GitHub | Double-cliquer |
| **reorganiser.bat** | Réorganise la structure du projet | Double-cliquer |

---

## 🚀 push_produits.bat

**Description :**  
Script automatique pour pusher le projet vers GitHub avec un message de commit détaillé.

**Actions :**
1. Ajoute tous les fichiers modifiés (`git add .`)
2. Crée un commit avec le message de `docs/COMMIT_FINAL.txt`
3. Push vers GitHub (`git push origin main`)
4. Affiche le statut Git

**Utilisation :**
```bash
# Double-cliquer sur le fichier, ou :
cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"
scripts\push_produits.bat
```

**Prérequis :**
- Git installé
- Repository GitHub configuré
- Identifiants Git configurés

---

## 🔄 push_to_github.bat

**Description :**  
Script alternatif pour pusher vers GitHub (sans message de commit prédéfini).

**Actions :**
1. Ajoute tous les fichiers modifiés
2. Demande le message de commit
3. Push vers GitHub

**Utilisation :**
```bash
# Double-cliquer sur le fichier, ou :
cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"
scripts\push_to_github.bat
```

---

## 📁 reorganiser.bat

**Description :**  
Réorganise la structure du projet en créant des dossiers dédiés.

**Actions :**
1. Crée les dossiers `docs/`, `sql_scripts/`, `scripts/`, `assets/`
2. Déplace la documentation dans `docs/`
3. Déplace les scripts SQL dans `sql_scripts/`
4. Déplace les scripts batch dans `scripts/`
5. Déplace les fichiers divers dans `assets/`

**Utilisation :**
```bash
# Double-cliquer sur le fichier, ou :
cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"
reorganiser.bat
```

**Résultat :**
```
TP3-ANALYSE/
├── docs/              - Documentation
├── sql_scripts/       - Scripts SQL
├── scripts/           - Scripts batch
├── assets/            - Images et fichiers divers
├── Analyse tp Maquette/ - Code source
├── README.md
└── .gitignore
```

---

## 🛠️ Créer Vos Propres Scripts

### Exemple : Script de Nettoyage

```batch
@echo off
echo Nettoyage des fichiers temporaires...

cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"
cd "Analyse tp Maquette\analyse\analyse\PGI"

echo Suppression de bin/ et obj/...
rmdir /s /q bin 2>nul
rmdir /s /q obj 2>nul

echo Nettoyage termine !
pause
```

### Exemple : Script de Build

```batch
@echo off
echo Build du projet...

cd "C:\Users\elida\OneDrive\Bureau\cette fois ci j'ai reussi"
cd "Analyse tp Maquette\analyse\analyse"

echo Restauration des packages NuGet...
dotnet restore PGI.sln

echo Build en mode Release...
dotnet build PGI.sln -c Release

echo Build termine !
pause
```

---

## 📚 Documentation Git

### Commandes Git Utiles

```bash
# Voir le statut
git status

# Ajouter tous les fichiers
git add .

# Commit avec message
git commit -m "Message de commit"

# Push vers GitHub
git push origin main

# Pull depuis GitHub
git pull origin main

# Voir l'historique
git log --oneline

# Annuler les modifications non commitées
git checkout .

# Créer une nouvelle branche
git checkout -b nouvelle-branche
```

---

## 🆘 Dépannage

### Erreur : "git n'est pas reconnu"
**Solution :** Installer Git pour Windows : https://git-scm.com/download/win

### Erreur : "Permission denied (publickey)"
**Solution :** Configurer une clé SSH ou utiliser HTTPS avec token

### Erreur : "fatal: not a git repository"
**Solution :** Initialiser le repository :
```bash
git init
git remote add origin https://github.com/eliDaniel007/TP3-ANALYSE.git
```

### Erreur : "Updates were rejected"
**Solution :** Pull avant de push :
```bash
git pull origin main --rebase
git push origin main
```

---

## 🔧 Configuration Git

### Première Utilisation

```bash
# Configurer nom et email
git config --global user.name "Votre Nom"
git config --global user.email "votre.email@exemple.com"

# Vérifier la configuration
git config --list
```

### Ignorer des Fichiers

Éditer le fichier `.gitignore` à la racine du projet :

```
# Fichiers compilés
bin/
obj/
*.exe
*.dll

# Fichiers temporaires
*.tmp
*.log
*.bak

# Visual Studio
.vs/
*.user
*.suo

# MySQL
*.mwb.bak
```

---

## 📝 Bonnes Pratiques

1. **Commits fréquents** : Commiter régulièrement avec des messages clairs
2. **Messages descriptifs** : `"Ajout module CRM"` plutôt que `"modifications"`
3. **Pull avant push** : Toujours pull avant de push pour éviter les conflits
4. **Branches** : Utiliser des branches pour les nouvelles fonctionnalités
5. **Tests** : Tester avant de commiter
6. **`.gitignore`** : Ne jamais commiter `bin/`, `obj/`, fichiers temporaires

---

**Retour au README principal : [../README.md](../README.md)**

