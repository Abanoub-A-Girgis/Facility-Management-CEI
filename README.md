# Facility Management CEI

Production-oriented **BIM / IFC issue-management system** for facility teams.

The system allows users to upload IFC building models, split them by floor, view them in a browser-based Angular viewer, and manage operational issues linked directly to building elements.

> **Frontend note:** Angular in `frontend/` is the only user-facing frontend. The ASP.NET Core project exposes JSON APIs and serves IFC/viewer assets only.

---

## Features

* Upload IFC building models.
* Split IFC models by floor/storey.
* Convert floor models to `.wexBIM` for browser viewing.
* View building models using the XBIM browser viewer.
* Toggle floor visibility.
* Select building elements in the viewer.
* Create issues linked to selected assets or spaces.
* Assign issue-related tasks to workers.
* Highlight elements with active issues.
* Toggle issue flashing/highlighting.
* Use severity-based colors:

  * Severe: red
  * High: orange
  * Medium: yellow
  * Low: green
* Support English and Arabic localization.

---

## Tech Stack

### Backend

* ASP.NET Core 5
* Entity Framework Core 5
* ASP.NET Core Identity
* SQL Server
* XBIM Essentials / XBIM Geometry

### Frontend

* Angular 17
* TypeScript
* XBIM browser viewer

---

## Project Structure

```txt
Facility Management CEI/
├── API/Controllers/          # Angular-facing API controllers
├── APIs/Controllers/         # Legacy IFC import compatibility code
├── Services/                 # IFC processing and viewer services
├── Configuration/            # Viewer and app configuration
├── Resources/                # Localization .resx files
├── Migrations/               # EF Core migrations
├── Startup.cs
└── appsettings.json

frontend/
├── src/app/                  # Angular pages, components, and services
├── src/environments/         # API base URL configuration
└── package.json

FacilityManagementCEI.Tests/
└── Backend automated tests
```

---

## Main Application Areas

The Angular frontend includes pages for:

* Login and registration
* Operations dashboard
* IFC upload
* Model viewer
* Floor visibility
* Issue management
* Worker assignment
* Sensor warnings
* Incidents
* Work orders
* Reports
* Building information
* User management

---

## IFC Processing

IFC upload, validation, splitting, and `.wexBIM` conversion are handled mainly by:

```txt
Facility Management CEI/Services/IfcProcessingService.cs
```

Model extraction and compatibility import logic currently live mainly in:

```txt
Facility Management CEI/APIs/Controllers/IFCController.cs
```

Viewer highlight configuration is handled by:

```txt
Facility Management CEI/Services/ViewerHighlightMapper.cs
Facility Management CEI/Configuration/ViewerOptions.cs
```

---

## Localization

Backend translations are stored in:

```txt
Facility Management CEI/Resources/SharedResource.resx
Facility Management CEI/Resources/SharedResource.ar.resx
```

Supported cultures:

* `en`
* `ar`

Angular loads translations from:

```http
GET /api/localization?culture=en
GET /api/localization?culture=ar
```

Arabic mode sets the Angular document direction to `rtl`.

---

## Prerequisites

* Windows development environment recommended
* .NET 5 SDK
* SQL Server / SQL Server Express / LocalDB
* Visual Studio 2019/2022 or another .NET-capable IDE
* Node.js 20.10+
* npm

---

## Local Development Setup

### 1. Backend

From the repository root:

```sh
cd "Facility Management CEI"
dotnet restore
dotnet build
dotnet run
```

The backend runs on:

```txt
https://localhost:5001
http://localhost:5000
```

### 2. Angular Frontend

From the repository root:

```sh
cd frontend
npm install
npm start
```

The Angular app runs on:

```txt
http://localhost:4200
```

The frontend API base URL is configured in:

```txt
frontend/src/environments/environment.ts
```

Default development API URL:

```ts
apiBaseUrl: 'https://localhost:5001'
```

---

## Database Setup

The project uses SQL Server with Entity Framework Core migrations.

Current connection string key:

```txt
ConnectionStrings:ProjectDataBase
```

Apply migrations using Visual Studio Package Manager Console:

```powershell
Update-Database -Context Facility_Management_CEI.IdentityDb.ApplicationDBContext
```

Or using the .NET CLI:

```sh
dotnet ef database update --context Facility_Management_CEI.IdentityDb.ApplicationDBContext
```

---

## Important Configuration

Configuration is currently stored in:

```txt
Facility Management CEI/appsettings.json
```

Important configuration areas:

* `ConnectionStrings:ProjectDataBase`
* `Cors:AllowedOrigins`
* `SeedAdmin`
* `IfcProcessing`
* `Viewer`
* `ASPNETCORE_ENVIRONMENT`
* `ASPNETCORE_URLS`

For production, secrets and environment-specific values should be moved out of source-controlled files and managed through environment variables, secret managers, or deployment-platform configuration.

---

## API Overview

The Angular frontend consumes JSON APIs for authentication, uploads, model metadata, floors, elements, issues, workers, dashboards, warnings, reports, user management, localization, and viewer highlighting.

Main API groups include:

```txt
/api/localization
/api/models
/api/elements
/api/issues
/api/workers
/api/operations
```

Example routes:

```http
POST /api/models/upload
GET  /api/models
GET  /api/models/{modelId}/floors
GET  /api/models/{modelId}/elements
GET  /api/models/{modelId}/viewer-highlights

GET  /api/elements/{elementId}
GET  /api/elements/{elementId}/issues
POST /api/elements/{elementId}/issues

PUT    /api/issues/{issueId}
DELETE /api/issues/{issueId}
PUT    /api/issues/{issueId}/assign-worker

GET /api/workers
GET /api/operations/dashboard
GET /api/operations/incidents
GET /api/operations/work-orders
GET /api/operations/report
```

---

## Tests

Backend automated tests live in:

```txt
FacilityManagementCEI.Tests/
```

Current tests cover:

* IFC upload validation
* Severity color normalization

Run tests with:

```sh
dotnet test
```

---

## Build

Build the backend:

```sh
cd "Facility Management CEI"
dotnet build
```

Publish the backend:

```sh
dotnet publish -c Release
```

Build the Angular frontend:

```sh
cd frontend
npm run build
```

---

## Production Notes

Before production deployment:

* Upgrade from .NET 5 to a supported .NET LTS version.
* Move secrets out of `appsettings.json`.
* Configure CORS for the deployed Angular origin only.
* Secure upload, issue, task, assignment, and admin endpoints with authorization policies.
* Store uploaded IFC and generated `.wexBIM` files in a durable backed-up location.
* Move large IFC processing into a background job queue.
* Add structured logging, health checks, monitoring, and CI/CD.
* Add role-based access control and audit logs.
* Validate Docker/container support separately before relying on it.

---

## Known Limitations

* The project currently targets .NET 5.
* IFC processing still runs during the upload request.
* IFC import compatibility code still mixes parsing and persistence concerns.
* Identity and domain entities currently share the same EF context.
* Docker setup is not included.
* Authorization is not fully enforced across every endpoint.
* More tests are needed before production use.

---

## Future Improvements

* Upgrade to a supported .NET LTS version.
* Extract IFC processing into dedicated services and background jobs.
* Add versioned REST APIs and DTO contracts.
* Add OpenAPI/Swagger documentation.
* Add CI/CD.
* Add cloud or network file-storage abstraction.
* Add audit logs for uploads, issues, assignments, and status changes.
* Add screenshots and architecture diagrams.

---

## License

This project is open source and available under the MIT License.
See the [LICENSE](LICENSE) file for details.
