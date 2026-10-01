using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Services.Interfaces
{
    public interface ILookupService
    {

        IEnumerable<LookupItem> GetAgencyTypes();
        IEnumerable<LookupItem> GetGuarantorTypes();
        IEnumerable<LookupItem> GetGenders();
        IEnumerable<LookupItem> GetEducationalQualifications();
        IEnumerable<LookupItem> GetInsuranceQualifications();
        IEnumerable<LookupItem> GetRaces();
        IEnumerable<LookupItem> GetStates();
        IEnumerable<LookupItem> GetICTypes();
        IEnumerable<LookupItem> GetIntermediaryTypes();
        IEnumerable<LookupItem> GetMemberLevels();
        IEnumerable<LookupItem> GetReligions();
        IEnumerable<LookupItem> GetMaritalStatuses();
        IEnumerable<LookupItem> GetRoles();
        IEnumerable<LookupItem> GetCompanies();
        IEnumerable<LookupItem> GetActiveCompanies();
        IEnumerable<LookupItem> GetEnquiryCompanies();
        IEnumerable<LookupItem> GetAdminActivities();
        IEnumerable<LookupItem> GetCourseCategories();
        IEnumerable<LookupItem> GetCourses();
        IEnumerable<LookupItem> GetTrainingStatuses();
        IEnumerable<LookupItem> GetBanks();
        IEnumerable<LookupItem> GetBankAccountTypes();
        IEnumerable<LookupItem> GetMailStatuses();
        IEnumerable<LookupItem> GetRenewalHeaderStatuses();
        IEnumerable<LookupItem> GetRenewalDetailStatuses();
        IEnumerable<LookupItem> GetTerminationStatuses();
        IEnumerable<LookupItem> GetTerminationActions();
        IEnumerable<LookupItem> GetReferredReasons();
        IEnumerable<LookupItem> GetReferredReasonsWithCategory(long category);
        IEnumerable<LookupItem> GetReferredCategories();
        IEnumerable<LookupItem> GetReferredCategoriesOptional();
        IEnumerable<LookupItem> GetTbeCategories();
        IEnumerable<LookupItem> GetReferredHeaderStatuses();
        IEnumerable<LookupItem> GetReferredDetailStatuses();
        IEnumerable<LookupItem> GetRecentYears();
        IEnumerable<LookupItem> GetRecentQuarters();
        IEnumerable<LookupItem> GetRecentMonths();
        IEnumerable<LookupItem> GetRecent6Years();
        IEnumerable<LookupItem> GetAllMonths();
        IEnumerable<LookupItem> GetLastFiveYears();
        IEnumerable<LookupItem> GetAllQuarters();
        IEnumerable<LookupItem> GetRegistrationCategories();
        IEnumerable<LookupItem> GetCPDYears();
        IEnumerable<LookupItem> GetInvoiceStatuses();
        IEnumerable<LookupItem> GetTrainingTypes();
        IEnumerable<LookupItem> GetTakafulExemptedExams();
        IEnumerable<LookupItem> GetTBEResultGrade();
        IEnumerable<LookupItem> BooleanItems();
        IEnumerable<LookupItem> GetTBEResultResult();
        IEnumerable<LookupItem> GetReferredReason(int categoryID);
        IEnumerable<LookupItem> GetRating();
        IEnumerable<LookupItem> GetTakafulExemptionFors();
        IEnumerable<LookupItem> GetYN();

        IEnumerable<LookupItem> GetM2Exams();
        IEnumerable<LookupItem> GetMTAAwards();

        IEnumerable<LookupItem> GetReferredActionTaken();

        IEnumerable<LookupItem> GetReferredPoliceReportLodged();
    }

}
