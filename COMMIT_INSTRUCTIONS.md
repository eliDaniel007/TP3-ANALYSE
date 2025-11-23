# 📝 INSTRUCTIONS POUR LE COMMIT

## 🎯 Commandes Git à exécuter

Ouvrez un terminal (Git Bash, CMD ou PowerShell) dans le dossier du projet et exécutez :

### Option 1 : Commit simple (recommandé)

```bash
git add .
git commit -m "feat: Nettoyage complet du projet et correction erreur XAML

- Suppression de 26 fichiers obsolètes
- SQL unifié en 1 seul fichier
- Documentation professionnelle créée
- Correction erreur XML ClientDetailsWindow.xaml
- Structure projet optimisée"
```

### Option 2 : Commit détaillé

```bash
git add .

git commit -m "feat: Nettoyage complet du projet et correction erreur XAML

- Suppression de 26 fichiers obsolètes (SQL redondants, anciennes fenêtres, docs doublons)
- SQL unifié en 1 seul fichier (SQL_COMPLET_NordikAdventuresERP.sql)
- Création de documentation professionnelle (README.md, guides, rapports)
- Correction erreur XML dans ClientDetailsWindow.xaml (balise Grid/StackPanel)
- Structure projet réorganisée et optimisée
- Prêt pour production et GitHub"
```

### Option 3 : Vérifier avant de commiter

```bash
# Voir les fichiers modifiés
git status

# Voir les différences
git diff

# Ajouter tous les fichiers
git add .

# Créer le commit
git commit -m "feat: Nettoyage complet du projet et correction erreur XAML"
```

---

## 📋 Résumé des changements

### Fichiers supprimés (26)
- 6 fichiers obsolètes à la racine
- 6 fichiers SQL redondants
- 14 fichiers dans PGI (anciennes fenêtres, debug)

### Fichiers créés/modifiés
- README.md (mis à jour)
- Documentation complète (8 nouveaux fichiers)
- ClientDetailsWindow.xaml (correction erreur XML)

### Structure
- SQL unifié : 7 fichiers → 2 fichiers (-71%)
- Documentation consolidée
- Code source propre

---

## ✅ Après le commit

Si vous voulez pousser vers GitHub :

```bash
git push origin main
```

ou

```bash
git push origin master
```

(dépend de votre branche principale)

---

**Note :** Si vous préférez, vous pouvez aussi utiliser l'interface graphique de Visual Studio ou GitHub Desktop pour faire le commit.

