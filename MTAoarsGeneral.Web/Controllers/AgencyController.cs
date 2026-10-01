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
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Web.Controllers
{

    [MTAoarsGeneralAuthorize]
    public class AgencyController : Controller
    {

        IObjectCreator objectCreator;
        IAgencyBuilder agencyBuilder;
        IScopeDataProvider dataProvider;
        IReferredBuilder referredBuilder;

        public AgencyController(IObjectCreator objectCreator, IAgencyBuilder agencyBuilder, IReferredBuilder referredBuilder)
        {
            this.objectCreator = objectCreator;
            this.agencyBuilder = agencyBuilder;
            this.referredBuilder = referredBuilder;
        }

        public ActionResult Enquiry()
        {
            var model = objectCreator.Create<EnquiryViewModel>();
            return View(model);
        }
        public ActionResult TBEEnquiry()
        {
            var model = objectCreator.Create<TBEEnquirySearchViewModel>();
            return View(model);
        }
        public ActionResult PhotoUploadEnquiry()
        {
            var model = objectCreator.Create<PhotoUploadEnquirySearchViewModel>();
            return View(model);
        }

        //public ActionResult Details(long id)
        //{
        //    var model = agencyBuilder.GetAgency(id);
        //    return PartialView(model);
        //}
        public ActionResult Details(long id, string agentTypeDescription, long agencyPrincipalID, long memberID)
        {
            var model = agencyBuilder.GetAgency(id, agencyPrincipalID, memberID);

            //To cater agency type (individual) to show norminee name 
            if (!string.IsNullOrEmpty(agentTypeDescription) && string.Equals(agentTypeDescription.ToLower(), nameof(LookupConstants.AgencyType.Individual).ToLower()))
                model.IsIndividual = true;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Search(AgencySearchRequestViewModel agencySearch)
        {
            var output = new JsonResponseModel();
            var agentEnquiries = agencyBuilder.Search<AgencySearchResponseViewModel>(agencySearch);
            //var agentEnquiryArchives = agencyBuilder.SearchArchive<AgencySearchResponseViewModel>(agencySearch);
            var referredMembers = new List<ViewModels.Administrative.ReferredResponseViewModel>();
            if (agentEnquiries.Count() > 0)
            {
                if (agentEnquiries.Count(i => i.IsReferredMember) > 0)
                    referredMembers = referredBuilder.SearchReferredMember(agentEnquiries.Where(i => i.IsReferredMember).First().NomineeNewICNumber).ReferredMembers;
            }

            var datas = new
            {
                agentEnquiries = agentEnquiries,
                referredMembers = referredMembers
                //agentEnquiryArchives = agentEnquiryArchives
            };
            //output.Data  = agencyBuilder.Search<AgencySearchResponseViewModel>(agencySearch);
            //output.Data = agencyBuilder.SearchArchive<AgencySearchResponseViewModel>(agencySearch);
            output.Data = datas;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchTBE(string icNumber)
        {
            var output = new JsonResponseModel();
            var result = agencyBuilder.Search(icNumber);
            output.Data = result.Results;
            output.Message = result.Message;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SearchUploadPhoto(string agencyNumber)
        {
            var output = new JsonResponseModel();
            var result = agencyBuilder.SearchPhotoUpload(agencyNumber);
            output.Data = result.Results;
            output.Message = result.Message;
            output.Result = true;
            return Json(output, JsonRequestBehavior.AllowGet);
        }

    }
}
