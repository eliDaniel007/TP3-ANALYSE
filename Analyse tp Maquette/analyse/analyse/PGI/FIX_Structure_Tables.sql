-- =====================================================
-- Script de correction structure tables
-- Vérifier et recréer la structure si nécessaire
-- =====================================================

USE NordikAdventuresERP;

-- =====================================================
-- 1. VÉRIFIER LA STRUCTURE ACTUELLE
-- =====================================================

-- Voir la structure de produits
DESCRIBE produits;

-- =====================================================
-- 2. SI ERREUR CI-DESSUS, RECRÉER LA TABLE PRODUITS
-- =====================================================

-- ATTENTION : Sauvegarder d'abord les données si la table existe
-- CREATE TABLE produits_backup AS SELECT * FROM produits;

-- Supprimer les contraintes de clés étrangères dépendantes
SET FOREIGN_KEY_CHECKS = 0;

-- Supprimer la table produits si elle existe
DROP TABLE IF EXISTS lignes_commande;
DROP TABLE IF EXISTS lignes_facture;
DROP TABLE IF EXISTS lignes_achat;
DROP TABLE IF EXISTS mouvements_stock;
DROP TABLE IF EXISTS niveaux_stock;
DROP TABLE IF EXISTS produits;

-- Recréer la table produits avec la bonne structure
CREATE TABLE produits (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sku VARCHAR(50) NOT NULL UNIQUE,
    categorie_id INT NOT NULL,
    nom VARCHAR(200) NOT NULL,
    description TEXT,
    cout DECIMAL(10,2) NOT NULL CHECK(cout >= 0),
    prix DECIMAL(10,2) NOT NULL CHECK(prix > 0),
    marge_brute DECIMAL(5,2) NOT NULL DEFAULT 0.00 CHECK(marge_brute >= 0 AND marge_brute <= 100),
    seuil_reapprovisionnement INT NOT NULL DEFAULT 10 CHECK(seuil_reapprovisionnement >= 0),
    stock_minimum INT NOT NULL DEFAULT 5 CHECK(stock_minimum >= 0),
    poids_kg DECIMAL(8,3) DEFAULT 0.00 CHECK(poids_kg >= 0),
    fournisseur_id INT NOT NULL,
    statut ENUM('Actif', 'Inactif') NOT NULL DEFAULT 'Actif',
    date_entree_stock DATE,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_produit_categorie FOREIGN KEY (categorie_id) REFERENCES categories(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_produit_fournisseur FOREIGN KEY (fournisseur_id) REFERENCES fournisseurs(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_categorie (categorie_id),
    INDEX idx_fournisseur (fournisseur_id),
    INDEX idx_sku (sku),
    INDEX idx_statut (statut),
    INDEX idx_date_entree (date_entree_stock)
) ENGINE=InnoDB COMMENT='Produits du catalogue';

-- Recréer la table niveaux_stock
CREATE TABLE niveaux_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL,
    emplacement VARCHAR(100) NOT NULL DEFAULT 'Entrepôt Principal',
    qte_disponible INT NOT NULL DEFAULT 0 CHECK(qte_disponible >= 0),
    qte_reservee INT NOT NULL DEFAULT 0 CHECK(qte_reservee >= 0),
    derniere_maj DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_stock_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_produit (produit_id),
    INDEX idx_emplacement (emplacement)
) ENGINE=InnoDB COMMENT='Niveaux de stock (emplacement intégré)';

-- Recréer la table mouvements_stock
CREATE TABLE mouvements_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL,
    type_mouvement ENUM('ENTREE', 'SORTIE') NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    raison ENUM('reception_achat', 'vente', 'ajustement', 'retour_entree', 'retour_sortie', 'manuel') NOT NULL,
    date_mouvement DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    note_utilisateur TEXT,
    employe_id INT,
    commande_vente_id INT,
    achat_id INT,
    CONSTRAINT fk_mouvement_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_produit_date (produit_id, date_mouvement),
    INDEX idx_type_mouvement (type_mouvement)
) ENGINE=InnoDB COMMENT='Mouvements de stock';

-- Réactiver les contraintes
SET FOREIGN_KEY_CHECKS = 1;

-- =====================================================
-- 3. VÉRIFICATION
-- =====================================================

-- Vérifier que la structure est correcte
DESCRIBE produits;

SELECT '✅ Structure de la table produits corrigée !' AS Message;

-- Vérifier les catégories
SELECT COUNT(*) AS 'Nombre de catégories' FROM categories;

-- Vérifier les fournisseurs
SELECT COUNT(*) AS 'Nombre de fournisseurs' FROM fournisseurs;

