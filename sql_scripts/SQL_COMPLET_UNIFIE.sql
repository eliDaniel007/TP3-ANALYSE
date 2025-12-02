-- =====================================================
-- NORDIKADVENTURES ERP - SCRIPT SQL COMPLET UNIFIÉ
-- =====================================================
-- Description: Script SQL complet pour l'installation complète du système ERP
-- Modules inclus: Stocks, Finances, CRM, RH
-- Version: 2.0 (Unifié - Sans hashage)
-- Date: 2025-01-28
-- Auteur: Système NordikAdventures
-- =====================================================
-- ORDRE D'EXÉCUTION:
-- 1. Création de la base de données
-- 2. Schéma principal (tables de base)
-- 3. Module Finances
-- 4. Module CRM
-- 5. Données initiales (employés, clients, produits)
-- =====================================================

-- =====================================================
-- PARTIE 1: CRÉATION DE LA BASE DE DONNÉES
-- =====================================================

DROP DATABASE IF EXISTS NordikAdventuresERP;
CREATE DATABASE NordikAdventuresERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE NordikAdventuresERP;

-- =====================================================
-- PARTIE 2: SCHÉMA PRINCIPAL - TABLES DE BASE
-- =====================================================

-- =====================================================
-- MODULE 0: RESSOURCES HUMAINES (Employés & Paies)
-- =====================================================

-- Table: employes
CREATE TABLE employes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    matricule VARCHAR(20) NOT NULL UNIQUE,
    nom VARCHAR(100) NOT NULL,
    prenom VARCHAR(100) NOT NULL,
    courriel VARCHAR(100) NOT NULL UNIQUE,
    telephone VARCHAR(20),
    departement ENUM('Administration', 'Ventes', 'Comptabilité', 'Logistique', 'RH', 'IT', 'Autre') NOT NULL DEFAULT 'Autre',
    poste VARCHAR(100) NOT NULL,
    role_systeme ENUM('Admin', 'Gestionnaire', 'Employé Ventes', 'Comptable', 'Employé') NOT NULL DEFAULT 'Employé',
    salaire_annuel DECIMAL(10,2) NOT NULL CHECK(salaire_annuel >= 0),
    date_embauche DATE NOT NULL,
    date_fin_emploi DATE,
    statut ENUM('Actif', 'Congé', 'Inactif') NOT NULL DEFAULT 'Actif',
    mot_de_passe VARCHAR(255) COMMENT 'Mot de passe en clair (non hashé)',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_courriel (courriel),
    INDEX idx_departement (departement),
    INDEX idx_statut (statut),
    INDEX idx_role (role_systeme)
) ENGINE=InnoDB COMMENT='Employés avec département intégré';

-- Table: paies
CREATE TABLE paies (
    id INT AUTO_INCREMENT PRIMARY KEY,
    employe_id INT NOT NULL,
    periode_debut DATE NOT NULL,
    periode_fin DATE NOT NULL,
    montant_brut DECIMAL(10,2) NOT NULL,
    deductions_total DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    montant_net DECIMAL(10,2) NOT NULL,
    heures_travaillees DECIMAL(6,2),
    heures_supplementaires DECIMAL(6,2) DEFAULT 0.00,
    statut_paiement ENUM('En attente', 'Approuvé', 'Payé') NOT NULL DEFAULT 'En attente',
    date_paiement DATE,
    commentaire TEXT,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE CASCADE,
    INDEX idx_employe (employe_id),
    INDEX idx_periode (periode_debut, periode_fin),
    INDEX idx_statut (statut_paiement)
) ENGINE=InnoDB COMMENT='Fiches de paie des employés';

-- =====================================================
-- MODULE 1: STOCKS & INVENTAIRE
-- =====================================================

-- Table: categories
CREATE TABLE categories (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    statut ENUM('Actif', 'Inactif') NOT NULL DEFAULT 'Actif',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Catégories de produits';

-- Table: fournisseurs
CREATE TABLE fournisseurs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(20) NOT NULL UNIQUE,
    nom VARCHAR(150) NOT NULL,
    courriel_contact VARCHAR(100) NOT NULL,
    telephone VARCHAR(20),
    adresse_ligne1 VARCHAR(255),
    adresse_ligne2 VARCHAR(255),
    ville VARCHAR(100),
    province VARCHAR(50),
    code_postal VARCHAR(10),
    pays VARCHAR(50) DEFAULT 'Canada',
    delai_livraison_jours INT DEFAULT 7 CHECK(delai_livraison_jours > 0),
    pourcentage_escompte DECIMAL(5,2) DEFAULT 0.00 CHECK(pourcentage_escompte BETWEEN 0 AND 100),
    statut ENUM('Actif', 'Inactif') NOT NULL DEFAULT 'Actif',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_code (code),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Fournisseurs de produits';

-- Table: produits
CREATE TABLE produits (
    id INT AUTO_INCREMENT PRIMARY KEY,
    sku VARCHAR(50) NOT NULL UNIQUE,
    categorie_id INT NOT NULL,
    nom VARCHAR(200) NOT NULL,
    description TEXT,
    cout DECIMAL(10,2) NOT NULL CHECK(cout >= 0),
    prix DECIMAL(10,2) NOT NULL CHECK(prix >= 0),
    marge_brute DECIMAL(5,2) GENERATED ALWAYS AS (CASE WHEN cout > 0 THEN ((prix - cout) / cout * 100) ELSE 0 END) STORED,
    seuil_reapprovisionnement INT NOT NULL DEFAULT 10 CHECK(seuil_reapprovisionnement >= 0),
    stock_minimum INT NOT NULL DEFAULT 5 CHECK(stock_minimum >= 0),
    poids_kg DECIMAL(8,3) DEFAULT 0.000 CHECK(poids_kg >= 0),
    fournisseur_id INT NOT NULL,
    statut ENUM('Actif', 'Archivé') NOT NULL DEFAULT 'Actif',
    date_entree_stock DATE,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (categorie_id) REFERENCES categories(id) ON DELETE RESTRICT,
    FOREIGN KEY (fournisseur_id) REFERENCES fournisseurs(id) ON DELETE RESTRICT,
    INDEX idx_sku (sku),
    INDEX idx_categorie (categorie_id),
    INDEX idx_fournisseur (fournisseur_id),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Produits avec marge calculée';

-- Table: niveaux_stock
CREATE TABLE niveaux_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL,
    emplacement VARCHAR(100) NOT NULL DEFAULT 'Entrepôt Principal',
    qte_disponible INT NOT NULL DEFAULT 0 CHECK(qte_disponible >= 0),
    qte_reservee INT NOT NULL DEFAULT 0 CHECK(qte_reservee >= 0),
    date_derniere_maj DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE CASCADE,
    UNIQUE KEY unique_produit_emplacement (produit_id, emplacement),
    INDEX idx_emplacement (emplacement)
) ENGINE=InnoDB COMMENT='Niveaux de stock par emplacement';

-- Table: mouvements_stock
CREATE TABLE mouvements_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL,
    type_mouvement ENUM('ENTREE', 'SORTIE', 'AJUSTEMENT', 'TRANSFERT', 'RETOUR') NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    raison ENUM('vente', 'achat', 'retour_client', 'retour_fournisseur', 'ajustement_inventaire', 'perte', 'don', 'reception_commande', 'manuel') NOT NULL DEFAULT 'manuel',
    emplacement_source VARCHAR(100),
    emplacement_destination VARCHAR(100),
    note_utilisateur TEXT,
    employe_id INT,
    date_mouvement DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE CASCADE,
    FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL,
    INDEX idx_produit (produit_id),
    INDEX idx_type (type_mouvement),
    INDEX idx_date (date_mouvement)
) ENGINE=InnoDB COMMENT='Historique des mouvements de stock';

-- =====================================================
-- MODULE 2: GESTION DES CLIENTS
-- =====================================================

-- Table: clients
CREATE TABLE clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    type ENUM('Particulier', 'Entreprise') NOT NULL DEFAULT 'Particulier',
    nom VARCHAR(200) NOT NULL,
    courriel_contact VARCHAR(100) NOT NULL UNIQUE,
    telephone VARCHAR(20),
    adresse_ligne1 VARCHAR(255),
    adresse_ligne2 VARCHAR(255),
    ville VARCHAR(100),
    province VARCHAR(50),
    code_postal VARCHAR(10),
    pays VARCHAR(50) DEFAULT 'Canada',
    statut ENUM('Prospect', 'Actif', 'Fidèle', 'Inactif') NOT NULL DEFAULT 'Prospect',
    mot_de_passe VARCHAR(255) COMMENT 'Mot de passe en clair (non hashé)',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_courriel (courriel_contact),
    INDEX idx_statut (statut),
    INDEX idx_type (type)
) ENGINE=InnoDB COMMENT='Clients (particuliers ou entreprises)';

-- =====================================================
-- PARTIE 3: MODULE FINANCES ET FACTURATION
-- =====================================================

-- Table: parametres_taxes
CREATE TABLE IF NOT EXISTS parametres_taxes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom_taxe VARCHAR(50) NOT NULL,
    taux DECIMAL(10, 5) NOT NULL COMMENT '0.05 pour 5%, 0.09975 pour 9.975%',
    actif BOOLEAN DEFAULT TRUE,
    date_effective DATETIME DEFAULT CURRENT_TIMESTAMP,
    description VARCHAR(255),
    UNIQUE KEY unique_nom_taxe (nom_taxe)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Paramètres configurables des taxes (TPS, TVQ)';

-- Insertion des taux de taxes par défaut
INSERT INTO parametres_taxes (nom_taxe, taux, actif, description) VALUES
('TPS', 0.05000, TRUE, 'Taxe sur les produits et services (Fédéral)'),
('TVQ', 0.09975, TRUE, 'Taxe de vente du Québec (Provincial)')
ON DUPLICATE KEY UPDATE taux=VALUES(taux);

-- Table: factures
CREATE TABLE IF NOT EXISTS factures (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_facture VARCHAR(50) NOT NULL UNIQUE COMMENT 'Format: FAC-2025-0001',
    date_facture DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_echeance DATETIME NOT NULL,
    client_id INT NOT NULL,
    employe_id INT NULL COMMENT 'Vendeur/Créateur',
    
    sous_total DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_tps DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_tvq DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_total DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_paye DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_du DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    
    statut_paiement ENUM('Impayée', 'Partielle', 'Payée') DEFAULT 'Impayée',
    statut ENUM('Active', 'Annulée') DEFAULT 'Active',
    
    note_interne TEXT NULL,
    conditions_paiement VARCHAR(255) DEFAULT 'Net 30 jours',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE RESTRICT,
    FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL,
    INDEX idx_client (client_id),
    INDEX idx_statut_paiement (statut_paiement),
    INDEX idx_date_facture (date_facture),
    INDEX idx_date_echeance (date_echeance)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Factures de vente aux clients';

-- Table: lignes_factures
CREATE TABLE IF NOT EXISTS lignes_factures (
    id INT AUTO_INCREMENT PRIMARY KEY,
    facture_id INT NOT NULL,
    produit_id INT NOT NULL,
    sku VARCHAR(50) NOT NULL,
    description VARCHAR(255) NOT NULL,
    quantite INT NOT NULL CHECK (quantite > 0),
    prix_unitaire DECIMAL(10, 2) NOT NULL CHECK (prix_unitaire >= 0),
    montant_ligne DECIMAL(10, 2) NOT NULL,
    
    FOREIGN KEY (facture_id) REFERENCES factures(id) ON DELETE CASCADE,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT,
    INDEX idx_facture (facture_id),
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Lignes de détail des factures';

-- Table: paiements
CREATE TABLE IF NOT EXISTS paiements (
    id INT AUTO_INCREMENT PRIMARY KEY,
    facture_id INT NOT NULL,
    date_paiement DATETIME DEFAULT CURRENT_TIMESTAMP,
    montant DECIMAL(10, 2) NOT NULL CHECK (montant > 0),
    methode_paiement ENUM('Comptant', 'Carte crédit', 'Carte débit', 'Virement', 'Chèque') NOT NULL DEFAULT 'Comptant',
    numero_reference VARCHAR(100) NULL,
    note TEXT NULL,
    
    FOREIGN KEY (facture_id) REFERENCES factures(id) ON DELETE CASCADE,
    INDEX idx_facture (facture_id),
    INDEX idx_date (date_paiement)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Paiements reçus pour les factures';

-- Table: commandes_fournisseurs
CREATE TABLE IF NOT EXISTS commandes_fournisseurs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_commande VARCHAR(50) NOT NULL UNIQUE COMMENT 'Format: CMD-2025-0001',
    date_commande DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_livraison_prevue DATE NULL,
    date_reception DATE NULL,
    fournisseur_id INT NOT NULL,
    employe_id INT NULL COMMENT 'Responsable de la commande',
    
    sous_total DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_tps DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_tvq DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    montant_total DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
    
    statut ENUM('En attente', 'Envoyée', 'Partiellement reçue', 'Reçue', 'Annulée') DEFAULT 'En attente',
    note_interne TEXT NULL,
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (fournisseur_id) REFERENCES fournisseurs(id) ON DELETE RESTRICT,
    FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL,
    INDEX idx_fournisseur (fournisseur_id),
    INDEX idx_statut (statut),
    INDEX idx_date_commande (date_commande)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Commandes passées aux fournisseurs';

-- Table: lignes_commandes_fournisseurs
CREATE TABLE IF NOT EXISTS lignes_commandes_fournisseurs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    commande_id INT NOT NULL,
    produit_id INT NOT NULL,
    sku VARCHAR(50) NOT NULL,
    description VARCHAR(255) NOT NULL,
    quantite_commandee INT NOT NULL CHECK (quantite_commandee > 0),
    quantite_recue INT NOT NULL DEFAULT 0 CHECK (quantite_recue >= 0),
    prix_unitaire DECIMAL(10, 2) NOT NULL CHECK (prix_unitaire >= 0),
    montant_ligne DECIMAL(10, 2) NOT NULL,
    
    FOREIGN KEY (commande_id) REFERENCES commandes_fournisseurs(id) ON DELETE CASCADE,
    FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT,
    INDEX idx_commande (commande_id),
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Lignes de détail des commandes fournisseurs';

-- =====================================================
-- PARTIE 4: MODULE CRM (GESTION DE LA RELATION CLIENT)
-- =====================================================

-- Table: interactions_clients
CREATE TABLE IF NOT EXISTS interactions_clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    employe_id INT NULL COMMENT 'Employé ayant créé l''interaction',
    type_interaction ENUM('Email', 'Téléphone', 'Réunion', 'Vente', 'Note', 'Réclamation') NOT NULL DEFAULT 'Note',
    sujet VARCHAR(255) NOT NULL,
    description TEXT NOT NULL,
    date_interaction DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    resultat_action TEXT NULL COMMENT 'Résultat ou suivi de l''action',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE,
    FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL,
    INDEX idx_client_date (client_id, date_interaction),
    INDEX idx_type (type_interaction)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Historique des interactions avec les clients';

-- Table: evaluations_clients
CREATE TABLE IF NOT EXISTS evaluations_clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    facture_id INT NULL COMMENT 'Facture liée si applicable',
    note_satisfaction INT NOT NULL CHECK (note_satisfaction BETWEEN 1 AND 5),
    commentaire TEXT NULL,
    date_evaluation DATETIME DEFAULT CURRENT_TIMESTAMP,
    alerte_generee BOOLEAN DEFAULT FALSE COMMENT 'TRUE si note ≤ 2',
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE,
    FOREIGN KEY (facture_id) REFERENCES factures(id) ON DELETE SET NULL,
    INDEX idx_client (client_id),
    INDEX idx_note (note_satisfaction),
    INDEX idx_alerte (alerte_generee)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Évaluations de satisfaction des clients (1 à 5 étoiles)';

-- Table: campagnes_marketing
CREATE TABLE IF NOT EXISTS campagnes_marketing (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom_campagne VARCHAR(255) NOT NULL,
    type ENUM('Email', 'SMS', 'Publicité', 'Promo', 'Événement') NOT NULL DEFAULT 'Email',
    description TEXT NULL,
    date_debut DATE NOT NULL,
    date_fin DATE NOT NULL,
    budget DECIMAL(10, 2) DEFAULT 0.00,
    nombre_destinataires INT DEFAULT 0 COMMENT 'Nombre de clients ciblés',
    nombre_reponses INT DEFAULT 0 COMMENT 'Nombre de réponses/conversions',
    taux_participation DECIMAL(5, 2) DEFAULT 0.00 COMMENT 'Calculé automatiquement (%)',
    statut ENUM('Planifiée', 'Active', 'Terminée', 'Annulée') DEFAULT 'Planifiée',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    CHECK (date_fin >= date_debut),
    INDEX idx_statut (statut),
    INDEX idx_dates (date_debut, date_fin)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Campagnes marketing et promotions';

-- Table: alertes_service_client
CREATE TABLE IF NOT EXISTS alertes_service_client (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    evaluation_id INT NULL COMMENT 'Lié à une évaluation si applicable',
    type_alerte ENUM('Satisfaction faible', 'Retard paiement', 'Inactivité', 'Réclamation', 'Autre') NOT NULL,
    description TEXT NOT NULL,
    priorite ENUM('Basse', 'Moyenne', 'Haute', 'Urgente') DEFAULT 'Moyenne',
    statut ENUM('Ouverte', 'En cours', 'Résolue', 'Fermée') DEFAULT 'Ouverte',
    employe_assigne_id INT NULL COMMENT 'Employé assigné à cette alerte',
    date_creation DATETIME DEFAULT CURRENT_TIMESTAMP,
    date_resolution DATETIME NULL,
    note_resolution TEXT NULL,
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE,
    FOREIGN KEY (evaluation_id) REFERENCES evaluations_clients(id) ON DELETE SET NULL,
    FOREIGN KEY (employe_assigne_id) REFERENCES employes(id) ON DELETE SET NULL,
    INDEX idx_client (client_id),
    INDEX idx_statut (statut),
    INDEX idx_priorite (priorite),
    INDEX idx_employe (employe_assigne_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Alertes et tickets du service client';

-- =====================================================
-- PARTIE 5: VUES CALCULÉES
-- =====================================================

-- Vue: Statistiques clients (KPIs)
CREATE OR REPLACE VIEW vue_statistiques_clients AS
SELECT 
    c.id AS client_id,
    c.nom AS nom_client,
    c.statut,
    c.type,
    
    -- Statistiques d'achats
    COUNT(DISTINCT f.id) AS nombre_commandes,
    COALESCE(SUM(f.montant_total), 0) AS chiffre_affaires_total,
    COALESCE(AVG(f.montant_total), 0) AS panier_moyen,
    MAX(f.date_facture) AS date_derniere_commande,
    DATEDIFF(NOW(), MAX(f.date_facture)) AS jours_sans_activite,
    
    -- Satisfaction
    COALESCE(AVG(ev.note_satisfaction), 0) AS note_satisfaction_moyenne,
    COUNT(DISTINCT ic.id) AS nombre_interactions,
    COUNT(DISTINCT CASE WHEN ic.type_interaction = 'Réclamation' THEN ic.id END) AS nombre_reclamations,
    
    -- Paiement
    COALESCE(SUM(f.montant_du), 0) AS montant_impaye,
    CASE 
        WHEN EXISTS (
            SELECT 1 FROM factures f2 
            WHERE f2.client_id = c.id 
            AND f2.date_echeance < NOW() 
            AND f2.statut_paiement != 'Payée'
        ) THEN TRUE 
        ELSE FALSE 
    END AS retard_paiement,
    
    -- Score composite: (CA / 1000) + (NbCommandes * 2) + (NoteMoyenne * 10)
    (COALESCE(SUM(f.montant_total), 0) / 1000) + 
    (COUNT(DISTINCT f.id) * 2) + 
    (COALESCE(AVG(ev.note_satisfaction), 0) * 10) AS score_composite
    
FROM clients c
LEFT JOIN factures f ON c.id = f.client_id AND f.statut = 'Active'
LEFT JOIN evaluations_clients ev ON c.id = ev.client_id
LEFT JOIN interactions_clients ic ON c.id = ic.client_id
GROUP BY c.id, c.nom, c.statut, c.type;

-- =====================================================
-- PARTIE 6: TRIGGERS ET AUTOMATISATIONS
-- =====================================================

DELIMITER $$

-- Trigger: Alerte automatique pour note faible
CREATE TRIGGER trg_evaluation_alerte_satisfaction
AFTER INSERT ON evaluations_clients
FOR EACH ROW
BEGIN
    IF NEW.note_satisfaction <= 2 THEN
        INSERT INTO alertes_service_client (
            client_id, 
            evaluation_id, 
            type_alerte, 
            description, 
            priorite, 
            statut
        ) VALUES (
            NEW.client_id,
            NEW.id,
            'Satisfaction faible',
            CONCAT('Client insatisfait (note: ', NEW.note_satisfaction, '/5). Commentaire: ', COALESCE(NEW.commentaire, 'Aucun')),
            CASE 
                WHEN NEW.note_satisfaction = 1 THEN 'Urgente'
                ELSE 'Haute'
            END,
            'Ouverte'
        );
        
        UPDATE evaluations_clients SET alerte_generee = TRUE WHERE id = NEW.id;
    END IF;
END$$

-- Trigger: Mise à jour statut client automatique
CREATE TRIGGER trg_facture_update_statut_client
AFTER INSERT ON factures
FOR EACH ROW
BEGIN
    DECLARE nb_commandes INT;
    DECLARE ca_total DECIMAL(10, 2);
    DECLARE statut_actuel VARCHAR(50);
    
    SELECT statut INTO statut_actuel FROM clients WHERE id = NEW.client_id;
    
    SELECT COUNT(*), COALESCE(SUM(montant_total), 0)
    INTO nb_commandes, ca_total
    FROM factures
    WHERE client_id = NEW.client_id AND statut = 'Active';
    
    -- Prospect → Actif après 1ère commande
    IF statut_actuel = 'Prospect' AND nb_commandes >= 1 THEN
        UPDATE clients SET statut = 'Actif' WHERE id = NEW.client_id;
        
        INSERT INTO interactions_clients (
            client_id, type_interaction, sujet, description, date_interaction
        ) VALUES (
            NEW.client_id, 
            'Note', 
            'Changement de statut: Prospect → Actif',
            'Client converti en client actif après sa première commande.',
            NOW()
        );
    END IF;
    
    -- Actif → Fidèle après >5 commandes OU >3000$ CA
    IF (statut_actuel = 'Actif' OR statut_actuel = 'Prospect') 
       AND (nb_commandes > 5 OR ca_total > 3000) THEN
        UPDATE clients SET statut = 'Fidèle' WHERE id = NEW.client_id;
        
        INSERT INTO interactions_clients (
            client_id, type_interaction, sujet, description, date_interaction
        ) VALUES (
            NEW.client_id, 
            'Note', 
            'Changement de statut: Actif → Fidèle',
            CONCAT('Client fidélisé! ', nb_commandes, ' commandes pour un total de ', ca_total, ' $.'),
            NOW()
        );
    END IF;
    
    -- Créer une interaction automatique pour cette vente
    INSERT INTO interactions_clients (
        client_id, type_interaction, sujet, description, date_interaction
    ) VALUES (
        NEW.client_id, 
        'Vente', 
        CONCAT('Vente ', NEW.numero_facture),
        CONCAT('Facture de ', NEW.montant_total, ' $ créée.'),
        NEW.date_facture
    );
END$$

DELIMITER ;

-- =====================================================
-- PARTIE 7: PROCÉDURES STOCKÉES
-- =====================================================

DELIMITER $$

-- Procédure: Générer numéro de facture
CREATE PROCEDURE sp_generer_numero_facture(OUT p_numero_facture VARCHAR(50))
BEGIN
    DECLARE v_annee INT;
    DECLARE v_dernier_numero INT;
    DECLARE v_nouveau_numero INT;
    
    SET v_annee = YEAR(NOW());
    
    SELECT COALESCE(MAX(CAST(SUBSTRING(numero_facture, -4) AS UNSIGNED)), 0) 
    INTO v_dernier_numero
    FROM factures
    WHERE numero_facture LIKE CONCAT('FAC-', v_annee, '-%');
    
    SET v_nouveau_numero = v_dernier_numero + 1;
    SET p_numero_facture = CONCAT('FAC-', v_annee, '-', LPAD(v_nouveau_numero, 4, '0'));
END$$

-- Procédure: Générer numéro de commande
CREATE PROCEDURE sp_generer_numero_commande(OUT p_numero_commande VARCHAR(50))
BEGIN
    DECLARE v_annee INT;
    DECLARE v_dernier_numero INT;
    DECLARE v_nouveau_numero INT;
    
    SET v_annee = YEAR(NOW());
    
    SELECT COALESCE(MAX(CAST(SUBSTRING(numero_commande, -4) AS UNSIGNED)), 0) 
    INTO v_dernier_numero
    FROM commandes_fournisseurs
    WHERE numero_commande LIKE CONCAT('CMD-', v_annee, '-%');
    
    SET v_nouveau_numero = v_dernier_numero + 1;
    SET p_numero_commande = CONCAT('CMD-', v_annee, '-', LPAD(v_nouveau_numero, 4, '0'));
END$$

-- Procédure: Marquer les clients inactifs
CREATE PROCEDURE sp_marquer_clients_inactifs()
BEGIN
    UPDATE clients c
    SET c.statut = 'Inactif'
    WHERE c.statut IN ('Actif', 'Fidèle')
    AND NOT EXISTS (
        SELECT 1 FROM factures f 
        WHERE f.client_id = c.id 
        AND f.date_facture >= DATE_SUB(NOW(), INTERVAL 12 MONTH)
    );
    
    INSERT INTO interactions_clients (client_id, type_interaction, sujet, description, date_interaction)
    SELECT 
        c.id,
        'Note',
        'Changement de statut: Inactif',
        'Client marqué comme inactif (aucune activité depuis 12 mois)',
        NOW()
    FROM clients c
    WHERE c.statut = 'Inactif'
    AND NOT EXISTS (
        SELECT 1 FROM interactions_clients ic 
        WHERE ic.client_id = c.id 
        AND ic.sujet = 'Changement de statut: Inactif'
        AND ic.date_interaction >= DATE_SUB(NOW(), INTERVAL 1 DAY)
    );
END$$

-- Procédure: Clôturer campagne marketing
CREATE PROCEDURE sp_cloture_campagne(IN p_campagne_id INT)
BEGIN
    DECLARE v_nb_destinataires INT;
    DECLARE v_nb_reponses INT;
    DECLARE v_taux DECIMAL(5, 2);
    
    SELECT nombre_destinataires, nombre_reponses 
    INTO v_nb_destinataires, v_nb_reponses
    FROM campagnes_marketing
    WHERE id = p_campagne_id;
    
    IF v_nb_destinataires > 0 THEN
        SET v_taux = (v_nb_reponses / v_nb_destinataires) * 100;
    ELSE
        SET v_taux = 0;
    END IF;
    
    UPDATE campagnes_marketing
    SET 
        taux_participation = v_taux,
        statut = 'Terminée'
    WHERE id = p_campagne_id;
END$$

DELIMITER ;

-- =====================================================
-- PARTIE 8: FONCTIONS
-- =====================================================

DELIMITER $$

-- Fonction: Vérifier si un client peut commander
CREATE FUNCTION fn_client_peut_commander(p_client_id INT) 
RETURNS BOOLEAN
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_statut VARCHAR(50);
    DECLARE v_retard_paiement BOOLEAN;
    
    SELECT 
        c.statut,
        EXISTS (
            SELECT 1 FROM factures f 
            WHERE f.client_id = c.id 
            AND f.date_echeance < NOW() 
            AND f.statut_paiement != 'Payée'
        )
    INTO v_statut, v_retard_paiement
    FROM clients c
    WHERE c.id = p_client_id;
    
    IF v_statut IN ('Actif', 'Fidèle') AND v_retard_paiement = FALSE THEN
        RETURN TRUE;
    ELSE
        RETURN FALSE;
    END IF;
END$$

DELIMITER ;

-- =====================================================
-- PARTIE 9: DONNÉES INITIALES - EMPLOYÉS ET CLIENTS
-- =====================================================

-- Insertion d'employés de test (avec mots de passe en clair)
INSERT INTO employes (matricule, nom, prenom, courriel, telephone, departement, poste, role_systeme, salaire_annuel, date_embauche, statut, mot_de_passe) VALUES
('EMP-001', 'Tremblay', 'Admin', 'admin@nordikadventures.com', '514-555-0001', 'Administration', 'Directeur', 'Admin', 85000.00, '2020-01-15', 'Actif', 'Admin123'),
('EMP-002', 'Bouchard', 'Marie', 'gestionnaire@nordikadventures.com', '514-555-0002', 'Logistique', 'Gestionnaire Stocks', 'Gestionnaire', 65000.00, '2020-03-20', 'Actif', 'Gestionnaire123'),
('EMP-003', 'Gagnon', 'Pierre', 'employe@nordikadventures.com', '514-555-0003', 'Ventes', 'Conseiller Ventes', 'Employé Ventes', 48000.00, '2021-06-10', 'Actif', 'Employe123'),
('EMP-004', 'Côté', 'Sophie', 'comptable@nordikadventures.com', '514-555-0004', 'Comptabilité', 'Comptable', 'Comptable', 60000.00, '2020-09-01', 'Actif', 'Comptable123')
ON DUPLICATE KEY UPDATE mot_de_passe = VALUES(mot_de_passe);

-- Insertion de clients de test (avec mots de passe en clair)
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe) VALUES
('Particulier', 'Jean Tremblay', 'jean.tremblay@client.com', '514-555-1001', 'Actif', 'Client123'),
('Particulier', 'Marie Martin', 'marie.client@test.com', '438-555-1002', 'Actif', 'Client123'),
('Entreprise', 'Pierre Tremblay', 'pierre.client@entreprise.com', '438-555-1003', 'Actif', 'Client123'),
('Particulier', 'Sophie Lavoie', 'client.sophie@gmail.com', '438-555-1004', 'Actif', 'Client123'),
('Entreprise', 'Nordik Sports Inc.', 'contact@nordikclient.com', '514-555-2000', 'Fidèle', 'Client123')
ON DUPLICATE KEY UPDATE mot_de_passe = VALUES(mot_de_passe);

-- =====================================================
-- PARTIE 10: DONNÉES INITIALES - PRODUITS
-- =====================================================

-- Insertion des catégories
INSERT INTO categories (nom, statut) VALUES
('Tentes & abris', 'Actif'),
('Sacs & portage', 'Actif'),
('Vêtements techniques', 'Actif'),
('Accessoires & cuisine', 'Actif'),
('Électronique & navigation', 'Actif')
ON DUPLICATE KEY UPDATE statut = statut;

-- Insertion des fournisseurs
INSERT INTO fournisseurs (nom, code, courriel_contact, delai_livraison_jours, pourcentage_escompte, statut) VALUES
('AventureX', 'AX-001', 'contact@aventurex.com', 10, 5.00, 'Actif'),
('TrekSupply', 'TS-001', 'info@treksupply.com', 7, 4.00, 'Actif'),
('MontNord', 'MN-001', 'ventes@montnord.com', 8, 3.00, 'Actif'),
('NordPack', 'NP-001', 'commandes@nordpack.com', 8, 6.00, 'Actif'),
('NordWear', 'NW-001', 'service@nordwear.com', 6, 5.00, 'Actif'),
('ArcticLine', 'AL-001', 'contact@arcticline.com', 8, 4.00, 'Actif'),
('TechTrail', 'TT-001', 'info@techtrail.com', 10, 4.00, 'Actif')
ON DUPLICATE KEY UPDATE statut = statut;

-- Insertion des produits (30 produits)
-- Tentes & abris (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-TNT-001', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tente légère 2 places', 'Tente ultra-légère pour 2 personnes, parfaite pour la randonnée', 145.00, 299.00, 5, 3, 2.8, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-03-02'),
('NC-TNT-002', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tente familiale 6 places', 'Tente spacieuse pour toute la famille avec vestibule', 260.00, 499.00, 3, 2, 6.5, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-18'),
('NC-TNT-003', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Toile imperméable 3x3 m', 'Toile de protection imperméable multi-usage', 25.00, 59.00, 8, 5, 1.1, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-10'),
('NC-TNT-004', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tapis de sol isolant', 'Tapis isolant thermique pour sol de tente', 18.00, 39.00, 10, 6, 0.9, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-05'),
('NC-TNT-005', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Abri cuisine pliable', 'Abri léger et pliable pour cuisine extérieure', 75.00, 149.00, 4, 3, 5.0, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-20'),
('NC-TNT-006', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Mât télescopique alu', 'Mât ajustable en aluminium pour tente', 12.00, 29.00, 10, 6, 0.7, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-08')
ON DUPLICATE KEY UPDATE statut = statut;

-- Sacs & portage (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-SAC-001', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac à dos 50 L étanche', 'Sac à dos grande capacité 100% étanche', 65.00, 139.00, 6, 4, 1.3, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-12'),
('NC-SAC-002', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac de jour 25 L', 'Sac à dos compact pour randonnées courtes', 32.00, 79.00, 8, 5, 0.9, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-10'),
('NC-SAC-003', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac de couchage -10°C', 'Sac de couchage 3 saisons confort jusqu''à -10°C', 80.00, 169.00, 5, 3, 2.2, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-02-25'),
('NC-SAC-004', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Tapis autogonflant', 'Matelas autogonflant ultra-confortable', 25.00, 59.00, 10, 5, 1.1, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-05'),
('NC-SAC-005', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Housse imperméable sac à dos', 'Protection imperméable pour sac à dos', 9.00, 19.00, 10, 6, 0.4, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-11'),
('NC-SAC-006', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Bâtons de marche carbone', 'Bâtons de randonnée ultra-légers en carbone', 35.00, 79.00, 5, 3, 0.8, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-02-28')
ON DUPLICATE KEY UPDATE statut = statut;

-- Vêtements techniques (7 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-VET-001', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Chandail thermique homme', 'Sous-vêtement thermique respirant pour homme', 22.00, 59.00, 15, 10, 0.6, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-09'),
('NC-VET-002', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Chandail thermique femme', 'Sous-vêtement thermique respirant pour femme', 22.00, 59.00, 15, 10, 0.6, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-09'),
('NC-VET-003', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Pantalon de randonnée homme', 'Pantalon technique stretch pour homme', 38.00, 89.00, 8, 4, 0.8, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-03'),
('NC-VET-004', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Pantalon de randonnée femme', 'Pantalon technique stretch pour femme', 38.00, 89.00, 8, 4, 0.8, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-03'),
('NC-VET-005', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Manteau coupe-vent', 'Veste coupe-vent imperméable respirante', 55.00, 129.00, 5, 3, 1.1, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-02-19'),
('NC-VET-006', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Tuque en laine mérinos', 'Bonnet chaud en laine mérinos naturelle', 10.00, 29.00, 10, 6, 0.3, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-03-10'),
('NC-VET-007', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Gants isolants Hiver+', 'Gants techniques isolés pour températures extrêmes', 18.00, 45.00, 8, 4, 0.5, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-02-22')
ON DUPLICATE KEY UPDATE statut = statut;

-- Accessoires & cuisine (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-ACC-001', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Réchaud portatif', 'Réchaud à gaz compact pour camping', 25.00, 59.00, 5, 3, 0.9, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-02-28'),
('NC-ACC-002', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Bouteille isotherme 1L', 'Bouteille isolante garde chaud/froid 24h', 12.00, 29.00, 12, 8, 0.4, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-10'),
('NC-ACC-003', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Lampe frontale 300 lumens', 'Lampe frontale LED rechargeable puissante', 14.00, 39.00, 10, 6, 0.2, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-03-12'),
('NC-ACC-004', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Ensemble vaisselle 4 pers.', 'Kit de vaisselle complet pour 4 personnes', 20.00, 49.00, 8, 5, 1.2, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-06'),
('NC-ACC-005', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Filtre à eau compact', 'Système de filtration d''eau portable', 28.00, 69.00, 5, 3, 0.7, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-25'),
('NC-ACC-006', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Couteau multifonction', 'Couteau suisse avec 12 fonctions', 15.00, 39.00, 10, 6, 0.5, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-09')
ON DUPLICATE KEY UPDATE statut = statut;

-- Électronique & navigation (5 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-ELE-001', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Montre GPS plein air', 'Montre GPS multisport avec cartographie', 120.00, 279.00, 3, 2, 0.9, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-02-17'),
('NC-ELE-002', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Chargeur solaire 20W', 'Panneau solaire portable pour appareils USB', 35.00, 79.00, 5, 3, 0.6, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-03-01'),
('NC-ELE-003', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Boussole de précision', 'Boussole professionnelle avec miroir de visée', 9.00, 24.00, 12, 8, 0.2, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-11'),
('NC-ELE-004', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Radio météo portable', 'Radio d''urgence avec alerte météo', 22.00, 49.00, 5, 3, 0.8, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-02-28'),
('NC-ELE-005', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Lampe USB rechargeable', 'Lampe de camping rechargeable par USB', 11.00, 25.00, 10, 6, 0.3, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-03-07')
ON DUPLICATE KEY UPDATE statut = statut;

-- Insertion des niveaux de stock
INSERT INTO niveaux_stock (produit_id, emplacement, qte_disponible, qte_reservee) VALUES
-- Tentes & abris
((SELECT id FROM produits WHERE sku = 'NC-TNT-001'), 'A1', 18, 0),
((SELECT id FROM produits WHERE sku = 'NC-TNT-002'), 'A1', 9, 0),
((SELECT id FROM produits WHERE sku = 'NC-TNT-003'), 'A2', 25, 0),
((SELECT id FROM produits WHERE sku = 'NC-TNT-004'), 'A2', 40, 0),
((SELECT id FROM produits WHERE sku = 'NC-TNT-005'), 'A1', 12, 0),
((SELECT id FROM produits WHERE sku = 'NC-TNT-006'), 'A3', 30, 0),
-- Sacs & portage
((SELECT id FROM produits WHERE sku = 'NC-SAC-001'), 'B1', 20, 0),
((SELECT id FROM produits WHERE sku = 'NC-SAC-002'), 'B2', 25, 0),
((SELECT id FROM produits WHERE sku = 'NC-SAC-003'), 'B3', 15, 0),
((SELECT id FROM produits WHERE sku = 'NC-SAC-004'), 'B3', 35, 0),
((SELECT id FROM produits WHERE sku = 'NC-SAC-005'), 'B2', 40, 0),
((SELECT id FROM produits WHERE sku = 'NC-SAC-006'), 'B1', 18, 0),
-- Vêtements techniques
((SELECT id FROM produits WHERE sku = 'NC-VET-001'), 'C1', 50, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-002'), 'C1', 48, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-003'), 'C2', 30, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-004'), 'C2', 32, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-005'), 'C3', 20, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-006'), 'C4', 40, 0),
((SELECT id FROM produits WHERE sku = 'NC-VET-007'), 'C4', 25, 0),
-- Accessoires & cuisine
((SELECT id FROM produits WHERE sku = 'NC-ACC-001'), 'D1', 20, 0),
((SELECT id FROM produits WHERE sku = 'NC-ACC-002'), 'D2', 40, 0),
((SELECT id FROM produits WHERE sku = 'NC-ACC-003'), 'D3', 35, 0),
((SELECT id FROM produits WHERE sku = 'NC-ACC-004'), 'D2', 25, 0),
((SELECT id FROM produits WHERE sku = 'NC-ACC-005'), 'D3', 18, 0),
((SELECT id FROM produits WHERE sku = 'NC-ACC-006'), 'D4', 28, 0),
-- Électronique & navigation
((SELECT id FROM produits WHERE sku = 'NC-ELE-001'), 'E1', 10, 0),
((SELECT id FROM produits WHERE sku = 'NC-ELE-002'), 'E2', 18, 0),
((SELECT id FROM produits WHERE sku = 'NC-ELE-003'), 'E3', 40, 0),
((SELECT id FROM produits WHERE sku = 'NC-ELE-004'), 'E4', 15, 0),
((SELECT id FROM produits WHERE sku = 'NC-ELE-005'), 'E5', 35, 0)
ON DUPLICATE KEY UPDATE qte_disponible = qte_disponible;

-- =====================================================
-- PARTIE 11: CONFIGURATION AUTHENTIFICATION MYSQL
-- =====================================================
-- Note: Les mots de passe sont stockés en clair dans les tables
-- (colonne mot_de_passe dans employes et clients)
-- 
-- IMPORTANT: La chaîne de connexion dans DatabaseHelper.cs utilise:
-- - AllowPublicKeyRetrieval=true (pour gérer caching_sha2_password sans SSL)
-- - SslMode=None (connexion non sécurisée pour développement local)
-- 
-- Les utilisateurs (employés et clients) se connectent avec:
-- - Email (courriel) + Mot de passe en clair
-- - Pas de hashage dans l'application
-- =====================================================

-- =====================================================
-- FIN DU SCRIPT SQL COMPLET UNIFIÉ
-- =====================================================

-- Message de confirmation
SELECT '✅ Base de données NordikAdventuresERP créée avec succès!' AS Message,
       '✅ Tous les modules sont installés: Stocks, Finances, CRM, RH' AS Modules,
       '✅ Triggers et automatisations sont actifs' AS Automatisations,
       '✅ 30 produits insérés' AS Produits,
       '✅ 4 employés de test créés' AS Employes,
       '✅ 5 clients de test créés' AS Clients,
       '✅ Authentification MySQL configurée' AS Authentification,
       '⚠️ Mots de passe en clair (non hashés)' AS Securite;

