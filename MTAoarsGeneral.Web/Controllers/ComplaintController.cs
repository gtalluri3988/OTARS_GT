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
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Masters;

namespace MTAoarsGeneral.Web.Controllers {

    [MTAoarsGeneralAuthorize]
    public class ComplaintController : BaseController {
        
        IAgencyBuilder agencyBuilder;
        IScopeDataProvider dataProvider;

        public ComplaintController(IAgencyBuilder agencyBuilder, IScopeDataProvider dataProvider) {
            this.agencyBuilder = agencyBuilder;
            this.dataProvider = dataProvider;
        }

        public ActionResult List() {
            return View(new ComplaintViewModel());
        }

        public ActionResult Create() {
            return PartialView();
        }

        public ActionResult Edit() {
            return PartialView();
        }

        public ActionResult Display() {
            return PartialView();
        }

        public ActionResult Search(string agencyNumber) {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var item = agencyBuilder.Search<AgencySearchResponseViewModel>(agencyNumber, identity.CompanyID);
            var output = GetResponse(agencyBuilder.CurrentContext);
            output.Data = item;
            return Json(output);
        }

        public ActionResult Save(ComplaintViewModel model) {
            return View();
        }

        public ActionResult ValidateComplaint(ComplaintViewModel model) {
            return Json(GetResponse());
        }
    }// class
}// namespace
