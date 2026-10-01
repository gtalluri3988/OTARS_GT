# Plan: Database Tables and Usage Split by General vs Family

This document identifies all places where **General** and **Family** intermediary types are used, which database tables have flags, and how to use those tables based on user type.

---

## 1. Source of User Type (General / Family)

- **Identity** (session) gets `IsGeneral` and `IsFamily` from the **Company** (Takaful Operator) the user belongs to.
- **Set at login**: `MTAoars.Mappers\Account\UserMapper.cs` maps `User` → `Identity` using `src.Company.IsGeneral` and `src.Company.IsFamily`.
- **Identity class**: `MTAoars.Utilities\Mvc\Identity.cs` — properties `IsGeneral`, `IsFamily`.
- **Rule**: For TO users (not MTA/ISM), the logged-in company’s `Company.IsGeneral` / `Company.IsFamily` define what the user can see/do (General only, Family only, or both).

---

## 2. Database Tables with IsGeneral / IsFamily Flags

| Table            | Columns     | Purpose |
|------------------|------------|---------|
| **Company**      | IsGeneral, IsFamily | Whether the TO operates in General and/or Family. Drives Identity and filtering. |
| **ReferredDetail** | IsGeneral, IsFamily | Referral applies to General and/or Family. |
| **ReferredMember** | IsGeneral, IsFamily | Referred member record is scoped to General and/or Family. |

**Lookup:**

- **LookupIntermediaryType**: codes `GEN` (General), `FAM` (Family) — see `LookupConstants.IntermediaryType`.

---

## 3. Tables Linked by IntermediaryTypeID (No Boolean Flags)

These tables use **IntermediaryTypeID** (FK to LookupIntermediaryType) to indicate General vs Family per row:

| Table                         | Usage |
|-------------------------------|--------|
| **AgencyPrincipal**           | Each principal is either General or Family (one IntermediaryTypeID per row). |
| **AgencyPrincipalHistory**    | Historical principal records by intermediary type. |
| **AgencyPrincipalArchive**   | Archived principals by type. |
| **AgencyPrincipalHistoryArchive** | Archived history by type. |
| **AgencyPrincipalConflict**   | Conflict scoped to an intermediary type. |
| **RenewalDetail**            | Renewal per agency/company/intermediary type. |
| **TrainingDetail**           | Training per intermediary type. |
| **CBCDetail**                | CBC per intermediary type. |
| **CPDDetail**                | CPD per intermediary type. |
| **Complaint**                | Complaint per intermediary type. |
| **Journal**                  | Optional IntermediaryTypeID. |

**Usage rule:** Filter by `IntermediaryTypeID` = Family ID or General ID (from LookupIntermediaryType) when you need “General only” or “Family only” data.

---

## 4. Places That Use General/Family and How

### 4.1 Identity / User type

| Location | What it does |
|----------|----------------|
| `MTAoars.Mappers\Account\UserMapper.cs` | Sets `Identity.IsGeneral`, `Identity.IsFamily` from `User.Company`. |
| `MTAoars.Utilities\Mvc\Identity.cs` | Defines `IsGeneral`, `IsFamily`. |

### 4.2 Registration (Builders / Mappers / Validators)

| Location | What it does |
|----------|----------------|
| `RegistrationBuilder.cs` | `SetGeneralFamily(ref model)` sets view model `IsGeneral`/`IsFamily` from Identity. |
| `RegistrationBuilder.cs` | `GetIndexViewModel(companyId, agencyTypeId)` calls `SetGeneralFamily`. |
| `RegistrationMapper.cs` | Maps to/from Agency: IsGeneral/IsFamily derived from AgencyPrincipals’ IntermediaryType; creates principals for Family and/or General from view model flags. |
| `RegistrationMapper.cs` | `AssignAgencyPrincipal` adds principal(s) for Family and/or General from `source.Agency.IsFamily` / `source.Agency.IsGeneral`. |
| `IntermediaryTypeSelectionValidator.cs` | Ensures user cannot select Family if `!currentIdentity.IsFamily`, or General if `!currentIdentity.IsGeneral`. |
| `RegistrationIndexViewModelValidator.cs` | Level required when `IsFamily`. |
| `BusinessRegistrationNumberValidator.cs` | Uses `item.IsFamily` vs General to pick IntermediaryType for BR number check. |
| `MemberIntermediaryTypeValidator.cs` | Family/General rules for individual/corporate and same TO. |
| `ReinstateValidator.cs` | Reinstate allowed separately for Family and General; validates per type. |
| `MemberTBEValidator.cs` | TBE/exemption checks per `item.IsFamily` / `item.IsGeneral` (IsFamilyExempted / IsGeneralExempted). |

### 4.3 ViewModels with IsGeneral / IsFamily

| ViewModel | Purpose |
|-----------|---------|
| `RegistrationIndexViewModel` | Registration form: user selects General and/or Family (constrained by Identity). |
| `AgencyViewModel` / `AgencyDisplayViewModel` | Agency General/Family (from AgencyPrincipals). |
| `ShareholderViewModel` | Shareholder-level General/Family. |
| `ReferredMemberViewModel` | Referred member: which type(s) the referral applies to. |
| `ReferredDetailViewModel` | Referral detail: which type(s). |

### 4.4 Repositories (Data Access and Filtering)

| Location | What it does |
|----------|----------------|
| **AgencyRepository.cs** | `Search(..., isGeneral, isFamily)` filters AgencyPrincipal / AgencyPrincipalHistory by `Company.IsGeneral == isGeneral \|\| Company.IsFamily == isFamily`. |
| **AgencyRepository.cs** | Get agency by company/principal: filters by `(i.IsFamily && company.IsFamily) \|\| (i.IsGeneral && company.IsGeneral)`. |
| **AgencyRepository.cs** | Reinstate: `GetReinstateHistory(..., intermediaryTypeId)` — pass Family or General ID. |
| **AgentsReportRepository.cs** | For non-ISM/MTA: sets `intermediaryTypeID` from Identity (Family only, General only, or null for both). |
| **MemberRepository.cs** | `IsMemeberInReferredList`: filters ReferredMember by `company.IsFamily == p.IsFamily && company.IsGeneral == p.IsGeneral`. |
| **MemberRepository.cs** | `IsMemeberReasonNotAllowedForRegistrationByIntermediary(icNumber, isFamily, isGeneral)`: filters ReferredMember by `rm.IsFamily` or `rm.IsGeneral`. |
| **RenewalRepository.cs** | `GetInProcessDetail(agencyId, companyId, intermediaryTypeId)` — per intermediary type. |
| **ConflictRepository.cs** | Uses `IntermediaryTypeID` for principal lookup and `IsAgencyPrincipalExists(agencyId, companyId, intermediaryTypeId)`. |
| **Report repositories** (TerminatedAgents, RenewalStatistics, NewRegistrationStatistics, ActiveAgents) | Take `intermediaryType` (code) and resolve to `intermediaryTypeId` for stored procedures. |

### 4.5 UI (Views)

| Location | What it does |
|----------|----------------|
| `Registration\Index.cshtml` | `isGeneralOnly` / `isFamilyOnly` from Identity; controls visibility/options for General vs Family. |
| `Referred\Detail.cshtml` | Shows sections based on `Model.Identity.IsGeneral` and `Model.Identity.IsFamily`. |
| `Setup\ReferredMembers.cshtml` | Grid/editor for ReferredMember with IsGeneral / IsFamily. |
| `Referred\Display.cshtml` | Displays IsGeneral / IsFamily. |

### 4.6 Controllers / Excel

| Location | What it does |
|----------|----------------|
| `RegistrationController.cs` | Some actions set `IsGeneral = true` for specific flows. |
| `RegistrationExcelMapper.cs` | Maps IntermediaryTypeCode to view model IsFamily / IsGeneral. |

---

## 5. How to Use Tables Based on User Type

### 5.1 User type from Identity

- After login, use `identity.IsGeneral` and `identity.IsFamily` (from Company).
- **ISM/MTA** (`identity.IsISMOrMTA`): no General/Family restriction; can see both.
- **TO (Takaful Operator)**:
  - **General only:** `identity.IsGeneral && !identity.IsFamily` → restrict to General.
  - **Family only:** `identity.IsFamily && !identity.IsGeneral` → restrict to Family.
  - **Both:** `identity.IsGeneral && identity.IsFamily` → allow both; optional filter by IntermediaryTypeID in UI/reports.

### 5.2 Filtering by table type

**Tables with IsGeneral / IsFamily (Company, ReferredDetail, ReferredMember):**

- **Company:** Only used to derive Identity and to join for “company supports General/Family”. Filter other entities by `Company` then by type.
- **ReferredDetail / ReferredMember:** When checking referrals for the current user:
  - Restrict to rows where `(IsFamily == identity.IsFamily && IsGeneral == identity.IsGeneral)` or where the row’s flags overlap with the identity’s (e.g. for “referred for Family” use `ReferredMember.IsFamily` when `identity.IsFamily`).

**Tables with IntermediaryTypeID (AgencyPrincipal, RenewalDetail, etc.):**

- Resolve IDs once:  
  `familyId = LookupIntermediaryType.Code == "FAM"`,  
  `generalId = LookupIntermediaryType.Code == "GEN"`.
- **General-only user:** restrict to `IntermediaryTypeID == generalId` (and same Company).
- **Family-only user:** restrict to `IntermediaryTypeID == familyId` (and same Company).
- **Both:** either no filter on type or optional report/UI filter.

### 5.3 Validation rules

- User must not be able to select **Family** if `!identity.IsFamily`.
- User must not be able to select **General** if `!identity.IsGeneral`.
- Referred member / referral checks must use the same type (Family/General) as the registration or context (use ReferredMember/ReferredDetail IsFamily/IsGeneral or IntermediaryTypeID consistently).
- Reinstate: handle Family and General separately (one reinstate per type where applicable).

---

## 6. Summary: Quick Reference

| Need | Use |
|------|-----|
| **User type** | `Identity.IsGeneral`, `Identity.IsFamily` (from Company at login). |
| **Company supports General/Family** | Table **Company** (IsGeneral, IsFamily). |
| **Referrals by type** | **ReferredDetail**, **ReferredMember** (IsGeneral, IsFamily). |
| **Agency / principal by type** | **AgencyPrincipal**, **AgencyPrincipalHistory** (IntermediaryTypeID). |
| **Other operations by type** | RenewalDetail, TrainingDetail, CBCDetail, CPDDetail, Complaint, Journal (IntermediaryTypeID). |
| **Lookup IDs** | LookupIntermediaryType: GEN, FAM. |
| **Where to enforce type** | Registration validators, Agency search, Referred checks, Reports (Agents, Renewal, etc.), Reinstate. |

---

## 7. Plan: Two Tables (Family vs General) and Usage by User Type

### 7.1 Strategy

- For each entity that is today split by **IntermediaryTypeID** or **IsGeneral/IsFamily**, introduce **two physical tables**: one for Family, one for General.
- **Company** stays as-is (single table with IsGeneral/IsFamily) — it defines the TO and drives Identity.
- At runtime, **choose which table to use** (or whether to use both) based on **Identity.IsGeneral** and **Identity.IsFamily**.

### 7.2 Tables to Split into Family + General

| Current table | New tables | Notes |
|---------------|------------|--------|
| AgencyPrincipal | AgencyPrincipalFamily, AgencyPrincipalGeneral | Same columns except drop IntermediaryTypeID (type implied by table). |
| AgencyPrincipalHistory | AgencyPrincipalHistoryFamily, AgencyPrincipalHistoryGeneral | Same structure, no IntermediaryTypeID. |
| ReferredMember | ReferredMemberFamily, ReferredMemberGeneral | Same columns except drop IsGeneral/IsFamily. A member referred for both has one row in each table. |
| ReferredDetail | ReferredDetailFamily, ReferredDetailGeneral | Same columns except drop IsGeneral/IsFamily. |

**Optional (later phase):** AgencyPrincipalArchive, AgencyPrincipalHistoryArchive, AgencyPrincipalConflict, RenewalDetail, TrainingDetail, CBCDetail, CPDDetail, Complaint — same idea: one table per type, no IntermediaryTypeID column.

### 7.3 How to Use Based on User Type

| User type (Identity) | Tables to use |
|----------------------|----------------|
| General only (IsGeneral and not IsFamily) | Only General tables (e.g. AgencyPrincipalGeneral, ReferredMemberGeneral). |
| Family only (IsFamily and not IsGeneral) | Only Family tables. |
| Both (IsGeneral and IsFamily) | Query both tables where needed (union or separate lists); or keep a single current-type context per screen. |
| ISM/MTA | Can use both sets of tables (no restriction). |

### 7.4 Implementation Plan (High Level)

1. **Database** — Add new tables (Family/General) with same structure as current, minus type column. Migrate existing data: IntermediaryTypeID = Family to Family table; General to General table. For ReferredMember/ReferredDetail: IsFamily to ReferredMemberFamily; IsGeneral to ReferredMemberGeneral (one row can produce rows in both). Decide whether to keep or deprecate old tables after cutover.

2. **EF / Domain** — Add new entity types and DbSets (or ObjectSets) for the new tables. Keep or remove old entities from model depending on migration strategy.

3. **Repositories / Services** — Introduce a user-type-aware access layer: e.g. get current principals from AgencyPrincipalFamily or AgencyPrincipalGeneral (or both) based on Identity. Agency search, reinstate, referral checks, and reports should call into this layer so all reads/writes use the correct table(s).

4. **Registration and Writes** — On create/update: write to AgencyPrincipalFamily and/or AgencyPrincipalGeneral (and History tables) according to the registration chosen type(s). ReferredMember/ReferredDetail: insert into ReferredMemberFamily and/or ReferredMemberGeneral (and same for Detail) based on selected type(s).

5. **Validators and UI** — No change to rules: still restrict choice to Identity.IsGeneral / Identity.IsFamily. Only the persistence target (which table) changes.

6. **Reports and Stored procedures** — Reports that today take IntermediaryTypeID: update to run against the appropriate Family or General table (or both with a type parameter).

### 7.5 Summary

- **Two tables per concept:** one Family, one General; type is implied by table name, not a column.
- **Usage:** Always derive user type from Identity; query (and write) the corresponding table(s). Company remains the single source of whether the TO supports General/Family and feeds Identity at login.
- This plan is **design and approach only**; actual DDL, migration scripts, and code changes follow from this.

Use this plan to implement or refactor any “split” of data or behaviour by General and Family: keep Identity and Company as the source of user type, and apply the filters above consistently in repositories, validators, and UI.
