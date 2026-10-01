using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Constants
{

    public class LookupConstants
    {

        public class AgencyType
        {
            public const string Individual = "IND";
            public const string SoleProprietorship = "SP";
            public const string Partnership = "PS";
            public const string PrivateLimitedCompany = "PVTL";
            public const string PublicLimitedCompany = "PUBL";
            public const string Cooperative = "CO";
            public const string GovernmentAgency = "GA";
        }

        public class AgencyStatus
        {
            public const string CPDTerminated = "CPDT";
            public const string CBCSuspended = "CBCS";
            public const string CBCTerminated = "CBCT";
        }

        public class AgencyPrincipalStatus
        {
            public const string NotReleased = "NR";
            public const string Resign = "RE";
        }

        public class IntermediaryType
        {
            public const string General = "GEN";
            public const string Family = "FAM";
        }

        public class Runner
        {
            public const string AgencyRegistrationNumber = "AgencyRegnNo";
            public const string InvoiceNumber = "InvoiceNo";
            public const string ReceiptNumber = "ReceiptNo";
        }

        public class Designation
        {
            public const string CorporateNominee = "CN";
            public const string Partner = "PS";
            public const string Shareholder = "SH";
            public const string Director = "DR";
            public const string SeniorManagement = "SM";
            public const string BoardCouncil = "BC";
            public const string AdditionalCorporateNominee = "ACN";
        }

        public class GuarantorTypes
        {
            public const string Bank = "BK";
            public const string Personal = "PR";
            public const string NoGuarantee = "NG";
            public const string Others = "OT";
        }

        public class ICTypes
        {
            public const string NewIc = "NEWIC";
            public const string PoliceArmyPassport = "POLICE";
        }

        public class ConflictReasons
        {
            public const string PrincipalLimitExceeded = "PrinciaplLimitExceeded";
        }

        public class Activities
        {
            public const string NewRegistration = "NewRegistration";
            public const string Inclusion = "Inclusion";
            public const string Renewal = "Renewal";
            public const string ChangeNominee = "ChangeNominee";
            public const string UpdatePartners = "UpdatePartners";
            public const string UpdateGuarantor = "UpdateGuarantor";
            public const string UpdateBoardMembers = "UpdateBoardMembers";
            public const string ChangeACN = "ChangeACN";
            public const string ChangeCorporateStatus = "ChangeStatus";
            public const string ChangeCompany = "ChangeCompanyName";
        }

        public class MaritalStatus
        {
            public const string Single = "S";
            public const string Married = "M";
            public const string Divorce = "D";
            public const string Widow = "W";
        }

        public class MailStatus
        {
            public const string New = "New";
            public const string Read = "Read";
            public const string Replied = "Replied";
            public const string Delete = "Delete";
        }
        public class InvoiceStatus
        {
            public const string Generated = "GEN";
            public const string Approved = "APR";
            public const string Rejected = "REJ";
        }
        public class Notifications
        {
            public const string NewRegistration = "NewRegistration";
            public const string Renewal = "Renewal";
            public const string Renewed = "Renewed";
            public const string Resigned = "Resigned";
            public const string Terminated = "Terminated";
            public const string NotReleased = "NotReleased";
            public const string Invoice = "Invoice";
            public const string RenewalSubmitted = "RenewalSubmitted";
            public const string BatchTermination = "BatchTermination";
            public const string BatchRenewal = "BatchRenewal";
            public const string RenewalRejection = "RenewalRejection";
            public const string RenewalReminder = "RenewalReminder";
            public const string ChangeNominee = "ChangeNominee";
            public const string UpdateAddress = "UpdateAddress";
            public const string UpdateGuarantor = "UpdateGuarantor";
            public const string UpdateBoardMembers = "UpdateBoardMembers";
            public const string UpdatePartners = "UpdatePartners";
            public const string UpdateAgency = "UpdateAgency";
            public const string ChangeCorporateStatus = "ChangeStatus";
            public const string ChangeACN = "ChangeACN";
            public const string ChangeCompany = "ChangeCompanyName";
            public const string CPDProcessed = "CPDProcessed";
            public const string CBCProcessed = "CBCProcessed";
            public const string ExcelUpload = "ExcelUpload";
            public const string SourceConflictInint = "SrcConflictInit";
            public const string DestinationConflictInit = "DstConflictInit";
            public const string SourceConflictFirstReminder = "SrcConfFirstRmndr";
            public const string DestinationConflictFirstReminder = "DstConfFirstRmndr";
            public const string SourceConflictSecondReminder = "SrcConfSecondRmndr";
            public const string DestinationConflictSecondReminder = "DstConfSecondRmndr";
            public const string SourceConflictThirdReminder = "SrcConfThirdRmndr";
            public const string DestinationConflictThirdReminder = "DstConfThirdRmndr";
            public const string DestinationConflictAutoClose = "DstConfAutoClose";
            public const string ConflictNotReleased = "ConflictNotReleased";
        }

        public class RenewalHeaderStatus
        {
            public const string Generated = "Generated";
            public const string Saved = "Saved";
            public const string Submitted = "Submitted";
            public const string Processed = "Processed";
        }

        public class RenewalDetailStatus
        {
            public const string Generated = "GEN";
            public const string Renew = "REN";
            public const string Terminate = "TER";
            public const string NotReleased = "NR";
            public const string Resign = "RE";
        }

        public class Gender
        {
            public const string Male = "M";
            public const string Female = "F";
        }

        public class TerminationAction
        {
            public const string Terminate = "TER";
            public const string NotRelease = "NR";
            public const string Resign = "RE";
        }

        public class TerminationStatus
        {
            public const string Scheduled = "SCH";
            public const string Processed = "PRO";
        }

        public class TbeCategory
        {
            public const string IBFIM = "IBFIM";
            public const string MII = "MII";
            public const string ExemptedFPAM = "EXMFPAM";
            public const string ExemptedMFPC = "EXMMFPC";
            public const string ExemptedLTEQ08 = "EXMLTEQ08";
            public const string SPECIAL = "SPECIAL";
            public const string TBEGE = "TBEGE";
            public const string EXP = "EXP";
        }
        public class TBEType
        {
            public const string Sponsered = "SPN";
            public const string Walkin = "WAK";
        }
        public class Maintenance
        {
            public const string ChangeNominee = "CCNO";
            public const string ChangeCorporateStatus = "CCS";
            public const string ChangeCopmanyName = "CCNA";
            public const string ChangeOfAddress = "COA";
            public const string ChangePartner = "CPA";
            public const string ChangeGuarantor = "CGU";
            public const string ChangeAgency = "CAG";
            public const string ChangeBoardMembers = "CBM";
            public const string ChangeAdditionalCorporateNominee = "CACN";
            public const string UploadPhoto = "UP";

        }

        public class Schedulers
        {
            public const string GenerateInvoice = "GenerateInvoice";
            public const string GenerateRenewal = "GenerateRenewal";
            public const string RenewalReminder = "RenewalReminder";
            public const string ProcessRenewal = "ProcessRenewal";
            public const string ProcessCPD = "ProcessCPD";
            public const string ProcessCBC = "ProcessCBC";
            public const string SendFirstConflictReminder = "SendFirstConflictReminder";
            public const string SendSecondConflictReminder = "SendSecondConflictReminder";
            public const string DefaultOpenConflictCases = "DefaultOpenConflictCases";
            public const string AutoCloseConflictCases = "AutoCloseConflictCases";
        }

        public class Reports
        {
            public const string ALC = "ALC";
            public const string Invoice = "Invoice";
            public const string Ascii = "Ascii";
            public const string RenewalAscii = "RenewalAscii";
            public const string UserLogin = "UserLogin";
            public const string AdminActivity = "AdminActivity";
            public const string NewRegistrationStatistics = "NewRegistrationStatistics";
            public const string RenewalStatistics = "RenewalStatistics";
            public const string ActiveAgentsStatistics = "ActiveAgentsStatistics";
            public const string TerminatedAgentsStatistics = "TerminatedAgentsStatistics";
            public const string CPD = "CPD";
            public const string BulkRegistration = "BulkRegistration";
            public const string Summary = "Summary";
            public const string Agents = "Agents";
            public const string Companies = "Companies";
            public const string TerminationList = "TerminationList";
            public const string TerminationSummary = "TerminationSummary";
            public const string Consolidate = "Consolidate";
            public const string ConsolidateSummary = "ConsolidateSummary";
            public const string TBEResult = "TBEResult";
            public const string InvoiceDetail = "InvoiceDetail";
            public const string AutoTermination = "AutoTermination";
            public const string TrainingDetail = "TrainingDetail";
            public const string Referred = "ReferredListing";
            public const string ReferredAdmin = "ReferredListingAdmin";
            public const string TBESpecialAgents = "TBESpecialAgents";
            public const string AdministrativeAuditTrail = "AdministrativeAuditTrail";
            public const string PhotoUploadSummary = "PhotoUploadSummary";
            public const string TBEExamination = "TBEExamination";
            public const string ResignedActiveAgents = "ResignedActiveAgents";
            public const string TBEExemption = "TBEExemption";

        }

        public class TrainingStatus
        {
            public const string Generated = "GEN";
            public const string Calculated = "CAL";
            public const string Processed = "PRO";
        }

        public class CPDStatus
        {
            public const string Generated = "GEN";
            public const string NoAction = "NOA";
            public const string Terminated = "TER";
        }

        public class CBCHeaderStatus
        {
            public const string Saved = "SAV";
            public const string Submitted = "SUB";
            public const string Approved = "APP";
            public const string Rejected = "REJ";
            public const string Processed = "PRO";
        }

        public class CBCDetailStatus
        {
            public const string Generated = "GEN";
            public const string NoAction = "NOA";
            public const string Suspended = "SUS";
            public const string Terminated = "TER";
        }

        public class Uploads
        {
            public const string Ibfim = "IBFIM";
            public const string Mii = "MII";
            public const string Registration = "Registration";
            public const string Termination = "Termination";
        }

        public class ReferredHeaderStatus
        {
            public const string Saved = "SAV";
            public const string Submitted = "SUB";
            public const string PartialyProcessed = "PAR";
            public const string Processed = "PRO";
        }


        public class ReferredDetailStatus
        {
            public const string Generated = "GEN";
            public const string Approved = "APP";
            public const string Rejected = "REJ";
        }

        public class ConflictStatus
        {
            public const string Open = "OP";
            public const string Closed = "CL";
            public const string Defaulted = "DE";
            public const string Rejected = "RE";
            public const string AutoClose = "AC";
        }

        public class TrainingType
        {
            public const string FullDay = "FD";
            public const string HalfDay = "HD";
            public const string EveningClasses = "EC";
            public const string ELearning = "EL";
            public const string DialogueCoaching = "DC";
            public const string ProfessionalQualification = "PQ";
            public const string MortageTakaful = "MC";
        }

        public class PhotoManageType
        {
            public const string Registration = "Registration";
            public const string Inclusion = "Inclusion";
            public const string Update = "Update";
        }

        public class TBEResultGrade
        {
            public const int A = 1;
            public const int B = 2;
            public const int C = 3;
            public const int F = 4;
            public const int X = 5;
            public const int Y = 6;
        }

        public class Role
        {
            public const string ADM = "ADM";
            public const string ADMIN = "ADMIN";
        }

        public class TakafulExemptionFor
        {
            public const string GeneralTakafulOnly = "GTO";
            public const string FamilyTakafulOnly = "FTO";
            public const string GeneralFamilyTakaful = "GFT";
        }

        public class TakafulExemptedExam
        {
            public const string FPAM = "FPAM";
            public const string MFPC = "MFPC";
            public const string EXP = "EXP";
            public const string Register2008OrBelow = "08OrBelow";
            public const string SPECIAL = "SPECIAL";
            public const string TBEGE = "TBEGE";
        }

    }// Lookup constants

}// namespace
