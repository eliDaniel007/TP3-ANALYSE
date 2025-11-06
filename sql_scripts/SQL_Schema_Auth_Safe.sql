-- =====================================================
-- Script SQL pour Authentification (VERSION SAFE)
-- NordikAdventuresERP - Module d'authentification
-- =====================================================
-- Cette version vérifie l'existence des colonnes avant de les ajouter
-- =====================================================

USE NordikAdventuresERP;

-- =====================================================
-- 1. AJOUT DES COLONNES MOT_DE_PASSE (SAFE)
-- =====================================================

-- Procédure pour ajouter une colonne si elle n'existe pas
DELIMITER $$

DROP PROCEDURE IF EXISTS AddColumnIfNotExists$$
CREATE PROCEDURE AddColumnIfNotExists(
    IN tableName VARCHAR(100),
    IN columnName VARCHAR(100),
    IN columnDefinition VARCHAR(255)
)
BEGIN
    DECLARE columnExists INT;
    
    -- Vérifier si la colonne existe
    SELECT COUNT(*) INTO columnExists
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = DATABASE()
        AND TABLE_NAME = tableName
        AND COLUMN_NAME = columnName;
    
    -- Ajouter la colonne si elle n'existe pas
    IF columnExists = 0 THEN
        SET @sql = CONCAT('ALTER TABLE ', tableName, ' ADD COLUMN ', columnName, ' ', columnDefinition);
        PREPARE stmt FROM @sql;
        EXECUTE stmt;
        DEALLOCATE PREPARE stmt;
        SELECT CONCAT('✅ Colonne ', columnName, ' ajoutée à ', tableName) AS Resultat;
    ELSE
        SELECT CONCAT('⚠️ Colonne ', columnName, ' existe déjà dans ', tableName) AS Resultat;
    END IF;
END$$

DELIMITER ;

-- Ajouter mot_de_passe à employes
CALL AddColumnIfNotExists('employes', 'mot_de_passe', 
    "VARCHAR(255) DEFAULT NULL COMMENT 'Mot de passe pour authentification (non hashé)'");

-- Ajouter mot_de_passe à clients
CALL AddColumnIfNotExists('clients', 'mot_de_passe', 
    "VARCHAR(255) DEFAULT NULL COMMENT 'Mot de passe pour authentification (non hashé)'");

-- =====================================================
-- 2. AJOUT D'EMPLOYÉS DE TEST
-- =====================================================

-- Admin (CORRIGÉ: departement + role_systeme + salaire_annuel + date_embauche)
INSERT INTO employes (matricule, nom, prenom, courriel, telephone, departement, poste, role_systeme, salaire_annuel, date_embauche, statut, mot_de_passe)
VALUES 
    ('EMP-001', 'Tremblay', 'Admin', 'admin@nordikadventures.com', '514-555-0001', 'Administration', 'Directeur', 'Admin', 85000.00, '2020-01-15', 'Actif', 'admin123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'admin123';

-- Gestionnaire (CORRIGÉ)
INSERT INTO employes (matricule, nom, prenom, courriel, telephone, departement, poste, role_systeme, salaire_annuel, date_embauche, statut, mot_de_passe)
VALUES 
    ('EMP-002', 'Bouchard', 'Marie', 'gestionnaire@nordikadventures.com', '514-555-0002', 'Logistique', 'Gestionnaire Stocks', 'Gestionnaire', 65000.00, '2020-03-20', 'Actif', 'gestionnaire123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'gestionnaire123';

-- Employé Ventes (CORRIGÉ)
INSERT INTO employes (matricule, nom, prenom, courriel, telephone, departement, poste, role_systeme, salaire_annuel, date_embauche, statut, mot_de_passe)
VALUES 
    ('EMP-003', 'Gagnon', 'Pierre', 'employe@nordikadventures.com', '514-555-0003', 'Ventes', 'Conseiller Ventes', 'Employé Ventes', 48000.00, '2021-06-10', 'Actif', 'employe123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'employe123';

-- Comptable (CORRIGÉ)
INSERT INTO employes (matricule, nom, prenom, courriel, telephone, departement, poste, role_systeme, salaire_annuel, date_embauche, statut, mot_de_passe)
VALUES 
    ('EMP-004', 'Côté', 'Sophie', 'comptable@nordikadventures.com', '514-555-0004', 'Comptabilité', 'Comptable', 'Comptable', 60000.00, '2020-09-01', 'Actif', 'comptable123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'comptable123';

-- =====================================================
-- 3. AJOUT DE CLIENTS DE TEST
-- =====================================================

-- Client 1
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe)
VALUES 
    ('Particulier', 'Jean Dupont', 'jean.client@test.com', '438-555-1001', 'Actif', 'client123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'client123';

-- Client 2
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe)
VALUES 
    ('Particulier', 'Marie Martin', 'marie.client@test.com', '438-555-1002', 'Actif', 'client123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'client123';

-- Client 3
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe)
VALUES 
    ('Entreprise', 'Pierre Tremblay', 'pierre.client@entreprise.com', '438-555-1003', 'Actif', 'client123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'client123';

-- Client 4
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe)
VALUES 
    ('Particulier', 'Sophie Lavoie', 'client.sophie@gmail.com', '438-555-1004', 'Actif', 'client123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'client123';

-- Client 5
INSERT INTO clients (type, nom, courriel_contact, telephone, statut, mot_de_passe)
VALUES 
    ('Entreprise', 'Nordik Sports Inc.', 'contact@nordikclient.com', '514-555-2000', 'Fidèle', 'client123')
ON DUPLICATE KEY UPDATE mot_de_passe = 'client123';

-- =====================================================
-- 4. VÉRIFICATION DES DONNÉES
-- =====================================================

-- Afficher les employés avec leurs identifiants
SELECT 
    matricule,
    CONCAT(prenom, ' ', nom) AS nom_complet,
    courriel AS email,
    role_systeme AS role,
    statut,
    '(mot de passe: voir script)' AS mdp_note
FROM employes
WHERE mot_de_passe IS NOT NULL
ORDER BY matricule;

-- Afficher les clients avec leurs identifiants
SELECT 
    id,
    nom,
    courriel_contact AS email,
    type,
    statut,
    '(mot de passe: voir script)' AS mdp_note
FROM clients
WHERE mot_de_passe IS NOT NULL
ORDER BY id;

-- =====================================================
-- 5. NETTOYAGE
-- =====================================================

-- Supprimer la procédure temporaire
DROP PROCEDURE IF EXISTS AddColumnIfNotExists;

-- =====================================================
-- SCRIPT TERMINÉ ✅
-- =====================================================

SELECT '✅ Script d''authentification exécuté avec succès !' AS Message;

