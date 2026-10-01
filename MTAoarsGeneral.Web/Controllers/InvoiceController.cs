using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using OfficeOpenXml;
using System.IO;
using System.Drawing;
using OfficeOpenXml.Style;
using MTAoarsGeneral.Utilities.Managers;

namespace MTAoarsGeneral.Web.Controllers
{
    [MTAoarsGeneralAuthorize]
    public class InvoiceController : BaseController
    {
        IInvoiceService invoiceService;
        IScopeDataProvider dataProvider;
        ILookupService lookupService;

        public InvoiceController(IInvoiceService invoiceService, IScopeDataProvider dataProvider, ILookupService lookupService)
        {
            this.invoiceService = invoiceService;
            this.dataProvider = dataProvider;
            this.lookupService = lookupService;
        }

        public ActionResult Index() {          
            return View();
        }

        [HttpPost]
        public ActionResult PagedList(GridPageViewModel input)
        {
            var model = invoiceService.GetInvoiceList(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Detail(long id)
        {
            return PartialView(id);
        }

        public ActionResult Pay(long id)
        {
            invoiceService.Pay(id);
            var output = GetResponse();
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Save(long? companyId,int year,int month,List<InvoiceViewModel> invoices)
        {
            invoiceService.Save(invoices);
            return Search(companyId, year, month);
        }

        public ActionResult ApproveReject()
        {
            var model = new InvoiceSearchViewModel();
            model.InvoiceStatuses = lookupService.GetInvoiceStatuses().ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(long? companyId,int year,int month)
        {
            if (companyId == 0) companyId = null;
            var model = invoiceService.GetInvoiceList(companyId,year, month);
            return Json(model.Data);
        }

        [HttpPost]
        public ActionResult Regenerate(long? companyId,int year,int month,long invoiceId)
        {
            invoiceService.Regenerate(invoiceId, year, month);
            return Search(companyId,year,month);
                
        }

        public ActionResult ExportDetail(long id)
        {
            var model = invoiceService.GetInvoiceViewDetail(id).ToList();
            var fileName = string.Format("InvoiceDetail_{0}.xlsx", DateTime.Now.ToString("yyyyMMddhhmmss"));
            var fileBytes = new ExcelManager().Generate(model, fileName);
            
            return File(fileBytes, "application/xlsx", fileName);

        }

    }// class
}// namespace
