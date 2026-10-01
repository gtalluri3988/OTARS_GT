# Registration Validations (Rules + Messages + Logic)

This document captures the **existing** registration validation rules in the codebase, including:

- **Rule**: what is validated
- **Validation message**: the user-facing message (if explicitly set)
- **Basic logic**: why the validation exists / how it is checked

---


## Common validations (apply across registration types)

### 1) Start step (`RegistrationStartViewModelValidator`)

- **Company**: must be selected  
  - **Message**: `Company is required`  
  - **Logic**: registration must be tied to a TO/company.

- **Registration Type (AgencyType)**: must be selected  
  - **Message**: `Please select registration type`  
  - **Logic**: decides which registration flow to start (Individual / Sole Proprietorship / Partnership / Corporate).

---

### 2) Index step (General/Family + basic info)

#### 2.1 General/Family selection (`IntermediaryTypeSelectionValidator`)

- **At least one intermediary type must be selected**  
  - **Message**: `Please select intermediary type`  
  - **Logic**: registration must be scoped to at least one intermediary type.

- **TO users cannot select a type their company does not allow** (ISM/MTA bypass)  
  - If `!Identity.IsFamily` and user selected Family:  
    - **Message**: `Your company not allow to register Family agent.`  
    - **Logic**: the logged-in company does not support Family.
  - If `!Identity.IsGeneral` and user selected General:  
    - **Message**: `Your company not allow to register General agent.`  
    - **Logic**: the logged-in company does not support General.

#### 2.2 Index mandatory fields + formats (`RegistrationIndexViewModelValidator`)

- **IC Type** required  
  - **Message**: `IC Type is required`
  - **Logic**: drives IC validation rules and lookup behavior.

- **TBE Category** required  
  - **Message**: `Takaful Entery Exam is required`
  - **Logic**: determines exam/exemption validation checks.

- **M2 Exam** required  
  - **Message**: `M2 Exam is required`
  - **Logic**: M2 is required unless exempted (rules below).

- **Date of Exam** required when M2 is not exempted  
  - **Message**: `Date of Exam is required`
  - **Logic**: if a candidate is not exempted, the exam date is needed.

- **Level/Rank** required when Family is selected  
  - **Message**: *(default FluentValidation message; no explicit message set in code)*  
  - **Logic**: Family registration requires rank/level selection.

- **New IC format** when `ICType == NewIc`  
  - **Message**: `New IC number should be 12 digits and begin with date of birth (yyMMdd) format`  
  - **Logic**: new IC must be 12 digits and start with DOB pattern.

> Note: several rules in FluentValidation do not specify `.WithMessage(...)`, so the UI will show default FluentValidation messages for those.

---

## Common business validations (eligibility / duplicates / gating)

These are service-layer validators executed during `RegistrationService.Check(RegistrationIndexViewModel model)` and/or during main form submit/confirm checks.

### 3) Duplicate under same company (`MemberCompanyValidator`) — Index stage

- **General**: block if already exists under General in same company  
  - **Message**: `The member is already exists under General intermediary type in the same company`
  - **Logic**: prevents duplicate principals under the same company/type.

- **Family**: block if already exists under Family in same company  
  - **Message**: `The member is already exists under Family intermediary type in the same company`
  - **Logic**: prevents duplicate principals under the same company/type.

- **Allowed exception (Family)**: Individual Family agent in same TO joining as Corporate Family agent is allowed (validator yields no error in that scenario).

---

### 4) Intermediary duplication + multi-company rules (`MemberIntermediaryValidator`) — Index stage

This validator enforces special constraints such as:

- **Family**: block if already registered under Family (subject to conflict/not-released logic)  
  - **Message**: `The member with IC number "{0}" is already registered under Family with {1}`

- **General**: block if already registered under General with multiple companies  
  - **Message**: `The member with IC number "{0}" is already registered under General with the following companies {1}`

- **General (same company)**: block with agency type context  
  - **Message**: `The member with IC number "{0}" is already registered under General with the following company {1} under {2} Agency type`

- **Banca staff**: mismatch between individual/corporate status  
  - **Message**: `The Banca member with IC number "{0}" is already registered under {1} with {2}`

---

### 5) Banca vs Non-Banca mismatch (`BancaStaffValidator`) — Index stage

- If existing principal has `IsBancaStaff` opposite to the registration’s banca flag (checked separately for Family and General):  
  - **Message**: `This Agent "{0}" is currently registered as an {1} as {2}. Agency Number "{3}".`
  - **Logic**: an agent active as Banca cannot register as Non-Banca (and vice versa).

---

### 6) Business Registration Number (BRN) checks (`BusinessRegistrationNumberValidator`) — Index stage (non-Individual only)

- **Skip for Individual**

- Block if BRN already registered under same company + same intermediary type + same agency type  
  - **Message**: `Business registration number {0} already registered under {1}`
  - **Logic**: prevents registering the same business again for the same scope.

- Block if BRN’s corporate nominee IC doesn’t match the index IC number  
  - **Message**: `Business registration number {0} is not matching with the IC number`
  - **Logic**: ensures the nominee identity matches the business record.

---

### 7) Referred listing block (`ReferredMemberValidator`) — Index + member validation

- If member is in referred listing for the selected intermediary type(s):  
  - **Message (per record)**: `This agent appears in the referred listing under ({0}) on {1}`
  - **Logic**: blocks/flags registrations based on referred categories and dates.

---

### 8) Conflict gating (`MemberConflictValidator`)

- If the member exists in conflict and cannot be processed:  
  - **Message**: `The member is already in conflict and cannot be processed now.`
  - **Logic**: prevents further processing while conflict is active.

---

### 9) Agency status gating (config-driven) (`AgencyStatusValidator`)

- Runs only if `config.CPDCheckingAgainstRegistration == true`.
- If there is an active agency status covering today:  
  - **Message**: `This Agent / Corporate Nominee  "{0}" is currently in the status {1}`
  - **Logic**: blocks registrations under certain active statuses.

---

### 10) TBE (exam / exemption) checks (`MemberTBEValidator`)

Applies when `item.IsOld == false` and validates depending on `TbeCategory.Code`:

- **IBFIM**  
  - Family requires PASS in parts **A + C**  
  - General requires PASS in parts **A + B**  
  - **Message**: `The candidate "{0}" either did not passed or sat for the IBFIM exam in order to register`

- **MII**  
  - Must have at least one grade in **A/B/C**  
  - **Message**: `The candidate "{0}" either did not passed or sat for the MII exam in order to register`

- **Exemptions** (FPAM / MFPC / Register2008OrBelow / SPECIAL / TBEGE / EXP)  
  - Must exist in exemption listing for the selected exemption type, else:  
    - **Message**: `The IC number {0} is not exempted from Takaful Exam` *(or the specific variant used in code)*  
  - Must match Family/General exemption-for selection, else:  
    - **Message**: `This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption`

---

### 11) Reinstate routing rule (`ReinstateValidator`)

- If eligible for reinstatement in one type but user selected **both Family and General**:  
  - **Messages**:
    - `This is eligible for reinstation on Family. Reinstate should happen separately for Family and General`
    - `This is eligible for reinstation on General. Reinstate should happen separately for Family and General`
  - **Logic**: reinstatement is processed separately per intermediary type.

---

## Validations by registration type (main form stage)

### A) Individual (`IndividualRegistrationViewModelValidator`)

- **Address** (`AddressViewModelValidator`)
  - Address1: NotNull + length 1–75 *(default message)*
  - Address2: length 0–75 *(default message)*
  - City: NotNull + length 1–50 *(default message)*
  - PostalCode: NotNull + length 1–6 *(default message)*
  - State: NotNull *(default message)*

- **CorporateNominee.Phone** required  
  - **Message**: *(default message)*

Also includes the shared validations below via model properties:
- `CorporateNomineeViewModelValidator`
- `QualificationViewModelValidator` (via nominee qualification)
- `GuarantorViewModelValidator` (depending on flow)

---

### B) Sole Proprietorship (`SoleProprietorshipRegistrationViewModelValidator`)

Inherits corporate-base validator rules (`CorporateRegistrationViewModelValidator<T>`):

- **Address**: same rules as above
- **Agency** (`AgencyViewModelValidator`)
  - Name required (1–50) *(default message)*
  - BusinessRegistrationNumber required (1–15) *(default message)*
  - If `ExcludeM2ExamValidation == false`:
    - M2Exam required *(default message)*
    - DateOfExam required when `M2Exam.Code.ToUpper() != "EXAMPTED"` *(default message)*
- **CorporateNominee.Phone** required *(default message)*

---

### C) Partnership (`PartnershipRegistrationViewModelValidator` + `PartnersValidator`)

Corporate-base rules plus:

- **At least one partner**  
  - **Message**: `Atleast one partner is required`

- **Corporate nominee is auto-added as partner** (prevent duplicates)  
  - **Message**: `Corporate nominee will be added automatically added as a partner, please delete the partner with IC number {0}`

- **No duplicate partners** by IC number  
  - **Message**: `Partner with IC Number {0} appears {1} times. Remove all except one`

---

### D) Corporate (`AgencyBoardMembersValidator<CorporateRegistrationViewModel>` + `BoardMembersValidator`)

Corporate-base rules plus:

- **At least one director**  
  - **Message**: `Atleast one director is required`

- **At least one shareholder**  
  - **Message**: `Atleast one shareholder is required`

- **AuthorizedCapital** required *(default message)*
- **PaidupCapital** required *(default message)*
- **AuthorizedCapital > PaidupCapital**  
  - **Message**: `Authorized capital should be greater than paid up capital`
- **PaidupCapital >= 50000**  
  - **Message**: `Paid up capital should not be less than 50000`

Board members consistency rules (`BoardMembersValidator`) include:

- Missing IC number for Director/Shareholder/Additional Corporate Nominee  
  - **Message**: `IC number is required for {0}`
- Invalid New IC format for those roles  
  - **Message**: `{0}'s New IC number {1} should be 12 digits and begin with date of birth (yyMMdd) format`
- Role member and corporate nominee are in different company  
  - **Message**: `{0}'s New IC number {1} and Corporate nomine IC number are in different company`
- Duplicate in list  
  - **Message**: `{0} with IC Number {1} appears {2} times. Remove all except one`

---

## Shared child validators (used by multiple registration types)

### Address (`AddressViewModelValidator`)

- Address1 required, length 1–75
- Address2 length 0–75
- City required, length 1–50
- PostalCode required, length 1–6
- State required

*(mostly default FluentValidation messages)*

---

### Agency (`AgencyViewModelValidator`)

- Name required (1–50)
- BusinessRegistrationNumber required (1–15)
- If `ExcludeM2ExamValidation == false`:
  - M2Exam required
  - DateOfExam required when `M2Exam.Code.ToUpper() != "EXAMPTED"`

*(default FluentValidation messages; no explicit `.WithMessage(...)` here)*

---

### Corporate Nominee (`CorporateNomineeViewModelValidator`)

- Name required (1–75)
- BirthDate required
- Race / Religion / MaritalStatus / Gender required
- Email must match pattern  
  - **Message**: `Invalid email address`
- IC Type required  
  - **Message**: `IC Type is required`
- TBE Category required based on `IsOld` flag (as coded)
- New IC format if IC Type is New IC  
  - **Message**: `New IC number should be 12 digits and begin with date of birth (yyMMdd) format`

---

### Qualification (`QualificationViewModelValidator`)

- EducationalQualification required
- Year required
- SchoolName length 0–100

---

### Guarantor (`GuarantorViewModelValidator`)

- GuarantorType required
- If GuarantorType != “NoGuarantee”: Details, Amount, FromDate, ToDate required

---

## Photo upload rule (when applicable) (`PhotoValidator`)

- If `PhotoPath` is not marked `[IgnoreValidate]` and photo is not uploaded:  
  - **Message**: `Please upload photograph`

