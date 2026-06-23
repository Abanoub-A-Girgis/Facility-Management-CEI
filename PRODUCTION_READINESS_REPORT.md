# Production Readiness Report

## What Was Changed

- Verified the ASP.NET Core backend builds successfully.
- Verified the Angular frontend builds successfully.
- Ran the available automated test suite successfully.
- Moved CORS origins into configuration with `Cors:AllowedOrigins`.
- Moved the bootstrap admin seed values into configuration with `SeedAdmin`.
- Added explicit `MapControllers()` registration for attribute-routed API controllers.
- Added consistent API model-validation responses through `ApiResponse<object>`.
- Kept IFC upload hardening in place: extension checks, content-type allowlist, max size, IFC STEP header sniffing, safe filename handling, upload-root containment, localized errors, and processing failure logging.
- Kept viewer hardening in place: configured severity colors, bounded flash interval, deduplicated highlight IDs, and Angular fallback defaults.
- Updated `appsettings.example.json` and README production notes.

## Verification

- Backend: `dotnet build --no-restore`
  - Result: passed.
  - Remaining warnings: `.NET 5` is out of support; XBIM geometry packages are restored through .NET Framework compatibility assets.
- Angular: `npm run build`
  - Result: passed.
- Tests: `dotnet test --no-restore`
  - Result: passed, 7 tests.

## How To Run Locally

1. Configure `Facility Management CEI/appsettings.json` from `Facility Management CEI/appsettings.example.json`.
2. Set `ConnectionStrings:ProjectDataBase`.
3. Optionally set `SeedAdmin:Enabled`, `SeedAdmin:UserName`, and `SeedAdmin:Password` for first-run bootstrap.
4. Run the backend:

```sh
cd "Facility Management CEI"
dotnet run
```

5. Run the Angular frontend:

```sh
cd frontend
npm install
npm start
```

6. Open `http://localhost:4200`.

## How To Test Main Flows

- Upload a valid `.ifc` file and confirm processing returns generated IFC/WexBIM paths.
- Open the Angular viewer and confirm floors load.
- Toggle each floor checkbox and confirm viewer visibility changes.
- Select an element and confirm element details appear.
- Create an issue for the selected element.
- Assign a worker to the issue.
- Confirm issue highlighting appears with severity colors.
- Toggle flashing off and confirm highlighted elements remain usable.
- Switch culture between English and Arabic and confirm labels and RTL direction update.

## Manual Review Still Needed

- Verify a real IFC upload with representative production-size files.
- Verify XBIM split/conversion behavior on the intended production host OS.
- Verify API authentication/authorization requirements for every endpoint.
- Verify the deployment-specific Angular API base URL and CORS origin.
- Review legacy MVC/Razor pages before removing them; Angular coexists with them today.
- Review old Razor localization tags that remain in legacy pages while Angular migration continues.
- Review upload storage retention, cleanup, backup, and antivirus/malware scanning policies.

## Known Risks

- The backend targets `.NET 5`, which is out of support.
- XBIM geometry packages emit compatibility warnings and should be validated before container or Linux deployment.
- IFC processing still happens during the upload request. Large files may require a background job queue with progress/status endpoints.
- Current upload storage must stay under the web root so `.wexBIM` files can be served to the browser.
- Some API endpoints still need production authorization policies.
- Docker packaging is not included because the current `.NET 5` plus XBIM runtime assumptions need environment-specific validation first.
- Legacy Razor pages still exist as fallback and should be retired only after Angular parity is verified.
