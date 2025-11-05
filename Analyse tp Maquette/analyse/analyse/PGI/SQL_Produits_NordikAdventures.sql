-- =====================================================
-- Script d'insertion des produits Nordik Adventures
-- 30 produits avec catégories et fournisseurs
-- =====================================================

USE NordikAdventuresERP;

-- =====================================================
-- 1. INSERTION DES CATÉGORIES
-- =====================================================

INSERT INTO categories (nom, statut) VALUES
('Tentes & abris', 'Actif'),
('Sacs & portage', 'Actif'),
('Vêtements techniques', 'Actif'),
('Accessoires & cuisine', 'Actif'),
('Électronique & navigation', 'Actif')
ON DUPLICATE KEY UPDATE statut = statut;

-- =====================================================
-- 2. INSERTION DES FOURNISSEURS
-- =====================================================

INSERT INTO fournisseurs (nom, code, courriel_contact, delai_livraison_jours, pourcentage_escompte, statut) VALUES
('AventureX', 'AX-001', 'contact@aventurex.com', 10, 5.00, 'Actif'),
('TrekSupply', 'TS-001', 'info@treksupply.com', 7, 4.00, 'Actif'),
('MontNord', 'MN-001', 'ventes@montnord.com', 8, 3.00, 'Actif'),
('NordPack', 'NP-001', 'commandes@nordpack.com', 8, 6.00, 'Actif'),
('NordWear', 'NW-001', 'service@nordwear.com', 6, 5.00, 'Actif'),
('ArcticLine', 'AL-001', 'contact@arcticline.com', 8, 4.00, 'Actif'),
('TechTrail', 'TT-001', 'info@techtrail.com', 10, 4.00, 'Actif')
ON DUPLICATE KEY UPDATE statut = statut;

-- =====================================================
-- 3. INSERTION DES PRODUITS (30 produits)
-- =====================================================

-- Tentes & abris (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, marge_brute, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-TNT-001', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tente légère 2 places', 'Tente ultra-légère pour 2 personnes, parfaite pour la randonnée', 145.00, 299.00, 51.50, 5, 3, 2.8, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-03-02'),
('NC-TNT-002', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tente familiale 6 places', 'Tente spacieuse pour toute la famille avec vestibule', 260.00, 499.00, 47.90, 3, 2, 6.5, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-18'),
('NC-TNT-003', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Toile imperméable 3x3 m', 'Toile de protection imperméable multi-usage', 25.00, 59.00, 57.60, 8, 5, 1.1, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-10'),
('NC-TNT-004', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Tapis de sol isolant', 'Tapis isolant thermique pour sol de tente', 18.00, 39.00, 53.80, 10, 6, 0.9, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-05'),
('NC-TNT-005', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Abri cuisine pliable', 'Abri léger et pliable pour cuisine extérieure', 75.00, 149.00, 49.70, 4, 3, 5.0, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-20'),
('NC-TNT-006', (SELECT id FROM categories WHERE nom = 'Tentes & abris'), 'Mât télescopique alu', 'Mât ajustable en aluminium pour tente', 12.00, 29.00, 58.60, 10, 6, 0.7, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-08')
ON DUPLICATE KEY UPDATE statut = statut;

-- Sacs & portage (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, marge_brute, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-SAC-001', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac à dos 50 L étanche', 'Sac à dos grande capacité 100% étanche', 65.00, 139.00, 53.20, 6, 4, 1.3, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-12'),
('NC-SAC-002', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac de jour 25 L', 'Sac à dos compact pour randonnées courtes', 32.00, 79.00, 59.50, 8, 5, 0.9, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-10'),
('NC-SAC-003', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Sac de couchage -10°C', 'Sac de couchage 3 saisons confort jusqu''à -10°C', 80.00, 169.00, 52.70, 5, 3, 2.2, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-02-25'),
('NC-SAC-004', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Tapis autogonflant', 'Matelas autogonflant ultra-confortable', 25.00, 59.00, 57.60, 10, 5, 1.1, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-05'),
('NC-SAC-005', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Housse imperméable sac à dos', 'Protection imperméable pour sac à dos', 9.00, 19.00, 52.60, 10, 6, 0.4, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-11'),
('NC-SAC-006', (SELECT id FROM categories WHERE nom = 'Sacs & portage'), 'Bâtons de marche carbone', 'Bâtons de randonnée ultra-légers en carbone', 35.00, 79.00, 55.70, 5, 3, 0.8, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-02-28')
ON DUPLICATE KEY UPDATE statut = statut;

-- Vêtements techniques (7 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, marge_brute, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-VET-001', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Chandail thermique homme', 'Sous-vêtement thermique respirant pour homme', 22.00, 59.00, 62.70, 15, 10, 0.6, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-09'),
('NC-VET-002', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Chandail thermique femme', 'Sous-vêtement thermique respirant pour femme', 22.00, 59.00, 62.70, 15, 10, 0.6, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-09'),
('NC-VET-003', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Pantalon de randonnée homme', 'Pantalon technique stretch pour homme', 38.00, 89.00, 57.30, 8, 4, 0.8, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-03'),
('NC-VET-004', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Pantalon de randonnée femme', 'Pantalon technique stretch pour femme', 38.00, 89.00, 57.30, 8, 4, 0.8, (SELECT id FROM fournisseurs WHERE code = 'NW-001'), 'Actif', '2025-03-03'),
('NC-VET-005', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Manteau coupe-vent', 'Veste coupe-vent imperméable respirante', 55.00, 129.00, 57.40, 5, 3, 1.1, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-02-19'),
('NC-VET-006', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Tuque en laine mérinos', 'Bonnet chaud en laine mérinos naturelle', 10.00, 29.00, 65.50, 10, 6, 0.3, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-03-10'),
('NC-VET-007', (SELECT id FROM categories WHERE nom = 'Vêtements techniques'), 'Gants isolants Hiver+', 'Gants techniques isolés pour températures extrêmes', 18.00, 45.00, 60.00, 8, 4, 0.5, (SELECT id FROM fournisseurs WHERE code = 'AL-001'), 'Actif', '2025-02-22')
ON DUPLICATE KEY UPDATE statut = statut;

-- Accessoires & cuisine (6 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, marge_brute, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-ACC-001', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Réchaud portatif', 'Réchaud à gaz compact pour camping', 25.00, 59.00, 57.60, 5, 3, 0.9, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-02-28'),
('NC-ACC-002', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Bouteille isotherme 1L', 'Bouteille isolante garde chaud/froid 24h', 12.00, 29.00, 58.60, 12, 8, 0.4, (SELECT id FROM fournisseurs WHERE code = 'MN-001'), 'Actif', '2025-03-10'),
('NC-ACC-003', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Lampe frontale 300 lumens', 'Lampe frontale LED rechargeable puissante', 14.00, 39.00, 64.10, 10, 6, 0.2, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-03-12'),
('NC-ACC-004', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Ensemble vaisselle 4 pers.', 'Kit de vaisselle complet pour 4 personnes', 20.00, 49.00, 59.20, 8, 5, 1.2, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-06'),
('NC-ACC-005', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Filtre à eau compact', 'Système de filtration d''eau portable', 28.00, 69.00, 59.40, 5, 3, 0.7, (SELECT id FROM fournisseurs WHERE code = 'AX-001'), 'Actif', '2025-02-25'),
('NC-ACC-006', (SELECT id FROM categories WHERE nom = 'Accessoires & cuisine'), 'Couteau multifonction', 'Couteau suisse avec 12 fonctions', 15.00, 39.00, 61.50, 10, 6, 0.5, (SELECT id FROM fournisseurs WHERE code = 'NP-001'), 'Actif', '2025-03-09')
ON DUPLICATE KEY UPDATE statut = statut;

-- Électronique & navigation (5 produits)
INSERT INTO produits (sku, categorie_id, nom, description, cout, prix, marge_brute, seuil_reapprovisionnement, stock_minimum, poids_kg, fournisseur_id, statut, date_entree_stock) VALUES
('NC-ELE-001', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Montre GPS plein air', 'Montre GPS multisport avec cartographie', 120.00, 279.00, 56.90, 3, 2, 0.9, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-02-17'),
('NC-ELE-002', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Chargeur solaire 20W', 'Panneau solaire portable pour appareils USB', 35.00, 79.00, 55.70, 5, 3, 0.6, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-03-01'),
('NC-ELE-003', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Boussole de précision', 'Boussole professionnelle avec miroir de visée', 9.00, 24.00, 62.50, 12, 8, 0.2, (SELECT id FROM fournisseurs WHERE code = 'TS-001'), 'Actif', '2025-03-11'),
('NC-ELE-004', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Radio météo portable', 'Radio d''urgence avec alerte météo', 22.00, 49.00, 55.10, 5, 3, 0.8, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-02-28'),
('NC-ELE-005', (SELECT id FROM categories WHERE nom = 'Électronique & navigation'), 'Lampe USB rechargeable', 'Lampe de camping rechargeable par USB', 11.00, 25.00, 56.00, 10, 6, 0.3, (SELECT id FROM fournisseurs WHERE code = 'TT-001'), 'Actif', '2025-03-07')
ON DUPLICATE KEY UPDATE statut = statut;

-- =====================================================
-- 4. INSERTION DES NIVEAUX DE STOCK
-- =====================================================

-- Stock pour Entrepôt Principal (emplacement par défaut)
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
-- 5. VÉRIFICATION DES DONNÉES
-- =====================================================

-- Compter les catégories
SELECT '✅ Catégories insérées:' AS Message, COUNT(*) AS Total FROM categories;

-- Compter les fournisseurs
SELECT '✅ Fournisseurs insérés:' AS Message, COUNT(*) AS Total FROM fournisseurs;

-- Compter les produits
SELECT '✅ Produits insérés:' AS Message, COUNT(*) AS Total FROM produits WHERE sku LIKE 'NC-%';

-- Compter les niveaux de stock
SELECT '✅ Niveaux de stock insérés:' AS Message, COUNT(*) AS Total FROM niveaux_stock;

-- Afficher un échantillon de produits
SELECT 
    p.sku AS SKU,
    c.nom AS Catégorie,
    p.nom AS Produit,
    p.cout AS 'Coût ($)',
    p.prix AS 'Prix ($)',
    p.marge_brute AS 'Marge (%)',
    COALESCE(SUM(ns.qte_disponible), 0) AS Stock,
    p.seuil_reapprovisionnement AS Seuil,
    f.nom AS Fournisseur,
    p.statut AS Statut
FROM produits p
INNER JOIN categories c ON p.categorie_id = c.id
INNER JOIN fournisseurs f ON p.fournisseur_id = f.id
LEFT JOIN niveaux_stock ns ON p.id = ns.produit_id
WHERE p.sku LIKE 'NC-%'
GROUP BY p.id
ORDER BY p.sku
LIMIT 10;

-- =====================================================
-- SCRIPT TERMINÉ ✅
-- =====================================================

SELECT '🎉 30 produits Nordik Adventures insérés avec succès !' AS Message;

