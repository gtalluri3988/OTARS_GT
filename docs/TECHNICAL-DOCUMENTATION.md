# MTA OARS General – Technical Documentation

## 1. System overview

MTA OARS General is a .NET Framework ASP.NET MVC application using a multi-project layered architecture. The web application is the presentation and HTTP entry point. Business operations are implemented in services, builders, mappers, validators, and repositories over an Entity Framework database model.

```text
Browser
  ↓
ASP.NET MVC controllers and Razor views (MTAoarsGeneral.Web)
  ↓
Services → Builders / Mappers / Validators
  ↓
Repositories
  ↓
Entity Framework model (Entities.edmx)
  ↓
SQL Server database (configured catalog: MTAOARS)
```

## 2. Solution projects

| Project | Responsibility | Target framework in project file |
|---|---|---|
| `MTAoarsGeneral.Web` | MVC controllers, Razor views, startup, configuration, reports, WCF service host | .NET Framework 4.6.1 |
| `MTAoarsGeneral.DomainModels` | EF database-first model and generated entity/context classes | .NET Framework 4.0 |
| `MTAoarsGeneral.ViewModels` | MVC input/output models and screen contracts | .NET Framework 4.0 |
| `MTAoarsGeneral.Services` | Application/business workflows and service interfaces | .NET Framework 4.0 |
| `MTAoarsGeneral.Builders` | View-model construction and workflow preparation | .NET Framework 4.0 |
| `MTAoarsGeneral.Mappers` | Entity/view-model transformations | .NET Framework 4.0 |
| `MTAoarsGeneral.Repositories` | Data access, queries, stored-procedure/report access, generic repository base | .NET Framework 4.0 |
| `MTAoarsGeneral.Validators` | FluentValidation and business validation providers | .NET Framework 4.0 |
| `MTAoarsGeneral.Utilities` | IoC, MVC binders, authorization, encryption, Excel/CSV, constants, extensions | .NET Framework 4.0 |
| `MTAoarsGeneral.Schedulers` | Executable scheduled/background processing support | .NET Framework 4.0 |
| `MTAoarsGeneral.Shell` | Shared shell/application integration library | .NET Framework 4.6.1 |
| `MTAoarsGeneral.Tests` | Repository and supporting tests | .NET Framework 4.0 |

## 3. Web application runtime

### Startup and dependency injection

`Global.asax.cs` performs the following during application startup:

- Registers MVC routes using `{controller}/{action}/{id}` with Home/Index as the default.
- Loads service registrations through `ServiceLoader.Load()`.
- Registers controllers, model binders, the Unity dependency resolver, and the Razor view engine.
- Configures FluentValidation to resolve validators from the Unity container.
- Replaces the default MVC filter provider with a Unity-aware provider.
- Registers a global no-cache filter.

### Request handling and security behavior

The application applies no-cache/no-store response headers at request start and disables the MVC response header. Additional security headers are present in `Web.config`, including `X-Content-Type-Options`, `X-Frame-Options`, `X-XSS-Protection`, and HSTS.

Most operational controllers use `MTAoarsGeneralAuthorizeAttribute`. The attribute checks that an identity is present in scoped session data and verifies the requested controller/action URL against the identity’s permitted menu tree. Failure redirects to `Account/Unauthorized`.

### Model binding

Custom binders handle the default model, dates, decimals, long values, JSON, and Excel lists. This centralizes parsing and allows MVC actions to receive strongly typed business models.

## 4. Core application layers

### Controllers and views

Controllers are grouped by business area: Account, Registration, Agency, Maintenance, Renewal, Termination, Referred, Conflict, Invoice, Mail, Setup, Upload, Enquiry, CPD, CBC, ALC, Administrative, and Report. Razor views under `MTAoarsGeneral.Web/Views` implement the browser UI.

Controllers generally coordinate requests, call one or more services/builders, and return views or JSON responses. They are not intended to contain the complete business rule set.

### Services

Services provide application workflows such as registration, agency operations, renewal, termination, uploads, invoices, conflict handling, CBC, CPD, ALC, administration, notifications, mail, and reports. `BaseService` and `ServiceContext` provide shared service context behavior.

### Builders and mappers

Builders prepare new and edit view models, populate lookups, and assemble workflow-specific data. Mappers translate between generated EF entities and MVC view models. Registration has dedicated builders/creators for Individual, Sole Proprietorship, Partnership, and Corporate flows.

### Validation

Validation is split between FluentValidation screen validators and service/business validators. Registration validators cover intermediary-type selection, duplicates, BRN, TBE, member/company rules, referrals, conflicts, photos, partners, and other eligibility checks. Validation results are returned through service context/JSON responses or MVC model-state errors.

### Repositories and persistence

Repositories use a generic repository base over Entity Framework `EntityObject` entities. The database-first model is in `MTAoarsGeneral.DomainModels/Entities.edmx`, with generated context/entity code in the same project. Repository groups include accounts, operations, maintenance, notifications, shared lookups, and report repositories.

## 5. Data model

The EF model contains core business entities including `Company`, `User`, `UserLogin`, `Agency`, `AgencyPrincipal`, `AgencyPrincipalHistory`, `Member`, `AgencyMember`, `RenewalHeader`, `RenewalDetail`, `ReferredHeader`, `ReferredDetail`, `ReferredMember`, `AgencyPrincipalConflict`, `Invoice`, `TrainingDetail`, `CBCHeader`, `CBCDetail`, `CPDHeader`, `CPDDetail`, `ALCHeader`, `ALCMember`, `UploadHistory`, `RegistrationUploadHistory`, `Mail`, `Notification`, audit entities, and lookup entities.

The model also exposes database views/results used by reports and enquiries, including agency/member, renewal, invoice, training, CBC, complaint, conflict, CPD, and termination projections.

## 6. General/Family type handling

`Identity.IsGeneral` and `Identity.IsFamily` are the runtime capability flags. They originate from the company associated with the logged-in user. `GEN` and `FAM` lookup codes identify intermediary types.

Repositories and reports resolve the relevant lookup ID and filter operational records accordingly. The important consistency rule is that registration, referral, conflict, renewal, agency, and report queries must use the same type context. Detailed impact analysis is in [PLAN-General-Family-Split.md](PLAN-General-Family-Split.md).

## 7. Reporting and document generation

The web project contains RDLC report definitions under `MTAoarsGeneral.Web/Rdls`. Report controllers create report-specific view models, while report repositories retrieve the report data. The configured ReportViewer HTTP handler is used to render/report the RDLC artifacts.

## 8. Integration points

- SQL Server through an Entity Framework EntityClient connection string.
- `MasterDataService.svc`, a WCF service hosted by the web project.
- SMTP/mail configuration and mail/notification persistence, subject to deployment configuration.
- Excel and CSV file processing for bulk registration, IBFIM, termination, renewal, and related uploads.
- File-system paths for uploaded/raw/success/error files, supplied through application configuration/services.

## 9. Configuration and deployment

The checked-in web configuration currently targets .NET Framework 4.6.1 and contains an Entity Framework connection named `EntityContext` pointing to SQL Server host `localhost` and catalog `MTAOARS` with integrated security. Production deployment must override this value through the approved environment configuration process.

The web application is intended to run under IIS/ASP.NET MVC. The solution also contains a separate scheduler executable. Deployment should therefore provision:

- Compatible .NET Framework runtime and ASP.NET/IIS features.
- SQL Server database and the EF model’s expected schema/stored procedures/views.
- Writable and readable upload/report directories.
- SMTP/mail settings if mail workflows are enabled.
- NLog configuration and log storage.
- Scheduler execution/hosting and its database configuration, if scheduled jobs are enabled.

## 10. Logging, errors, and caching

NLog is configured through `NLog.config`. Unhandled application exceptions are written through the application logger. MVC errors are represented by shared error views. Response caching is intentionally disabled globally and at request level because the application handles authenticated and sensitive operational data.

## 11. Testing and verification

The test project includes repository-focused tests and database test helpers such as `FakeDbSet` and `TestObjectContext`. Before release, add/confirm coverage for:

- Login, authorization, and menu restrictions.
- General-only, Family-only, and dual-capability users.
- Registration validation and duplicate scenarios.
- File upload success/error paths.
- Renewal, termination, reinstatement, and conflict state transitions.
- Report filters and data-type partitioning.
- Configuration against a non-local SQL Server environment.

## 12. Known technical considerations

- The solution mixes .NET Framework 4.0 class libraries with .NET Framework 4.6.1 web/shell projects.
- The EF model is database-first and generated code should not be edited manually.
- Several dependencies are legacy versions, including older MVC/UI and Unity-era packages; upgrades require regression testing across MVC binding, validation, reporting, and authentication.
- The checked-in connection string uses local SQL Server assumptions and should not be treated as production configuration.
- Exact scheduled job names, execution frequency, production infrastructure, and external WCF consumers require confirmation from deployment/runbook sources not present in the repository.
