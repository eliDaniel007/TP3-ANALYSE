using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGI.Services;
using PGI.Models;
using PGI.Helpers;
using MySql.Data.MySqlClient;

namespace PGI.Tests
{
    /// <summary>
    /// Bot de test automatisé complet pour tester TOUTES les fonctionnalités du PGI
    /// </summary>
    class Program
    {
        private static List<TestError> errors = new List<TestError>();
        private static string? currentUserEmail = null;
        private static string? currentUserRole = null;
        private static int? currentEmployeId = null;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("🤖 BOT DE TEST AUTOMATISÉ COMPLET - PGI NordikAdventures");
            Console.WriteLine("========================================================\n");

            try
            {
                int errorCountBefore;

                // ========== SECTION 1: AUTHENTIFICATION & CONNEXION ==========
                Console.WriteLine("═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 1: AUTHENTIFICATION & CONNEXION");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("🔐 Test 1.1: Authentification Employé...");
                errorCountBefore = errors.Count;
                TestAuthentification();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Authentification réussie\n");
                else
                    Console.WriteLine("⚠️ Authentification échouée (continuation des tests)\n");

                Console.WriteLine("📡 Test 1.2: Vérification de la connexion à la base de données...");
                errorCountBefore = errors.Count;
                TestConnection();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Connexion réussie\n");

                // ========== SECTION 2: CRM - CLIENTS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 2: CRM - GESTION DES CLIENTS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("👥 Test 2.1: Récupération de tous les clients...");
                errorCountBefore = errors.Count;
                TestGetAllClients();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Clients récupérés\n");

                Console.WriteLine("🔍 Test 2.2: Récupération d'un client par ID...");
                errorCountBefore = errors.Count;
                TestGetClientById();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Client récupéré par ID\n");

                Console.WriteLine("📊 Test 2.3: Statistiques des clients...");
                errorCountBefore = errors.Count;
                TestClientStatistiques();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Statistiques clients récupérées\n");

                // ========== SECTION 3: CRM - INTERACTIONS CLIENTS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 3: CRM - INTERACTIONS CLIENTS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("💬 Test 3.1: Création d'une interaction client...");
                errorCountBefore = errors.Count;
                TestCreerInteraction();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Interaction créée\n");

                Console.WriteLine("📋 Test 3.2: Récupération des interactions d'un client...");
                errorCountBefore = errors.Count;
                TestGetInteractionsByClient();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Interactions récupérées\n");

                // ========== SECTION 4: CRM - ÉVALUATIONS CLIENTS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 4: CRM - ÉVALUATIONS CLIENTS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("⭐ Test 4.1: Création d'une évaluation client...");
                errorCountBefore = errors.Count;
                TestCreerEvaluation();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Évaluation créée\n");

                Console.WriteLine("📋 Test 4.2: Récupération des évaluations d'un client...");
                errorCountBefore = errors.Count;
                TestGetEvaluationsByClient();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Évaluations récupérées\n");

                // ========== SECTION 5: CRM - CAMPAGNES MARKETING ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 5: CRM - CAMPAGNES MARKETING");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📢 Test 5.1: Création d'une campagne marketing...");
                errorCountBefore = errors.Count;
                TestCreerCampagne();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Campagne créée\n");

                Console.WriteLine("📋 Test 5.2: Récupération de toutes les campagnes...");
                errorCountBefore = errors.Count;
                TestGetAllCampagnes();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Campagnes récupérées\n");

                Console.WriteLine("🔍 Test 5.3: Récupération d'une campagne par ID...");
                errorCountBefore = errors.Count;
                TestGetCampagneById();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Campagne récupérée par ID\n");

                // ========== SECTION 6: STOCKS - PRODUITS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 6: STOCKS - GESTION DES PRODUITS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📦 Test 6.1: Récupération de tous les produits...");
                errorCountBefore = errors.Count;
                TestGetAllProduits();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Produits récupérés\n");

                Console.WriteLine("🔍 Test 6.2: Récupération d'un produit par ID...");
                errorCountBefore = errors.Count;
                TestGetProduitById();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Produit récupéré par ID\n");

                // ========== SECTION 7: STOCKS - CATÉGORIES ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 7: STOCKS - GESTION DES CATÉGORIES");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📁 Test 7.1: Récupération de toutes les catégories...");
                errorCountBefore = errors.Count;
                TestGetAllCategories();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Catégories récupérées\n");

                Console.WriteLine("🔍 Test 7.2: Récupération d'une catégorie par ID...");
                errorCountBefore = errors.Count;
                TestGetCategorieById();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Catégorie récupérée par ID\n");

                // ========== SECTION 8: STOCKS - FOURNISSEURS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 8: STOCKS - GESTION DES FOURNISSEURS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("🏭 Test 8.1: Récupération de tous les fournisseurs...");
                errorCountBefore = errors.Count;
                TestGetAllFournisseurs();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Fournisseurs récupérés\n");

                Console.WriteLine("🔍 Test 8.2: Récupération d'un fournisseur par ID...");
                errorCountBefore = errors.Count;
                TestGetFournisseurById();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Fournisseur récupéré par ID\n");

                // ========== SECTION 9: STOCKS - MOUVEMENTS DE STOCK ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 9: STOCKS - MOUVEMENTS DE STOCK");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📊 Test 9.1: Récupération des mouvements de stock d'un produit...");
                errorCountBefore = errors.Count;
                TestGetMouvementsByProduit();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Mouvements récupérés\n");

                // ========== SECTION 10: FINANCES - FACTURES ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 10: FINANCES - GESTION DES FACTURES");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📄 Test 10.1: Génération d'un numéro de facture...");
                errorCountBefore = errors.Count;
                TestGenererNumeroFacture();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Numéro de facture généré\n");

                Console.WriteLine("📋 Test 10.2: Récupération de toutes les factures...");
                errorCountBefore = errors.Count;
                TestGetAllFactures();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Factures récupérées\n");

                Console.WriteLine("🔍 Test 10.3: Récupération d'une facture par numéro...");
                errorCountBefore = errors.Count;
                TestGetFactureByNumero();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Facture récupérée par numéro\n");

                Console.WriteLine("📋 Test 10.4: Récupération des factures d'un client...");
                errorCountBefore = errors.Count;
                TestGetFacturesByClient();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Factures client récupérées\n");

                // ========== SECTION 11: FINANCES - PAIEMENTS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 11: FINANCES - GESTION DES PAIEMENTS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("💳 Test 11.1: Récupération de tous les paiements...");
                errorCountBefore = errors.Count;
                TestGetAllPaiements();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Paiements récupérés\n");

                Console.WriteLine("📋 Test 11.2: Récupération des paiements d'une facture...");
                errorCountBefore = errors.Count;
                TestGetPaiementsByFacture();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Paiements facture récupérés\n");

                Console.WriteLine("📅 Test 11.3: Récupération des paiements par période...");
                errorCountBefore = errors.Count;
                TestGetPaiementsByPeriode();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Paiements période récupérés\n");

                // ========== SECTION 12: FINANCES - COMMANDES FOURNISSEURS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 12: FINANCES - COMMANDES FOURNISSEURS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📦 Test 12.1: Génération d'un numéro de commande fournisseur...");
                errorCountBefore = errors.Count;
                TestGenererNumeroCommandeFournisseur();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Numéro de commande généré\n");

                Console.WriteLine("📋 Test 12.2: Récupération de toutes les commandes fournisseurs...");
                errorCountBefore = errors.Count;
                TestGetAllCommandesFournisseurs();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Commandes fournisseurs récupérées\n");

                Console.WriteLine("🔍 Test 12.3: Récupération d'une commande fournisseur par numéro...");
                errorCountBefore = errors.Count;
                TestGetCommandeFournisseurByNumero();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Commande fournisseur récupérée par numéro\n");

                // ========== SECTION 13: FINANCES - DÉPENSES ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 13: FINANCES - GESTION DES DÉPENSES");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("💸 Test 13.1: Récupération de toutes les dépenses...");
                errorCountBefore = errors.Count;
                TestGetAllDepenses();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Dépenses récupérées\n");

                // ========== SECTION 14: FINANCES - JOURNAL COMPTABLE ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 14: FINANCES - JOURNAL COMPTABLE");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📒 Test 14.1: Récupération des écritures du journal comptable...");
                errorCountBefore = errors.Count;
                TestGetJournalEntries();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Écritures journal récupérées\n");

                Console.WriteLine("⚖️ Test 14.2: Vérification de l'équilibre du journal comptable...");
                errorCountBefore = errors.Count;
                TestEquilibreJournal();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Journal comptable équilibré\n");

                // ========== SECTION 15: FINANCES - TRANSACTIONS COMPTABLES ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 15: FINANCES - TRANSACTIONS COMPTABLES");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("💰 Test 15.1: Enregistrement d'une vente...");
                errorCountBefore = errors.Count;
                TestEnregistrerVente();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Vente enregistrée\n");

                Console.WriteLine("🏭 Test 15.2: Enregistrement d'un achat de stock...");
                errorCountBefore = errors.Count;
                TestEnregistrerAchatStock();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Achat de stock enregistré\n");

                Console.WriteLine("💸 Test 15.3: Enregistrement d'une dépense...");
                errorCountBefore = errors.Count;
                TestEnregistrerDepense();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Dépense enregistrée\n");

                // ========== SECTION 16: FINANCES - RAPPORTS ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 16: FINANCES - GÉNÉRATION DE RAPPORTS");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📊 Test 16.1: Génération du rapport de taxes...");
                errorCountBefore = errors.Count;
                TestRapportTaxes();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Rapport de taxes généré\n");

                Console.WriteLine("📊 Test 16.2: Génération du rapport des ventes...");
                errorCountBefore = errors.Count;
                TestRapportVentes();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Rapport des ventes généré\n");

                // ========== SECTION 17: FINANCES - PARAMÈTRES FISCAUX ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 17: FINANCES - PARAMÈTRES FISCAUX");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("⚙️ Test 17.1: Récupération des taux de taxes...");
                errorCountBefore = errors.Count;
                TestParametresFiscaux();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Paramètres fiscaux récupérés\n");

                Console.WriteLine("🧮 Test 17.2: Calcul des taxes...");
                errorCountBefore = errors.Count;
                TestCalculerTaxes();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Calcul des taxes validé\n");

                // ========== SECTION 18: DASHBOARD ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 18: DASHBOARD - STATISTIQUES GÉNÉRALES");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📊 Test 18.1: Chiffre d'affaires total...");
                errorCountBefore = errors.Count;
                TestGetChiffreAffairesTotal();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Chiffre d'affaires récupéré\n");

                Console.WriteLine("📊 Test 18.2: Ventes totales par période...");
                errorCountBefore = errors.Count;
                TestGetVentesTotales();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Ventes totales récupérées\n");

                Console.WriteLine("📊 Test 18.3: Dépenses d'exploitation par période...");
                errorCountBefore = errors.Count;
                TestGetDepensesExploitation();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Dépenses d'exploitation récupérées\n");

                Console.WriteLine("📊 Test 18.4: Factures en attente...");
                errorCountBefore = errors.Count;
                TestGetFacturesEnAttente();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Factures en attente récupérées\n");

                Console.WriteLine("📊 Test 18.5: Dernières transactions...");
                errorCountBefore = errors.Count;
                TestGetDernieresTransactions();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Dernières transactions récupérées\n");

                // ========== SECTION 19: COMMANDES VENTE ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 19: COMMANDES VENTE");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("🛒 Test 19.1: Récupération de toutes les commandes...");
                errorCountBefore = errors.Count;
                TestGetAllCommandes();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Commandes récupérées\n");

                // ========== SECTION 20: TRANSACTIONS CLIENT ==========
                Console.WriteLine("\n═══════════════════════════════════════════════════════");
                Console.WriteLine("SECTION 20: TRANSACTIONS CLIENT");
                Console.WriteLine("═══════════════════════════════════════════════════════\n");

                Console.WriteLine("📝 Test 20.1: Inscription d'un nouveau client...");
                errorCountBefore = errors.Count;
                int? testClientId = TestRegisterClient();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Client inscrit\n");

                Console.WriteLine("🔐 Test 20.2: Authentification d'un client...");
                errorCountBefore = errors.Count;
                TestAuthenticateClient();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Client authentifié\n");

                Console.WriteLine("🔍 Test 20.3: Récupération d'un client par email...");
                errorCountBefore = errors.Count;
                TestGetClientByEmail();
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Client récupéré par email\n");

                Console.WriteLine("🛒 Test 20.4: Création d'une commande par un client...");
                errorCountBefore = errors.Count;
                TestCreateCommandeByClient(testClientId);
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Commande créée par le client\n");

                Console.WriteLine("📋 Test 20.5: Récupération des commandes d'un client...");
                errorCountBefore = errors.Count;
                TestGetCommandesByClient(testClientId);
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Commandes client récupérées\n");

                Console.WriteLine("⭐ Test 20.6: Création d'une évaluation par un client...");
                errorCountBefore = errors.Count;
                TestCreateEvaluationByClient(testClientId);
                if (errors.Count == errorCountBefore)
                    Console.WriteLine("✅ Évaluation créée par le client\n");

                // Afficher le rapport final
                AfficherRapportFinal();
            }
            catch (Exception ex)
            {
                errors.Add(new TestError
                {
                    TestName = "Exception globale",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace ?? "",
                    Timestamp = DateTime.Now
                });
                AfficherRapportFinal();
                Environment.Exit(1);
            }
        }

        // ========== SECTION 1: AUTHENTIFICATION & CONNEXION ==========
        static void TestAuthentification()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT id, courriel, mot_de_passe, nom, prenom, role_systeme
                        FROM employes
                        WHERE statut = 'Actif'
                        ORDER BY role_systeme DESC, id ASC";

                    var employees = new List<(int id, string email, string password, string nom, string prenom, string role)>();

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32("id");
                            var email = reader.GetString("courriel");
                            var password = reader.IsDBNull(reader.GetOrdinal("mot_de_passe")) 
                                ? "" 
                                : reader.GetString("mot_de_passe");
                            var nom = reader.GetString("nom");
                            var prenom = reader.GetString("prenom");
                            var role = reader.IsDBNull(reader.GetOrdinal("role_systeme"))
                                ? "Employé"
                                : reader.GetString("role_systeme");

                            if (!string.IsNullOrEmpty(password))
                            {
                                employees.Add((id, email, password, nom, prenom, role));
                            }
                        }
                    }

                    if (employees.Count == 0)
                    {
                        AddError("TestAuthentification",
                            "Aucun employé actif avec mot de passe trouvé",
                            "Créez au moins un employé avec statut 'Actif' et un mot de passe");
                        return;
                    }

                    bool authenticated = false;
                    foreach (var emp in employees)
                    {
                        try
                        {
                            var (success, nom, prenom, role) = EmployeService.Authenticate(emp.email, emp.password);

                            if (success)
                            {
                                currentUserEmail = emp.email;
                                currentUserRole = role;
                                currentEmployeId = emp.id;
                                Console.WriteLine($"   ✅ Authentifié: {prenom} {nom} ({role})");
                                authenticated = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"   ⚠️ Échec pour {emp.email}: {ex.Message}");
                        }
                    }

                    if (!authenticated)
                    {
                        AddError("TestAuthentification",
                            $"Échec pour tous les {employees.Count} employé(s)",
                            "Vérifiez les mots de passe dans la base");
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("TestAuthentification", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestConnection()
        {
            try
            {
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT 1", conn))
                    {
                        var result = cmd.ExecuteScalar();
                        if (result == null || result.ToString() != "1")
                        {
                            AddError("TestConnection", "Connexion échouée", "Vérifiez DatabaseHelper.cs");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("TestConnection", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 2: CRM - CLIENTS ==========
        static void TestGetAllClients()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                Console.WriteLine($"   📊 {clients.Count} client(s) trouvé(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllClients", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetClientById()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var client = ClientService.GetClientById(clients[0].Id);
                    if (client != null)
                        Console.WriteLine($"   ✅ Client trouvé: {client.Nom}");
                    else
                        AddError("TestGetClientById", "Client non trouvé", "");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetClientById", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestClientStatistiques()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var stats = ClientStatistiquesService.GetStatistiquesClient(clients[0].Id);
                    if (stats != null)
                    {
                        Console.WriteLine($"   📊 CA: {stats.ChiffreAffairesTotal:C}, Commandes: {stats.NombreCommandes}");
                    }
                    else
                    {
                        Console.WriteLine("   ⚠️ Aucune statistique trouvée pour ce client");
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("TestClientStatistiques", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 3: CRM - INTERACTIONS ==========
        static void TestCreerInteraction()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var interaction = new InteractionClient
                    {
                        ClientId = clients[0].Id,
                        EmployeId = currentEmployeId,
                        TypeInteraction = "Téléphone",
                        Sujet = "Test Bot - Interaction automatique",
                        Description = "Interaction créée par le bot de test",
                        DateInteraction = DateTime.Now,
                        ResultatAction = "Test réussi"
                    };
                    var id = InteractionClientService.CreerInteraction(interaction);
                    Console.WriteLine($"   ✅ Interaction créée (ID: {id})");
                }
            }
            catch (Exception ex)
            {
                AddError("TestCreerInteraction", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetInteractionsByClient()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var interactions = InteractionClientService.GetInteractionsByClient(clients[0].Id);
                    Console.WriteLine($"   📊 {interactions.Count} interaction(s) trouvée(s)");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetInteractionsByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 4: CRM - ÉVALUATIONS ==========
        static void TestCreerEvaluation()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                var factures = FactureService.GetAllFactures();
                if (clients.Count > 0)
                {
                    int? factureId = null;
                    if (factures.Count > 0)
                    {
                        factureId = factures[0].Id;
                    }

                    var evaluation = new EvaluationClient
                    {
                        ClientId = clients[0].Id,
                        FactureId = factureId,
                        NoteSatisfaction = 4,
                        Commentaire = "Test Bot - Évaluation automatique",
                        DateEvaluation = DateTime.Now
                    };
                    var id = EvaluationClientService.CreerEvaluation(evaluation);
                    Console.WriteLine($"   ✅ Évaluation créée (ID: {id})");
                }
                else
                {
                    Console.WriteLine("   ⚠️ Aucun client trouvé pour créer une évaluation");
                }
            }
            catch (Exception ex)
            {
                AddError("TestCreerEvaluation", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetEvaluationsByClient()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var evaluations = EvaluationClientService.GetEvaluationsByClient(clients[0].Id);
                    Console.WriteLine($"   📊 {evaluations.Count} évaluation(s) trouvée(s)");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetEvaluationsByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 5: CRM - CAMPAGNES ==========
        static void TestCreerCampagne()
        {
            try
            {
                var campagne = new CampagneMarketing
                {
                    NomCampagne = $"Test Bot - Campagne {DateTime.Now:yyyyMMddHHmmss}",
                    Type = "Email",
                    Description = "Campagne créée par le bot de test",
                    DateDebut = DateTime.Now,
                    DateFin = DateTime.Now.AddDays(30),
                    Budget = 1000.00m,
                    NombreDestinataires = 100
                };
                var id = CampagneMarketingService.CreerCampagne(campagne);
                Console.WriteLine($"   ✅ Campagne créée (ID: {id})");
            }
            catch (Exception ex)
            {
                AddError("TestCreerCampagne", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetAllCampagnes()
        {
            try
            {
                var campagnes = CampagneMarketingService.GetAllCampagnes();
                Console.WriteLine($"   📊 {campagnes.Count} campagne(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllCampagnes", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetCampagneById()
        {
            try
            {
                var campagnes = CampagneMarketingService.GetAllCampagnes();
                if (campagnes.Count > 0)
                {
                    var campagne = CampagneMarketingService.GetCampagneById(campagnes[0].Id);
                    if (campagne != null)
                        Console.WriteLine($"   ✅ Campagne trouvée: {campagne.NomCampagne}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetCampagneById", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 6: STOCKS - PRODUITS ==========
        static void TestGetAllProduits()
        {
            try
            {
                var produits = ProduitService.GetAllProduits();
                Console.WriteLine($"   📊 {produits.Count} produit(s) trouvé(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllProduits", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetProduitById()
        {
            try
            {
                var produits = ProduitService.GetAllProduits();
                if (produits.Count > 0)
                {
                    var produit = ProduitService.GetProduitById(produits[0].Id);
                    if (produit != null)
                        Console.WriteLine($"   ✅ Produit trouvé: {produit.Nom}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetProduitById", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 7: STOCKS - CATÉGORIES ==========
        static void TestGetAllCategories()
        {
            try
            {
                var categories = CategorieService.GetAllCategories();
                Console.WriteLine($"   📊 {categories.Count} catégorie(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllCategories", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetCategorieById()
        {
            try
            {
                var categories = CategorieService.GetAllCategories();
                if (categories.Count > 0)
                {
                    var categorie = CategorieService.GetCategorieById(categories[0].Id);
                    if (categorie != null)
                        Console.WriteLine($"   ✅ Catégorie trouvée: {categorie.Nom}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetCategorieById", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 8: STOCKS - FOURNISSEURS ==========
        static void TestGetAllFournisseurs()
        {
            try
            {
                var fournisseurs = FournisseurService.GetAllFournisseurs();
                Console.WriteLine($"   📊 {fournisseurs.Count} fournisseur(s) trouvé(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllFournisseurs", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetFournisseurById()
        {
            try
            {
                var fournisseurs = FournisseurService.GetAllFournisseurs();
                if (fournisseurs.Count > 0)
                {
                    var fournisseur = FournisseurService.GetFournisseurById(fournisseurs[0].Id);
                    if (fournisseur != null)
                        Console.WriteLine($"   ✅ Fournisseur trouvé: {fournisseur.Nom}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetFournisseurById", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 9: STOCKS - MOUVEMENTS ==========
        static void TestGetMouvementsByProduit()
        {
            try
            {
                var produits = ProduitService.GetAllProduits();
                if (produits.Count > 0)
                {
                    var mouvements = MouvementStockService.GetMouvementsByProduitId(produits[0].Id);
                    Console.WriteLine($"   📊 {mouvements.Count} mouvement(s) trouvé(s)");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetMouvementsByProduit", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 10: FINANCES - FACTURES ==========
        static void TestGenererNumeroFacture()
        {
            try
            {
                var numero = FactureService.GenererNumeroFacture();
                Console.WriteLine($"   ✅ Numéro généré: {numero}");
            }
            catch (Exception ex)
            {
                AddError("TestGenererNumeroFacture", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetAllFactures()
        {
            try
            {
                var factures = FactureService.GetAllFactures();
                Console.WriteLine($"   📊 {factures.Count} facture(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllFactures", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetFactureByNumero()
        {
            try
            {
                var factures = FactureService.GetAllFactures();
                if (factures.Count > 0)
                {
                    var facture = FactureService.GetFactureByNumero(factures[0].NumeroFacture);
                    if (facture != null)
                        Console.WriteLine($"   ✅ Facture trouvée: {facture.NumeroFacture}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetFactureByNumero", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetFacturesByClient()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var factures = FactureService.GetFacturesByClient(clients[0].Id);
                    Console.WriteLine($"   📊 {factures.Count} facture(s) trouvée(s) pour le client");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetFacturesByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 11: FINANCES - PAIEMENTS ==========
        static void TestGetAllPaiements()
        {
            try
            {
                var paiements = PaiementService.GetAllPaiements();
                Console.WriteLine($"   📊 {paiements.Count} paiement(s) trouvé(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllPaiements", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetPaiementsByFacture()
        {
            try
            {
                var factures = FactureService.GetAllFactures();
                if (factures.Count > 0)
                {
                    var paiements = PaiementService.GetPaiementsByFactureId(factures[0].Id);
                    Console.WriteLine($"   📊 {paiements.Count} paiement(s) trouvé(s) pour la facture");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetPaiementsByFacture", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetPaiementsByPeriode()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var paiements = PaiementService.GetPaiementsByPeriode(dateDebut, dateFin);
                Console.WriteLine($"   📊 {paiements.Count} paiement(s) trouvé(s) pour la période");
            }
            catch (Exception ex)
            {
                AddError("TestGetPaiementsByPeriode", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 12: FINANCES - COMMANDES FOURNISSEURS ==========
        static void TestGenererNumeroCommandeFournisseur()
        {
            try
            {
                var numero = CommandeFournisseurService.GenererNumeroCommande();
                Console.WriteLine($"   ✅ Numéro généré: {numero}");
            }
            catch (Exception ex)
            {
                AddError("TestGenererNumeroCommandeFournisseur", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetAllCommandesFournisseurs()
        {
            try
            {
                var commandes = CommandeFournisseurService.GetAllCommandes();
                Console.WriteLine($"   📊 {commandes.Count} commande(s) fournisseur(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllCommandesFournisseurs", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetCommandeFournisseurByNumero()
        {
            try
            {
                var commandes = CommandeFournisseurService.GetAllCommandes();
                if (commandes.Count > 0)
                {
                    var commande = CommandeFournisseurService.GetCommandeByNumero(commandes[0].NumeroCommande);
                    if (commande != null)
                        Console.WriteLine($"   ✅ Commande trouvée: {commande.NumeroCommande}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetCommandeFournisseurByNumero", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 13: FINANCES - DÉPENSES ==========
        static void TestGetAllDepenses()
        {
            try
            {
                var depenses = DepenseService.GetAllDepenses();
                Console.WriteLine($"   📊 {depenses.Count} dépense(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetAllDepenses", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 14: FINANCES - JOURNAL COMPTABLE ==========
        static void TestGetJournalEntries()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var entries = JournalComptableService.GetJournalEntries(dateDebut, dateFin);
                Console.WriteLine($"   📊 {entries.Count} écriture(s) trouvée(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetJournalEntries", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestEquilibreJournal()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var entries = JournalComptableService.GetJournalEntries(dateDebut, dateFin);
                
                if (entries.Count == 0)
                {
                    Console.WriteLine("   ⚠️ Aucune écriture trouvée");
                    return;
                }

                var (totalDebit, totalCredit) = JournalComptableService.CalculateTotals(entries);
                Console.WriteLine($"   📊 Débit: {totalDebit:C}, Crédit: {totalCredit:C}");

                if (Math.Abs(totalDebit - totalCredit) > 0.01m)
                {
                    AddError("TestEquilibreJournal",
                        $"Déséquilibre: Débit ({totalDebit:C}) ≠ Crédit ({totalCredit:C})",
                        "Vérifiez la partie double");
                }
                else
                {
                    Console.WriteLine("   ✅ Journal équilibré");
                }
            }
            catch (Exception ex)
            {
                AddError("TestEquilibreJournal", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 15: FINANCES - TRANSACTIONS COMPTABLES ==========
        static void TestEnregistrerVente()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var numero = $"FAC-TEST-{DateTime.Now:yyyyMMddHHmmss}";
                    var montantHT = 100.00m;
                    var (tps, tvq, total) = TaxesService.CalculerTaxes(montantHT);
                    
                    AccountingService.EnregistrerVente(numero, DateTime.Now, montantHT, tps, tvq, total, clients[0].Nom);
                    Console.WriteLine($"   ✅ Vente enregistrée: {total:C}");
                }
            }
            catch (Exception ex)
            {
                AddError("TestEnregistrerVente", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestEnregistrerAchatStock()
        {
            try
            {
                var fournisseurs = FournisseurService.GetAllFournisseurs();
                if (fournisseurs.Count > 0)
                {
                    var numero = $"CMD-TEST-{DateTime.Now:yyyyMMddHHmmss}";
                    AccountingService.EnregistrerAchatStock(numero, DateTime.Now, 500.00m, fournisseurs[0].Nom);
                    Console.WriteLine($"   ✅ Achat enregistré: 500,00 $");
                }
            }
            catch (Exception ex)
            {
                AddError("TestEnregistrerAchatStock", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestEnregistrerDepense()
        {
            try
            {
                AccountingService.EnregistrerDepense("Test Bot - Dépense", DateTime.Now, 50.00m, "Marketing");
                Console.WriteLine($"   ✅ Dépense enregistrée: 50,00 $");
            }
            catch (Exception ex)
            {
                AddError("TestEnregistrerDepense", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 16: FINANCES - RAPPORTS ==========
        static void TestRapportTaxes()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var rapport = RapportService.GetRapportTaxes(dateDebut, dateFin);
                Console.WriteLine($"   ✅ Rapport généré: {rapport.Rows.Count} lignes");
            }
            catch (Exception ex)
            {
                AddError("TestRapportTaxes", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestRapportVentes()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var rapport = RapportService.GetRapportVentes(dateDebut, dateFin);
                Console.WriteLine($"   ✅ Rapport généré: {rapport.Rows.Count} lignes");
            }
            catch (Exception ex)
            {
                AddError("TestRapportVentes", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 17: FINANCES - PARAMÈTRES FISCAUX ==========
        static void TestParametresFiscaux()
        {
            try
            {
                var tauxTPS = TaxesService.GetTauxTPS();
                var tauxTVQ = TaxesService.GetTauxTVQ();
                Console.WriteLine($"   📊 TPS: {tauxTPS * 100:F3}%, TVQ: {tauxTVQ * 100:F3}%");
                
                if (tauxTPS <= 0 || tauxTVQ <= 0)
                {
                    throw new Exception("Taux de taxes invalides");
                }
            }
            catch (Exception ex)
            {
                AddError("TestParametresFiscaux", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestCalculerTaxes()
        {
            try
            {
                var montantHT = 100.00m;
                var (tps, tvq, total) = TaxesService.CalculerTaxes(montantHT);
                Console.WriteLine($"   📊 HT: {montantHT:C}, TPS: {tps:C}, TVQ: {tvq:C}, Total: {total:C}");
                
                if (total != montantHT + tps + tvq)
                {
                    throw new Exception("Calcul des taxes incorrect");
                }
            }
            catch (Exception ex)
            {
                AddError("TestCalculerTaxes", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 18: DASHBOARD ==========
        static void TestGetChiffreAffairesTotal()
        {
            try
            {
                var ca = DashboardService.GetChiffreAffairesTotal();
                Console.WriteLine($"   📊 CA Total: {ca:C}");
            }
            catch (Exception ex)
            {
                AddError("TestGetChiffreAffairesTotal", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetVentesTotales()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var ventes = DashboardService.GetVentesTotales(dateDebut, dateFin);
                Console.WriteLine($"   📊 Ventes: {ventes:C}");
            }
            catch (Exception ex)
            {
                AddError("TestGetVentesTotales", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetDepensesExploitation()
        {
            try
            {
                var dateDebut = DateTime.Now.AddMonths(-1);
                var dateFin = DateTime.Now;
                var depenses = DashboardService.GetDepensesExploitation(dateDebut, dateFin);
                Console.WriteLine($"   📊 Dépenses: {depenses:C}");
            }
            catch (Exception ex)
            {
                AddError("TestGetDepensesExploitation", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetFacturesEnAttente()
        {
            try
            {
                var factures = DashboardService.GetFacturesEnAttente();
                Console.WriteLine($"   📊 {factures.Rows.Count} facture(s) en attente");
            }
            catch (Exception ex)
            {
                AddError("TestGetFacturesEnAttente", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetDernieresTransactions()
        {
            try
            {
                var transactions = DashboardService.GetDernieresTransactions();
                Console.WriteLine($"   📊 {transactions.Rows.Count} transaction(s) récente(s)");
            }
            catch (Exception ex)
            {
                AddError("TestGetDernieresTransactions", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 19: COMMANDES VENTE ==========
        static void TestGetAllCommandes()
        {
            try
            {
                var clients = ClientService.GetAllClients();
                if (clients.Count > 0)
                {
                    var commandes = CommandeService.GetCommandesByClient(clients[0].Id);
                    Console.WriteLine($"   📊 {commandes.Count} commande(s) trouvée(s) pour le client");
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetAllCommandes", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== SECTION 20: TRANSACTIONS CLIENT ==========
        static int? TestRegisterClient()
        {
            try
            {
                // Générer un email unique pour le test
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var email = $"clienttest{timestamp}@test.com";
                var nom = $"Client Test {timestamp}";
                var telephone = "514-555-0000";
                var password = "TestClient123";

                var (success, message, clientId) = ClientService.Register(nom, email, telephone, password);

                if (success)
                {
                    Console.WriteLine($"   ✅ Client inscrit (ID: {clientId}, Email: {email})");
                    return clientId;
                }
                else
                {
                    AddError("TestRegisterClient", message, "");
                    return null;
                }
            }
            catch (Exception ex)
            {
                AddError("TestRegisterClient", ex.Message, ex.StackTrace ?? "");
                return null;
            }
        }

        static void TestAuthenticateClient()
        {
            try
            {
                // Essayer de trouver un client existant avec mot de passe
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT courriel_contact, mot_de_passe, nom
                        FROM clients
                        WHERE statut != 'Inactif' AND mot_de_passe IS NOT NULL AND mot_de_passe != ''
                        LIMIT 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var email = reader.GetString("courriel_contact");
                            var password = reader.GetString("mot_de_passe");
                            var nom = reader.GetString("nom");

                            var (success, nomRetourne, clientId) = ClientService.Authenticate(email, password);

                            if (success)
                            {
                                Console.WriteLine($"   ✅ Client authentifié: {nomRetourne} (ID: {clientId})");
                            }
                            else
                            {
                                AddError("TestAuthenticateClient",
                                    $"Échec d'authentification pour {email}",
                                    "Vérifiez que le mot de passe correspond");
                            }
                        }
                        else
                        {
                            Console.WriteLine("   ⚠️ Aucun client avec mot de passe trouvé pour tester l'authentification");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("TestAuthenticateClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetClientByEmail()
        {
            try
            {
                // Essayer de trouver un client existant
                using (var conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT courriel_contact
                        FROM clients
                        LIMIT 1";

                    using (var cmd = new MySqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var email = reader.GetString("courriel_contact");
                            var client = ClientService.GetClientByEmail(email);

                            if (client != null)
                            {
                                Console.WriteLine($"   ✅ Client trouvé: {client.Nom} ({email})");
                            }
                            else
                            {
                                AddError("TestGetClientByEmail", $"Client non trouvé pour {email}", "");
                            }
                        }
                        else
                        {
                            Console.WriteLine("   ⚠️ Aucun client trouvé pour tester");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("TestGetClientByEmail", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestCreateCommandeByClient(int? clientId)
        {
            try
            {
                if (!clientId.HasValue)
                {
                    // Essayer de trouver un client existant
                    var clients = ClientService.GetAllClients();
                    if (clients.Count == 0)
                    {
                        Console.WriteLine("   ⚠️ Aucun client trouvé pour créer une commande");
                        return;
                    }
                    clientId = clients[0].Id;
                }

                // Récupérer des produits disponibles
                var produits = ProduitService.GetAllProduits();
                if (produits.Count == 0)
                {
                    Console.WriteLine("   ⚠️ Aucun produit trouvé pour créer une commande");
                    return;
                }

                // Créer une commande avec un produit
                var commande = new Commande
                {
                    ClientId = clientId.Value,
                    DateCommande = DateTime.Now,
                    Statut = "Brouillon",
                    MontantTotal = 0,
                    AdresseLivraison = "123 Rue Test, Montréal, QC",
                    Notes = "Commande créée par le bot de test"
                };

                // Créer une ligne de commande
                var ligne = new LigneCommande
                {
                    ProduitId = produits[0].Id,
                    Quantite = 1,
                    PrixUnitaire = produits[0].Prix,
                    SousTotal = produits[0].Prix
                };

                commande.Lignes = new List<LigneCommande> { ligne };
                commande.MontantTotal = ligne.SousTotal;

                var commandeId = CommandeService.CreateCommande(commande);
                Console.WriteLine($"   ✅ Commande créée (ID: {commandeId}, Montant: {commande.MontantTotal:C})");
            }
            catch (Exception ex)
            {
                AddError("TestCreateCommandeByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestGetCommandesByClient(int? clientId)
        {
            try
            {
                if (!clientId.HasValue)
                {
                    var clients = ClientService.GetAllClients();
                    if (clients.Count == 0)
                    {
                        Console.WriteLine("   ⚠️ Aucun client trouvé");
                        return;
                    }
                    clientId = clients[0].Id;
                }

                var commandes = CommandeService.GetCommandesByClient(clientId.Value);
                Console.WriteLine($"   📊 {commandes.Count} commande(s) trouvée(s) pour le client ID {clientId}");
            }
            catch (Exception ex)
            {
                AddError("TestGetCommandesByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        static void TestCreateEvaluationByClient(int? clientId)
        {
            try
            {
                if (!clientId.HasValue)
                {
                    var clients = ClientService.GetAllClients();
                    if (clients.Count == 0)
                    {
                        Console.WriteLine("   ⚠️ Aucun client trouvé pour créer une évaluation");
                        return;
                    }
                    clientId = clients[0].Id;
                }

                // Récupérer une facture du client
                var factures = FactureService.GetFacturesByClient(clientId.Value);
                int? factureId = null;
                if (factures.Count > 0)
                {
                    factureId = factures[0].Id;
                }

                var evaluation = new EvaluationClient
                {
                    ClientId = clientId.Value,
                    FactureId = factureId,
                    NoteSatisfaction = 5,
                    Commentaire = "Évaluation créée par le bot de test - Excellent service !",
                    DateEvaluation = DateTime.Now
                };

                var evaluationId = EvaluationClientService.CreerEvaluation(evaluation);
                Console.WriteLine($"   ✅ Évaluation créée (ID: {evaluationId}, Note: {evaluation.NoteSatisfaction}/5)");
            }
            catch (Exception ex)
            {
                AddError("TestCreateEvaluationByClient", ex.Message, ex.StackTrace ?? "");
            }
        }

        // ========== UTILITAIRES ==========
        static void AddError(string testName, string message, string details = "")
        {
            errors.Add(new TestError
            {
                TestName = testName,
                Message = message,
                StackTrace = details,
                Timestamp = DateTime.Now
            });
        }

        static void AfficherRapportFinal()
        {
            Console.WriteLine("\n" + new string('=', 70));
            Console.WriteLine("📋 RAPPORT FINAL DES TESTS");
            Console.WriteLine(new string('=', 70) + "\n");

            if (errors.Count == 0)
            {
                Console.WriteLine("🎉 TOUS LES TESTS SONT PASSÉS AVEC SUCCÈS !");
                Console.WriteLine("✅ Aucune erreur détectée\n");
            }
            else
            {
                Console.WriteLine($"❌ {errors.Count} ERREUR(S) DÉTECTÉE(S)\n");
                Console.WriteLine("DÉTAILS DES ERREURS :\n");
                Console.WriteLine(new string('-', 70));

                for (int i = 0; i < errors.Count; i++)
                {
                    var error = errors[i];
                    Console.WriteLine($"\n[{i + 1}] Test: {error.TestName}");
                    Console.WriteLine($"    ⏰ Heure: {error.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"    ❌ Erreur: {error.Message}");
                    
                    if (!string.IsNullOrWhiteSpace(error.StackTrace))
                    {
                        Console.WriteLine($"    📝 Détails:");
                        var lines = error.StackTrace.Split('\n').Take(5);
                        foreach (var line in lines)
                        {
                            Console.WriteLine($"       {line.Trim()}");
                        }
                        if (error.StackTrace.Split('\n').Length > 5)
                        {
                            Console.WriteLine($"       ... ({error.StackTrace.Split('\n').Length - 5} lignes supplémentaires)");
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine(new string('-', 70));
                Console.WriteLine("\n💡 ACTIONS RECOMMANDÉES :");
                Console.WriteLine("   1. Vérifiez les erreurs ci-dessus");
                Console.WriteLine("   2. Corrigez les problèmes identifiés");
                Console.WriteLine("   3. Ré-exécutez le bot de test\n");
            }

            Console.WriteLine(new string('=', 70));
        }
    }

    public class TestError
    {
        public string TestName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string StackTrace { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
