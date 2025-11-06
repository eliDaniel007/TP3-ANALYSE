-- =====================================================
-- NordikAdventuresERP - Schéma Relationnel MySQL 8.0+
-- VERSION SIMPLIFIÉE (22 TABLES)
-- =====================================================
-- Description: Système de gestion intégré (PGI/ERP)
-- Modules: 1) Stocks/Produits  2) Finances/Facturation  3) CRM  4) RH/Paies
-- Auteur: Généré pour TP#2 - INF27523
-- Compatible: MySQL 8.0+ / MySQL Workbench (Forward & Reverse Engineering)
-- =====================================================
-- SIMPLIFICATIONS APPLIQUÉES:
-- ✅ departements → ENUM dans employes.departement
-- ✅ taxes → Taux fixes (TPS 5%, TVQ 9.975%)
-- ✅ emplacements_stock → Fusionné dans niveaux_stock.emplacement
-- ✅ lignes_reception → Intégré dans receptions_marchandises (quantités par produit)
-- =====================================================

-- Création et sélection de la base de données
DROP DATABASE IF EXISTS NordikAdventuresERP;
CREATE DATABASE NordikAdventuresERP CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE NordikAdventuresERP;

-- =====================================================
-- MODULE 0: RESSOURCES HUMAINES (Employés & Paies)
-- =====================================================

-- Table: employes
-- Description: Employés de l'entreprise avec accès au système (département intégré)
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
    mot_de_passe_hash VARCHAR(255),
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_courriel (courriel),
    INDEX idx_departement (departement),
    INDEX idx_statut (statut),
    INDEX idx_role (role_systeme)
) ENGINE=InnoDB COMMENT='Employés avec département intégré';

-- Table: paies
-- Description: Paies des employés avec période intégrée
CREATE TABLE paies (
    id INT AUTO_INCREMENT PRIMARY KEY,
    employe_id INT NOT NULL,
    periode_type ENUM('Hebdomadaire', 'Bimensuelle', 'Mensuelle') NOT NULL DEFAULT 'Mensuelle',
    date_debut_periode DATE NOT NULL,
    date_fin_periode DATE NOT NULL,
    date_paiement DATE NOT NULL,
    salaire_brut DECIMAL(10,2) NOT NULL CHECK(salaire_brut >= 0),
    heures_travaillees DECIMAL(6,2) DEFAULT 0.00 CHECK(heures_travaillees >= 0),
    heures_supplementaires DECIMAL(6,2) DEFAULT 0.00 CHECK(heures_supplementaires >= 0),
    primes DECIMAL(10,2) DEFAULT 0.00 CHECK(primes >= 0),
    deductions DECIMAL(10,2) DEFAULT 0.00 CHECK(deductions >= 0),
    impot_federal DECIMAL(10,2) DEFAULT 0.00 CHECK(impot_federal >= 0),
    impot_provincial DECIMAL(10,2) DEFAULT 0.00 CHECK(impot_provincial >= 0),
    cotisation_rqap DECIMAL(10,2) DEFAULT 0.00 CHECK(cotisation_rqap >= 0),
    cotisation_rrq DECIMAL(10,2) DEFAULT 0.00 CHECK(cotisation_rrq >= 0),
    cotisation_ae DECIMAL(10,2) DEFAULT 0.00 CHECK(cotisation_ae >= 0),
    salaire_net DECIMAL(10,2) NOT NULL CHECK(salaire_net >= 0),
    statut ENUM('Brouillon', 'Validée', 'Payée', 'Annulée') NOT NULL DEFAULT 'Brouillon',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_paie_employe FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT chk_dates_paie CHECK(date_fin_periode >= date_debut_periode AND date_paiement >= date_fin_periode),
    UNIQUE KEY uk_employe_periode (employe_id, date_debut_periode, date_fin_periode),
    INDEX idx_employe (employe_id),
    INDEX idx_periode (date_debut_periode, date_fin_periode),
    INDEX idx_date_paiement (date_paiement),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Paies des employés';

-- =====================================================
-- MODULE 1: PRODUCTS & INVENTORY (Produits & Stocks)
-- =====================================================

-- Table: categories
-- Description: Catégories de produits (ex: Vêtements, Équipement, etc.)
CREATE TABLE categories (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(100) NOT NULL UNIQUE,
    statut ENUM('Actif', 'Inactif') NOT NULL DEFAULT 'Actif',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Catégories de produits';

-- Table: fournisseurs
-- Description: Fournisseurs de produits avec délais de livraison et escomptes
CREATE TABLE fournisseurs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nom VARCHAR(150) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    courriel_contact VARCHAR(100),
    delai_livraison_jours INT NOT NULL DEFAULT 7 CHECK(delai_livraison_jours >= 0),
    pourcentage_escompte DECIMAL(5,2) NOT NULL DEFAULT 0.00 CHECK(pourcentage_escompte BETWEEN 0 AND 100),
    statut ENUM('Actif', 'Inactif') NOT NULL DEFAULT 'Actif',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Fournisseurs';

-- Table: produits
-- Description: Catalogue de produits avec coût, prix de vente et seuils de stock
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

-- Table: niveaux_stock
-- Description: Niveaux de stock par produit (emplacement intégré)
CREATE TABLE niveaux_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL UNIQUE,
    emplacement VARCHAR(100) NOT NULL DEFAULT 'Entrepôt Principal',
    qte_disponible INT NOT NULL DEFAULT 0 CHECK(qte_disponible >= 0),
    qte_reservee INT NOT NULL DEFAULT 0 CHECK(qte_reservee >= 0),
    derniere_maj DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_stock_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB COMMENT='Niveaux de stock (emplacement intégré)';

-- Table: mouvements_stock
-- Description: Historique des mouvements de stock (entrées/sorties)
CREATE TABLE mouvements_stock (
    id INT AUTO_INCREMENT PRIMARY KEY,
    produit_id INT NOT NULL,
    type_mouvement ENUM('ENTREE', 'SORTIE') NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    raison ENUM('reception_achat', 'vente', 'ajustement', 'retour_entree', 'retour_sortie', 'manuel') NOT NULL,
    date_mouvement DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    employe_id INT,
    note_utilisateur TEXT,
    commande_vente_id INT,
    achat_id INT,
    CONSTRAINT fk_mouvement_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_mouvement_employe FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_produit_date (produit_id, date_mouvement),
    INDEX idx_type_mouvement (type_mouvement),
    INDEX idx_employe (employe_id),
    INDEX idx_commande (commande_vente_id),
    INDEX idx_achat (achat_id)
) ENGINE=InnoDB COMMENT='Mouvements de stock';

-- =====================================================
-- MODULE 2: FINANCE & BILLING (Finances & Facturation)
-- =====================================================

-- Table: clients
-- Description: Clients (particuliers ou entreprises) avec statut et informations de contact
CREATE TABLE clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    type ENUM('Particulier', 'Entreprise') NOT NULL DEFAULT 'Particulier',
    nom VARCHAR(200) NOT NULL,
    courriel_contact VARCHAR(100) UNIQUE,
    telephone VARCHAR(20),
    statut ENUM('Actif', 'Inactif', 'Prospect', 'Fidèle') NOT NULL DEFAULT 'Prospect',
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_courriel (courriel_contact),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Clients';

-- Table: adresses
-- Description: Adresses de facturation et de livraison des clients
CREATE TABLE adresses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    type ENUM('facturation', 'livraison') NOT NULL,
    ligne1 VARCHAR(255) NOT NULL,
    ligne2 VARCHAR(255),
    ville VARCHAR(100) NOT NULL,
    province VARCHAR(100) NOT NULL,
    code_postal VARCHAR(20) NOT NULL,
    pays VARCHAR(100) NOT NULL DEFAULT 'Canada',
    par_defaut BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_adresse_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_client (client_id),
    INDEX idx_type (type)
) ENGINE=InnoDB COMMENT='Adresses clients';

-- Table: commandes_vente
-- Description: Commandes de vente avec calculs de taxes (TPS 5%, TVQ 9.975% fixes)
CREATE TABLE commandes_vente (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    employe_id INT,
    date_commande DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    statut ENUM('Brouillon', 'Confirmée', 'Facturée', 'Annulée') NOT NULL DEFAULT 'Brouillon',
    devise CHAR(3) NOT NULL DEFAULT 'CAD',
    sous_total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    taxe_tps DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    taxe_tvq DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    stock_reserve BOOLEAN NOT NULL DEFAULT FALSE,
    note TEXT,
    CONSTRAINT fk_commande_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_commande_employe FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_client (client_id),
    INDEX idx_employe (employe_id),
    INDEX idx_date_commande (date_commande),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Commandes de vente (taxes fixes)';

-- Table: lignes_commande
-- Description: Lignes de commande de vente
CREATE TABLE lignes_commande (
    id INT AUTO_INCREMENT PRIMARY KEY,
    commande_id INT NOT NULL,
    produit_id INT NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    prix_unitaire DECIMAL(10,2) NOT NULL CHECK(prix_unitaire >= 0),
    total_ligne DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT fk_lignecommande_commande FOREIGN KEY (commande_id) REFERENCES commandes_vente(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_lignecommande_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_commande (commande_id),
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB COMMENT='Lignes de commande';

-- Table: factures
-- Description: Factures émises pour les commandes avec gestion des paiements
CREATE TABLE factures (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_facture VARCHAR(30) NOT NULL UNIQUE,
    commande_id INT,
    employe_emetteur_id INT,
    date_emission DATE NOT NULL,
    date_echeance DATE NOT NULL,
    statut ENUM('Brouillon', 'Émise', 'Payée', 'PayéePartiellement', 'Annulée') NOT NULL DEFAULT 'Émise',
    sous_total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    taxe_tps DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    taxe_tvq DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    solde_du DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT fk_facture_commande FOREIGN KEY (commande_id) REFERENCES commandes_vente(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_facture_employe FOREIGN KEY (employe_emetteur_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_numero_facture (numero_facture),
    INDEX idx_commande (commande_id),
    INDEX idx_employe (employe_emetteur_id),
    INDEX idx_date_echeance (date_echeance),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Factures';

-- Table: lignes_facture
-- Description: Lignes de facture
CREATE TABLE lignes_facture (
    id INT AUTO_INCREMENT PRIMARY KEY,
    facture_id INT NOT NULL,
    produit_id INT NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    prix_unitaire DECIMAL(10,2) NOT NULL CHECK(prix_unitaire >= 0),
    total_ligne DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT fk_lignefacture_facture FOREIGN KEY (facture_id) REFERENCES factures(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_lignefacture_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_facture (facture_id),
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB COMMENT='Lignes de facture';

-- Table: paiements
-- Description: Paiements reçus pour les factures
CREATE TABLE paiements (
    id INT AUTO_INCREMENT PRIMARY KEY,
    facture_id INT NOT NULL,
    employe_recepteur_id INT,
    date_paiement DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    methode ENUM('Carte', 'Interac', 'Comptant', 'VirementBancaire', 'Autre') NOT NULL,
    montant DECIMAL(12,2) NOT NULL CHECK(montant > 0),
    reference VARCHAR(100),
    CONSTRAINT fk_paiement_facture FOREIGN KEY (facture_id) REFERENCES factures(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_paiement_employe FOREIGN KEY (employe_recepteur_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_facture (facture_id),
    INDEX idx_employe (employe_recepteur_id),
    INDEX idx_date_paiement (date_paiement)
) ENGINE=InnoDB COMMENT='Paiements reçus';

-- Table: achats
-- Description: Bons de commande d'achat auprès des fournisseurs
CREATE TABLE achats (
    id INT AUTO_INCREMENT PRIMARY KEY,
    fournisseur_id INT NOT NULL,
    employe_acheteur_id INT,
    date_bc DATE NOT NULL,
    statut ENUM('Brouillon', 'Commandé', 'Reçu', 'Annulé') NOT NULL DEFAULT 'Brouillon',
    sous_total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    taxe DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    total DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT fk_achat_fournisseur FOREIGN KEY (fournisseur_id) REFERENCES fournisseurs(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_achat_employe FOREIGN KEY (employe_acheteur_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_fournisseur (fournisseur_id),
    INDEX idx_employe (employe_acheteur_id),
    INDEX idx_date_bc (date_bc),
    INDEX idx_statut (statut)
) ENGINE=InnoDB COMMENT='Commandes d\'achat';

-- Table: lignes_achat
-- Description: Lignes de commande d'achat
CREATE TABLE lignes_achat (
    id INT AUTO_INCREMENT PRIMARY KEY,
    achat_id INT NOT NULL,
    produit_id INT NOT NULL,
    quantite INT NOT NULL CHECK(quantite > 0),
    cout_unitaire DECIMAL(10,2) NOT NULL CHECK(cout_unitaire >= 0),
    total_ligne DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT fk_ligneachat_achat FOREIGN KEY (achat_id) REFERENCES achats(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_ligneachat_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_achat (achat_id),
    INDEX idx_produit (produit_id)
) ENGINE=InnoDB COMMENT='Lignes de commande d\'achat';

-- Table: receptions_marchandises
-- Description: Réceptions de marchandises (liées directement aux achats)
CREATE TABLE receptions_marchandises (
    id INT AUTO_INCREMENT PRIMARY KEY,
    achat_id INT NOT NULL,
    produit_id INT NOT NULL,
    quantite_recue INT NOT NULL CHECK(quantite_recue > 0),
    date_reception DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    note TEXT,
    CONSTRAINT fk_reception_achat FOREIGN KEY (achat_id) REFERENCES achats(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    CONSTRAINT fk_reception_produit FOREIGN KEY (produit_id) REFERENCES produits(id) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_achat (achat_id),
    INDEX idx_produit (produit_id),
    INDEX idx_date_reception (date_reception)
) ENGINE=InnoDB COMMENT='Réceptions de marchandises (simplifié)';

-- Table: depenses
-- Description: Dépenses d'exploitation de l'entreprise (catégorie intégrée comme ENUM)
CREATE TABLE depenses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    numero_depense VARCHAR(30) NOT NULL UNIQUE,
    categorie ENUM('Loyer', 'Électricité', 'Téléphone_Internet', 'Marketing_Publicité', 'Fournitures_Bureau', 'Entretien_Réparation', 'Assurances', 'Frais_Bancaires', 'Transport_Livraison', 'Formation', 'Services_Professionnels', 'Autre') NOT NULL DEFAULT 'Autre',
    fournisseur_id INT,
    employe_createur_id INT,
    date_depense DATE NOT NULL,
    description VARCHAR(255) NOT NULL,
    montant DECIMAL(12,2) NOT NULL CHECK(montant > 0),
    montant_taxe DECIMAL(12,2) DEFAULT 0.00 CHECK(montant_taxe >= 0),
    montant_total DECIMAL(12,2) NOT NULL,
    statut ENUM('Brouillon', 'Approuvée', 'Payée', 'Annulée') NOT NULL DEFAULT 'Brouillon',
    methode_paiement ENUM('Carte', 'Interac', 'Comptant', 'VirementBancaire', 'Chèque', 'Autre') DEFAULT NULL,
    reference_paiement VARCHAR(100),
    date_paiement DATE,
    piece_jointe_url VARCHAR(500),
    note TEXT,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_depense_fournisseur FOREIGN KEY (fournisseur_id) REFERENCES fournisseurs(id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_depense_employe FOREIGN KEY (employe_createur_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_numero (numero_depense),
    INDEX idx_categorie (categorie),
    INDEX idx_fournisseur (fournisseur_id),
    INDEX idx_employe (employe_createur_id),
    INDEX idx_date_depense (date_depense),
    INDEX idx_statut (statut),
    INDEX idx_date_paiement (date_paiement)
) ENGINE=InnoDB COMMENT='Dépenses d\'exploitation';

-- =====================================================
-- MODULE 3: CRM (Customer Relationship Management)
-- =====================================================

-- Table: interactions_clients
-- Description: Historique des interactions avec les clients
CREATE TABLE interactions_clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    employe_id INT,
    date_interaction DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    canal ENUM('courriel', 'appel', 'rencontre', 'systeme', 'autre') NOT NULL,
    sujet VARCHAR(255),
    contenu TEXT,
    commande_vente_id INT,
    CONSTRAINT fk_interaction_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_interaction_employe FOREIGN KEY (employe_id) REFERENCES employes(id) ON DELETE SET NULL ON UPDATE CASCADE,
    CONSTRAINT fk_interaction_commande FOREIGN KEY (commande_vente_id) REFERENCES commandes_vente(id) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_client (client_id),
    INDEX idx_employe (employe_id),
    INDEX idx_commande (commande_vente_id),
    INDEX idx_date_interaction (date_interaction),
    FULLTEXT idx_recherche_texte (sujet, contenu)
) ENGINE=InnoDB COMMENT='Interactions clients';

-- Table: scores_clients
-- Description: Scores de fidélisation client (RFM, satisfaction, etc.)
CREATE TABLE scores_clients (
    client_id INT PRIMARY KEY,
    poids_achat INT NOT NULL DEFAULT 0,
    poids_frequence INT NOT NULL DEFAULT 0,
    satisfaction_moyenne DECIMAL(3,2) DEFAULT 0.00 CHECK(satisfaction_moyenne BETWEEN 0 AND 5),
    score_total DECIMAL(6,2) NOT NULL DEFAULT 0.00,
    derniere_maj DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_score_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB COMMENT='Scores clients';

-- Table: avis_clients
-- Description: Rétroaction des clients (évaluations, commentaires)
CREATE TABLE avis_clients (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    note TINYINT NOT NULL CHECK(note BETWEEN 1 AND 5),
    commentaire TEXT,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_avis_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_client (client_id),
    INDEX idx_note (note),
    INDEX idx_date_creation (date_creation)
) ENGINE=InnoDB COMMENT='Rétroaction clients';

-- Table: alertes_crm
-- Description: Alertes CRM (clients insatisfaits, comptes en retard, etc.)
CREATE TABLE alertes_crm (
    id INT AUTO_INCREMENT PRIMARY KEY,
    client_id INT NOT NULL,
    date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    raison VARCHAR(255) NOT NULL,
    resolue BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_alerte_client FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_client (client_id),
    INDEX idx_resolue (resolue)
) ENGINE=InnoDB COMMENT='Alertes CRM';

-- =====================================================
-- VUES (Views) - Rapports et Requêtes Fréquentes
-- =====================================================

-- Vue: v_ventes_par_client
-- Description: Résumé des ventes par client (nombre de commandes, ventes totales, dernière commande)
CREATE OR REPLACE VIEW v_ventes_par_client AS
SELECT 
    c.id AS client_id,
    c.nom AS nom_client,
    c.statut,
    COUNT(DISTINCT cv.id) AS total_commandes,
    COALESCE(SUM(f.total), 0) AS ventes_totales,
    MAX(cv.date_commande) AS date_derniere_commande
FROM clients c
LEFT JOIN commandes_vente cv ON c.id = cv.client_id AND cv.statut != 'Annulée'
LEFT JOIN factures f ON cv.id = f.commande_id AND f.statut != 'Annulée'
GROUP BY c.id, c.nom, c.statut;

-- Vue: v_valorisation_stock
-- Description: Valorisation du stock (coût et prix de vente)
CREATE OR REPLACE VIEW v_valorisation_stock AS
SELECT 
    p.id AS produit_id,
    p.sku,
    p.nom AS nom_produit,
    p.cout,
    p.prix,
    p.marge_brute,
    p.poids_kg,
    ns.emplacement,
    ns.qte_disponible,
    ns.qte_reservee,
    (ns.qte_disponible - ns.qte_reservee) AS qte_en_main,
    ns.qte_disponible * p.cout AS valeur_cout,
    ns.qte_disponible * p.prix AS valeur_prix
FROM produits p
LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id;

-- Vue: v_produits_a_reapprovisionner
-- Description: Produits à réapprovisionner (quantité <= point de réapprovisionnement)
CREATE OR REPLACE VIEW v_produits_a_reapprovisionner AS
SELECT 
    p.id AS produit_id,
    p.sku,
    p.nom AS nom_produit,
    p.seuil_reapprovisionnement,
    p.stock_minimum,
    COALESCE(ns.qte_disponible, 0) AS qte_disponible,
    COALESCE(ns.qte_reservee, 0) AS qte_reservee,
    f.nom AS nom_fournisseur,
    f.delai_livraison_jours
FROM produits p
LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
INNER JOIN fournisseurs f ON p.fournisseur_id = f.id
WHERE p.statut = 'Actif'
  AND COALESCE(ns.qte_disponible, 0) <= p.seuil_reapprovisionnement;

-- Vue: v_factures_en_retard
-- Description: Factures en retard de paiement
CREATE OR REPLACE VIEW v_factures_en_retard AS
SELECT 
    f.id AS facture_id,
    f.numero_facture,
    f.date_emission,
    f.date_echeance,
    DATEDIFF(CURDATE(), f.date_echeance) AS jours_retard,
    f.total,
    f.solde_du,
    c.id AS client_id,
    c.nom AS nom_client,
    c.courriel_contact
FROM factures f
INNER JOIN commandes_vente cv ON f.commande_id = cv.id
INNER JOIN clients c ON cv.client_id = c.id
WHERE f.date_echeance < CURDATE()
  AND f.solde_du > 0
  AND f.statut NOT IN ('Annulée', 'Payée');

-- Vue: v_depenses_par_categorie
-- Description: Résumé des dépenses par catégorie (ENUM)
CREATE OR REPLACE VIEW v_depenses_par_categorie AS
SELECT 
    d.categorie AS nom_categorie,
    COUNT(d.id) AS nombre_depenses,
    COALESCE(SUM(CASE WHEN d.statut = 'Payée' THEN d.montant_total ELSE 0 END), 0) AS total_paye,
    COALESCE(SUM(CASE WHEN d.statut = 'Approuvée' THEN d.montant_total ELSE 0 END), 0) AS total_approuve,
    COALESCE(SUM(CASE WHEN d.statut IN ('Payée', 'Approuvée') THEN d.montant_total ELSE 0 END), 0) AS total_general
FROM depenses d
WHERE d.statut != 'Annulée'
GROUP BY d.categorie;

-- Vue: v_depenses_mensuelles
-- Description: Dépenses mensuelles pour analyse de tendances
CREATE OR REPLACE VIEW v_depenses_mensuelles AS
SELECT 
    YEAR(d.date_depense) AS annee,
    MONTH(d.date_depense) AS mois,
    DATE_FORMAT(d.date_depense, '%Y-%m') AS periode,
    d.categorie,
    COUNT(d.id) AS nombre_depenses,
    COALESCE(SUM(d.montant_total), 0) AS total_depenses
FROM depenses d
WHERE d.statut IN ('Payée', 'Approuvée')
GROUP BY YEAR(d.date_depense), MONTH(d.date_depense), DATE_FORMAT(d.date_depense, '%Y-%m'), d.categorie;

-- Vue: v_etat_financier_simplifie
-- Description: État financier simplifié (Ventes - Coût produits - Dépenses = Profit Net)
CREATE OR REPLACE VIEW v_etat_financier_simplifie AS
SELECT 
    DATE_FORMAT(periode.date_ref, '%Y-%m') AS periode,
    YEAR(periode.date_ref) AS annee,
    MONTH(periode.date_ref) AS mois,
    -- Ventes
    COALESCE(ventes.total_ventes, 0) AS ventes_totales,
    -- Coût des produits vendus (calculé depuis les lignes de facture)
    COALESCE(couts.cout_produits_vendus, 0) AS cout_produits_vendus,
    -- Profit Brut
    COALESCE(ventes.total_ventes, 0) - COALESCE(couts.cout_produits_vendus, 0) AS profit_brut,
    -- Dépenses d'exploitation (sans salaires)
    COALESCE(depenses_exploit.total_depenses, 0) AS depenses_exploitation,
    -- Salaires
    COALESCE(salaires.total_salaires, 0) AS depenses_salaires,
    -- Total dépenses
    COALESCE(depenses_exploit.total_depenses, 0) + COALESCE(salaires.total_salaires, 0) AS total_depenses,
    -- Profit Net
    (COALESCE(ventes.total_ventes, 0) - COALESCE(couts.cout_produits_vendus, 0)) - 
    (COALESCE(depenses_exploit.total_depenses, 0) + COALESCE(salaires.total_salaires, 0)) AS profit_net
FROM (
    -- Générer les périodes (derniers 12 mois)
    SELECT DISTINCT DATE_FORMAT(date_emission, '%Y-%m-01') AS date_ref
    FROM factures
    WHERE date_emission >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
) periode
LEFT JOIN (
    -- Ventes par période
    SELECT 
        DATE_FORMAT(f.date_emission, '%Y-%m-01') AS periode,
        SUM(f.total) AS total_ventes
    FROM factures f
    WHERE f.statut IN ('Émise', 'Payée', 'PayéePartiellement')
    GROUP BY DATE_FORMAT(f.date_emission, '%Y-%m-01')
) ventes ON periode.date_ref = ventes.periode
LEFT JOIN (
    -- Coût des produits vendus par période
    SELECT 
        DATE_FORMAT(f.date_emission, '%Y-%m-01') AS periode,
        SUM(lf.quantite * p.cout) AS cout_produits_vendus
    FROM factures f
    INNER JOIN lignes_facture lf ON f.id = lf.facture_id
    INNER JOIN produits p ON lf.produit_id = p.id
    WHERE f.statut IN ('Émise', 'Payée', 'PayéePartiellement')
    GROUP BY DATE_FORMAT(f.date_emission, '%Y-%m-01')
) couts ON periode.date_ref = couts.periode
LEFT JOIN (
    -- Dépenses d'exploitation par période
    SELECT 
        DATE_FORMAT(d.date_depense, '%Y-%m-01') AS periode,
        SUM(d.montant_total) AS total_depenses
    FROM depenses d
    WHERE d.statut IN ('Payée', 'Approuvée')
    GROUP BY DATE_FORMAT(d.date_depense, '%Y-%m-01')
) depenses_exploit ON periode.date_ref = depenses_exploit.periode
LEFT JOIN (
    -- Salaires par période
    SELECT 
        DATE_FORMAT(p.date_paiement, '%Y-%m-01') AS periode,
        SUM(p.salaire_net) AS total_salaires
    FROM paies p
    WHERE p.statut = 'Payée'
    GROUP BY DATE_FORMAT(p.date_paiement, '%Y-%m-01')
) salaires ON periode.date_ref = salaires.periode
ORDER BY periode.date_ref DESC;

-- =====================================================
-- FONCTIONS STOCKÉES (Stored Functions)
-- =====================================================

DELIMITER $$

-- Fonction: fn_obtenir_stock_disponible
-- Description: Retourne la quantité disponible (non réservée) pour un produit
CREATE FUNCTION fn_obtenir_stock_disponible(
    p_produit_id INT
) RETURNS INT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE v_disponible INT;
    
    SELECT (qte_disponible - qte_reservee) INTO v_disponible
    FROM niveaux_stock
    WHERE produit_id = p_produit_id;
    
    RETURN COALESCE(v_disponible, 0);
END$$

-- Fonction: fn_client_a_factures_en_retard
-- Description: Vérifie si un client a des factures en retard
CREATE FUNCTION fn_client_a_factures_en_retard(
    p_client_id INT
) RETURNS BOOLEAN
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE v_nombre INT;
    
    SELECT COUNT(*) INTO v_nombre
    FROM factures f
    INNER JOIN commandes_vente cv ON f.commande_id = cv.id
    WHERE cv.client_id = p_client_id
      AND f.date_echeance < CURDATE()
      AND f.solde_du > 0
      AND f.statut NOT IN ('Annulée', 'Payée');
    
    RETURN v_nombre > 0;
END$$

DELIMITER ;

-- =====================================================
-- PROCÉDURES STOCKÉES (Stored Procedures)
-- =====================================================

DELIMITER $$

-- Procédure: sp_recalculer_totaux_facture
-- Description: Recalcule les totaux d'une facture (sous-total, taxes fixes TPS 5% / TVQ 9.975%, total)
CREATE PROCEDURE sp_recalculer_totaux_facture(
    IN p_facture_id INT
)
BEGIN
    DECLARE v_sous_total DECIMAL(12,2);
    DECLARE v_tps DECIMAL(12,2);
    DECLARE v_tvq DECIMAL(12,2);
    DECLARE v_total DECIMAL(12,2);
    DECLARE v_paye DECIMAL(12,2);
    DECLARE v_solde DECIMAL(12,2);
    
    -- Calculer le sous-total
    SELECT COALESCE(SUM(total_ligne), 0) INTO v_sous_total
    FROM lignes_facture
    WHERE facture_id = p_facture_id;
    
    -- Calculer les taxes avec taux fixes
    SET v_tps = ROUND(v_sous_total * 0.05, 2);    -- TPS 5%
    SET v_tvq = ROUND(v_sous_total * 0.09975, 2); -- TVQ 9.975%
    SET v_total = v_sous_total + v_tps + v_tvq;
    
    -- Calculer le solde dû
    SELECT COALESCE(SUM(montant), 0) INTO v_paye
    FROM paiements
    WHERE facture_id = p_facture_id;
    
    SET v_solde = v_total - v_paye;
    
    -- Mettre à jour la facture
    UPDATE factures
    SET sous_total = v_sous_total,
        taxe_tps = v_tps,
        taxe_tvq = v_tvq,
        total = v_total,
        solde_du = v_solde
    WHERE id = p_facture_id;
END$$

-- Procédure: sp_recalculer_totaux_commande
-- Description: Recalcule les totaux d'une commande (taxes fixes TPS 5% / TVQ 9.975%)
CREATE PROCEDURE sp_recalculer_totaux_commande(
    IN p_commande_id INT
)
BEGIN
    DECLARE v_sous_total DECIMAL(12,2);
    DECLARE v_tps DECIMAL(12,2);
    DECLARE v_tvq DECIMAL(12,2);
    DECLARE v_total DECIMAL(12,2);
    
    -- Calculer le sous-total
    SELECT COALESCE(SUM(total_ligne), 0) INTO v_sous_total
    FROM lignes_commande
    WHERE commande_id = p_commande_id;
    
    -- Calculer les taxes avec taux fixes
    SET v_tps = ROUND(v_sous_total * 0.05, 2);    -- TPS 5%
    SET v_tvq = ROUND(v_sous_total * 0.09975, 2); -- TVQ 9.975%
    SET v_total = v_sous_total + v_tps + v_tvq;
    
    -- Mettre à jour la commande
    UPDATE commandes_vente
    SET sous_total = v_sous_total,
        taxe_tps = v_tps,
        taxe_tvq = v_tvq,
        total = v_total
    WHERE id = p_commande_id;
END$$

DELIMITER ;

-- =====================================================
-- TRIGGERS - Automatisations et Validations
-- =====================================================

DELIMITER $$

-- Trigger: trg_valider_prix_produit
-- Description: Valide que le prix > coût et calcule automatiquement la marge brute
CREATE TRIGGER trg_valider_prix_produit
BEFORE INSERT ON produits
FOR EACH ROW
BEGIN
    IF NEW.prix <= NEW.cout THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Le prix de vente doit être supérieur au coût';
    END IF;
    
    IF NEW.seuil_reapprovisionnement < 0 OR NEW.stock_minimum < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Les seuils de stock doivent être positifs';
    END IF;
    
    -- Calculer automatiquement la marge brute (en pourcentage)
    SET NEW.marge_brute = ROUND(((NEW.prix - NEW.cout) / NEW.prix) * 100, 2);
END$$

CREATE TRIGGER trg_valider_prix_produit_upd
BEFORE UPDATE ON produits
FOR EACH ROW
BEGIN
    IF NEW.prix <= NEW.cout THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Le prix de vente doit être supérieur au coût';
    END IF;
    
    IF NEW.seuil_reapprovisionnement < 0 OR NEW.stock_minimum < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Les seuils de stock doivent être positifs';
    END IF;
    
    -- Recalculer automatiquement la marge brute si prix ou coût changent
    IF (NEW.prix != OLD.prix OR NEW.cout != OLD.cout) THEN
        SET NEW.marge_brute = ROUND(((NEW.prix - NEW.cout) / NEW.prix) * 100, 2);
    END IF;
END$$

-- Trigger: trg_calculer_total_ligne_commande
-- Description: Calcule automatiquement le total de ligne lors de l'insertion
CREATE TRIGGER trg_calculer_total_ligne_commande
BEFORE INSERT ON lignes_commande
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.prix_unitaire;
END$$

CREATE TRIGGER trg_calculer_total_ligne_commande_upd
BEFORE UPDATE ON lignes_commande
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.prix_unitaire;
END$$

-- Trigger: trg_maj_commande_apres_ligne
-- Description: Recalcule les totaux de commande après insertion/modification/suppression de ligne
CREATE TRIGGER trg_maj_commande_apres_ligne
AFTER INSERT ON lignes_commande
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_commande(NEW.commande_id);
END$$

CREATE TRIGGER trg_maj_commande_apres_ligne_upd
AFTER UPDATE ON lignes_commande
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_commande(NEW.commande_id);
END$$

CREATE TRIGGER trg_maj_commande_apres_ligne_del
AFTER DELETE ON lignes_commande
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_commande(OLD.commande_id);
END$$

-- Trigger: trg_calculer_total_ligne_facture
-- Description: Calcule le total de ligne de facture
CREATE TRIGGER trg_calculer_total_ligne_facture
BEFORE INSERT ON lignes_facture
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.prix_unitaire;
END$$

CREATE TRIGGER trg_calculer_total_ligne_facture_upd
BEFORE UPDATE ON lignes_facture
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.prix_unitaire;
END$$

-- Trigger: trg_maj_facture_apres_ligne
-- Description: Recalcule les totaux de facture après insertion/modification/suppression de ligne
CREATE TRIGGER trg_maj_facture_apres_ligne
AFTER INSERT ON lignes_facture
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_facture(NEW.facture_id);
END$$

CREATE TRIGGER trg_maj_facture_apres_ligne_upd
AFTER UPDATE ON lignes_facture
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_facture(NEW.facture_id);
END$$

CREATE TRIGGER trg_maj_facture_apres_ligne_del
AFTER DELETE ON lignes_facture
FOR EACH ROW
BEGIN
    CALL sp_recalculer_totaux_facture(OLD.facture_id);
END$$

-- Trigger: trg_commande_confirmer_avant
-- Description: Marque la réservation dans la commande (avant UPDATE)
CREATE TRIGGER trg_commande_confirmer_avant
BEFORE UPDATE ON commandes_vente
FOR EACH ROW
BEGIN
    -- Quand la commande passe à Confirmée, marquer comme réservé
    IF NEW.statut = 'Confirmée' AND OLD.statut != 'Confirmée' THEN
        SET NEW.stock_reserve = TRUE;
    END IF;
    
    -- Quand la commande est annulée, enlever la marque de réservation
    IF NEW.statut = 'Annulée' AND OLD.statut != 'Annulée' THEN
        SET NEW.stock_reserve = FALSE;
    END IF;
END$$

-- Trigger: trg_commande_confirmer_apres
-- Description: Réserve le stock physiquement (après UPDATE)
CREATE TRIGGER trg_commande_confirmer_apres
AFTER UPDATE ON commandes_vente
FOR EACH ROW
BEGIN
    -- Quand la commande passe à Confirmée
    IF NEW.statut = 'Confirmée' AND OLD.statut != 'Confirmée' THEN
        -- Réserver le stock pour chaque ligne de commande
        UPDATE niveaux_stock ns
        INNER JOIN lignes_commande lc ON ns.produit_id = lc.produit_id
        SET ns.qte_reservee = ns.qte_reservee + lc.quantite
        WHERE lc.commande_id = NEW.id;
    END IF;
END$$

-- Trigger: trg_commande_annuler_liberer
-- Description: Libère les réservations de stock lors de l'annulation d'une commande
CREATE TRIGGER trg_commande_annuler_liberer
AFTER UPDATE ON commandes_vente
FOR EACH ROW
BEGIN
    -- Quand la commande est annulée
    IF NEW.statut = 'Annulée' AND OLD.statut != 'Annulée' AND OLD.stock_reserve = TRUE THEN
        -- Libérer les réservations
        UPDATE niveaux_stock ns
        INNER JOIN lignes_commande lc ON ns.produit_id = lc.produit_id
        SET ns.qte_reservee = GREATEST(0, ns.qte_reservee - lc.quantite)
        WHERE lc.commande_id = NEW.id;
    END IF;
END$$

-- Trigger: trg_facture_emission_diminuer_stock
-- Description: Diminue le stock physique lorsqu'une facture est émise
CREATE TRIGGER trg_facture_emission_diminuer_stock
AFTER INSERT ON factures
FOR EACH ROW
BEGIN
    -- Si la facture est émise (statut = 'Émise')
    IF NEW.statut = 'Émise' THEN
        -- Créer des mouvements de stock SORTIE pour chaque ligne de facture
        INSERT INTO mouvements_stock (produit_id, type_mouvement, quantite, raison, date_mouvement, commande_vente_id)
        SELECT 
            lf.produit_id,
            'SORTIE',
            lf.quantite,
            'vente',
            NEW.date_emission,
            NEW.commande_id
        FROM lignes_facture lf
        WHERE lf.facture_id = NEW.id;
        
        -- Mettre à jour les niveaux de stock
        UPDATE niveaux_stock ns
        INNER JOIN lignes_facture lf ON ns.produit_id = lf.produit_id
        SET 
            ns.qte_disponible = GREATEST(0, ns.qte_disponible - lf.quantite),
            ns.qte_reservee = GREATEST(0, ns.qte_reservee - lf.quantite)
        WHERE lf.facture_id = NEW.id;
        
        -- Marquer la commande comme facturée
        UPDATE commandes_vente
        SET statut = 'Facturée'
        WHERE id = NEW.commande_id;
    END IF;
END$$

-- Trigger: trg_paiement_maj_facture
-- Description: Met à jour le statut de facture après réception d'un paiement
CREATE TRIGGER trg_paiement_maj_facture
AFTER INSERT ON paiements
FOR EACH ROW
BEGIN
    DECLARE v_total DECIMAL(12,2);
    DECLARE v_paye DECIMAL(12,2);
    DECLARE v_solde DECIMAL(12,2);
    
    -- Récupérer le total de la facture
    SELECT total INTO v_total
    FROM factures
    WHERE id = NEW.facture_id;
    
    -- Calculer le total payé
    SELECT COALESCE(SUM(montant), 0) INTO v_paye
    FROM paiements
    WHERE facture_id = NEW.facture_id;
    
    -- Calculer le solde
    SET v_solde = v_total - v_paye;
    
    -- Mettre à jour la facture
    UPDATE factures
    SET 
        solde_du = v_solde,
        statut = CASE
            WHEN v_solde <= 0 THEN 'Payée'
            WHEN v_paye > 0 AND v_solde > 0 THEN 'PayéePartiellement'
            ELSE statut
        END
    WHERE id = NEW.facture_id;
END$$

-- Trigger: trg_reception_augmenter_stock
-- Description: Augmente le stock lors de la réception de marchandises
CREATE TRIGGER trg_reception_augmenter_stock
AFTER INSERT ON receptions_marchandises
FOR EACH ROW
BEGIN
    -- Créer un mouvement de stock ENTREE
    INSERT INTO mouvements_stock (produit_id, type_mouvement, quantite, raison, date_mouvement, achat_id)
    VALUES (
        NEW.produit_id,
        'ENTREE',
        NEW.quantite_recue,
        'reception_achat',
        NEW.date_reception,
        NEW.achat_id
    );
    
    -- Mettre à jour ou créer le niveau de stock
    INSERT INTO niveaux_stock (produit_id, qte_disponible, qte_reservee)
    VALUES (NEW.produit_id, NEW.quantite_recue, 0)
    ON DUPLICATE KEY UPDATE
        qte_disponible = qte_disponible + NEW.quantite_recue;
    
    -- Marquer le bon de commande comme reçu
    UPDATE achats
    SET statut = 'Reçu'
    WHERE id = NEW.achat_id;
END$$

-- Trigger: trg_premiere_commande_activer
-- Description: Change le statut d'un prospect à actif lors de sa première commande
CREATE TRIGGER trg_premiere_commande_activer
AFTER INSERT ON commandes_vente
FOR EACH ROW
BEGIN
    DECLARE v_nb_commandes INT;
    
    -- Compter le nombre de commandes du client
    SELECT COUNT(*) INTO v_nb_commandes
    FROM commandes_vente
    WHERE client_id = NEW.client_id
      AND statut != 'Annulée';
    
    -- Si c'est la première commande et que le client est prospect
    IF v_nb_commandes = 1 THEN
        UPDATE clients
        SET statut = 'Actif'
        WHERE id = NEW.client_id
          AND statut = 'Prospect';
    END IF;
END$$

-- Trigger: trg_statut_client_fidele
-- Description: Élève un client au statut 'Fidèle' selon critères (5+ commandes OU 3000$+ ventes)
CREATE TRIGGER trg_statut_client_fidele
AFTER INSERT ON factures
FOR EACH ROW
BEGIN
    DECLARE v_client_id INT;
    DECLARE v_nb_commandes INT;
    DECLARE v_ventes_totales DECIMAL(12,2);
    
    -- Obtenir le client de la facture
    SELECT client_id INTO v_client_id
    FROM commandes_vente
    WHERE id = NEW.commande_id;
    
    -- Compter les commandes
    SELECT COUNT(DISTINCT cv.id) INTO v_nb_commandes
    FROM commandes_vente cv
    WHERE cv.client_id = v_client_id
      AND cv.statut NOT IN ('Annulée', 'Brouillon');
    
    -- Calculer les ventes totales
    SELECT COALESCE(SUM(f.total), 0) INTO v_ventes_totales
    FROM factures f
    INNER JOIN commandes_vente cv ON f.commande_id = cv.id
    WHERE cv.client_id = v_client_id
      AND f.statut NOT IN ('Annulée');
    
    -- Promouvoir à Fidèle si critères atteints
    IF v_nb_commandes >= 5 OR v_ventes_totales >= 3000 THEN
        UPDATE clients
        SET statut = 'Fidèle'
        WHERE id = v_client_id
          AND statut != 'Fidèle';
    END IF;
END$$

-- Trigger: trg_bloquer_client_en_retard
-- Description: Empêche la création de commande pour un client ayant des factures en retard
CREATE TRIGGER trg_bloquer_client_en_retard
BEFORE INSERT ON commandes_vente
FOR EACH ROW
BEGIN
    DECLARE v_a_retard BOOLEAN;
    
    -- Vérifier si le client a des factures en retard
    SET v_a_retard = fn_client_a_factures_en_retard(NEW.client_id);
    
    IF v_a_retard THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Impossible de créer une commande : le client a des factures en retard';
    END IF;
END$$

-- Trigger: trg_avis_note_basse_alerte
-- Description: Crée une alerte CRM pour les évaluations basses (<=2)
CREATE TRIGGER trg_avis_note_basse_alerte
AFTER INSERT ON avis_clients
FOR EACH ROW
BEGIN
    IF NEW.note <= 2 THEN
        INSERT INTO alertes_crm (client_id, date_creation, raison)
        VALUES (NEW.client_id, NOW(), 'note_basse');
    END IF;
END$$

-- Trigger: trg_calculer_total_ligne_achat
-- Description: Calcule le total de ligne d'achat
CREATE TRIGGER trg_calculer_total_ligne_achat
BEFORE INSERT ON lignes_achat
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.cout_unitaire;
END$$

CREATE TRIGGER trg_calculer_total_ligne_achat_upd
BEFORE UPDATE ON lignes_achat
FOR EACH ROW
BEGIN
    SET NEW.total_ligne = NEW.quantite * NEW.cout_unitaire;
END$$

-- Trigger: trg_valider_depense
-- Description: Valide que montant_total >= montant (montant + taxes)
CREATE TRIGGER trg_valider_depense
BEFORE INSERT ON depenses
FOR EACH ROW
BEGIN
    IF NEW.montant_total < NEW.montant THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Le montant total doit être supérieur ou égal au montant hors taxes';
    END IF;
    
    -- Si montant_taxe n'est pas spécifié, calculer automatiquement
    IF NEW.montant_taxe = 0 AND NEW.montant_total > NEW.montant THEN
        SET NEW.montant_taxe = NEW.montant_total - NEW.montant;
    END IF;
END$$

CREATE TRIGGER trg_valider_depense_upd
BEFORE UPDATE ON depenses
FOR EACH ROW
BEGIN
    IF NEW.montant_total < NEW.montant THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Le montant total doit être supérieur ou égal au montant hors taxes';
    END IF;
    
    -- Recalculer les taxes si nécessaire
    IF NEW.montant_taxe = 0 AND NEW.montant_total > NEW.montant THEN
        SET NEW.montant_taxe = NEW.montant_total - NEW.montant;
    END IF;
END$$

DELIMITER ;

-- =====================================================
-- AJOUT DES CONTRAINTES DE CLÉS ÉTRANGÈRES DIFFÉRÉES
-- =====================================================
-- Ces FK sont ajoutées après la création de toutes les tables
-- pour éviter les erreurs de dépendances circulaires

-- Contraintes pour mouvements_stock
ALTER TABLE mouvements_stock
ADD CONSTRAINT fk_mouvement_commande 
FOREIGN KEY (commande_vente_id) REFERENCES commandes_vente(id) 
ON DELETE SET NULL ON UPDATE CASCADE;

ALTER TABLE mouvements_stock
ADD CONSTRAINT fk_mouvement_achat 
FOREIGN KEY (achat_id) REFERENCES achats(id) 
ON DELETE SET NULL ON UPDATE CASCADE;

-- =====================================================
-- FIN DU SCRIPT
-- =====================================================
-- La base de données NordikAdventuresERP est maintenant créée!
-- Structure SIMPLIFIÉE avec 22 tables, 7 vues, 2 fonctions, 2 procédures et 20+ triggers.
-- 
-- SIMPLIFICATIONS APPLIQUÉES:
-- ✅ departements → ENUM (7 valeurs)
-- ✅ taxes → Taux fixes TPS 5% / TVQ 9.975%
-- ✅ emplacements_stock → Fusionné dans niveaux_stock
-- ✅ lignes_reception → Intégré dans receptions_marchandises
-- 
-- GAIN: -4 tables (26 → 22), -4 écrans CRUD, -2 jours de développement
--
-- Pour insérer des données de test, utilisez le script séparé: 
-- TEST_NordikAdventuresERP.sql ou créez vos propres données.
