using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Masters;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;


namespace MTAoarsGeneral.Web.Controllers {
    [MTAoarsGeneralAuthorize]
    public class CPDController : BaseController {
        IAgencyBuilder agencyBuilder;
        ILookupService lookupService;
        IScopeDataProvider dataProvider;
        ICPDService cpdService;

        public CPDController(IAgencyBuilder agencyBuilder, ILookupService lookupService, IScopeDataProvider dataProvider, ICPDService cpdService) {
            this.agencyBuilder = agencyBuilder;
            this.lookupService = lookupService;
            this.dataProvider = dataProvider;
            this.cpdService = cpdService;
        }

        public ActionResult List() {
            var model = new TrainingListViewModel();
            var currentDate = DateTime.Now.ToShortDateString();
            model.TrainingModel.StartDate = DateTime.Parse(currentDate);
            model.TrainingModel.EndDate = DateTime.Parse(currentDate) ;
            model.TrainingModel.StatusID = lookupService.GetTrainingStatuses().First(p => p.Code == LookupConstants.TrainingStatus.Generated).ID;
            return View(model);
        }

        public ActionResult Search(string agencyNumber) {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var item = agencyBuilder.Search<AgencySearchResponseViewModel>(agencyNumber, identity.CompanyID);
            var output = GetResponse(agencyBuilder.CurrentContext);
            output.Data = item;
            return Json(output);
        }

        public JsonResult ValidateTrainingDetail(TrainingViewModel model, int Year) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var listModel = new TrainingListViewModel { TrainingModel = model, Year = Year };
            cpdService.Check(listModel);
            output = GetResponse(cpdService.CurrentContext);
            return Json(output);
        }
    }// class
}// namespace
