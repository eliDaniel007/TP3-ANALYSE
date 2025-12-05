<!-- Copilot / AI agent guidance for the TP3-ANALYSE (Nordik Adventures ERP) repository -->
# Copilot instructions — Nordik Adventures ERP

Short, actionable directions to help an AI coding assistant be immediately productive in this repository.

1) Big picture
- **Type & UI:** WPF desktop application targeting `net8.0-windows` (see `Analyse tp/analyse/PGI/PGI.csproj`).
- **Storage:** MySQL database created from `sql_scripts/SQL_COMPLET_UNIFIE.sql` (schema + test data).
- **Pattern:** Thin UI (Views / XAML) + Models + services that call `PGI.Helpers.DatabaseHelper` (ADO.NET / `MySql.Data`).
- **Assets:** Product images are stored in the repo root `assets/IMAGES PRODUITS` and are linked into the WPF project via `PGI.csproj` (project copies these into output).

2) Where to start (developer flows)
- To run locally: import `Analyse tp/analyse/PGI.sln` into Visual Studio 2022+ and press F5 (README has credentials).
- CLI build: from repository root you can run:
  - `dotnet build "Analyse tp\analyse\PGI\PGI.csproj"` (must run on Windows with .NET 8 SDK).
- Database setup: open `sql_scripts/SQL_COMPLET_UNIFIE.sql` in MySQL Workbench and execute. Confirm DB `NordikAdventuresERP` exists.
- Configure DB credentials by editing `Analyse tp/analyse/PGI/Helpers/DatabaseHelper.cs` or call `DatabaseHelper.SetConnectionString(...)` at startup.

3) Important code patterns and conventions
- **DB Access:** Use `PGI.Helpers.DatabaseHelper` for all DB operations:
  - `ExecuteQuery(string, Dictionary<string, object>?)` returns `DataTable`.
  - `ExecuteNonQuery(...)` and `ExecuteScalar(...)` are used for writes/aggregates.
  - Parameter dictionaries pass parameter names as keys (include `@` prefix when used in SQL).
  - Example: `DatabaseHelper.ExecuteQuery("SELECT * FROM produits WHERE id = @id", new Dictionary<string, object>{{"@id", id}});`
- **Plaintext passwords:** The project currently stores and uses plaintext passwords (both in DB and default `DatabaseHelper.connectionString`). Treat this as development-only and do not migrate to production as-is.
- **View loading:** Views are loaded dynamically by path strings (see `MainWindow.LoadView("Views/Stocks/StocksMainView.xaml")`). Use that pattern when adding UI modules.
- **Image mapping:** `Models/Produit.cs` contains SKU → filename mapping used by `Produit.ImagePath`. Product images are referenced by SKU-to-filename mapping, not by database filenames.

4) Files to inspect when changing behavior
- Connection / environment: `Analyse tp/analyse/PGI/Helpers/DatabaseHelper.cs`
- Project file & runtime info: `Analyse tp/analyse/PGI/PGI.csproj`
- App entry / navigation: `Analyse tp/analyse/PGI/App.xaml.cs` and `MainWindow.xaml.cs`
- Domain models: `Analyse tp/analyse/PGI/Models/` (e.g. `Produit.cs`, `Client.cs`)
- SQL seed: `sql_scripts/SQL_COMPLET_UNIFIE.sql`

5) Typical change patterns and guidance for PRs
- When adding DB queries, prefer using the existing helper methods and parameter dictionaries instead of opening raw connections.
- If adding images, place them under `assets/IMAGES PRODUITS` and update `PGI.csproj` if new extensions are used.
- Keep UI navigation consistent with `MainWindow` (use `LoadView` string paths). New modules should live in `Views/<Module>/`.

6) Debugging tips
- If views fail to load: check the relative path used in `LoadView` and confirm the XAML `Build Action` is correct.
- If DB connection fails: update `DatabaseHelper.connectionString` or call `SetConnectionString` programmatically; use `DatabaseHelper.TestConnection(out msg)` to get the error message.
- Image not found at runtime: confirm that the file name mapping in `Produit.ImagePath` matches a file in `assets/IMAGES PRODUITS` and that the asset was copied to the output directory.

7) Security & cautions for AI changes
- Do NOT commit real credentials. Credential edits should be parameterized or moved to a local config during refactor.
- Avoid changing authentication semantics broadly — the login and password handling is intentionally simplistic for the assignment.

8) Quick examples (copyable)
- Set connection string at runtime:
  ```csharp
  PGI.Helpers.DatabaseHelper.SetConnectionString("localhost","NordikAdventuresERP","root","your_password");
  ```
- Simple parameterized query:
  ```csharp
  var dt = PGI.Helpers.DatabaseHelper.ExecuteQuery(
      "SELECT * FROM produits WHERE id = @id",
      new Dictionary<string, object>{{"@id", 42}}
  );
  ```

If anything here is unclear or you'd like me to emphasize a different area (tests, adding a CI workflow, or security hardening), tell me which sections to expand and I'll iterate. 
