-- =====================================================
-- DONNÉES DE TEST - EMPLOYÉS ET CLIENTS
-- NordikAdventuresERP
-- =====================================================
-- Ce script ajoute des utilisateurs de test pour le développement
-- =====================================================

USE NordikAdventuresERP;

-- =====================================================
-- EMPLOYÉS DE TEST (ajoutés par l'admin)
-- =====================================================

-- Employé 1 : Administrateur
INSERT INTO employes (
    matricule, nom, prenom, courriel, telephone, 
    departement, poste, role_systeme, 
    salaire_annuel, date_embauche, statut, mot_de_passe_hash
) VALUES (
    'EMP001', 'Admin', 'Système', 'admin@nordikadventures.com', '418-123-4567',
    'Administration', 'Administrateur Système', 'Admin',
    75000.00, '2020-01-15', 'Actif', SHA2('admin123', 256)
)
ON DUPLICATE KEY UPDATE mot_de_passe_hash = SHA2('admin123', 256);

-- Employé 2 : Gestionnaire
INSERT INTO employes (
    matricule, nom, prenom, courriel, telephone, 
    departement, poste, role_systeme, 
    salaire_annuel, date_embauche, statut, mot_de_passe_hash
) VALUES (
    'EMP002', 'Gestion', 'Marie', 'gestionnaire@nordikadventures.com', '418-123-4568',
    'Ventes', 'Gestionnaire des Ventes', 'Gestionnaire',
    65000.00, '2021-03-20', 'Actif', SHA2('gestionnaire123', 256)
)
ON DUPLICATE KEY UPDATE mot_de_passe_hash = SHA2('gestionnaire123', 256);

-- Employé 3 : Employé Ventes
INSERT INTO employes (
    matricule, nom, prenom, courriel, telephone, 
    departement, poste, role_systeme, 
    salaire_annuel, date_embauche, statut, mot_de_passe_hash
) VALUES (
    'EMP003', 'Employé', 'Jean', 'employe@nordikadventures.com', '418-123-4569',
    'Ventes', 'Employé Ventes', 'Employé Ventes',
    45000.00, '2022-06-10', 'Actif', SHA2('employe123', 256)
)
ON DUPLICATE KEY UPDATE mot_de_passe_hash = SHA2('employe123', 256);

-- Employé 4 : Comptable
INSERT INTO employes (
    matricule, nom, prenom, courriel, telephone, 
    departement, poste, role_systeme, 
    salaire_annuel, date_embauche, statut, mot_de_passe_hash
) VALUES (
    'EMP004', 'Comptable', 'Paul', 'comptable@nordikadventures.com', '418-123-4570',
    'Comptabilité', 'Comptable', 'Comptable',
    55000.00, '2021-09-01', 'Actif', SHA2('comptable123', 256)
)
ON DUPLICATE KEY UPDATE mot_de_passe_hash = SHA2('comptable123', 256);

-- =====================================================
-- CLIENTS DE TEST (inscrits via le site)
-- =====================================================

-- Client 1 : Jean Dupont
INSERT INTO clients (
    type, nom, courriel_contact, telephone, statut, date_creation
) VALUES (
    'Particulier', 'Jean Dupont', 'client1@test.com', '418-555-0001', 'Actif', NOW()
)
ON DUPLICATE KEY UPDATE courriel_contact = 'client1@test.com';

-- Client 2 : Marie Martin
INSERT INTO clients (
    type, nom, courriel_contact, telephone, statut, date_creation
) VALUES (
    'Particulier', 'Marie Martin', 'client2@test.com', '418-555-0002', 'Actif', NOW()
)
ON DUPLICATE KEY UPDATE courriel_contact = 'client2@test.com';

-- Client 3 : Pierre Tremblay
INSERT INTO clients (
    type, nom, courriel_contact, telephone, statut, date_creation
) VALUES (
    'Particulier', 'Pierre Tremblay', 'client3@test.com', '418-555-0003', 'Actif', NOW()
)
ON DUPLICATE KEY UPDATE courriel_contact = 'client3@test.com';

-- =====================================================
-- NOTES IMPORTANTES
-- =====================================================
-- 
-- ⚠️ Les mots de passe sont hashés avec SHA2 (256 bits)
-- ⚠️ En production, utiliser bcrypt ou Argon2 pour plus de sécurité
-- 
-- =====================================================
-- COMPTES DE TEST DISPONIBLES
-- =====================================================
-- 
-- 👔 EMPLOYÉS (accès au PGI) :
-- 
-- 1. Administrateur
--    Username: admin
--    Mot de passe: admin123
--    Accès: Tous les modules
-- 
-- 2. Gestionnaire
--    Username: gestionnaire
--    Mot de passe: gestionnaire123
--    Accès: Tous les modules
-- 
-- 3. Employé Ventes
--    Username: employe
--    Mot de passe: employe123
--    Accès: Modules selon permissions
-- 
-- 4. Comptable
--    Username: comptable
--    Mot de passe: comptable123
--    Accès: Module Finances principalement
-- 
-- =====================================================
-- 
-- 🛒 CLIENTS (accès au site d'achat) :
-- 
-- 1. Jean Dupont
--    Email: client1@test.com
--    Mot de passe: client123
-- 
-- 2. Marie Martin
--    Email: client2@test.com
--    Mot de passe: client123
-- 
-- 3. Pierre Tremblay
--    Email: client3@test.com
--    Mot de passe: client123
-- 
-- =====================================================
-- 
-- ⚠️ IMPORTANT :
-- Pour l'instant, les clients n'ont pas de mot de passe dans la table clients.
-- La vérification des mots de passe clients se fait dans le code C# (dictionnaire).
-- Pour la production, il faudrait ajouter une colonne mot_de_passe_hash dans la table clients.
-- 
-- =====================================================

