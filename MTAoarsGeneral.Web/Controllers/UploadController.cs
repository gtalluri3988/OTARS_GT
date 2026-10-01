using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Services.Interfaces;
using System.Threading.Tasks;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Web.Controllers {

    [MTAoarsGeneralAuthorize]
    public class UploadController : Controller {

        IObjectCreator objectCreator;
        IAgencyBuilder agencyBuilder;
        IUploadService uploadService;

        public UploadController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder, IUploadService uploadService) {
            this.objectCreator = objectCreator;
            this.agencyBuilder = agencyBuilder;
            this.uploadService = uploadService;
        }

        public ActionResult Ibfim() {
            var model = new UploadViewModel {
                Caption = "Select file to upload IBFIM result",
                Code = LookupConstants.Uploads.Ibfim, FileControlID = "ibfimList", PostAction = "Ibfim"
            }; 
            return View("Excel", model);
        }

        [HttpPost]
        public ActionResult Ibfim([ModelBinder(typeof(ExcelModelBinder))]ExcelList<IbfimExcelViewModel> ibfimList) {
            /*Task.Factory.StartNew(() => {
                uploadService.UploadIbfim(ibfimList);
            });*/
            uploadService.UploadIbfim(ibfimList);
            return RedirectToAction("Complete", new { Code = LookupConstants.Uploads.Ibfim });
        }

        [HttpGet]
        public ActionResult RegistrationFile() {
            var model = new UploadViewModel {
                Caption = "Select file to upload Bulk Registration",
                Code = LookupConstants.Uploads.Registration,
                FileControlID = "registrationList",
                PostAction = "RegistrationFile"
            };
            return View("Excel", model);
        }

        [HttpPost]
        public ActionResult RegistrationFile([ModelBinder(typeof(ExcelModelBinder))]ExcelList<RegistrationExcelViewModel> registrationList) {
           /* Task.Factory.StartNew(() => {
                uploadService.UploadIbfim(ibfimList);
            });
            return RedirectToAction("Complete", new { Code = LookupConstants.Uploads.Ibfim });*/
          
            registrationList.ForEach(r => { r.IsOld = Request["is-Old"] == "on" ? true : false; });
            uploadService.UploadRegistration(registrationList);
            return RedirectToAction("Complete", new { Code = LookupConstants.Uploads.Ibfim });
        }

        public ActionResult History() {
            return View();
        }

        public ActionResult Complete(string code) {
            return View((object)code);
        }

        public ActionResult Error() {
            return View();
        }

        public ActionResult RawContent(long id) {
            var path = uploadService.GetHistoryRawPath(id);
            return new FilePathResult(path, "text/csv");
        }

        public ActionResult SuccessContent(long id) {
            var path = uploadService.GetHistorySuccessPath(id);
            return new FilePathResult(path, "text/csv");
        }

        public ActionResult ErrorContent(long id) {
            var path = uploadService.GetHistoryErrorPath(id);
            return new FilePathResult(path, "text/csv");
        }

    }// class
}// namespace

