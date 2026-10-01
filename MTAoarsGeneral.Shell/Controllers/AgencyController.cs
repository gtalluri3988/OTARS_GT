using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Shell.Controllers {
    [MTAoarsGeneralAuthorize]
    public class AgencyController : Controller {

        IObjectCreator objectCreator;
        IAgencyBuilder agencyBuilder;

        public AgencyController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder) {
            this.objectCreator = objectCreator;
            this.agencyBuilder = agencyBuilder;
        }

        public ActionResult Enquiry() {
            var model = objectCreator.Create<EnquiryViewModel>();
            return View(model);
        }

        public ActionResult Details(long id) {
            var model = agencyBuilder.GetAgency(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AjaxList(EnquiryViewModel request) {
            var model = agencyBuilder.Search<AgencySearchResponseViewModel>(request.AgencySearch);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}
