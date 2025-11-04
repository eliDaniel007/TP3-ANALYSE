-- =====================================================
-- SYSTÈME D'AUTHENTIFICATION - PGI NORDIK ADVENTURES
-- =====================================================
-- À ajouter à la base NordikAdventuresERP

USE NordikAdventuresERP;

-- Table: utilisateurs
-- Description: Utilisateurs du système avec rôles et permissions
CREATE TABLE IF NOT EXISTS utilisateurs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom_utilisateur VARCHAR(50) NOT NULL UNIQUE,
    mot_de_passe VARCHAR(255) NOT NULL, -- Hash du mot de passe (SHA256 ou bcrypt)
    nom_complet VARCHAR(150) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    role ENUM('Admin', 'Gestionnaire', 'Employe', 'Comptable') NOT NULL DEFAULT 'Employe',
    acces_stocks BOOLEAN NOT NULL DEFAULT TRUE,
    acces_finances BOOLEAN NOT NULL DEFAULT FALSE,
    acces_crm BOOLEAN NOT NULL DEFAULT TRUE,
    statut ENUM('Actif', 'Inactif', 'Suspendu') NOT NULL DEFAULT 'Actif',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    derniere_connexion DATETIME,
    INDEX idx_nom_utilisateur (nom_utilisateur),
    INDEX idx_email (email),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Utilisateurs du système';

-- Insertion d'utilisateurs de test
-- Mot de passe: "admin123" (à hasher en production)
INSERT INTO utilisateurs (nom_utilisateur, mot_de_passe, nom_complet, email, role, acces_stocks, acces_finances, acces_crm, statut) VALUES
('admin', SHA2('admin123', 256), 'Administrateur Système', 'admin@nordikadventures.com', 'Admin', TRUE, TRUE, TRUE, 'Actif'),
('gestionnaire', SHA2('gestionnaire123', 256), 'Marie Gestion', 'marie.gestion@nordikadventures.com', 'Gestionnaire', TRUE, TRUE, TRUE, 'Actif'),
('employe', SHA2('employe123', 256), 'Jean Employé', 'jean.employe@nordikadventures.com', 'Employe', TRUE, FALSE, TRUE, 'Actif'),
('comptable', SHA2('comptable123', 256), 'Paul Comptable', 'paul.comptable@nordikadventures.com', 'Comptable', FALSE, TRUE, FALSE, 'Actif');

-- Table: sessions
-- Description: Sessions actives des utilisateurs
CREATE TABLE IF NOT EXISTS sessions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utilisateur_id INT NOT NULL,
    token_session VARCHAR(255) NOT NULL UNIQUE,
    date_debut DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    date_expiration DATETIME NOT NULL,
    adresse_ip VARCHAR(45),
    CONSTRAINT fk_session_utilisateur FOREIGN KEY (utilisateur_id) REFERENCES utilisateurs(id) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_token (token_session),
    INDEX idx_utilisateur (utilisateur_id)
) ENGINE=InnoDB COMMENT='Sessions utilisateurs actives';

-- Table: log_connexions
-- Description: Historique des connexions (pour audit de sécurité)
CREATE TABLE IF NOT EXISTS log_connexions (
    id INT AUTO_INCREMENT PRIMARY KEY,
    utilisateur_id INT,
    nom_utilisateur VARCHAR(50) NOT NULL,
    date_tentative DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    succes BOOLEAN NOT NULL,
    adresse_ip VARCHAR(45),
    message VARCHAR(255),
    INDEX idx_utilisateur (utilisateur_id),
    INDEX idx_date (date_tentative),
    INDEX idx_succes (succes)
) ENGINE=InnoDB COMMENT='Log des tentatives de connexion';

-- Trigger: Mise à jour de la dernière connexion
DELIMITER $$

CREATE TRIGGER trg_maj_derniere_connexion
AFTER INSERT ON log_connexions
FOR EACH ROW
BEGIN
    IF NEW.succes = TRUE AND NEW.utilisateur_id IS NOT NULL THEN
        UPDATE utilisateurs
        SET derniere_connexion = NEW.date_tentative
        WHERE id = NEW.utilisateur_id;
    END IF;
END$$

DELIMITER ;

-- =====================================================
-- COMPTES DE TEST DISPONIBLES
-- =====================================================
-- 
-- Administrateur:
-- Nom d'utilisateur: admin
-- Mot de passe: admin123
-- Accès: Tous les modules
-- 
-- Gestionnaire:
-- Nom d'utilisateur: gestionnaire
-- Mot de passe: gestionnaire123
-- Accès: Tous les modules
-- 
-- Employé:
-- Nom d'utilisateur: employe
-- Mot de passe: employe123
-- Accès: Stocks + CRM uniquement
-- 
-- Comptable:
-- Nom d'utilisateur: comptable
-- Mot de passe: comptable123
-- Accès: Finances uniquement
-- =====================================================

