# CLAUDE.md — MTA OARS General

Guidance for Claude Code when working in this repository.

## What this system is

MTA OARS General is a legacy ASP.NET MVC 3 / .NET Framework web application for administering
Takaful intermediaries (agents/agencies) for **General** business: registration, maintenance,
renewal, termination/reinstatement, referrals, conflicts, invoices, CBC/CPD/ALC/TBE, mail,
master-data setup, bulk uploads and RDLC reports.

It is the **General** half of a split. Sibling solutions live next to this folder
(`..\MTAoarsFamily`, `..\MTAoars` = original combined app, `..\MTAORSEnquiry`). Projects and
namespaces mirror each other (`MTAoarsGeneral.*` vs `MTAoarsFamily.*`). When a fix is made here,
ask whether the same fix is needed in `MTAoarsFamily` — do not edit the sibling unless asked.

Read these before non-trivial work:
- `docs/TECHNICAL-DOCUMENTATION.md` — architecture, layers, runtime, deployment
- `docs/FUNCTIONAL-DOCUMENTATION.md` — modules and business lifecycle
- `docs/REGISTRATION-VALIDATION-RULES.md` — every registration rule + exact user message
- `docs/PLAN-General-Family-Split.md` — where General/Family is decided, filtered and stored

## Solution layout (`MTAoarsGeneral.sln`)

| Project | Role | Framework |
|---|---|---|
| `MTAoarsGeneral.Web` | **The real web app**: controllers, Razor views, `Global.asax`, OWIN `Startup` (OpenID Connect SSO), `Rdls/*.rdlc`, `MasterDataService.svc` (WCF), `NLog.config` | 4.6.1 |
| `MTAoarsGeneral.Shell` | Older/partial MVC host with a subset of controllers. Not the primary app — don't add features here unless asked | 4.6.1 |
| `MTAoarsGeneral.Services` | Business workflows; `ServiceLoader.cs` = all Unity DI registration | 4.0 |
| `MTAoarsGeneral.Builders` | Build view models for screens; registration creators per agency type | 4.0 |
| `MTAoarsGeneral.Mappers` | AutoMapper 2.2 profiles (`IMapper.Map()`), resolvers | 4.0 |
| `MTAoarsGeneral.Validators` | Business validators (`IValidator<T>`) + FluentValidation screen validators (`Operations/Fluents`) | 4.0 |
| `MTAoarsGeneral.Repositories` | EF ObjectContext data access, unit-of-work classes, report repositories (stored procs) | 4.0 |
| `MTAoarsGeneral.DomainModels` | **Database-first** EF model `Entities.edmx` + generated `Entities.Designer.cs` | 4.0 |
| `MTAoarsGeneral.ViewModels` | Screen input/output models | 4.0 |
| `MTAoarsGeneral.Utilities` | IoC (`ObjectContainer`), MVC binders, `Identity`, `MTAoarsGeneralAuthorizeAttribute`, constants, Excel/CSV, extensions | 4.0 |
| `MTAoarsGeneral.Schedulers` | Console exe; `Program.cs` switches on `args[0]` (`LookupConstants.Schedulers.*`: invoice generation, renewal generate/remind/process, CPD, CBC, conflict reminders/auto-close) | 4.0 |
| `MTAoarsGeneral.Tests` | NUnit 2.6.4 + Moq; currently only `ReferredRepository` tests | 4.0 |

Key packages: MVC 3, Razor 1, EntityFramework 4.1 (ObjectContext/`EntityObject`, not DbContext),
Unity (Microsoft.Practices.Unity), AutoMapper 2.2, FluentValidation 3.4 (MVC3), NLog 4.5,
EPPlus 4.5, ReportViewer WebForms, OWIN 3.1, jQuery 3.6, Kendo UI. Do not upgrade packages
unless explicitly asked — binding, validation, reports and auth all depend on these versions.

## Request flow and conventions

```
Controller (Web) → Builder (read/prepare VM) / Service (workflow + validation)
                 → Validators via IValidationProvider → Repository / UnitOfWork → EntityContext (EF) → SQL Server
```

- **Controllers** inherit `BaseController`, are decorated `[MTAoarsGeneralAuthorize]`, take
  builder/service interfaces by constructor injection, and stay thin. AJAX actions return
  `Json(JsonResponseModel)`: first `GetResponse()` (ModelState/Fluent errors), then call the
  service, then `GetResponse(service.CurrentContext)` (business errors), set `RedirectUrl`/`Data`.
- **Services** inherit `BaseService`; call `Validate(model)` which runs every `IValidator<T>`
  registered for the model type (and its base types/interfaces) and pushes messages into
  `CurrentContext.ValidationMessages`. Check `CurrentContext.IsSuccess` instead of throwing.
- **Business validators**: implement `MTAoarsGeneral.Validators.IValidator<T>`, `yield return new
  ValidationMessage(key, message)`. They are discovered automatically by reflection in
  `ValidationProvider` — no registration needed. Get the user via
  `IScopeDataProvider.Get<Identity>(GlobalConstants.CurrentIdentity)`.
- **Fluent validators**: inherit FluentValidation `AbstractValidator<TViewModel>` and add
  `[FluentRegisterable]` so `ServiceLoader.RegisterValidators` wires them for MVC model binding.
- **Mappers**: a class implementing `IMapper` with `Map()` calling `Mapper.CreateMap<,>()`;
  discovered automatically by `ServiceLoader.RegisterMaps`.
- **Repositories / services / builders**: add an interface under `Interfaces/` and register it
  in `ServiceLoader.cs` (repositories, services) — builders are also registered there. Property
  injection uses `[Dependency]`.
- Generic CRUD: `IRepository<T>` → `GenericRepository<T>` (`T : EntityObject, IEntity`).
- Lookup codes live in `Utilities/Constants/LookupConstants.cs`; compare by `.Code`, never by
  hard-coded IDs.
- Code style: braces on the same line in most library code, `}// class` / `}// namespace`
  trailing comments, `logger` = NLog static per class. Match the surrounding file.

## General / Family rules (most common source of bugs)

- `Identity.IsGeneral` / `Identity.IsFamily` come from the logged-in user's `Company` (set in
  `Mappers/Account/UserMapper.cs`). `Identity.IsISMOrMTA` users are not restricted.
- Rows on `AgencyPrincipal(+History/Archive/Conflict)`, `RenewalDetail`, `TrainingDetail`,
  `CBCDetail`, `CPDDetail`, `Complaint`, `Journal` carry `IntermediaryTypeID`
  → `LookupIntermediaryType` codes `GEN` / `FAM` (`LookupConstants.IntermediaryType.General/Family`).
- `ReferredDetail` / `ReferredMember` / `Company` carry `IsGeneral` + `IsFamily` booleans.
- Every query in registration, referral, conflict, renewal, agency search and reports must use the
  same type context. When touching any of these, check both patterns and the ISM/MTA bypass.

## Database / EF

- Database-first. **Never hand-edit `Entities.Designer.cs`** or the edmx XML for schema changes;
  schema changes are made in SQL Server and the model is updated from the database in Visual
  Studio. If a change needs new columns/procs, provide the SQL script and say the model must be
  refreshed.
- Connection string `EntityContext` in `MTAoarsGeneral.Web/Web.config` (and each `App.config`)
  points at `localhost`, catalog `MTAOARS_GEN`, integrated security. (The technical doc says
  `MTAOARS`; Web.config is authoritative.) Family DB script sample: `..\MTAOARS_FAM.sql`.
- Reports call stored procedures through `Repositories/Reports/*`; RDLC files in `Web/Rdls`.

## Building and testing (Windows only)

There is no .NET Core / `dotnet` build. Use Visual Studio 2017+ or MSBuild from a Developer prompt:

```powershell
nuget restore MTAoarsGeneral.sln          # or .nuget\NuGet.exe restore
msbuild MTAoarsGeneral.sln /p:Configuration=Debug
vstest.console.exe MTAoarsGeneral.Tests\bin\Debug\MTAoarsGeneral.Tests.dll   # or nunit-console 2.6.4
```

Run the web app under IIS Express from Visual Studio (`MTAoarsGeneral.Web`, default
`http://localhost:52077/`). Login goes through an external SSO (`Authority`/`SSOUrl` app
settings), so end-to-end UI runs need that SSO server and a local SQL Server database.
If you cannot build or run, say so plainly rather than claiming a change works.

## Configuration gotchas

- `UploadBaseDirectory` in Web.config is a developer's absolute path — must be overridden per
  environment. Upload sub-folders (raw/success/error/photo) hang off it.
- Conflict reminder/auto-close durations and date formats (`dd/MM/yyyy`) are app settings.
- `Schedulers/Program.cs` resolves a hard-coded service identity by e-mail before running jobs.
- Response caching is deliberately disabled (global `NoCacheAttribute` + `Application_BeginRequest`);
  keep it that way — pages contain personal data (IC numbers, addresses).

## Working rules for Claude

1. Locate the full vertical slice before editing: controller → builder/service → validator(s) →
   mapper → repository → view (`Web/Views/<Controller>/*.cshtml`) → script under `Web/Scripts`.
2. Keep business rules out of controllers and views; put them in validators/services.
3. New user-facing validation messages: add them to `docs/REGISTRATION-VALIDATION-RULES.md`
   when they concern registration.
4. Respect General/Family scoping and `IsISMOrMTA` on every new query.
5. Stay on C# features supported by the projects' compilers and .NET 4.0 for class libraries
   (no `async/await` in 4.0 libraries, no newer BCL APIs).
6. Don't log or echo personal data beyond what existing NLog statements already log.
7. Don't touch `packages/`, `lib/`, `bin/`, `obj/`, `.vs/`, `ThirdPartyDLLs/`.
8. Mention whether the same change likely applies to `..\MTAoarsFamily`.
