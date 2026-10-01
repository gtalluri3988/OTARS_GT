using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Web.Controllers {
    [MTAoarsGeneralAuthorize]
    public class ConflictController : BaseController {

        IConflictService conflictService;
        public ConflictController(IConflictService conflictService) {
            this.conflictService = conflictService;
        }

        public ActionResult List() {
            return View();
        }

        public ActionResult Attachments(long id) {
            var model = conflictService.GetAttachments(id);
            return PartialView(model);
        }

        public ActionResult Download(long id) {
            var attachment = conflictService.GetAttachment(id);
            return new FilePathResult(attachment.FilePath, "text/csv");
        }

        public ActionResult Close(long id) {
            conflictService.Close(id);
            return Json(GetResponse());
        }

        public ActionResult Reject(long id) {
            conflictService.Close(id);
            return Json(GetResponse());
        }

        public ActionResult AutoClose()
        {
            conflictService.DefaultOpenCases();
            conflictService.AutoClose();
            var js = Json(GetResponse());
            js.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return js;
        }
    }// class
}// namespace
