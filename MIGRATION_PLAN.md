# Migration Plan

## Current Architecture

- The solution is a single ASP.NET Core 5 web project: `Facility Management CEI/Facility Management CEI.csproj`.
- The app is currently MVC/Razor, not Blazor. I did not find Blazor host files, `MapBlazorHub`, Blazor package references, or `.razor` components. Razor `.cshtml` views are used throughout `Views/`.
- `Startup.cs` configures MVC controllers with views, ASP.NET Identity, EF Core SQL Server, static files, and `.wexBIM` static serving.
- Data access is coupled to the web app through `IdentityDb/ApplicationDBContext.cs`, which includes both Identity tables and facility-management domain tables. There is also an older/non-wired API context in `APIs/DB/ApplicationDBContext.cs`.
- Backend code is mixed:
  - MVC page controllers live in `Controllers/`.
  - API-like controllers live in `APIs/Controllers/`, especially `IFCController.cs`, but they still inherit from `Controller` and are invoked directly from MVC code in some places.
- IFC processing uses XBIM packages from the .NET backend and static XBIM viewer scripts in `wwwroot/Scripts/XBIM`.
- The browser viewer is mostly inline JavaScript embedded in Razor views under `Views/Viewer/`.
- Localization now has ASP.NET Core `.resx` resources in `Resources/SharedResource.resx` and `Resources/SharedResource.ar.resx`, with runtime Angular loading through `GET /api/localization?culture=en|ar`.
- Legacy Razor views still contain older custom inline tags such as `<english>` and `<arabic class="d-none">...`; examples include `Views/Account/*.cshtml` and many feature pages. These should be migrated gradually as Razor pages are retired or localized.

## Current Feature Flow

- IFC upload starts in `Controllers/UploaderController.cs`.
  - `Index(IFormFile file)` saves the uploaded IFC.
  - `SplitByBuildingStorey(...)` splits the IFC by building storey/floor.
  - `ConvertToWexBIM(...)` creates `.wexBIM` files.
  - The controller directly constructs `APIs.Controllers.IFCController` and calls `AddBuildingDataFromIFC(...)` to persist extracted data.
- IFC/domain extraction is in `APIs/Controllers/IFCController.cs`.
  - `AddBuildingDataFromIFC(...)` opens the IFC with `IfcStore`, extracts building, floors, spaces, assets, sensors, and stores floor `.wexBIM` paths.
  - DTO routes include building, floor, space, asset, sensor, task, and database-clear endpoints.
- Model viewing is in `Views/Viewer/*.cshtml`.
  - `ViewerScriptsPartial.cshtml` loads XBIM viewer scripts.
  - `ViewerToggleFloorsPartial.cshtml` loads each floor `.wexBIM` path into `xViewer` and toggles visibility by calling `viewer.setState(...)`.
  - `ViewerCanvasPartial.cshtml` and role-specific viewer pages configure `xViewer`, load models, handle pick events, zoom, hide products/types, and highlight selected elements.
- Element selection is handled in inline `viewer.on('pick', ...)` JavaScript, which stores `pickedId`, sets camera target, highlights selected products, and updates page text/inputs.
- Issue assignment is handled by `Controllers/IncidentController.cs`.
  - `EditOrAdd(...)` creates/updates incidents and links them to an asset or space.
  - Severity is stored as `Incident.Priority`.
- Worker assignment is handled by `Controllers/TaskPagesController.cs`.
  - `CreateTask(...)` creates a task from an incident.
  - `Assign(...)` assigns a task to an agent and moves it to `WorkInProgress`.
- Severity flashing/highlighting is driven by `Controllers/ViewerController.cs` plus inline viewer JavaScript.
  - `fillViewParameterForAgents(...)` groups active assigned task targets into `Severe`, `High`, `Medium`, `Low`, and `Rooms` lists.
  - Viewer pages define XBIM styles: severe red, high orange, medium yellow, low green.
  - Toggle switches call functions such as `ToggleHighLightItems`, `ToggleHighLightSevere`, `ToggleHighLightHigh`, `ToggleHighLightMedium`, `ToggleHighLightLow`, and `ToggleHighLightRooms`.

## Completed API Refactor Work

- Added an additive ASP.NET Core Web API layer for Angular-facing workflows while keeping the existing MVC/Razor pages intact.
- Added request/response DTOs under `Facility Management CEI/API/Dtos/` so the API does not expose EF entities directly.
- Added API controllers under `Facility Management CEI/API/Controllers/`:
  - `ModelsController`: upload IFC files, list models, retrieve model metadata, floors, elements, floor elements, and viewer highlight data.
  - `ElementsController`: retrieve element details, retrieve element issues, and create element-linked issues.
  - `IssuesController`: update/delete issues, assign issues to workers, and expose severity/status lookup values.
  - `WorkersController`: retrieve assignable workers/agents.
- Extracted IFC upload validation, storage, floor splitting, and `.wexBIM` conversion into `IIfcProcessingService` / `IfcProcessingService`.
- Rewired the existing `UploaderController` to use the new IFC processing service so MVC upload behavior and API upload behavior share the same processing path.
- Added upload configuration through `IfcProcessing` settings in `appsettings.json`.
- Added upload validation for missing files, empty files, file extension, and max file size.
- Added Swagger/OpenAPI registration for development.
- Added a minimal xUnit test project with focused upload-validation tests.
- Added ASP.NET Core localization with supported cultures `en` and `ar`.
- Added `SharedResource` `.resx` files for server-side and Angular-facing translation keys.
- Added `LocalizationController` for Angular runtime translations and document direction metadata.
- Updated the Angular shell and BIM issue-management components to consume translation keys instead of hardcoded UI strings.
- Replaced the active shared sidebar/topbar language-switch dependency on custom tags with ASP.NET Core request culture handling.

## Current API Surface

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
- `GET /api/localization?culture=en|ar`

Floor visibility remains frontend-only because no existing persisted floor visibility state was found.

## Target Production Architecture

- Keep the existing ASP.NET Core backend, but separate it into a production API surface and shared application services.
- Introduce Angular as a separate frontend app, preferably in `ClientApp/` or `frontend/`, consuming versioned API endpoints.
- Keep XBIM viewer behavior initially, but move viewer state, floor loading, element selection, severity highlighting, and issue/task UI into Angular services/components.
- Extract IFC upload/conversion/splitting into backend services instead of invoking controllers from controllers.
- Use a single EF Core context intentionally, or split Identity and domain contexts cleanly with explicit migrations.
- Keep `.resx` files as the backend/server source of truth for localized strings and expose translation keys to Angular through the localization API.
- Preserve current behaviors during migration: upload, floor splitting, viewer display, element selection, issue creation, worker assignment, floor visibility toggles, and severity highlighting.

## Migration Steps

1. Baseline the current app with smoke tests and screenshots for upload, viewer, incident, task assignment, and floor toggle behavior.
2. Completed: create backend service classes for IFC upload, floor splitting, WexBIM conversion, and keep existing MVC routes calling these services.
3. Completed: add API controllers for models, floors, elements, issues, workers, viewer highlight metadata, and IFC upload.
4. Introduce Angular in a separate folder without replacing Razor pages yet.
5. Build Angular API clients and authentication/session handling against the new API endpoints.
6. Migrate the viewer screen first as an Angular route/component because it is the highest-value interactive surface.
7. Move floor visibility, selection, severity highlighting, and flashing toggles into Angular viewer services while reusing existing `.wexBIM` files.
8. Migrate issue creation/editing and task/worker assignment into Angular forms.
9. Completed: replace current Angular localization with structured translation keys and Arabic/English `.resx` resource files.
10. Retire Razor pages gradually only after matching Angular behavior is verified.
11. Continue removing remaining legacy Razor inline translation tags view-by-view.

## Risks

- IFC processing is stateful and file-path sensitive; changing upload paths or `.wexBIM` URL generation can break the viewer.
- Viewer code is duplicated across role-specific Razor views and partials, so behavior may drift during extraction.
- `ViewBag` currently passes critical runtime data into inline JavaScript; Angular needs explicit API contracts for the same data.
- EF context duplication can lead to migration confusion if both contexts are used accidentally.
- Some API-like methods are destructive, such as `ClearDatabase`; these should not be exposed casually in production.
- Remaining inline localization tags are spread across legacy Razor views and JavaScript strings, so Razor cleanup should be incremental.
- Current code targets .NET 5, which is out of support; framework upgrade should be planned separately from the Angular migration.

## Files Likely To Change

- `Startup.cs`, `Program.cs`, `appsettings.json`
- `Facility Management CEI.csproj`
- `Controllers/UploaderController.cs`
- `Controllers/ViewerController.cs`
- `Controllers/IncidentController.cs`
- `Controllers/TaskPagesController.cs`
- `APIs/Controllers/IFCController.cs`
- `IdentityDb/ApplicationDBContext.cs`
- `APIs/Models/*.cs`, `APIs/Moldels DTOs/*.cs`, `APIs/Enums/*.cs`
- `Views/Viewer/*.cshtml`
- `Views/_Shared/_sidebar.cshtml`, `Views/_Shared/_topbar.cshtml`, other views containing `<english>`/`<arabic>`
- `Resources/SharedResource.resx`, `Resources/SharedResource.ar.resx`
- `API/Controllers/LocalizationController.cs`
- `frontend/src/app/core/services/localization.service.ts`
- `frontend/src/app/core/pipes/translate.pipe.ts`
- `wwwroot/Scripts/XBIM/**`
- `wwwroot/assets/lang/**`
- New Angular app folder: `ClientApp/` or `frontend/`
- New backend folders: `Services/`, `Dtos/`, `APIs/Controllers/V1/`

## Testing Checklist

- Upload a valid IFC file and verify it is stored.
- Verify floors are split correctly and each generated `.wexBIM` file is served.
- Verify building, floor, space, asset, and sensor records are created.
- Open the viewer and confirm all floor models load.
- Toggle each floor on/off and confirm only that floor changes visibility.
- Select an element and confirm the selected id is captured and highlighted.
- Create an incident for an asset and for a space.
- Assign a task to an agent and verify task status changes.
- Verify severe/high/medium/low issue colors match existing behavior.
- Toggle flashing/highlighting off and on.
- Verify role-specific viewer data for agent, inspector, supervisor, manager, and owner.
- Verify Arabic and English UI after localization migration.
- Verify `GET /api/localization?culture=en` and `GET /api/localization?culture=ar` return the same keys.
- Verify Angular sets `dir="rtl"` for Arabic and `dir="ltr"` for English.

## Recommended Implementation Order

1. Add tests/smoke scripts and document current behavior.
2. Extract backend services behind existing MVC pages.
3. Add stable API endpoints without changing the UI.
4. Add Angular shell and API clients.
5. Migrate viewer read-only display and floor toggles.
6. Migrate element selection and severity highlighting.
7. Migrate incident and task assignment workflows.
8. Completed: migrate Angular/server localization.
9. Remove remaining legacy Razor inline translation tags view-by-view.
10. Remove duplicated Razor viewer code after Angular parity is confirmed.
11. Plan .NET/framework/package upgrades as a separate hardening phase.
