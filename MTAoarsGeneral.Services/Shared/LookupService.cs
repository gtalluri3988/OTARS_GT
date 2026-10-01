using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.DomainModels;
using AutoMapper;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Repositories.Operations;
using System.Globalization;
using MTAoarsGeneral.Builders.Interfaces;
namespace MTAoarsGeneral.Services.Shared
{

    public class LookupService : ILookupService
    {
        ILookupRepository lookupRepository;
        IRepository<Company> companyRepository;
        IRepository<Activity> activityRepository;
        IRepository<LookupReferredReason> referredReasonRepository;
        ICPDRepository cpdRepository;
        IAgencyBuilder agencyBuilder;
        IReferredMemberRepository referredMemberRepository;

        public LookupService(ILookupRepository lookupRepository, IRepository<Company> companyRepository, IRepository<Activity> activityRepository, ICPDRepository cpdRepository, IRepository<LookupReferredReason> referredReasonRepository, IAgencyBuilder agencyBuilder, IReferredMemberRepository referredMemberRepository)
        {
            this.lookupRepository = lookupRepository;
            this.companyRepository = companyRepository;
            this.activityRepository = activityRepository;
            this.cpdRepository = cpdRepository;
            this.referredReasonRepository = referredReasonRepository;
            this.agencyBuilder = agencyBuilder;
            this.referredMemberRepository = referredMemberRepository;
        }

        public IEnumerable<LookupItem> GetAgencyTypes()
        {
            return GetLookup<LookupAgencyType>();
        }

        public IEnumerable<LookupItem> GetGuarantorTypes()
        {
            return GetLookup<LookupGuarantorType>();
        }

        public IEnumerable<LookupItem> GetGenders()
        {
            return GetLookup<LookupGender>();
        }

        public IEnumerable<LookupItem> GetEducationalQualifications()
        {
            return GetLookup<LookupEducationalQualification>();
        }

        public IEnumerable<LookupItem> GetInsuranceQualifications()
        {
            return GetLookup<LookupInsuranceQualification>();
        }

        public IEnumerable<LookupItem> GetRaces()
        {
            return GetLookup<LookupRace>();
        }

        public IEnumerable<LookupItem> GetStates()
        {
            return GetLookup<LookupState>();
        }

        public IEnumerable<LookupItem> GetICTypes()
        {
            return GetLookup<LookupICType>();
        }

        public IEnumerable<LookupItem> GetMemberLevels()
        {
            return GetLookup<LookupMemberLevel>();
        }

        public IEnumerable<LookupItem> GetReligions()
        {
            return GetLookup<LookupReligion>();
        }
        public IEnumerable<LookupItem> GetMTAAwards()
        {
            return GetLookup<LookupMTAAward>();
        }

        public IEnumerable<LookupItem> GetMaritalStatuses()
        {
            return GetLookup<LookupMaritalStatus>();
        }

        public IEnumerable<LookupItem> GetBanks()
        {
            return GetLookup<LookupBank>();
        }

        public IEnumerable<LookupItem> GetBankAccountTypes()
        {
            return GetLookup<LookupBankAccountType>();
        }

        public IEnumerable<LookupItem> GetMailStatuses()
        {
            return GetLookup<LookupMailStatus>();
        }

        public IEnumerable<LookupItem> GetIntermediaryTypes()
        {
            return GetLookup<LookupIntermediaryType>();
        }

        public IEnumerable<LookupItem> GetRenewalHeaderStatuses()
        {
            return GetLookup<LookupRenewalHeaderStatus>();
        }

        public IEnumerable<LookupItem> GetRenewalDetailStatuses()
        {
            return GetLookup<LookupRenewalDetailStatus>();
        }

        public IEnumerable<LookupItem> GetTerminationStatuses()
        {
            return GetLookup<LookupTerminationStatus>();
        }

        public IEnumerable<LookupItem> GetTerminationActions()
        {
            return GetLookup<LookupTerminationAction>();
        }

        public IEnumerable<LookupItem> GetTbeCategories()
        {
            return GetLookup<LookupTbeCategory>();
        }

        public IEnumerable<LookupItem> GetRoles()
        {
            return GetLookup<LookupRole>();
        }

        public IEnumerable<LookupItem> GetCourseCategories()
        {
            return GetLookup<LookupCourseCategory>();
        }

        public IEnumerable<LookupItem> GetCourses()
        {
            return GetLookup<LookupCourse>();
        }

        public IEnumerable<LookupItem> GetTrainingStatuses()
        {
            return GetLookup<LookupTrainingStatus>();
        }

        public IEnumerable<LookupItem> GetCompanies()
        {
            var companies = companyRepository.GetAll().Where(m => m.IsDropDown);
            return Mapper.Map<List<LookupItem>>(companies);
        }

        public IEnumerable<LookupItem> GetActiveCompanies()
        {
            var companies = companyRepository.GetAll().Where(p => p.IsActive);
            return Mapper.Map<List<LookupItem>>(companies);
        }

        public IEnumerable<LookupItem> GetEnquiryCompanies()
        {
            var list = new List<String> { "819", "822", "995", "996", "999" };
            return GetActiveCompanies().Where(p => !list.Contains(p.Code)).OrderBy(i => i.Description);
        }

        public IEnumerable<LookupItem> GetAdminActivities()
        {
            var activities = activityRepository.GetAll().Where(p => p.IsAdminActivity);
            return Mapper.Map<List<LookupItem>>(activities);
        }

        public IEnumerable<LookupItem> GetReferredReasons()
        {
            return GetLookup<LookupReferredReason>();
        }

        public IEnumerable<LookupItem> GetReferredReasonsWithCategory(long category)
        {
            var reasons = referredReasonRepository.GetAll().Where(p => p.IsActive && p.ReferredCategoryID == category);
            return Mapper.Map<List<LookupItem>>(reasons);
        }

        public IEnumerable<LookupItem> GetReferredCategories()
        {
            return GetLookup<LookupReferredCategory>();
        }

        public IEnumerable<LookupItem> GetReferredCategoriesOptional()
        {
            List<LookupItem> list = new List<LookupItem>();
            list.Add(new LookupItem() { Code = null, Description = "All", ID = 0 });
            list.AddRange(GetLookup<LookupReferredCategory>());
            return list;
        }

        public IEnumerable<LookupItem> GetReferredHeaderStatuses()
        {
            return GetLookup<LookupReferredHeaderStatus>();
        }

        public IEnumerable<LookupItem> GetReferredDetailStatuses()
        {
            return GetLookup<LookupReferredDetailStatus>();
        }

        public IEnumerable<LookupItem> GetInvoiceStatuses()
        {
            return GetLookup<LookupInvoiceStatus>();
        }

        public IEnumerable<LookupItem> GetTrainingTypes()
        {
            return GetLookup<LookupTrainingType>();
        }

        public IEnumerable<LookupItem> GetTakafulExemptedExams()
        {
            return GetLookup<LookupTakafulExemptedExam>();
        }

        /* public IEnumerable<LookupItem> GetRecentYears() {
             var current = DateTime.Now.GetCurrentQuarterEndDate();
             var previous = DateTime.Now.GetPreviousQuarterEndDate();
             yield return new LookupItem { ID = previous.Year, Code = previous.Year.ToString(), Description = previous.Year.ToString() };
             if (current.Year != previous.Year) yield return new LookupItem { ID = current.Year, Code = current.Year.ToString(), Description = current.Year.ToString() };
         }

         public IEnumerable<LookupItem> GetRecentQuarters() {
             var current = DateTime.Now.GetQuarter();
             var previous = DateTime.Now.GetPreviousQuarterEndDate().GetQuarter();
             yield return new LookupItem { ID = previous, Code = previous.ToString(), Description = previous.ToString() };
             yield return new LookupItem { ID = current, Code = current.ToString(), Description = current.ToString() };
         }*/

        public IEnumerable<LookupItem> GetRecentYears()
        {
            var year = DateTime.Now.Year;
            for (var i = year; i >= year - 44; i--)
            {
                yield return new LookupItem { ID = i, Code = i.ToString(), Description = i.ToString() };
            }
        }
        public IEnumerable<LookupItem> GetRecent6Years()
        {
            var year = DateTime.Now.Year;
            for (var i = year; i >= year - 5; i--)
            {
                yield return new LookupItem { ID = i, Code = i.ToString(), Description = i.ToString() };
            }
        }

        public IEnumerable<LookupItem> GetRecentQuarters()
        {
            for (var i = 1; i <= 4; i++)
            {
                yield return new LookupItem { ID = i, Code = i.ToString(), Description = i.ToString() };
            }
        }

        public IEnumerable<LookupItem> GetRecentMonths()
        {
            var current = DateTime.Now;
            for (int i = 1; i < 3; i++)
            {
                yield return new LookupItem { ID = current.Month, Code = current.Month.ToString(), Description = current.Month.ToString() };
                current = DateTime.Now.AddMonths(-1);
            }
        }

        public IEnumerable<LookupItem> GetAllMonths()
        {
            var current = DateTime.Now;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

            for (int i = 1; i < 13; i++)
            {
                yield return new LookupItem { ID = i, Code = dtfi.GetMonthName(i).ToString(), Description = dtfi.GetMonthName(i) };
                //current = DateTime.Now.AddMonths(-1);
            }
        }

        public IEnumerable<LookupItem> GetLastFiveYears()
        {
            var current = DateTime.Now;
            DateTimeFormatInfo dtfi = new DateTimeFormatInfo();

            for (int i = 0; i < 5; i++)
            {
                yield return new LookupItem { ID = current.Year, Code = current.Year.ToString(), Description = current.Year.ToString() };
                current = current.AddYears(-1);
            }
        }

        public IEnumerable<LookupItem> GetAllQuarters()
        {
            for (int i = 1; i < 5; i++)
            {
                yield return new LookupItem { ID = i, Code = i.ToString(), Description = String.Format("Q{0}", i) };
            }
        }

        public IEnumerable<LookupItem> GetCPDYears()
        {
            var current = DateTime.Now.Year;
            var previous = DateTime.Now.AddYears(-1).Year;
            yield return new LookupItem { ID = current, Code = current.ToString(), Description = current.ToString() };
            if (cpdRepository.IsProcessed(previous) == false)
            {
                yield return new LookupItem { ID = previous, Code = previous.ToString(), Description = previous.ToString() };
            }
        }

        public IEnumerable<LookupItem> GetRegistrationCategories()
        {
            yield return new LookupItem { ID = 1, Code = "IND", Description = "Individual" };
            yield return new LookupItem { ID = 2, Code = "CORP", Description = "Corporate" };
        }

        public IEnumerable<LookupItem> GetTBEResultGrade()
        {
            yield return new LookupItem { ID = 1, Code = "A", Description = "A" };
            yield return new LookupItem { ID = 2, Code = "B", Description = "B" };
            yield return new LookupItem { ID = 3, Code = "C", Description = "C" };
            yield return new LookupItem { ID = 4, Code = "F", Description = "F" };
            yield return new LookupItem { ID = 5, Code = "X", Description = "X" };
            yield return new LookupItem { ID = 6, Code = "Y", Description = "Y" };
        }

        public IEnumerable<LookupItem> GetYN()
        {
            yield return new LookupItem { ID = 1, Code = "Y", Description = "Y" };
            yield return new LookupItem { ID = 2, Code = "N", Description = "N" };

        }
        public IEnumerable<LookupItem> BooleanItems()
        {
            yield return new LookupItem { ID = 1, Code = "Yes", Description = "Yes" };
            yield return new LookupItem { ID = 0, Code = "No", Description = "No" };
        }

        public IEnumerable<LookupItem> GetM2Exams()
        {
            yield return new LookupItem { ID = 1, Code = "MFPC", Description = "Malaysian Financial Planning Council (MFPC)" };
            yield return new LookupItem { ID = 2, Code = "FPAM", Description = "Financial Planning Association of Malaysia (FPAM)" };
            yield return new LookupItem { ID = 3, Code = "Exempted", Description = "Exempted" };
            yield return new LookupItem { ID = 4, Code = "NotYetCompleted", Description = "Not yet completed" };

        }
        public IEnumerable<LookupItem> GetTBEResultResult()
        {
            yield return new LookupItem { ID = 1, Code = "Fail", Description = "Fail" };
            yield return new LookupItem { ID = 2, Code = "Pass", Description = "Pass" };
            yield return new LookupItem { ID = 3, Code = "Absent", Description = "Absent" };
            yield return new LookupItem { ID = 4, Code = "Applied", Description = "Applied" };
        }

        public IEnumerable<LookupItem> GetReferredReason(int categoryID)
        {
            var items = referredMemberRepository.GetLookupReasons(categoryID);
            return items.Select(x => new LookupItem() { ID = x.ID, Code = x.Code, Description = x.Description });
        }

        IEnumerable<LookupItem> GetLookup<Tin>() where Tin : EntityObject, ILookupEntity
        {
            return Mapper.Map<List<LookupItem>>(lookupRepository.GetAll<Tin>());
        }

        public IEnumerable<LookupItem> GetRating()
        {
            for (var i = 1; i <= 10; i++)
            {
                yield return new LookupItem { ID = i, Code = i.ToString(), Description = i.ToString() };
            }
        }

        public IEnumerable<LookupItem> GetTakafulExemptionFors()
        {
            return GetLookup<LookupTakafulExemptionFor>();
        }

        public IEnumerable<LookupItem> GetReferredActionTaken()
        {
            return GetLookup<LookupReferredActionTaken>();
        }
        public IEnumerable<LookupItem> GetReferredPoliceReportLodged()
        {
            return GetLookup<LookupReferredPoliceReportLodged>();
        }

    }// class

}// namespace
