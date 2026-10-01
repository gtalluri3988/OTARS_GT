using System.Web.Mvc;
using AutoMapper;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Shell.Controllers {
    [MTAoarsGeneralAuthorize]
    public class RegistrationController : BaseController {
        IRegistrationService registrationService;
        IRegistrationBuilder registrationBuilder;
        IAgencyBuilder agencyBuilder;
        ILookupService lookupService;

        public RegistrationController(IRegistrationService registrationService, IRegistrationBuilder registrationBuilder, IAgencyBuilder agencyBuilder, ILookupService lookupService) {
            this.registrationService = registrationService;
            this.registrationBuilder = registrationBuilder;
            this.agencyBuilder = agencyBuilder;
            this.lookupService = lookupService;
        }

        static void EnsureGeneralFork(RegistrationIndexViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
        }

        static void EnsureGeneralFork(RegistrationViewModel model)
        {
            if (model?.Agency == null) return;
            model.Agency.IsGeneral = true;
            model.Agency.IsFamily = false;
        }

        static void EnsureGeneralFork(RegistrationInclusionViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        static void EnsureGeneralFork(RegistrationConflictViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        static void EnsureGeneralFork(RegistrationReinstationViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        public ActionResult Start() {
            var model = registrationBuilder.GetStartViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Start(RegistrationStartViewModel input) {
            if (ModelState.IsValid == false) {
               // registrationBuilder.Populate(input);
                return View(input);
            }
            var model = registrationBuilder.GetIndexViewModel(input.Company.ID, input.AgencyType.ID);
            return View("Index", model);
        }

      
        [HttpPost]
        public ActionResult Index([Bind(Exclude = "IsFamily,IsGeneral")] RegistrationIndexViewModel model)
        {
            EnsureGeneralFork(model);
            var serviceModel = registrationBuilder.GetIndexViewModel(model.CompanyID, model.AgencyType.ID);
            if (ModelState.IsValid == false)
                return View(Mapper.Map(serviceModel, model));

            var option = registrationService.Check(model);
            Check(registrationService.CurrentContext);

            if (ModelState.IsValid == false)
                return View(Mapper.Map(serviceModel, model));
            if (option == RegistrationCheckOptions.Include) 
                return View("Inclusion", registrationBuilder.GetInclusion(model));
            if (option == RegistrationCheckOptions.Reinstate) 
                return View("Reinstation", registrationBuilder.GetReinstation(model));
            if (option == RegistrationCheckOptions.Conflict) 
                return View("Conflict", registrationBuilder.GetConflict(model));
            
            switch (model.AgencyType.Code)
            {
                case LookupConstants.AgencyType.Individual:
                    return View("Individual", registrationBuilder.GetNew<IndividualRegistrationViewModel>(model));
                case LookupConstants.AgencyType.SoleProprietorship:
                    return View("SoleProprietorship", registrationBuilder.GetNew<SoleProprietorshipRegistrationViewModel>(model));
                case LookupConstants.AgencyType.Partnership:
                    return View("Partnership", registrationBuilder.GetNew<PartnershipRegistrationViewModel>(model));
               /* case LookupConstants.AgencyType.PrivateLimitedCompany:
                    return View("PrivateLimited", registrationBuilder.GetNew<PrivateLimitedRegistrationViewModel>(model));
                case LookupConstants.AgencyType.PublicLimitedCompany:
                    return View("PublicLimited", registrationBuilder.GetNew<PublicLimitedRegistrationViewModel>(model));
                case LookupConstants.AgencyType.Cooperative:
                    return View("Cooperative", registrationBuilder.GetNew<CooperativeRegistrationViewModel>(model));
                case LookupConstants.AgencyType.GovernmentAgency:
                    return View("GovernmentAgency", registrationBuilder.GetNew<GovernmentAgencyRegistrationViewModel>(model));*/
            }
            return new EmptyResult();
        }

        [HttpPost]
        public JsonResult IndividualCheck(IndividualRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("IndividualConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult IndividualConfirm(IndividualRegistrationViewModel model) {
            return Confirm(model);
        }
       
        [HttpPost]
        public JsonResult Individual(IndividualRegistrationViewModel model) {
            return Add(model);
        }

        [HttpPost]
        public JsonResult PartnershipCheck(PartnershipRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("PartnershipConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult PartnershipConfirm(PartnershipRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult Partnership(PartnershipRegistrationViewModel model) {
            return Add(model);
        }

        [HttpPost]
        public JsonResult SoleProprietorshipCheck(SoleProprietorshipRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("SoleProprietorshipConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult SoleProprietorshipConfirm(SoleProprietorshipRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult SoleProprietorship(SoleProprietorshipRegistrationViewModel model) {
            return Add(model);
        }

       /* [HttpPost]
        public JsonResult PrivateLimitedCheck(PrivateLimitedRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("PrivateLimitedConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult PrivateLimitedConfirm(PrivateLimitedRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult PrivateLimited(PrivateLimitedRegistrationViewModel model) {
            return Add(model);
        }

        [HttpPost]
        public JsonResult PublicLimitedCheck(PublicLimitedRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("PublicLimitedConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult PublicLimitedConfirm(PublicLimitedRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult PublicLimited(PublicLimitedRegistrationViewModel model) {
            return Add(model);
        }

        [HttpPost]
        public JsonResult CooperativeCheck(CooperativeRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("CooperativeConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult CooperativeConfirm(CooperativeRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult Cooperative(CooperativeRegistrationViewModel model) {
            return Add(model);
        }

        [HttpPost]
        public JsonResult GovernmentAgencyCheck(GovernmentAgencyRegistrationViewModel model) {
            if (Validate(model)) {
                return new AjaxResponse(ModelState, "", Url.Action("GovernmentAgencyConfirm"), null);
            }
            return new AjaxResponse(ModelState, "");
        }

        [HttpPost]
        public ActionResult GovernmentAgencyConfirm(GovernmentAgencyRegistrationViewModel model) {
            return Confirm(model);
        }

        [HttpPost]
        public JsonResult GovernmentAgency(GovernmentAgencyRegistrationViewModel model) {
            return Add(model);
        }*/

        [HttpPost]
        public JsonResult Inclusion(RegistrationInclusionViewModel model) {
            EnsureGeneralFork(model);
            registrationService.Include(model);
            Check(registrationService.CurrentContext);
            if (!ModelState.IsValid) return new AjaxResponse(ModelState, "");
            return new AjaxResponse(ModelState, "", Url.Action("Complete"), model.AgencyID.ToString());
        }

        [HttpPost]
        public JsonResult Reinstation(RegistrationReinstationViewModel model) {
            EnsureGeneralFork(model);
            registrationService.Reinstate(model);
            return new AjaxResponse(ModelState, "", Url.Action("Complete"), model.AgencyID.ToString());
        }

        [HttpPost]
        public JsonResult Conflict(RegistrationConflictViewModel model) {
            EnsureGeneralFork(model);
            if (!ModelState.IsValid) return new AjaxResponse(ModelState, "");
            registrationService.Add(model);
            Check(registrationService.CurrentContext);
            if (!ModelState.IsValid) return new AjaxResponse(ModelState, "");
            return new AjaxResponse(ModelState, "", Url.Action("ConflictComplete"), model.AgencyID.ToString());
        }

        JsonResult Add(RegistrationViewModel model) {
            EnsureGeneralFork(model);
            if (!ModelState.IsValid) return new AjaxResponse(ModelState, "");
            var id = registrationService.Add(model);
            Check(registrationService.CurrentContext);
            if (!ModelState.IsValid) return new AjaxResponse(ModelState, "");
            return new AjaxResponse(ModelState, "", Url.Action("Complete"), id.ToString());
        }

        ActionResult Confirm(RegistrationViewModel model) {
            EnsureGeneralFork(model);
            var output = agencyBuilder.GetAgency(model);
            return PartialView("Confirm", output);
        }

        bool Validate(RegistrationViewModel model) {
            EnsureGeneralFork(model);
            if (!ModelState.IsValid) return false;
            var id = registrationService.Check(model);
            Check(registrationService.CurrentContext);
            return ModelState.IsValid;
        }


        public ActionResult Complete(long id) {
            var model = agencyBuilder.GetAgency(id);
            return View(model);
        }

        public ActionResult ConflictComplete(long id) {
            var model = agencyBuilder.GetAgency(id);
            return View(model);
        }

    }// class
}// namespace
