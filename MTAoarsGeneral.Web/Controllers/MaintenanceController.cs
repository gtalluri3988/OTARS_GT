using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using System.IO;
using MTAoarsGeneral.Utilities.Config;

namespace MTAoarsGeneral.Web.Controllers {
    [MTAoarsGeneralAuthorize]
    public class MaintenanceController : BaseController {
        IAgencyBuilder agencyBuilder;
        IRegistrationBuilder registrationBuilder;
        IRegistrationService registrationService;
        IObjectCreator objectCreator;
        IScopeDataProvider dataProvider;
        IActivityService activityService;
        ConfigManager config;

        public MaintenanceController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder, IRegistrationBuilder registrationBuilder, IRegistrationService registrationService, IActivityService activityService, IScopeDataProvider dataProvider, ConfigManager config)
        {
            this.agencyBuilder = agencyBuilder;
            this.registrationBuilder = registrationBuilder;
            this.registrationService = registrationService;
            this.objectCreator = objectCreator;
            this.dataProvider = dataProvider;
            this.activityService = activityService;
            this.config = config;
        }

        public ActionResult Search(string maintenanceType) {
            Session["MaintenanceType"] = maintenanceType;
            if (maintenanceType == null) {
                Session["MaintenanceType"] = LookupConstants.Maintenance.ChangeOfAddress;
            }
            var model = objectCreator.Create<MaintenanceSearchViewModel>();
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchAgent(AgencySearchRequestViewModel request) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var agency = agencyBuilder.Search<MaintenanceSearchResponseViewModel>(request, (string)Session["MaintenanceType"] ?? string.Empty);
            output = GetResponse(agencyBuilder.CurrentContext);
            output.Data = agency;
            return Json(output);

        }

        public JsonResult Edit(string AgencyPrincipalID, string AgencyNumber)
        {

            Session["AgencyPrincipalID"] = AgencyPrincipalID;
            Session["AgencyNumber"] = AgencyNumber;
            return Json(new { Status = "Ok", Url = "Start" });
        }

        public ActionResult Start() {

            MaintenanceViewModel maintenanceModel = new MaintenanceViewModel();
            RegistrationViewModel model = GetRegistration();
            maintenanceModel.Registration = model;
            ViewData["ReadOnly"] = true;
            ViewData["AgencyNumber"] = Session["AgencyNumber"] ?? "";
            switch ((string)(Session["MaintenanceType"] ?? string.Empty)) {
                case LookupConstants.Maintenance.ChangeNominee:
                    maintenanceModel.ViewName = "ChangeNominee";
                    maintenanceModel.ActionName = "SaveNominee";
                    ViewData["ReadOnly"] = false;
                    break;
                case LookupConstants.Maintenance.ChangeOfAddress:
                    maintenanceModel.ActionName = "SaveAddress";
                    maintenanceModel.ViewName = "ChangeAddress";
                    break;
                case LookupConstants.Maintenance.ChangeCopmanyName:
                    maintenanceModel.ActionName = "SaveCompany";
                    maintenanceModel.ViewName = "ChangeCompany";
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
                    ViewData["ReadOnly"] = false;
                    break;

                case LookupConstants.Maintenance.ChangeBoardMembers:
                    maintenanceModel.ActionName = "SaveBoardMembers";
                    maintenanceModel.ViewName = "ChangeBoardMembers";
                    break;
                case LookupConstants.Maintenance.ChangeCorporateStatus:
                    maintenanceModel.ActionName = "ChangeAgencyType";
                    maintenanceModel.ViewName = "ChangeCorporateStatus";
                    break;
                case LookupConstants.Maintenance.ChangeAdditionalCorporateNominee:
                    maintenanceModel.ActionName = "SaveACN";
                    maintenanceModel.ViewName = "ChangeACN";
                    break;
                case LookupConstants.Maintenance.UploadPhoto:
                    maintenanceModel.ActionName = "SavePhoto";
                    maintenanceModel.ViewName = "ChangePhoto";
                    break;
                default:
                    break;
            }
            return View("Edit", maintenanceModel);
        }

        [HttpPost]
        public JsonResult SavePhoto(HttpPostedFileBase photo)
        {
            string path = Path.Combine(config.UploadBaseDirectory, "Photo");
            path = Path.Combine(path, String.Format("{0}{1}", Guid.NewGuid(), photo.FileName));
            photo.SaveAs(path);
            var attachment = new PhotoViewModel { PhotoPath = path};
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            registrationService.UpdatePhoto(path, GetAgencyPrincipalID());
            output = GetResponse();
            return Json(output);
        }

        [HttpPost]
        public JsonResult SaveAddress(AddressViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            registrationModel.Address = model;
            if (Validate(registrationModel)) {
                registrationService.UpdateAddress(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse();
            return Json(output);
        }


        public ActionResult SaveNominee(CorporateNomineeViewModel model) {
            ModelState.Clear();
            var registrationModel = GetRegistration();
            registrationModel.CorporateNominee = model;
            try {
                ValidateModel(registrationModel.CorporateNominee, "CorporateNominee");
            } catch {
                return Json(GetResponse());
            }
            var viewModel = new ChangeNomineeViewModel { RegistrationModel = registrationModel, PrincipalID = GetAgencyPrincipalID() };
            registrationService.ChangeNominee(viewModel);
            var output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }



        public ActionResult SaveGuarantor(GuarantorViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            registrationModel.Guarantor = model;
            if (Validate(registrationModel)) {


                registrationService.UpdateGuarantor(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        public ActionResult SavePartners(List<PartnerViewModel> model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration() as PartnershipRegistrationViewModel;
            registrationModel.Partners = model;
            if (Validate(registrationModel)) {

                registrationService.UpdatePartners(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);

        }


        public ActionResult SaveCompany(AgencyViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            registrationModel.Agency = model;
            var changeCompModel = new ChangeCompanyViewModel { RegistrationModel = registrationModel, PrincipalID = GetAgencyPrincipalID() };

            if (Validate(registrationModel)) {
                registrationService.UpdateCompanyName(changeCompModel);
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        public ActionResult SaveBoardMembers(List<DirectorViewModel> directors, List<ShareholderViewModel> shareHolders) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            ((IExistableBoardMembers)registrationModel).Directors = directors;
            ((IExistableBoardMembers)registrationModel).Shareholders = shareHolders;
            if (Validate(registrationModel)) {
                registrationService.UpdateBoardMembers(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        public ActionResult SaveACN(List<AdditionalCorporateNomineeViewModel> additionalCorporateNominees) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            ((IExistableBoardMembers)registrationModel).AdditionalCorporateNominees = additionalCorporateNominees;
            if (Validate(registrationModel)) {
                registrationService.UpdateACN(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        public ActionResult SaveAgency(AgencyViewModel agency, CorporateNomineeViewModel corporateNominee, DateTime? dateAppointed) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var registrationModel = GetRegistration();
            registrationModel.Agency = agency;
            registrationModel.Agency.ExcludeM2ExamValidation = true;
            registrationModel.CorporateNominee = corporateNominee;
            registrationModel.DateAppointed = dateAppointed;
            if (Validate(registrationModel)) {

                registrationService.UpdateAgency(registrationModel, GetAgencyPrincipalID());
            }
            output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        public ActionResult ChangeAgencyType(string typeCode) {
            registrationService.ChangeAgencyType(GetAgencyPrincipalID(), typeCode);
            var output = GetResponse(registrationService.CurrentContext);
            return Json(output);
        }

        bool Validate(RegistrationViewModel model) {
            if (!ModelState.IsValid) return false;
            var id = registrationService.Check(model);
            return ModelState.IsValid;
        }

        RegistrationViewModel GetRegistration() {
            return registrationService.GetByPrincipal(GetAgencyPrincipalID());
        }

        long GetAgencyPrincipalID() {
            return Convert.ToInt64(Session["AgencyPrincipalID"] ?? 0);
        }


        public ActionResult AdminActivity() {
            var model = new AdminActivityViewModel() {
                InvoiceDate = DateTime.Now, SearchFromDate = DateTime.Now, SearchToDate = DateTime.Now
            };
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (!identity.IsISMOrMTA) {
                model.Company = new LookupItem { ID = identity.CompanyID, Code = identity.CompanyCode, Description = identity.CompanyName };
            }
            return View(model);
        }

        public ActionResult SearchByAgencyNumber(string agencyNumber) {
            var output = agencyBuilder.Search<AdminActivityDetailViewModel>(agencyNumber);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchByDate(DateTime fromDate, DateTime toDate) {
            var output = agencyBuilder.Search<AdminActivityDetailViewModel>(fromDate, toDate);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveAdminActivity(AdminActivityViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            activityService.Save(model);
            output = GetResponse(registrationService.CurrentContext);
            dataProvider.Register(GlobalConstants.CurrentAdminActivity, model);
            return Json(output);
        }

    }// class
}// namespace
