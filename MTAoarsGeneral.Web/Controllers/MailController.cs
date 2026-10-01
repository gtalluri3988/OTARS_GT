using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.Builders.Interfaces;

namespace MTAoarsGeneral.Web.Controllers {
    [MTAoarsGeneralAuthorize]
    public class MailController : BaseController {
        IMailService mailService;
        IMailBuilder mailBuilder;

        public MailController(IMailService mailService, IMailBuilder mailBuilder) {
            this.mailService = mailService;
            this.mailBuilder = mailBuilder;
        }

        public ActionResult Index() {
            return View();
        }
       
        public ActionResult Inbox() {
            return PartialView();  
        }

        public ActionResult Outbox() {
            return PartialView();
        }
        
        public ActionResult ReceivedList(GridPageViewModel input) {
            var model = mailService.GetReceivedMails(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SentList(GridPageViewModel input) {
            var model = mailService.GetSentMails(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(long id) {
            var model = mailService.Read(id);
            return PartialView(model);
        }

        public ActionResult Reply(long id) {
            var model = mailService.GetReplyMail(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ReplySubmit(MailReplyViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            mailService.Send(model);
            output = GetResponse(mailService.CurrentContext);
            output.RedirectUrl = Url.Action("Inbox");
            return Json(output);
        }

        public ActionResult Display(long id) {
            var model = mailService.Read(id);
            return View(model);
        }

        public ActionResult New() {
            var model = mailBuilder.GetNewMail();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult NewSubmit(NewMailViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            mailService.Send(model);
            output = GetResponse(mailService.CurrentContext);
            output.RedirectUrl = Url.Action("Inbox");
            return Json(output);
        }
    }// class
}// namespace
