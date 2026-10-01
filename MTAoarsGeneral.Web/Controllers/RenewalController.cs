using System.Web.Mvc;
using AutoMapper;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Shared;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using System;
using System.Web;

namespace MTAoarsGeneral.Web.Controllers
{
    [MTAoarsGeneralAuthorize]
    public class RenewalController : BaseController
    {
        IRenewalService renewalService;
        IRenewalBuilder renewalBuilder;
        ILookupService lookupService;
        IScopeDataProvider dataProvider;
        IObjectCreator objectCreator;
        string sessionName = "UploadRenewal";
        public RenewalController(IRenewalService renewalService, IRenewalBuilder renewalBuilder, ILookupService lookupService, IScopeDataProvider dataProvider, IObjectCreator objectCreator)
        {
            this.renewalService = renewalService;
            this.renewalBuilder = renewalBuilder;
            this.lookupService = lookupService;
            this.dataProvider = dataProvider;
            this.objectCreator = objectCreator;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var model = renewalBuilder.GetRenewalHeaderList();
            return PartialView(model);
        }

        public ActionResult Details(long id)
        {
            var model = renewalBuilder.GetRenewlHeader(id);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ApproveDetails(RenewalApproveHeaderViewModel approveHeader)
        {
            var model = renewalBuilder.GetRenewlHeader(approveHeader);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult PagedDetails(GridPageViewModel input, long? headerId)
        {
            dataProvider.Register(GlobalConstants.CurrentRenewalPageSettings, input);
            var model = renewalService.GetRenwalDetails(input, headerId.Value, false);
            return Json(model);
        }

        [HttpPost]
        public FileResult Export(long? headerId)
        {
            var input = dataProvider.Get<GridPageViewModel>(GlobalConstants.CurrentRenewalPageSettings);
            var gridPage = new GridPageViewModel { Filter = input.Filter, Page = 1, PageSize = 1000000 };
            var model = renewalService.GetRenwalDetails(gridPage, headerId.Value, false);
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write("Agency Name,");
            writer.Write("Agency Number,");
            writer.Write("Corporate Status,");
            writer.Write("Banca,");
            writer.Write("Renewal Status,");
            writer.Write("Remarks,");
            writer.Write("Not To Release,");
            writer.Write("IC No#/Buiness Reg. No#,");
            writer.Write("Address");
            writer.WriteLine();

            foreach (var detail in model.Data)
            {
                writer.Write("\"");
                writer.Write(detail.AgencyName);
                writer.Write("\"");
                writer.Write(",");
                writer.Write(detail.AgencyNumber);
                writer.Write(",");
                writer.Write(detail.AgencyTypeDescription);
                writer.Write(",");
                writer.Write(detail.IsBancaStaff ? "Yes" : "No");
                writer.Write(",");
                writer.Write(detail.StatusDescription);
                writer.Write(",");
                writer.Write(detail.NotToRelease == 1 ? "Yes" : "No");
                writer.Write(",");
                writer.Write(detail.Remarks);
                writer.Write(",\"");
                writer.Write(detail.ICNoOrBusinessRegistrationNo);
                writer.Write("\",\"");
                writer.Write(detail.Address);
                writer.Write("\"");

                writer.WriteLine();
            }
            writer.Flush();
            output.Position = 0;
            return new FileStreamResult(output, "text/csv");
            //return File(output, "text/comma-separated-values", "Products.csv");
        }

        [HttpPost]
        public JsonResult ApprovedPagedDetails(GridPageViewModel input, long? headerId)
        {
            var model = renewalService.GetRenwalDetails(input, headerId.Value, true);
            return Json(model);
        }

        public ActionResult Approve()
        {
            return View();
        }

        public ActionResult ApproveHeader()
        {
            var model = new RenewalApproveHeaderViewModel();
            return PartialView(model);

        }


        public ActionResult Process()
        {
            //renewalService.Process();
            return RedirectToAction("Complete");
        }

        public ActionResult Complete()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveDetails(long id, List<RenewalDetailViewModel> items)
        {
            var header = new RenewalHeaderViewModel { ID = id, Details = items };
            renewalService.Save(header);
            return Json(new { Status = "Renewal detail Saved Successfully" });
        }

        [HttpPost]
        public JsonResult SubmitDetails(long id, List<RenewalDetailViewModel> items)
        {
            var header = new RenewalHeaderViewModel { ID = id, Details = items };
            renewalService.Submit(header);
            return Json(new { Status = "Renewal detail Submitted Successfully" });
        }

        [HttpPost]
        public JsonResult AcceptDetails(long id)
        {
            renewalService.Accept(id);
            return Json(new { Status = "Renewal detail Accepted Successfully" });
        }

        [HttpPost]
        public JsonResult RejectDetails(long id)
        {
            renewalService.Reject(id);
            return Json(new { Status = "Renewal detail Rejected" });
        }

        public ActionResult Upload()
        {
            Session[sessionName] = null;
            var model = objectCreator.Create<RenewalUploadViewModel>();
            model.Header = new RenewalHeaderViewModel();
            return View(model);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileTermination, RenewalUploadViewModel uplodaViewModel)
        {

            var model = objectCreator.Create<RenewalUploadViewModel>();
            model.Header = new RenewalHeaderViewModel();
            if (fileTermination == null)
            {
                ModelState.AddModelError("", "Please select file");
                return View(model);
            }
            if (fileTermination.FileName.ToLower().EndsWith(".csv"))
            {
                try
                {
                    var header = renewalBuilder.GetRenewalDetails(fileTermination.InputStream, uplodaViewModel.Header.Year, uplodaViewModel.Header.Quarter);
                    header.RenewalDetailStatuses = lookupService.GetRenewalDetailStatuses();
                    model.Header = header;
                    if (header.Details.Count == 0)
                    {
                        ModelState.AddModelError("", "No records found");
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Invalid file format");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid file format");
            }

            return View(model);
        }
    }// class
}// namespace
