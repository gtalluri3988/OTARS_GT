using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Web.Controllers
{
    public class LookupController : Controller
    {

        ILookupService lookupService;
        public LookupController(ILookupService lookupService)
        {
            this.lookupService = lookupService;
        }
        public ActionResult GetM2Exams()
        {
            return Json(lookupService.GetM2Exams(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult MTAAwards()
        {
            return Json(lookupService.GetMTAAwards(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult States()
        {
            return Json(lookupService.GetStates(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Companies()
        {
            return Json(lookupService.GetCompanies(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ActiveCompanies()
        {
            return Json(lookupService.GetActiveCompanies(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EnquiryCompanies()
        {
            return Json(lookupService.GetEnquiryCompanies(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AgencyTypes()
        {
            return Json(lookupService.GetAgencyTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Races()
        {
            return Json(lookupService.GetRaces(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Religions()
        {
            return Json(lookupService.GetReligions(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult IntermediaryTypes()
        {
            var items = lookupService.GetIntermediaryTypes()
                .Where(i => i.Code == LookupConstants.IntermediaryType.General)
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaritalStatuses()
        {
            return Json(lookupService.GetMaritalStatuses(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetYN()
        {
            return Json(lookupService.GetYN(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EducationalQualifications()
        {
            return Json(lookupService.GetEducationalQualifications(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GuarantorTypes()
        {
            return Json(lookupService.GetGuarantorTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Genders()
        {
            return Json(lookupService.GetGenders(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ICTypes()
        {
            return Json(lookupService.GetICTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MemberLevels()
        {
            return Json(lookupService.GetMemberLevels(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReferredActionTaken()
        {
            return Json(lookupService.GetReferredActionTaken(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReferredPoliceReportLodged()
        {
            return Json(lookupService.GetReferredPoliceReportLodged(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TbeCategories()
        {
            var excludedExemptCategories = new string[] {
                LookupConstants.TbeCategory.SPECIAL,
                //LookupConstants.TbeCategory.TBEGE, /* 20200325 - Remove this line when TBE Exception For go LIVE */
                //LookupConstants.TbeCategory.EXP /* 20200325 - Remove this line when TBE Exception For go LIVE */
            };
            return Json(lookupService.GetTbeCategories().Where(i => !excludedExemptCategories.Contains(i.Code)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TbeCategoriesAdmin()
        {
            return Json(lookupService.GetTbeCategories(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredReasons(long? Category)
        {
            if (Category == null)
                return Json(lookupService.GetReferredReasons(), JsonRequestBehavior.AllowGet);
            else return Json(lookupService.GetReferredReasonsWithCategory(Category ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredCategories()
        {
            return Json(lookupService.GetReferredCategories(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredCategoriesOptional()
        {
            return Json(lookupService.GetReferredCategoriesOptional(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Banks()
        {
            return Json(lookupService.GetBanks(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminActivities()
        {
            return Json(lookupService.GetAdminActivities(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Courses()
        {
            return Json(lookupService.GetCourses(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CourseCategories()
        {
            return Json(lookupService.GetCourseCategories(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult BankAccountTypes()
        {
            return Json(lookupService.GetBankAccountTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Roles()
        {
            return Json(lookupService.GetRoles(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RegistrationCategories()
        {
            return Json(lookupService.GetRegistrationCategories(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredHeaderStatuses()
        {
            return Json(lookupService.GetReferredHeaderStatuses(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredDetailStatuses()
        {
            return Json(lookupService.GetReferredDetailStatuses(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecentYears()
        {
            return Json(lookupService.GetRecentYears(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Recent6Years()
        {
            return Json(lookupService.GetRecent6Years(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RecentQuarters()
        {
            return Json(lookupService.GetRecentQuarters(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RecentMonths()
        {
            return Json(lookupService.GetRecentMonths(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllMonths()
        {
            return Json(lookupService.GetAllMonths(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult LastFiveYears()
        {
            return Json(lookupService.GetLastFiveYears(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AllQuarters()
        {
            return Json(lookupService.GetAllQuarters(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CPDYears()
        {
            return Json(lookupService.GetCPDYears(), JsonRequestBehavior.AllowGet);
        }
        //public ActionResult TerminationActions() {
        //    return Json(lookupService.GetTerminationActions(), JsonRequestBehavior.AllowGet);
        //}
        public ActionResult TerminationActions(string selected)
        {
            var items = lookupService.GetTerminationActions();
            if (!string.IsNullOrEmpty(selected))
            {
                var newItems = items.Where(i => selected.Split(';').Contains(i.Code)).ToList();
                return Json(newItems, JsonRequestBehavior.AllowGet);
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrainingTypes()
        {
            return Json(lookupService.GetTrainingTypes(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TerminationActionsAndRenewal()
        {
            return Json(lookupService.GetTerminationActions().Union(lookupService.GetRenewalDetailStatuses().ToList().Where(s => s.Code == LookupConstants.RenewalDetailStatus.Renew))
                , JsonRequestBehavior.AllowGet);
        }

        public ActionResult TakafulExemptedExams()
        {
            var excludedExemptedExams = new string[] {
                LookupConstants.TakafulExemptedExam.Register2008OrBelow,
            };
            return Json(lookupService.GetTakafulExemptedExams().Where(i => !excludedExemptedExams.Contains(i.Code)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TakafulExemptionFors()
        {
            return Json(lookupService.GetTakafulExemptionFors(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredReportAgentType()
        {
            return Json(new List<LookupItem>() { new LookupItem() { Code = "Admin", Description = "My Agents", ID = 1 }, new LookupItem() { Code = "", Description = "Other Agents", ID = 0 } }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Grade()
        {
            return Json(lookupService.GetTBEResultGrade(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Booleans()
        {
            return Json(lookupService.BooleanItems(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Result()
        {
            return Json(lookupService.GetTBEResultResult(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reason(int categoryID)
        {
            return Json(lookupService.GetReferredReason(categoryID), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Rating()
        {
            return Json(lookupService.GetRating(), JsonRequestBehavior.AllowGet);
        }
    }// class
}// namespace
