# MTA OARS General – Functional Documentation

## 1. Purpose and scope

MTA OARS General is a browser-based intermediary and agency administration system. It supports the lifecycle of agents/agencies for General and Family intermediary business, from registration through maintenance, renewal, termination, reinstatement, reporting, and related operational activities.

This document describes the behavior identifiable in the current source code. It is a baseline for business-owner validation; it is not a replacement for an approved requirements specification.

## 2. Users and access model

Users authenticate through the Account module. After login, the application keeps an `Identity` in session scope containing user, company, roles, menu permissions, and General/Family capability flags.

The principal user contexts are:

| User context | Functional access behavior |
|---|---|
| MTA | Broad operational access; not restricted by a TO’s General/Family flags. |
| ISM/ICI | Broad operational access; not restricted by a TO’s General/Family flags. |
| Takaful Operator user | Access is constrained by the company’s `IsGeneral` and `IsFamily` settings. |
| Administrator/role-based user | Menu/action access is controlled through configured roles and menus. |

The application uses both menu visibility and server-side authorization. Unauthorized requests are redirected to the Account/Unauthorized page.

## 3. Functional modules

### 3.1 Authentication and home

- Log on and log out users.
- Capture login activity for reporting.
- Display a role/company-specific home page and menu.
- Redirect unauthorized or unauthenticated users to an unauthorized page.
- Provide an error page and print-related home functionality.

### 3.2 Agency and agent enquiry

- Search agencies and agents.
- View agency details.
- Search TBE information.
- Search agents requiring photo upload.
- Provide enquiry-oriented views without exposing functions not permitted by the current identity.

### 3.3 New registration

Registration is a staged workflow:

1. Select the company and registration/agency type.
2. Select General and/or Family intermediary type, subject to the logged-in company’s capabilities.
3. Enter common eligibility information such as IC type, TBE category, M2 exam, exam date, and Family level/rank where applicable.
4. Run eligibility, duplicate, conflict, referral, BRN, TBE, and other business checks.
5. Complete the relevant registration form:
   - Individual
   - Sole proprietorship
   - Partnership
   - Corporate
6. Upload photos and supporting/conflict documents where required.
7. Confirm and submit.
8. Display a completion result and registration identifier.

The registration service also supports inclusion, reinstatement, and conflict submission flows. Registration data is represented by dedicated view models and is persisted through service, builder, mapper, and repository layers.

### 3.4 Registration validation

The current implementation validates, among other things:

- Required company, registration type, intermediary type, IC type, TBE category, M2 exam, and exam date fields.
- New IC format: 12 digits beginning with a date-of-birth pattern.
- Family level/rank requirements.
- Duplicate registration under the same company and intermediary type.
- General/Family registration conflicts and multi-company restrictions.
- Banca versus non-Banca consistency.
- BRN uniqueness for applicable non-Individual registrations.
- TBE/exemption eligibility.
- Referred-member restrictions.
- Required and optional supporting files.

The detailed rule inventory is maintained in [REGISTRATION-VALIDATION-RULES.md](REGISTRATION-VALIDATION-RULES.md).

### 3.5 Agency and member maintenance

Authorized users can search and update existing records, including:

- Agent/agency information.
- Photos and addresses.
- Nominees, guarantors, partners, board members, and company details.
- Agency changes and agency-type changes.
- Administrative activity and audit-related information.

### 3.6 Renewal

- List renewal headers and details.
- View renewal detail records.
- Save and submit renewal details.
- Accept or reject renewal details.
- Approve renewal headers/details.
- Upload renewal files.
- Export or complete renewal processing where enabled.

Renewal records are scoped by company and intermediary type where applicable.

### 3.7 Termination, reinstatement, and renewal recovery

- Search agents for termination actions.
- Upload termination input files.
- Process termination selections.
- Reinstate eligible terminated agents.
- Renew eligible terminated agents.
- Process recovery reinstatement and recovery renewal uploads.
- Add reinstated agents into renewal detail processing.

### 3.8 Referrals and conflicts

Referrals support creation/modification of a referral header, addition of details, attachment upload/removal, submission, approval, display, download, and completion. Conflict records support listing, attachments, download, close, reject, and automatic close operations.

Referral and conflict behavior is General/Family-aware. A referral may be associated with one or both intermediary types, while other operational records use an intermediary type identifier.

### 3.9 Invoices and payments

- View invoice lists and details.
- Search and page invoice data.
- Save invoice changes.
- Pay invoices where enabled.
- Approve/reject invoices.
- Regenerate and export invoice details.

### 3.10 Training, TBE, CPD, CBC, and ALC

The system provides operational screens for:

- Training and TBE-related enquiries/results.
- Training-detail validation.
- Continuing professional development (CPD) searches and validation.
- Central background check (CBC) creation, search, submit, approve, and reject.
- ALC listing, validation, and save operations.

### 3.11 Mail and notifications

Users can access inbox/outbox views, inspect messages, reply, compose new messages, and submit messages. The service layer also contains notification and mail services/builders used by business workflows.

### 3.12 Setup and master data

Administrators can maintain users, roles, menus, companies, agency types, states, religions, races, awards, guarantor types, activities, course categories, designations, educational/insurance qualifications, member levels, TBE exemptions, and referral lookup data. Validation endpoints are provided for most editable master-data screens.

### 3.13 Bulk upload and file history

The upload module supports IBFIM and registration file uploads, Excel parsing, upload history, and access to raw, successful, and error output content. Upload results are presented through complete/error/history screens.

### 3.14 Reports

The application exposes report pages for invoices, ASCII exports, user logins, new registrations, renewals, active/terminated agents, CPD, bulk registration, summaries, agents, ALC, automatic termination, termination lists/summaries, consolidated results, TBE results/examinations/exemptions/special agents, training details, referrals, audit trails, photo-upload summaries, and resigned active agents.

## 4. General and Family business partitioning

The current design uses two patterns:

- `Company.IsGeneral` and `Company.IsFamily` describe which intermediary businesses a company supports.
- `IntermediaryTypeID` identifies the business type on operational records such as agency principals, renewals, training, CBC, CPD, complaints, and conflicts.
- Referral entities use `IsGeneral` and `IsFamily` flags.

General-only users should receive General data; Family-only users should receive Family data; users with both capabilities can work with both. The existing split design and affected tables are described in [PLAN-General-Family-Split.md](PLAN-General-Family-Split.md).

## 5. Typical end-to-end lifecycle

```text
Login → Permission/menu resolution → Registration or enquiry
     → Validation and eligibility checks → Data entry and documents
     → Confirmation/submission → Agency/member maintenance
     → Renewal, termination, reinstatement, or reporting
```

## 6. Functional assumptions and open confirmations

The source code does not fully define business ownership, service-level targets, production URLs, exact role matrix, retention policy, or external system contracts. These should be confirmed with product owners and operations before this document is treated as a formal functional specification.
