using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Web.Controllers
{
    [MTAoarsGeneralAuthorize]
    public class TerminationController : BaseController
    {
        IObjectCreator objectCreator;
        IAgencyBuilder agencyBuilder;
        IAgencyService agencyService;
        ITerminationService terminationService;
        ITerminationBuilder terminationBuilder;
        ILookupService lookupService;

        public TerminationController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder, IAgencyService agencyService,
            ITerminationService terminationService, ITerminationBuilder terminationBuilder, ILookupService lookupService
           )
        {
            this.objectCreator = objectCreator;
            this.agencyBuilder = agencyBuilder;
            this.agencyService = agencyService;
            this.terminationService = terminationService;
            this.terminationBuilder = terminationBuilder;
            this.lookupService = lookupService;
        }

        public ActionResult Search()
        {
            var model = objectCreator.Create<TerminationSearchViewModel>();
            model.TerminationActions = lookupService.GetTerminationActions();
            return View(model);
        }

        public ActionResult SearchAgent(AgencySearchRequestViewModel agencySearch)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var agency = terminationBuilder.Search(agencySearch.AgencyNumber);
            output = GetResponse(terminationBuilder.CurrentContext);
            output.Data = agency;
            return Json(output);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileTermination)
        {

            var model = objectCreator.Create<TerminationSearchViewModel>();
            if (fileTermination == null)
            {
                ModelState.AddModelError("", "Please select file");
                return View("search", model);
            }
            if (fileTermination.FileName.ToLower().EndsWith(".csv"))
            {
                try
                {
                    var agencies = terminationBuilder.Search(fileTermination.InputStream);
                    model.TerminationAgents = agencies.ToList();
                    model.TerminationActions = lookupService.GetTerminationActions();
                    if (model.TerminationAgents.Count == 0)
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
            return View("search", model);
        }

        [HttpPost]
        public ActionResult Process(TerminationSearchViewModel request)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            terminationService.Process(request.TerminationAgents);
            output.RedirectUrl = Url.Action("Complete");
            return Json(output);
        }

        [HttpPost]
        public ActionResult Reinstate(TerminationSearchViewModel request)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            terminationService.Reinstate(request.TerminationAgents);
            //if (output.Result == false) return Json(output);
            terminationService.AddIntoRenewalDetail(request.TerminationAgents);
            output.RedirectUrl = Url.Action("Complete");
            return Json(output);
        }

        [HttpPost]
        public ActionResult Renew(TerminationSearchViewModel request)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            terminationService.Renew(request.TerminationAgents);
            output.RedirectUrl = Url.Action("Complete");
            return Json(output);
        }

        public ActionResult Reinstate()
        {
            var model = objectCreator.Create<TerminationSearchViewModel>();
            return View(model);
        }

        public ActionResult Renew()
        {
            var model = objectCreator.Create<TerminationSearchViewModel>();
            return View(model);
        }

        public ActionResult SearchRecoveryAgent(AgencySearchRequestViewModel agencySearch)
        {
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var agency = terminationBuilder.SearchRecovery(agencySearch.AgencyNumber);
            output = GetResponse(terminationBuilder.CurrentContext);
            output.Data = agency;
            return Json(output);
        }
        [HttpPost]
        public ActionResult UploadRecoveryReinstate(HttpPostedFileBase fileTermination)
        {

            var model = objectCreator.Create<TerminationSearchViewModel>();
            if (fileTermination == null)
            {
                ModelState.AddModelError("", "Please select file");
                return View("reinstate", model);
            }
            if (fileTermination.FileName.ToLower().EndsWith(".csv"))
            {
                try
                {
                    var agencies = terminationBuilder.SearchRecovery(fileTermination.InputStream);
                    model.TerminationAgents = agencies.ToList();
                    if (model.TerminationAgents.Count == 0)
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
            return View("reinstate", model);
        }

        [HttpPost]
        public ActionResult UploadRecoveryRenew(HttpPostedFileBase fileTermination)
        {

            var model = objectCreator.Create<TerminationSearchViewModel>();
            if (fileTermination == null)
            {
                ModelState.AddModelError("", "Please select file");
                return View("renew", model);
            }
            if (fileTermination.FileName.ToLower().EndsWith(".csv"))
            {
                try
                {
                    var agencies = terminationBuilder.SearchRecovery(fileTermination.InputStream);
                    model.TerminationAgents = agencies.ToList();
                    if (model.TerminationAgents.Count == 0)
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
            return View("renew", model);
        }
    }// class
}// namespace
