using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Web.Controllers
{

    [MTAoarsGeneralAuthorize]
    public class AdministrativeController : BaseController
    {
        IObjectCreator objectCreator;
        IAgencyBuilder agencyBuilder;
        IReferredMemberRepository referredMemberRepository;
        IReferredBuilder referredBuilder;
        IRenewalRepository renewalRepository;
        IRenewalBuilder renewalBuilder;
        IRegistrationService registrationService;
        IAdministrativeService administrativeService;
        IScopeDataProvider dataProvider;
        IRenewalService renewalService;
        ILookupService lookupService;

        public AdministrativeController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder, IReferredMemberRepository referredMemberRepository,
            IReferredBuilder referredBuilder, IRenewalRepository renewalRepository, IRenewalBuilder renewalBuilder, IRegistrationService registrationService,
            IAdministrativeService administrativeService, IScopeDataProvider dataProvider, IRenewalService renewalService, ILookupService lookupService)
        {
            this.objectCreator = objectCreator;
            this.agencyBuilder = agencyBuilder;
            this.registrationService = registrationService;
            this.administrativeService = administrativeService;
            this.referredMemberRepository = referredMemberRepository;
            this.referredBuilder = referredBuilder;
            this.renewalRepository = renewalRepository;
            this.renewalBuilder = renewalBuilder;
            this.dataProvider = dataProvider;
            this.renewalService = renewalService;
            this.lookupService = lookupService;
        }

        public ActionResult TBEResult()
        {
            var model = objectCreator.Create<TBEEnquirySearchViewModel>();

            return View(model);
        }

        public ActionResult SearchTBE(string ICNumber)
        {
            var output = new JsonResponseModel();
            var result = agencyBuilder.Search(ICNumber);
            output.Data = result.Results;
            output.Message = result.Message;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id, string ResultType)
        {
            var model = new TBEEnquirySearchViewModel();

            model.Result = agencyBuilder.getResult(id, ResultType);

            var grade = (Grade)System.Enum.Parse(typeof(Grade), model.Result.Grade);

            var result = (ExamResult)System.Enum.Parse(typeof(ExamResult), model.Result.Result ?? "none");

            model.Result.Grade = Convert.ToInt32(grade).ToString();
            model.Result.Result = Convert.ToInt32(result).ToString();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Edit(TBEEnquirySearchViewModel model)
        {
            var Message = "";

            var output = new JsonResponseModel();


            var grade = (Grade)Int32.Parse(model.Result.Grade);

            var result = (ExamResult)Int32.Parse(model.Result.Result);

            model.Result.Grade = grade.ToString();
            model.Result.Result = result.ToString();
            model.Result.Name = model.Result.Name.ToUpper();
            switch (model.Result.Exam)
            {
                case LookupConstants.TbeCategory.IBFIM:
                    administrativeService.InsertTBEResultToAuditTrail(model, "IbfimResult");
                    break;
                case LookupConstants.TbeCategory.MII:
                    administrativeService.InsertTBEResultToAuditTrail(model, "MiiResult");
                    break;
            }

            agencyBuilder.SaveTBEResult(model, out Message);

            output.Data = model;
            output.Message = Message;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Referred()
        {
            var model = objectCreator.Create<ReferredSearchViewModel>();
            return View(model);
        }

        public ActionResult ReferredDetails(string Icnumber)
        {
            var output = new JsonResponseModel();

            output.Data = referredBuilder.SearchReferredMember(Icnumber).ReferredMembers;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReferredEdit(int id)
        {
            var model = new ReferredSearchViewModel();
            model.ReferredMember = referredBuilder.SearchReferredMemberDetail(id).ReferredMember;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult UpdateReferred(ReferredSearchViewModel model)
        {
            var output = new JsonResponseModel();
            var Message = "";

            administrativeService.InsertReferredToAuditTrail(model);
            referredMemberRepository.UpdateReferred(model, out Message);
            output.Message = Message;
            output.Data = model.ReferredMember.ID;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Renewal()
        {
            var model = objectCreator.Create<RenewalDetailsViewModel>();
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchCompany(string CompanyName)
        {
            var output = new JsonResponseModel();

            if (string.IsNullOrWhiteSpace(CompanyName))
            {

                output.Result = true;
                output.Data = null;
                return Json(false);
            }

            var items = renewalRepository.GetCompanyNames(CompanyName);

            output.Data = items;

            output.Result = true;

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RenewalHeader(long companyID)
        {
            var items = renewalBuilder.GetRenewalHeaderByCompany(companyID);



            return Json(items);
        }

        public ActionResult RenewalDetails(long headerID)
        {
            var model = renewalBuilder.GetRenewalHeaderRef(headerID);

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult PagedDetails(GridPageViewModel input, long? headerId)
        {
            dataProvider.Register(GlobalConstants.CurrentRenewalPageSettings, input);
            var model = renewalService.GetItemsRenewalDetails(input, headerId.Value, true);
            return Json(model);
        }

        [HttpPost]
        public JsonResult SaveDetails(int id, List<RenewalDetailViewModel> items)
        {
            var header = new RenewalDetailsViewModel { ID = id, RenewalDetail = items };
            administrativeService.InsertRenewalToAuditTrail(header);
            renewalService.SaveAdminstrativeRenewalDetails(header);
            return Json(new { Status = "Renewal detail Saved Successfully" });
        }

        [HttpPost]
        public JsonResult SubmitDetails(int id, List<RenewalDetailViewModel> items)
        {
            var header = new RenewalDetailsViewModel { ID = id, RenewalDetail = items };
            renewalService.SubmitRenewalDetails(header);
            return Json(new { Status = "Renewal detail Submitted Successfully" });
        }

        #region Agency

        public ActionResult Agency()
        {
            var model = objectCreator.Create<AdministrativeSearchAgentViewModel>();
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchAgency(AgencySearchRequestViewModel request)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var agency = agencyBuilder.Search<AdministrativeSearchAgentResponseViewModel>(request);
            output = GetResponse(agencyBuilder.CurrentContext);
            output.Data = agency;
            return Json(output);
        }

        public ActionResult EditAgency(string key, bool key2)
        {
            AdministrativeAgentViewModel model;
            var isHistorical = Convert.ToBoolean(key2);
            model = administrativeService.GetByPrincipal(Convert.ToInt64(key.Decrypt()));
            if (model == null || isHistorical)
                model = administrativeService.GetByPrincipalHistory(Convert.ToInt64(key.Decrypt()));
            model.AgencyPrincipalKey = key;
            //model.Agency.TerminationActions = lookupService.GetTerminationActions().Where(i => (new string[] { LookupConstants.TerminationAction.Resign, LookupConstants.TerminationAction.Terminate }).Contains(i.Code));

            return View("EditAgency", model);
        }

        public ActionResult SaveAgency(string AgencyPrincipalKey, bool IsTerminated, AgencyViewModel agency, CorporateNomineeViewModel corporateNominee, DateTime? dateAppointed, DateTime? dateTerminated)
        {
            if (string.IsNullOrEmpty(AgencyPrincipalKey))
                return Json(null);

            var agencyPrincipalID = Convert.ToInt64(AgencyPrincipalKey.Decrypt());

            //var output = GetResponse();
            //if (output.Result == false) return Json(output);
            var agentViewModel = administrativeService.GetByPrincipal(agencyPrincipalID);
            if (agentViewModel == null || IsTerminated)
            {
                agentViewModel = administrativeService.GetByPrincipalHistory(agencyPrincipalID);
                agentViewModel.DateTerminated = dateTerminated;
            }
            agentViewModel.Agency = agency;
            agentViewModel.CorporateNominee = corporateNominee;
            agentViewModel.DateAppointed = dateAppointed;

            //if (Validate(registrationModel))
            administrativeService.UpdateAgency(agentViewModel, agencyPrincipalID, IsTerminated);

            var output = GetResponse(administrativeService.CurrentContext);
            return Json(output);
        }

        #endregion 
    }
}
