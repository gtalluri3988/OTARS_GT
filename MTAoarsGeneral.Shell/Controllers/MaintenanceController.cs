using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.Shell.Controllers
{
    public class MaintenanceController : BaseController
    {
        IAgencyBuilder agencyBuilder;
        IRegistrationBuilder registrationBuilder;
        IRegistrationService registrationService;

        public MaintenanceController(IAgencyBuilder agencyBuilder, IRegistrationBuilder registrationBuilder, IRegistrationService registrationService)
        {
            this.agencyBuilder = agencyBuilder;
            this.registrationBuilder = registrationBuilder;
            this.registrationService = registrationService;
        }

        public ActionResult Search(string maintenanceType)
        {
            Session["MaintenanceType"] = maintenanceType;
            if (maintenanceType == null)
            {
                Session["MaintenanceType"] = LookupConstants.Maintenance.ChangeOfAddress;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel request)
        {
            var model = agencyBuilder.Search<TerminationSearchResponseViewModel>(request.AgencySearch);
            return Json(model, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Edit(string AgencyID)
        {

            Session["AgencyID"] = AgencyID;
            return Json(new { Status = "Ok", Url = "Start" });
        }

        public ActionResult Start()
        {

            MaintenanceViewModel maintenanceModel = new MaintenanceViewModel();
            RegistrationViewModel model = GetRegistration();
            maintenanceModel.Registration = model;
         //   registrationBuilder.PopulateLookups(model);
            switch ((string)(Session["MaintenanceType"] ?? string.Empty))
            {
                case LookupConstants.Maintenance.ChangeNominee:
                    maintenanceModel.ViewName = "ChangeNominee";
                    maintenanceModel.ActionName = "SaveNominee";
                    break;
                case LookupConstants.Maintenance.ChangeOfAddress:
                    maintenanceModel.ActionName = "SaveAddress";
                    maintenanceModel.ViewName = "ChangeAddress";
                    break;

                case LookupConstants.Maintenance.ChangeGuarantor:

                    maintenanceModel.ActionName = "SaveGuarantor";
                    maintenanceModel.ViewName = "ChangeGuarantor";
                    break;
                case LookupConstants.Maintenance.ChangePartner:
                    maintenanceModel.ActionName = "SavePartners";
                    maintenanceModel.ViewName = "ChangePartners";
                    break;
                case LookupConstants.Maintenance.ChangeAgency:
                    maintenanceModel.ActionName = "SaveAgency";
                    maintenanceModel.ViewName = "ChangeAgency";
                    break;

                case LookupConstants.Maintenance.ChangeBoardMembers:
                    maintenanceModel.ActionName = "SaveBoardMembers";
                    maintenanceModel.ViewName = "ChangeBoardMembers";
                    break;

                default:
                    break;
            }
            return View("Edit", maintenanceModel);
        }

        public ActionResult SaveAddress(AddressViewModel model)
        {
            var registrationModel = GetRegistration();
            registrationModel.Address = model;
            if (Validate(registrationModel))
            {
                registrationService.UpdateAddress(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }


        public ActionResult SaveNominee(CorporateNomineeViewModel model)
        {
            var registrationModel = GetRegistration();
            registrationModel.CorporateNominee = model;
            if (Validate(registrationModel))
            {
               
                //registrationService.UpdateNominee(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }



        public ActionResult SaveGuarantor(GuarantorViewModel model)
        {
            var registrationModel = GetRegistration();
            registrationModel.Guarantor = model;
            if (Validate(registrationModel))
            {
               

                registrationService.UpdateGuarantor(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }

        public ActionResult SavePartners(List<PartnerViewModel> model)
        {
            var registrationModel = GetRegistration() as PartnershipRegistrationViewModel;
            registrationModel.Partners = model;
            if (Validate(registrationModel))
            {
                
                registrationService.UpdatePartners(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);

        }


        public ActionResult SaveCompany(AgencyViewModel model)
        {
            var registrationModel = GetRegistration();
            registrationModel.Agency = model;
            if (Validate(registrationModel))
            {                
                ///registrationService.UpdateCompanyName(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }

        public ActionResult SaveBoardMembers(List<DirectorViewModel> directors, List<ShareholderViewModel> shareHolders)
        {
            var registrationModel = GetRegistration();
            ((IExistableBoardMembers)registrationModel).Directors = directors;
            ((IExistableBoardMembers)registrationModel).Shareholders = shareHolders;
            if (Validate(registrationModel))
            {                
                registrationService.UpdateBoardMembers(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }

        public ActionResult SaveAgency(AgencyViewModel model)
        {
            var registrationModel = GetRegistration();
            registrationModel.Agency = model;
            if (Validate(registrationModel))
            {

                registrationService.UpdateAgency(registrationModel, GetAgencyID());
            }
            return new AjaxResponse(ModelState, string.Empty);
        }

        bool Validate(RegistrationViewModel model)
        {
            if (!ModelState.IsValid) return false;
            var id = registrationService.Check(model);
            Check(registrationService.CurrentContext);
            return ModelState.IsValid;
        }

        RegistrationViewModel GetRegistration()
        {
            return  registrationService.Get(GetAgencyID());
        }

        long GetAgencyID()
        {
            return Convert.ToInt64(Session["AgencyID"] ?? 0);
        }
    }
}
