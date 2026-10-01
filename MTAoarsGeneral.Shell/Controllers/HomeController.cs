using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Shell.Controllers {
    [MTAoarsGeneralAuthorize]
    public class HomeController : BaseController {

        IHomeService homeService;
        public HomeController(IHomeService homeService) {
            this.homeService = homeService;
        }
       
        public ActionResult Index() {
            var model = homeService.GetModel();
            return View(model);
        }

    }// class
}// namespace
