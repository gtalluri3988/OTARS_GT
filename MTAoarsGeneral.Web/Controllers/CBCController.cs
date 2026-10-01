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

namespace MTAoarsGeneral.Web.Controllers {

    [MTAoarsGeneralAuthorize]
    public class CBCController : BaseController {
        ICBCBuilder cbcBuilder;
        ICBCService cbcService;

        public CBCController(ICBCBuilder cbcBuilder, ICBCService cbcService) {
            this.cbcBuilder = cbcBuilder;
            this.cbcService = cbcService;
        }

        public ActionResult Create() {
            return View();
        }

        public ActionResult Start() {
            var model = cbcBuilder.GetStartViewModel();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult StartCheck(CBCStartViewModel input) {
            var output = GetResponse();
            if (output.Result == false) return Json(output, JsonRequestBehavior.AllowGet);
            cbcService.Check(input);
            output = GetResponse(cbcService.CurrentContext);
            output.RedirectUrl = Url.Action("List");
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult List(CBCStartViewModel input) {
            var model = cbcService.GetHeader(input);
            return PartialView(model);
        }

        public ActionResult Search(long headerId, string agencyNumber, long companyId) {
            var result = cbcBuilder.Search(agencyNumber, companyId);
            var output = GetResponse(cbcBuilder.CurrentContext);
            if (output.Result == false) return Json(output);
            output.Data = cbcService.Add(headerId, agencyNumber);
            output = GetResponse(cbcService.CurrentContext);
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Submit(long headerId) {
            cbcService.ChangeHeaderStatus(headerId, LookupConstants.CBCHeaderStatus.Submitted);
            var output = GetResponse();
            output.Message = "CBC Details are Submitted succesfully";
            return Json(output, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Approve(long headerId) {
            cbcService.ChangeHeaderStatus(headerId, LookupConstants.CBCHeaderStatus.Approved);
            var output = GetResponse();
            output.Message = "CBC Details are Approved succesfully";
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reject(long headerId) {
            cbcService.ChangeHeaderStatus(headerId, LookupConstants.CBCHeaderStatus.Rejected);
            var output = GetResponse();
            output.Message = "CBC Details are Rejected succesfully";
            return Json(output, JsonRequestBehavior.AllowGet);
        }
    }// class
}// namespace
