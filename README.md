# Facility Management CEI

Production-oriented BIM / IFC issue-management system for facility teams. The system imports IFC building models, splits them by floor, displays the model in a browser-based viewer, and links operational issues and work assignments directly to building elements.

> Migration note: the legacy UI is an ASP.NET Core MVC/Razor application. A new Angular frontend now exists in `frontend/` and consumes the ASP.NET Core Web API. The legacy Razor UI is kept temporarily until Angular reaches full feature parity.

## Features

- Upload IFC files from the web application.
- Automatically split IFC models by building floor/storey.
- Convert split floor models to `.wexBIM` for browser viewing.
- View building models in the browser using the XBIM viewer.
- Toggle floor visibility using floor checkboxes.
- Select individual building elements in the viewer.
- Create issues/incidents linked to selected assets or spaces.
- Create tasks from issues and assign them to workers/agents.
- Highlight elements and rooms that have active assigned issues.
- Toggle issue highlighting/flashing indicators on and off.
- Use different indicator colors based on severity:
  - Severe: red
  - High: orange
  - Medium: yellow
  - Low: green
- Role-based viewer pages for account managers, owners, managers, supervisors, inspectors, and agents.
- English/Arabic localization through ASP.NET Core `.resx` resources and Angular runtime translation loading.

## Tech Stack

- ASP.NET Core 5 MVC/Razor
- Entity Framework Core 5
- ASP.NET Core Identity
- SQL Server
- XBIM Essentials / XBIM Geometry
- XBIM browser viewer scripts
- Razor views, JavaScript, jQuery, Bootstrap
- DataTables, Chart.js, Skote MVC template assets
- Angular 17 frontend in `frontend/`
- TypeScript

## Target Architecture

### Backend

- ASP.NET Core backend exposing versioned API endpoints for authentication, IFC upload, buildings, floors, spaces, assets, incidents, tasks, workers, and viewer metadata.
- Existing MVC controllers should be gradually refactored into application services and API controllers.
- Current backend entry points include `Facility Management CEI/Startup.cs`, `Controllers/`, and `APIs/Controllers/`.

### Angular Frontend

- Angular application lives in `frontend/`.
- Angular consumes the ASP.NET Core API through services such as `models.service.ts`, `floors.service.ts`, `elements.service.ts`, `issues.service.ts`, and `workers.service.ts`.
- Current Angular pages/components include model upload, model viewer, floor visibility, issue management, worker assignment, and flashing/highlighting controls.
- The existing Razor UI remains as legacy fallback during migration.

### IFC Processing

- Current IFC upload and processing live in `Controllers/UploaderController.cs`.
- IFC model extraction lives mainly in `APIs/Controllers/IFCController.cs`.
- Target state: extract upload, floor splitting, `.wexBIM` conversion, and IFC metadata extraction into backend services.

### Database

- SQL Server with Entity Framework Core migrations.
- Current configured connection string key: `ProjectDataBase`.
- Current Identity-backed EF context: `Facility_Management_CEI.IdentityDb.ApplicationDBContext`.
- Domain entities include buildings, floors, spaces, assets, sensors, incidents, tasks, and users.

### Localization

- Backend/server-side translations are stored in `.resx` files:
  - `Facility Management CEI/Resources/SharedResource.resx`
  - `Facility Management CEI/Resources/SharedResource.ar.resx`
- Supported cultures are `en` and `ar`.
- ASP.NET Core request localization supports query-string culture selection and culture cookies.
- The Angular frontend loads translations from `GET /api/localization?culture=en|ar`.
- Arabic sets `dir="rtl"` on the document root in Angular.
- Add or change UI strings by updating the same key in both resource files.
- Some inactive/commented legacy Razor markup may still contain old inline translation tags while the Angular migration continues.

## Screenshots

Screenshots are not currently included in the repository.

TODO:

- Upload IFC screen
- Building viewer with floor checkboxes
- Selected element with linked issue
- Severity highlighting/flashing
- Task assignment workflow

## Prerequisites

- Windows development environment recommended
- .NET 5 SDK
- SQL Server or SQL Server Express / LocalDB
- Visual Studio 2019/2022 or another .NET-capable IDE
- Node.js 20.10+ for the current Angular 17 frontend
- Angular CLI is installed through project npm scripts / `npx`

## Local Development Setup

1. Clone the repository.
2. Open `Facility Management CEI.sln`.
3. Restore NuGet packages.
4. Configure the SQL Server connection string in `Facility Management CEI/appsettings.json`.
5. Apply EF Core migrations.
6. Run the ASP.NET Core project.
7. Install and run the Angular frontend from `frontend/`.

From the command line:

```sh
cd "Facility Management CEI"
dotnet restore
dotnet build
dotnet run
```

The project launch profile uses:

- `https://localhost:5001`
- `http://localhost:5000`
- IIS Express: `http://localhost:9628` with SSL port `44313`

Run Angular separately:

```sh
cd frontend
npm install
npm start
```

The Angular dev server runs on `http://localhost:4200` and expects the API base URL from `frontend/src/environments/environment.ts`.

## Environment Variables / Configuration

Configuration is currently stored in `Facility Management CEI/appsettings.json`.

```json
{
  "ConnectionStrings": {
    "ProjectDataBase": "Server=.;Database=ProjectDataBaseWithAPINewViewer;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Cors": {
    "AllowedOrigins": [ "http://localhost:4200", "https://localhost:4200" ]
  },
  "SeedAdmin": {
    "Enabled": false,
    "UserName": "",
    "Email": "",
    "FirstName": "Admin",
    "Password": ""
  },
  "IfcProcessing": {
    "UploadDirectory": "data",
    "MaxUploadBytes": 2147483648,
    "AllowedExtensions": [ ".ifc" ],
    "AllowedContentTypes": [ "application/octet-stream", "application/ifc", "application/x-step", "text/plain" ],
    "ValidateIfcHeader": true,
    "HeaderValidationBytes": 8192
  },
  "Viewer": {
    "FlashIntervalMs": 750,
    "SeverityColors": {
      "Severe": [ 255, 0, 0, 255 ],
      "High": [ 255, 160, 16, 255 ],
      "Medium": [ 255, 224, 32, 255 ],
      "Low": [ 0, 255, 0, 255 ]
    }
  }
}
```

For production, move secrets and environment-specific values out of source-controlled files and use environment variables, secret managers, or deployment-platform configuration.

Important configuration items:

- `ConnectionStrings:ProjectDataBase`
- `Cors:AllowedOrigins`
- `SeedAdmin:Enabled`
- `SeedAdmin:UserName`
- `SeedAdmin:Password`
- `IfcProcessing:UploadDirectory`
- `IfcProcessing:MaxUploadBytes`
- `IfcProcessing:AllowedExtensions`
- `IfcProcessing:AllowedContentTypes`
- `IfcProcessing:ValidateIfcHeader`
- `IfcProcessing:HeaderValidationBytes`
- `Viewer:FlashIntervalMs`
- `Viewer:SeverityColors`
- `ASPNETCORE_ENVIRONMENT`
- `ASPNETCORE_URLS` when self-hosting with Kestrel

## Database Setup and Migrations

The repository includes an EF Core migration under `Facility Management CEI/Migrations`.

Using Visual Studio Package Manager Console:

```powershell
Update-Database -Context Facility_Management_CEI.IdentityDb.ApplicationDBContext
```

If a new migration is needed:

```powershell
Add-Migration MyMigration -Context Facility_Management_CEI.IdentityDb.ApplicationDBContext
Update-Database -Context Facility_Management_CEI.IdentityDb.ApplicationDBContext
```

Using the .NET CLI from the project folder may require the EF tool:

```sh
dotnet tool restore
dotnet ef database update --context Facility_Management_CEI.IdentityDb.ApplicationDBContext
```

TODO: verify the CLI migration command in a clean environment.

## Running the Backend

From the project folder:

```sh
cd "Facility Management CEI"
dotnet run
```

Then open:

- `https://localhost:5001`
- `http://localhost:5000`

The default route currently points to the account login page.

## Running the Angular Frontend

From the repository root:

```sh
cd frontend
npm install
npm start
```

Open `http://localhost:4200`.

Default development API base URL:

```ts
apiBaseUrl: 'https://localhost:5001'
```

Change it in `frontend/src/environments/environment.ts` if your backend uses another URL.

## Build Instructions

Build the ASP.NET Core project:

```sh
cd "Facility Management CEI"
dotnet build
```

Publish the backend:

```sh
cd "Facility Management CEI"
dotnet publish -c Release
```

Build the Angular frontend:

```sh
cd frontend
npm run build
```

## Production Deployment Notes

- Upgrade from .NET 5 to a supported .NET LTS version before production deployment.
- Configure `Cors:AllowedOrigins` to the deployed Angular origin; do not leave broad or unneeded localhost origins in production.
- Configure `SeedAdmin` only for controlled bootstrap environments, and store any seed password in a secret manager or deployment secret.
- Store uploaded IFC and generated `.wexBIM` files in a durable, backed-up location.
- Validate uploaded IFC files by extension, configured MIME/content type, configured size limit, safe file name, safe upload path, and IFC STEP header.
- Protect upload, database-clear, incident, task, and assignment endpoints with authorization policies.
- IFC processing is wrapped with logging and clear failures, but large models are still processed during the upload request. TODO: move splitting/conversion into a durable background job queue with progress/status endpoints.
- Serve static `.wexBIM` files with the correct content type.
- Use HTTPS, secure cookies, production Identity settings, and externalized secrets.
- Add structured logging, health checks, and monitoring.
- Add automated tests for upload, IFC processing, viewer metadata, incidents, tasks, and role-based access.

## Docker / Deployment

No Docker setup is currently included. The project targets .NET 5 and uses XBIM geometry packages with compatibility warnings, so containerization should be validated separately before relying on it for production. A safe first deployment path is:

- Publish the ASP.NET Core backend with `dotnet publish -c Release`.
- Build Angular with `npm run build`.
- Deploy the Angular static output and configure `environment.prod.ts` / hosting rewrite rules for the API origin.
- Deploy SQL Server separately and provide `ConnectionStrings:ProjectDataBase` through environment or platform secrets.
- Ensure generated `.wexBIM` files are served from the configured upload location.

## API Overview

The current repository contains API-like endpoints in `Facility Management CEI/APIs/Controllers/IFCController.cs`.

Known routes include:

- `api/IFC/AddBuildingDataFromIFC`
- `api/IFC/BuilingDTOData`
- `api/IFC/FloorDTO`
- `api/IFC/SpaceDTO`
- `api/IFC/AssetDTO`
- `api/IFC/SensorDTO`
- `api/IFC/GetTaskData`
- `api/IFC/StoreTaskData`
- `api/IFC/ClearDatabase`

Additional workflows currently use MVC controllers and Razor views:

- Upload: `Controllers/UploaderController.cs`
- Viewer pages: `Controllers/ViewerController.cs`
- Issues/incidents: `Controllers/IncidentController.cs`
- Task and worker assignment: `Controllers/TaskPagesController.cs`
- Users, buildings, floors, spaces, assets, sensors, and warnings: `Controllers/*Controller.cs`

The Angular frontend currently consumes the new JSON API layer for upload, model metadata, floors, elements, issues, workers, severities/statuses, and viewer highlighting.

Current Angular-facing routes include:

- `GET /api/localization?culture=en|ar`
- `POST /api/models/upload`
- `GET /api/models`
- `GET /api/models/{modelId}`
- `GET /api/models/{modelId}/floors`
- `GET /api/models/{modelId}/elements`
- `GET /api/models/{modelId}/floors/{floorId}/elements`
- `GET /api/models/{modelId}/viewer-highlights`
- `GET /api/elements/{elementId}`
- `GET /api/elements/{elementId}/issues`
- `POST /api/elements/{elementId}/issues`
- `PUT /api/issues/{issueId}`
- `DELETE /api/issues/{issueId}`
- `PUT /api/issues/{issueId}/assign-worker`
- `GET /api/issues/severities`
- `GET /api/issues/statuses`
- `GET /api/workers`

## Known Limitations

- Angular frontend is implemented but still coexists with the legacy MVC/Razor UI.
- Some legacy UI is MVC/Razor with significant inline JavaScript.
- Viewer behavior is duplicated across role-specific Razor pages and partials.
- Some legacy Razor views still need gradual cleanup as Angular reaches full replacement coverage.
- API boundaries are incomplete; some controllers mix view rendering, data access, and business logic.
- The wired EF context includes both Identity and domain data.
- IFC processing still happens during the upload request and may not scale for large models despite improved validation/logging.
- IFC upload storage must remain under the web root in the current viewer architecture so generated `.wexBIM` files can be served to the browser.
- Element IDs are based on IFC labels and exposed with an `ElementKey` such as `Asset:123` or `Space:456`; future issue endpoints should accept this typed key to avoid ambiguity if an asset and space share the same numeric label.
- Docker packaging is not yet provided.
- Authorization is not yet fully enforced across every new API endpoint.
- The project targets .NET 5, which is no longer supported.
- Some endpoints and methods need production hardening before public exposure.

## Future Improvements

- Continue Angular migration until the legacy Razor viewer pages can be retired.
- Add versioned REST APIs and DTO contracts for all frontend workflows.
- Extract IFC processing into services and optional background jobs.
- Add automated tests and CI/CD.
- Continue removing old inline translation tags from inactive legacy Razor markup.
- Add audit logs for issue creation, assignment, status changes, and model uploads.
- Add file storage abstraction for local, cloud, or network storage.
- Improve role-based authorization and least-privilege access policies.
- Add OpenAPI/Swagger documentation.
- Add screenshots and architecture diagrams.

## License

TODO: add license information. No license file was found in the repository.
