#!/bin/bash
# Script pour pousser le projet sur GitHub

echo "🚀 Initialisation du dépôt Git..."
git init

echo "📝 Ajout du fichier .gitignore..."
git add .gitignore

echo "📦 Ajout de tous les fichiers du projet..."
git add .

echo "💾 Création du commit initial..."
git commit -m "Initial commit - NordikAdventuresERP (PGI complet: WPF + MySQL)"

echo "🔗 Ajout du remote GitHub..."
git remote add origin https://github.com/eliDaniel007/TP3-ANALYSE.git

echo "📤 Poussée vers GitHub..."
git branch -M main
git push -u origin main

echo "✅ Projet poussé sur GitHub avec succès !"
echo "🌐 Vérifiez sur : https://github.com/eliDaniel007/TP3-ANALYSE"

