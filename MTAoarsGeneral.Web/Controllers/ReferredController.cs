using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Config;
using System.IO;

namespace MTAoarsGeneral.Web.Controllers {
    [MTAoarsGeneralAuthorize]
    public class ReferredController : BaseController {
        
        IReferredService referredService;
        IScopeDataProvider dataProvider;
        ConfigManager config;
		Identity identity;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ReferredController(IReferredService referredService, IScopeDataProvider dataProvider, ConfigManager config) {
            this.referredService = referredService;
            this.dataProvider = dataProvider;
            this.config = config;

            this.identity = this.dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
        }

        public ActionResult Create() {
            return View();
        }

        public ActionResult Modify() {
            return View();
        }

        public ActionResult Start() {
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var model = new ReferredHeaderViewModel();
            if (!identity.IsISMOrMTA) {
                model.Company = new LookupItem() {
                    ID = identity.CompanyID, Code = identity.CompanyCode, Description = identity.CompanyName
                };
            }
            return PartialView(model);
        }

        public ActionResult HeaderList() {
            return PartialView();
        }

        public ActionResult List(long id) {
            var model = referredService.GetHeader(id);
            return PartialView(model);
        }

        public ActionResult AddHeader(ReferredHeaderViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var id = referredService.AddHeader(model.Company.ID);
            output = GetResponse(referredService.CurrentContext);
            output.Data = id;
            output.RedirectUrl = String.Format("{0}/{1}", Url.Action("List"), id) ;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewDetail() {
            var model = new ReferredDetailViewModel();
            dataProvider.Register(GlobalConstants.CurrentReferredAttachments, new List<ReferredAttachmentViewModel>());
            model.Identity = this.identity;
            return PartialView("Detail", model);
        }

        public ActionResult AddDetail(ReferredDetailViewModel model) {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var list = dataProvider.Get<List<ReferredAttachmentViewModel>>(GlobalConstants.CurrentReferredAttachments);
            var header = referredService.GetHeader(model.HeaderID);
            model.TOName = header.Company.Description;
            foreach (var item in list) model.Attachments.Add(item);
            var id = referredService.AddDetail(model);
            output = GetResponse(referredService.CurrentContext);
            output.Data = id;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public void UploadFile(HttpPostedFileBase[] referredDocuments) {
            var list = dataProvider.Get<List<ReferredAttachmentViewModel>>(GlobalConstants.CurrentReferredAttachments);
            foreach (var file in referredDocuments) {
                string path = Path.Combine(config.UploadBaseDirectory, "ReferredAttachments");
                path = Path.Combine(path, String.Format("{0}{1}", Guid.NewGuid(), file.FileName));
                file.SaveAs(path);
                var attachment = new ReferredAttachmentViewModel {
                    FilePath = path, UploadFileName = file.FileName
                };
                list.Add(attachment);
            }
        }



        public void RemoveFile(string[] fileNames) {
            var list = dataProvider.Get<List<ReferredAttachmentViewModel>>(GlobalConstants.CurrentReferredAttachments);
            foreach (var name in fileNames) {
                var attachment = list.Where(p => p.UploadFileName == name).First();
                System.IO.File.Delete(attachment.FilePath);
                list.Remove(attachment);
            }
        }

        public ActionResult Submit(long headerId) {
            referredService.SubmitHeader(headerId);
            var output = GetResponse();
            return Json(output);
        }

        public ActionResult Approve(ReferredDetailViewModel model) {
            referredService.Approve(model.ID, model.ApproverComments);
            var output = GetResponse();
            return Json(output);
        }


        public ActionResult Complete() {
            return PartialView();
        }

        public ActionResult Display(long detailId) {
            var detail = referredService.GetDetail(detailId);
            return PartialView(detail);
        }

        public ActionResult Download(long id) {
            var attachment = referredService.GetAttachment(id);
            return new FilePathResult(attachment.FilePath, "text/csv");
        }

    }// class
}// namespace

