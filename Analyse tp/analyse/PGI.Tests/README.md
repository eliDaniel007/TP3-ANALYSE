# 🤖 Bot de Test Automatisé - PGI NordikAdventures

## Description

Ce bot de test automatisé permet de tester les transactions et valider le bon fonctionnement du PGI. Il inclut :
- **Authentification automatique** : Teste l'authentification avec un employé actif
- **Capture d'erreurs détaillée** : Récupère toutes les erreurs avec stack trace complète
- **Rapport final** : Affiche un rapport détaillé de tous les tests et erreurs

## Prérequis

1. Base de données MySQL configurée et accessible
2. Script `SQL_COMPLET_UNIFIE.sql` exécuté (pour créer la table `journal_comptable`)
3. Au moins un **employé actif avec mot de passe** dans la base de données
4. Données de test dans la base (clients, fournisseurs, produits)

## Exécution

### Via Visual Studio
1. Ouvrir la solution `PGI.sln`
2. Définir `PGI.Tests` comme projet de démarrage
3. Exécuter (F5)

### Via ligne de commande
```bash
cd "Analyse tp/analyse/PGI.Tests"
dotnet run
```

## Tests effectués

Le bot exécute automatiquement les tests suivants :

0. **Test d'authentification** : Teste l'authentification automatique avec un employé actif
1. **Test de connexion** : Vérifie que la connexion à la base de données fonctionne
2. **Test des ventes** : Crée une transaction de vente et vérifie l'enregistrement comptable
3. **Test des achats** : Crée une transaction d'achat et vérifie l'enregistrement comptable
4. **Test des dépenses** : Crée une dépense et vérifie l'enregistrement comptable
5. **Test d'équilibre** : Vérifie que le journal comptable respecte Débit = Crédit
6. **Test des rapports** : Génère les rapports de taxes et ventes
7. **Test des paramètres fiscaux** : Vérifie que les taux TPS/TVQ sont configurés

## Capture d'Erreurs

Le bot capture **toutes les erreurs** rencontrées pendant les tests et génère un **rapport final détaillé** incluant :
- Le nom du test qui a échoué
- Le message d'erreur complet
- La stack trace (détails techniques)
- L'heure de l'erreur
- Des recommandations pour corriger les problèmes

## Résultat attendu

### Si tous les tests passent :
```
🎉 TOUS LES TESTS SONT PASSÉS AVEC SUCCÈS !
✅ Aucune erreur détectée
```

### Si des erreurs sont détectées :
```
❌ 2 ERREUR(S) DÉTECTÉE(S)

DÉTAILS DES ERREURS :

------------------------------------------------------------

[1] Test: TestVentes
    ⏰ Heure: 2025-01-15 14:30:25
    ❌ Erreur: Unknown column 'x' in 'field list'
    📝 Détails:
       at PGI.Services.AccountingService.ValidateAndRecord...
       ...

💡 ACTIONS RECOMMANDÉES :
   1. Vérifiez les erreurs ci-dessus
   2. Corrigez les problèmes identifiés
   3. Ré-exécutez le bot de test
```

## Notes

- Le bot continue d'exécuter tous les tests même si certains échouent
- Toutes les erreurs sont collectées et affichées à la fin dans un rapport détaillé
- L'authentification essaie plusieurs comptes si le premier échoue
