using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Masters;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;


namespace MTAoarsGeneral.Web.Controllers
{
    [MTAoarsGeneralAuthorize]
    public class SetupController : BaseController
    {

        IMenuService menuService;
        ILookupService lookupService;
        public SetupController(IMenuService menuService, ILookupService lookupService)
        {
            this.menuService = menuService;
            this.lookupService = lookupService;
        }
        public ActionResult MTAAwards()
        {
            return View(new MTAAwardViewModel());
        }
        public JsonResult ValidateMTAAward(MTAAwardViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Users()
        {
            return View(new UserMasterViewModel());
        }

        public ActionResult Roles()
        {
            var model = new RoleMasterViewModel();
            model.Menus = menuService.GetMenus();
            return View(model);
        }

        public ActionResult GuarantorTypes()
        {
            return View(new GuarantorTypeViewModel());
        }

        public JsonResult ValidateGuarantorType(GuarantorTypeViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Activities()
        {
            return View(new ActivityViewModel());
        }

        public JsonResult ValidateActivity(ActivityViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Companies()
        {
            return View(new CompanyViewModel());
        }

        public JsonResult ValidateCompany(CompanyViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult AgencyTypes()
        {
            return View(new AgencyTypeViewModel());
        }

        public JsonResult ValidateAgencyType(AgencyTypeViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult CourseCategories()
        {
            return View(new CourseCategoryViewModel());
        }

        public JsonResult ValidateCourseCategory(CourseCategoryViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Designations()
        {
            return View(new DesignationViewModel());
        }

        public JsonResult ValidateDesignation(DesignationViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult EducationalQualifications()
        {
            return View(new EducationalQualificationViewModel());
        }

        public JsonResult ValidateEducationalQualification(EducationalQualificationViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult InsuranceQualifications()
        {
            return View(new InsuranceQualificationViewModel());
        }

        public JsonResult ValidateInsuranceQualification(InsuranceQualificationViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult MemberLevels()
        {
            return View(new MemberLevelViewModel());
        }

        public JsonResult ValidateMemberLevel(MemberLevelViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Races()
        {
            return View(new RaceViewModel());
        }

        public JsonResult ValidateRace(RaceViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult Religions()
        {
            return View(new ReligionViewModel());
        }

        public ActionResult ReferredMembers()
        {
            return View(new ReferredMemberViewModel());
        }

        public JsonResult ValidateReligion(ReligionViewModel model)
        {
            return Json(GetResponse());
        }

        public ActionResult States()
        {
            return View(new StateViewModel());
        }

        public ActionResult TbeExemption()
        {
            return View(new TbeExemptionViewModel());
        }

        public JsonResult ValidateState(StateViewModel model)
        {
            return Json(GetResponse());
        }

        public JsonResult ValidateUser(UserMasterViewModel model)
        {
            return Json(GetResponse());
        }

        public JsonResult ValidateRole(RoleMasterViewModel model)
        {
            return Json(GetResponse());
        }
        public JsonResult ValidateReason(ReferredReasonViewModel mode)
        {
            return Json(GetResponse());
        }
        public JsonResult ValidateCategory(ReferredCategoryViewModel mode)
        {
            return Json(GetResponse());
        }
        public JsonResult ValidateReferredMember(ReferredMemberViewModel model)
        {
            return Json(GetResponse());
        }

        public JsonResult ValidateTbeExemption(TbeExemptionViewModel model)
        {
            return Json(GetResponse());
        }

        public JsonResult Menus()
        {
            var menus = menuService.GetMenus();
            return Json(menus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RoleMenus(long roleId)
        {
            var roleMenus = menuService.GetRoleMenus(roleId);
            return Json(roleMenus, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveRoleMenus(string roleCode, long[] roleMenus)
        {
            var id = lookupService.GetRoles().First(p => p.Code == roleCode).ID;
            menuService.Save(id, roleMenus);
            return Json(roleMenus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredReasons()
        {
            return View(new ReferredReasonViewModel());
        }

        public ActionResult ReferredCategories()
        {
            return View(new ReferredCategoryViewModel());
        }

        public ActionResult ReferredActionTaken()
        {
            return View(new ReferredActionTakenViewModel());
        }
        public JsonResult ValidateActionTaken(ReferredActionTakenViewModel mode)
        {
            return Json(GetResponse());
        }
    }// class
}// namespace
