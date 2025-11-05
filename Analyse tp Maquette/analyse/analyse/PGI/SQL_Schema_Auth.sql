-- =====================================================
-- Script SQL pour Authentification
-- NordikAdventuresERP - Module d'authentification
-- =====================================================
-- Ce script ajoute les colonnes nécessaires pour l'authentification
-- des employés et des clients, ainsi que des données de test.
-- =====================================================

USE NordikAdventuresERP;

-- =====================================================
-- 1. AJOUT DES COLONNES MOT_DE_PASSE
-- =====================================================

-- Ajouter mot_de_passe à la table employes
-- NOTE: Si la colonne existe déjà, vous pouvez ignorer l'erreur ou commenter cette ligne
ALTER TABLE employes 
ADD COLUMN mot_de_passe VARCHAR(255) DEFAULT NULL 
COMMENT 'Mot de passe pour authentification (non hashé)';

-- Ajouter mot_de_passe à la table clients
-- NOTE: Si la colonne existe déjà, vous pouvez ignorer l'erreur ou commenter cette ligne
ALTER TABLE clients 
ADD COLUMN mot_de_passe VARCHAR(255) DEFAULT NULL 
COMMENT 'Mot de passe pour authentification (non hashé)';

-- =====================================================
-- 2. AJOUT D'EMPLOYÉS DE TEST
-- =====================================================

-- Insérer des employés de test avec leurs mots de passe
-- NOTE: Les mots de passe sont en clair (pas de hashage pour simplifier)

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

-- Insérer des clients de test avec leurs mots de passe
-- NOTE: Les emails contiennent tous "client" pour respecter la règle de validation

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
-- 5. NOTES D'UTILISATION
-- =====================================================

/*
=== IDENTIFIANTS DE TEST ===

--- EMPLOYÉS (Accès PGI) ---
1. Administrateur
   Email: admin@nordikadventures.com
   Mot de passe: admin123

2. Gestionnaire
   Email: gestionnaire@nordikadventures.com
   Mot de passe: gestionnaire123

3. Employé Ventes
   Email: employe@nordikadventures.com
   Mot de passe: employe123

4. Comptable
   Email: comptable@nordikadventures.com
   Mot de passe: comptable123

--- CLIENTS (Accès Site d'achat) ---
1. Jean Dupont
   Email: jean.client@test.com
   Mot de passe: client123

2. Marie Martin
   Email: marie.client@test.com
   Mot de passe: client123

3. Pierre Tremblay
   Email: pierre.client@entreprise.com
   Mot de passe: client123

4. Sophie Lavoie
   Email: client.sophie@gmail.com
   Mot de passe: client123

5. Nordik Sports Inc.
   Email: contact@nordikclient.com
   Mot de passe: client123

=== RÈGLES D'AUTHENTIFICATION ===

1. Si l'email contient "client" → Redirection vers ClientShoppingWindow
2. Sinon → Redirection vers ModuleSelectionWindow (PGI)

3. Les mots de passe sont stockés EN CLAIR (pas de hashage pour simplifier)

4. Les clients peuvent s'inscrire eux-mêmes via RegisterWindow
   - L'email DOIT contenir "client"
   - Ils sont automatiquement ajoutés à la table clients

5. Les employés sont ajoutés manuellement par l'administrateur dans la BDD
*/

