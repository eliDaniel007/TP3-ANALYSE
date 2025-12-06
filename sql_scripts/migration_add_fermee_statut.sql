-- Migration: Ajouter le statut 'Fermée' à l'ENUM de la table commandes_fournisseurs
-- Date: 2025-01-XX
-- Description: Ajoute la valeur 'Fermée' à l'ENUM de la colonne statut pour permettre la fermeture des commandes

ALTER TABLE commandes_fournisseurs 
MODIFY COLUMN statut ENUM('En attente', 'Envoyée', 'Partiellement reçue', 'Reçue', 'Fermée', 'Annulée') DEFAULT 'En attente';

