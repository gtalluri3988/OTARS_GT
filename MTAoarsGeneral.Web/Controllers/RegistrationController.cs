using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using AutoMapper;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using System.IO;
using MTAoarsGeneral.Utilities.Config;


namespace MTAoarsGeneral.Web.Controllers
{
    [MTAoarsGeneralAuthorize]
    public class RegistrationController : BaseController
    {

        IRegistrationService registrationService;
        IRegistrationBuilder registrationBuilder;
        IAgencyBuilder agencyBuilder;
        ILookupService lookupService;
        IObjectCreator objectCreator;
        IScopeDataProvider dataProvider;
        IReferredBuilder referredBuilder;
        ConfigManager config;



        string sessionName = "UploadRegistration";
        public RegistrationController(IObjectCreator objectCreator, IRegistrationService registrationService,
            IRegistrationBuilder registrationBuilder, IAgencyBuilder agencyBuilder, ConfigManager config,
            ILookupService lookupService, IScopeDataProvider dataProvider, IReferredBuilder referredBuilder)
        {
            this.registrationService = registrationService;
            this.registrationBuilder = registrationBuilder;
            this.agencyBuilder = agencyBuilder;
            this.lookupService = lookupService;
            this.objectCreator = objectCreator;
            this.config = config;
            this.dataProvider = dataProvider;
            this.referredBuilder = referredBuilder;
        }

        static void EnsureGeneralFork(RegistrationIndexViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
        }

        static void EnsureGeneralFork(RegistrationViewModel model)
        {
            if (model?.Agency == null) return;
            model.Agency.IsGeneral = true;
            model.Agency.IsFamily = false;
        }

        static void EnsureGeneralFork(RegistrationInclusionViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        static void EnsureGeneralFork(RegistrationConflictViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        static void EnsureGeneralFork(RegistrationReinstationViewModel model)
        {
            if (model == null) return;
            model.IsGeneral = true;
            model.IsFamily = false;
            if (model.Agency != null)
            {
                model.Agency.IsGeneral = true;
                model.Agency.IsFamily = false;
            }
        }

        public ActionResult Create()
        {
            dataProvider.Remove(GlobalConstants.CurrentPhoto);
            return View();
        }

        public ActionResult Upload()
        {
            Session[sessionName] = null;
            var model = objectCreator.Create<RegistrationUploadViewModel>();
            return View(model);
        }
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileTermination)
        {
            var model = objectCreator.Create<RegistrationUploadViewModel>();
            if (fileTermination == null)
            {
                ModelState.AddModelError("", "Please select file");
                return View(model);
            }
            if (fileTermination.FileName.ToLower().EndsWith(".csv"))
            {
                try
                {
                    var registrations = registrationBuilder.GetRegistrations(fileTermination.InputStream);
                    model.Registrations = registrations.ToList();
                    if (model.Registrations.Count == 0)
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
            Session[sessionName] = model;
            return View(model);
        }

        [HttpGet]
        public ActionResult LoadRegistration(int ID)
        {
            var model = Session[sessionName] as RegistrationUploadViewModel;
            if (model != null)
            {
                if (model.Registrations != null && model.Registrations.Count > ID)
                {
                    var registration = model.Registrations[ID];
                    foreach (var error in registration.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Value);
                    }
                    ViewData["IsUpload"] = true;
                    return Confirm(registration.Model);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult Process()
        {
            var model = Session[sessionName] as RegistrationUploadViewModel;
            if (model != null)
            {
                registrationService.Add(model);
                var result = Json(new { Registrations = model.Registrations.Select(s => new { ID = model.Registrations.IndexOf(s), AgencyName = s.Model.CorporateNominee == null ? "" : s.Model.CorporateNominee.Name, isError = (s.Errors == null ? 0 : s.Errors.Count) > 0, Messages = s.Errors == null ? new List<string>() : s.Errors.Values.ToList() }) });
                return result;
            }
            else
            {
                model = objectCreator.Create<RegistrationUploadViewModel>();
                return Json(model);
            }

        }
        public ActionResult Start()
        {
            var model = registrationBuilder.GetStartViewModel();
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult StartCheck(RegistrationStartViewModel input)
        {
            var model = GetResponse();
            model.RedirectUrl = Url.Action("Index");
            return Json(model);
        }

        [HttpPost]
        public ActionResult Index(RegistrationStartViewModel input)
        {
            var model = registrationBuilder.GetIndexViewModel(input.Company.ID, input.AgencyType.ID);
            return PartialView("Index", model);
        }

        [HttpPost]
        public ActionResult IndexCheck([Bind(Exclude = "IsFamily,IsGeneral")] RegistrationIndexViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var option = registrationService.Check(model);
            output = GetResponse(registrationService.CurrentContext);
            if (output.Result == false) return Json(output);
            output.RedirectUrl = (option == RegistrationCheckOptions.Referred) ? Url.Action("Referred") : Url.Action("Main");
            return Json(output);
        }

        [HttpPost]
        public ActionResult Referred([Bind(Exclude = "IsFamily,IsGeneral")] RegistrationIndexViewModel model)
        {
            EnsureGeneralFork(model);
            model.ReferredMembers = referredBuilder.SearchReferredMember(model.ICNumber).ReferredMembers;
            //model.ReferredMemberCategoriesDescription = referredBuilder.GetCategoryDescription(model.ICNumber);
            //model.ReferredMemberDateCreated = referredBuilder.GetReferredMemberDateCreated(model.ICNumber);

            return PartialView(model);
        }

        public ActionResult Main([Bind(Exclude = "IsFamily,IsGeneral")] RegistrationIndexViewModel model)
        {
            EnsureGeneralFork(model);
            var option = registrationService.Check(model);
            if (option == RegistrationCheckOptions.Include)
            {
                return PartialView("Inclusion", registrationBuilder.GetInclusion(model));
            }
            if (option == RegistrationCheckOptions.Reinstate)
            {
                return PartialView("Reinstation", registrationBuilder.GetReinstation(model));
            }
            if (option == RegistrationCheckOptions.Conflict)
            {
                dataProvider.Register(GlobalConstants.CurrentConflictAttachments, new List<ConflictAttachmentViewModel>());
                return PartialView("Conflict", registrationBuilder.GetConflict(model));
            }
            switch (model.AgencyType.Code)
            {
                case LookupConstants.AgencyType.Individual:
                    return PartialView("Individual", registrationBuilder.GetNew<IndividualRegistrationViewModel>(model));
                case LookupConstants.AgencyType.SoleProprietorship:
                    return PartialView("SoleProprietorship", registrationBuilder.GetNew<SoleProprietorshipRegistrationViewModel>(model));
                case LookupConstants.AgencyType.Partnership:
                    return PartialView("Partnership", registrationBuilder.GetNew<PartnershipRegistrationViewModel>(model));
                case LookupConstants.AgencyType.PrivateLimitedCompany:
                case LookupConstants.AgencyType.PublicLimitedCompany:
                case LookupConstants.AgencyType.Cooperative:
                case LookupConstants.AgencyType.GovernmentAgency:
                    return PartialView("Corporate", registrationBuilder.GetNew<CorporateRegistrationViewModel>(model));
            }
            return new EmptyResult();
        }

        public ActionResult CorporateNominee()
        {
            return PartialView();
        }

        public ActionResult Individual()
        {
            var indexViewModel = new RegistrationIndexViewModel()
            {
                ICType = new LookupItem { ID = 1, Code = LookupConstants.ICTypes.NewIc, Description = "Individual" },
                ICNumber = "78060612345",
                IsGeneral = true,
                IsFamily = false,
                Level = new LookupItem { ID = 1, Code = "1" },
                AgencyType = new LookupItem { ID = 1, Code = LookupConstants.AgencyType.Individual }
            };
            return View("Individual", registrationBuilder.GetNew<IndividualRegistrationViewModel>(indexViewModel));
        }

        public ActionResult Partnership()
        {
            var indexViewModel = new RegistrationIndexViewModel()
            {
                ICType = new LookupItem { ID = 1, Code = LookupConstants.ICTypes.NewIc },
                ICNumber = "78060612345",
                IsGeneral = true,
                IsFamily = false,
                Level = new LookupItem { ID = 1, Code = "1" },
                AgencyType = new LookupItem { ID = 2, Code = LookupConstants.AgencyType.Partnership }
            };
            return View("Partnership", registrationBuilder.GetNew<PartnershipRegistrationViewModel>(indexViewModel));
        }

        public ActionResult Corporate()
        {
            var indexViewModel = new RegistrationIndexViewModel()
            {
                ICType = new LookupItem { ID = 1, Code = LookupConstants.ICTypes.NewIc },
                ICNumber = "78060612345",
                IsGeneral = true,
                IsFamily = false,
                Level = new LookupItem { ID = 1, Code = "1" },
                AgencyType = new LookupItem { ID = 4, Code = LookupConstants.AgencyType.PrivateLimitedCompany }
            };
            return View("Corporate", registrationBuilder.GetNew<CorporateRegistrationViewModel>(indexViewModel));
        }




        [HttpPost]
        public JsonResult IndividualCheck(IndividualRegistrationViewModel model)
        {
            var output = Validate(model);
            output.RedirectUrl = Url.Action("IndividualConfirm");
            return Json(output);
        }

        [HttpPost]
        public ActionResult IndividualConfirm(IndividualRegistrationViewModel model)
        {
            return Confirm(model);
        }

        [HttpPost]
        public ActionResult IndividualSubmit(IndividualRegistrationViewModel model)
        {
            var output = Add(model);
            return Json(output);
        }

        [HttpPost]
        public JsonResult PartnershipCheck(PartnershipRegistrationViewModel model)
        {
            var output = Validate(model);
            output.RedirectUrl = Url.Action("PartnershipConfirm");
            return Json(output);
        }

        [HttpPost]
        public ActionResult PartnershipConfirm(PartnershipRegistrationViewModel model)
        {
            return Confirm(model);
        }

        [HttpPost]
        public ActionResult PartnershipSubmit(PartnershipRegistrationViewModel model)
        {
            var output = Add(model);
            return Json(output);
        }

        [HttpPost]
        public JsonResult SoleProprietorshipCheck(SoleProprietorshipRegistrationViewModel model)
        {
            var output = Validate(model);
            output.RedirectUrl = Url.Action("SoleProprietorshipConfirm");
            return Json(output);
        }

        [HttpPost]
        public ActionResult SoleProprietorshipConfirm(SoleProprietorshipRegistrationViewModel model)
        {
            return Confirm(model);
        }

        [HttpPost]
        public ActionResult SoleProprietorshipSubmit(SoleProprietorshipRegistrationViewModel model)
        {
            var output = Add(model);
            return Json(output);
        }

        [HttpPost]
        public JsonResult CorporateCheck(CorporateRegistrationViewModel model)
        {
            //ModelState.Remove("Agency.DateOfExam");
            //if (model.Shareholders != null)
            //{
            //    for (var i = 0; i < model.Shareholders.Count; i++)
            //    {
            //        //string key = $"Shareholders[{i}].ICType.ID";
            //        //ModelState.Remove(key);
            //        if(model.Shareholders[i].ICType!=null && string.IsNullOrEmpty(model.Shareholders[i].ICType.Code))
            //        {
            //            var shareholdersKeys = ModelState.Where(x => x.Key.Contains($"Shareholders[{i}].ICType")).Select(y => y.Key).ToList();
            //            if (shareholdersKeys.Any())
            //            {
            //                ModelState.Remove($"Shareholders[{i}].ICNumber");
            //            }
            //            foreach (string key in shareholdersKeys)
            //            {
            //                ModelState.Remove(key);
            //            }
            //        }

            //    }
            //}
            //if (model.Directors != null)
            //{
            //    for (var i = 0; i < model.Directors.Count; i++)
            //    {
            //        //string key = $"Directors[{i}].ICType.ID";
            //        //ModelState.Remove(key);
            //        if (model.Directors[i].ICType != null && string.IsNullOrEmpty(model.Directors[i].ICType.Code))
            //        {
            //            var directorsKeys = ModelState.Where(x => x.Key.Contains($"Directors[{i}].ICType")).Select(y => y.Key).ToList();
            //            if (directorsKeys.Any())
            //            {
            //                ModelState.Remove($"Directors[{i}].ICNumber");
            //            }
            //            foreach (string key in directorsKeys)
            //            {
            //                ModelState.Remove(key);
            //            }
            //        }
            //    }
            //}

            RemoveValidationsForNotRequiredFileds(model);
            var output = Validate(model);
            output.RedirectUrl = Url.Action("CorporateConfirm");
            return Json(output);
        }

        public void RemoveValidationsForNotRequiredFileds(CorporateRegistrationViewModel model)
        {
            ModelState.Remove("Agency.DateOfExam");
            if (model.Shareholders != null)
            {
                for (var i = 0; i < model.Shareholders.Count; i++)
                {
                    //string key = $"Shareholders[{i}].ICType.ID";
                    //ModelState.Remove(key);
                    if (model.Shareholders[i].ICType != null && string.IsNullOrEmpty(model.Shareholders[i].ICType.Code))
                    {
                        var shareholdersKeys = ModelState.Where(x => x.Key.Contains($"Shareholders[{i}].ICType")).Select(y => y.Key).ToList();
                        if (shareholdersKeys.Any())
                        {
                            ModelState.Remove($"Shareholders[{i}].ICNumber");
                        }
                        foreach (string key in shareholdersKeys)
                        {
                            ModelState.Remove(key);
                        }
                    }

                }
            }
            if (model.Directors != null)
            {
                for (var i = 0; i < model.Directors.Count; i++)
                {
                    //string key = $"Directors[{i}].ICType.ID";
                    //ModelState.Remove(key);
                    if (model.Directors[i].ICType != null && string.IsNullOrEmpty(model.Directors[i].ICType.Code))
                    {
                        var directorsKeys = ModelState.Where(x => x.Key.Contains($"Directors[{i}].ICType")).Select(y => y.Key).ToList();
                        if (directorsKeys.Any())
                        {
                            ModelState.Remove($"Directors[{i}].ICNumber");
                        }
                        foreach (string key in directorsKeys)
                        {
                            ModelState.Remove(key);
                        }
                    }
                }
            }

        }

        [HttpPost]
        public ActionResult CorporateConfirm(CorporateRegistrationViewModel model)
        {
            RemoveValidationsForNotRequiredFileds(model);

            return Confirm(model);
        }

        [HttpPost]
        public ActionResult CorporateSubmit(CorporateRegistrationViewModel model)
        {
            RemoveValidationsForNotRequiredFileds(model);
            var output = Add(model);
            return Json(output);
        }

        [HttpPost]
        public JsonResult InclusionSubmit(RegistrationInclusionViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var photo = dataProvider.Get<string>(GlobalConstants.CurrentPhoto);
            model.PhotoPath = photo;
            registrationService.Include(model);
            output = GetResponse(registrationService.CurrentContext);
            if (output.Result == false) return Json(output);
            output.RedirectUrl = Url.Action("Complete");
            output.Data = new { ID = model.AgencyID };
            dataProvider.Remove(GlobalConstants.CurrentPhoto);
            return Json(output);
        }

        [HttpPost]
        public JsonResult ReinstationSubmit(RegistrationReinstationViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            registrationService.Reinstate(model);
            output = GetResponse(registrationService.CurrentContext);
            output.RedirectUrl = Url.Action("Complete");
            output.Data = new { ID = model.AgencyID };
            return Json(output);
        }

        [HttpPost]
        public JsonResult ConflictSubmit(RegistrationConflictViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return Json(output);
            var list = dataProvider.Get<List<ConflictAttachmentViewModel>>(GlobalConstants.CurrentConflictAttachments);
            foreach (var item in list) model.ConflictAttachments.Add(item);
            var id = registrationService.Add(model);
            output = GetResponse(registrationService.CurrentContext);
            output.RedirectUrl = Url.Action("ConflictComplete");
            output.Data = new { ID = id };
            return Json(output);
        }

        public ActionResult Complete(long id)
        {
            var model = agencyBuilder.GetAgency(id);
            return PartialView(model);
        }

        public ActionResult ConflictComplete(long id)
        {
            var model = agencyBuilder.GetAgency(id);
            return PartialView(model);
        }

        JsonResponseModel Validate(RegistrationViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return output;
            var photo = dataProvider.Get<string>(GlobalConstants.CurrentPhoto);
            model.PhotoPath = photo;
            var option = registrationService.Check(model);
            output = GetResponse(registrationService.CurrentContext);
            return output;
        }

        ActionResult Confirm(RegistrationViewModel model)
        {
            EnsureGeneralFork(model);
            var output = agencyBuilder.GetAgency(model);
            var photo = dataProvider.Get<string>(GlobalConstants.CurrentPhoto);
            output.PhotoPath = string.IsNullOrEmpty(photo) ? "" : photo.Replace(config.UploadBaseDirectory + @"\Photo\", config.PhotoUrl);
            return PartialView("Confirm", output);
        }

        JsonResponseModel Add(RegistrationViewModel model)
        {
            EnsureGeneralFork(model);
            var output = GetResponse();
            if (output.Result == false) return output;
            var photo = dataProvider.Get<string>(GlobalConstants.CurrentPhoto);
            model.PhotoPath = photo;
            var id = registrationService.Add(model);
            output = GetResponse(registrationService.CurrentContext);
            output.RedirectUrl = Url.Action("Complete");
            output.Data = new { ID = id };
            dataProvider.Remove(GlobalConstants.CurrentPhoto);
            return output;
        }


        /* Conflict documents upload */

        public void UploadConflictDocuments(HttpPostedFileBase[] conflictDocuments)
        {
            var list = dataProvider.Get<List<ConflictAttachmentViewModel>>(GlobalConstants.CurrentConflictAttachments);
            foreach (var file in conflictDocuments)
            {
                string path = Path.Combine(config.UploadBaseDirectory, "ConflictAttachments");
                path = Path.Combine(path, String.Format("{0}{1}", Guid.NewGuid(), file.FileName));
                file.SaveAs(path);
                var attachment = new ConflictAttachmentViewModel
                {
                    FilePath = path,
                    UploadFileName = file.FileName
                };
                list.Add(attachment);
            }
        }



        public void RemoveConflictDocuments(string[] fileNames)
        {
            var list = dataProvider.Get<List<ConflictAttachmentViewModel>>(GlobalConstants.CurrentConflictAttachments);
            foreach (var name in fileNames)
            {
                var attachment = list.Where(p => p.UploadFileName == name).First();
                System.IO.File.Delete(attachment.FilePath);
                list.Remove(attachment);
            }
        }

        public void UploadPhoto(HttpPostedFileBase photo)
        {
            string path = Path.Combine(config.UploadBaseDirectory, "Photo");
            path = Path.Combine(path, String.Format("{0}{1}", Guid.NewGuid(), photo.FileName));
            photo.SaveAs(path);
            dataProvider.Register(GlobalConstants.CurrentPhoto, path);
        }

    }// class
}// namesapce
