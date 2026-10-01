using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTAoarsGeneral.ViewModels.Accounts;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.ViewModels.Enquiries;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Config;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace MTAoarsGeneral.Web.Controllers
{
    public class EnquiryController : BaseController
    {

        IAgencyService agencyService;
        ConfigManager config;
        public EnquiryController(IAgencyService agencyService, ConfigManager config)
        {
            this.agencyService = agencyService;
            this.config = config;
        }

        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }

        //public ActionResult Search2()
        //{
        //    var imagepath = @"C:\tom3.jpg";
        //    var watermark = "MTA-F88990";

        //    Bitmap bitMapImage = new Bitmap(imagepath);
        //    Graphics graphicImage = Graphics.FromImage(bitMapImage);
        //    graphicImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        //    //graphicImage.DrawRectangle(new Pen(Color.Black), 0, bitMapImage.Height - 30, bitMapImage.Width, 30);
        //    //graphicImage.FillRectangle(new SolidBrush(Color.Black), 0, bitMapImage.Height - 30, bitMapImage.Width, 30);

        //    //var pen = new Pen(Color.Black, 5);
        //    //pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
        //    //graphicImage.DrawRectangle(pen, 30, bitMapImage.Height - 30, bitMapImage.Width - 60, 30);

        //    //graphicImage.DrawRectangle(new Pen(Color.LightBlue, 5), bitMapImage.Width / 2, bitMapImage.Height - 40, bitMapImage.Width / 2, 30);
        //    //graphicImage.FillRectangle(new SolidBrush(Color.DarkBlue), bitMapImage.Width / 2, bitMapImage.Height - 40, bitMapImage.Width / 2, 30);
        //    //graphicImage.FillRectangle(new SolidBrush(Color.FromArgb(58, 100, 168)), bitMapImage.Width - 170, bitMapImage.Height - 40, 170, 30);
        //    graphicImage.FillRectangle(new SolidBrush(Color.FromArgb(63, 160, 79)), bitMapImage.Width - 210, bitMapImage.Height - 70, 210, 50);

        //    //graphicImage.DrawString(watermark, new Font("Arial", 14, FontStyle.Bold), new SolidBrush(Color.DarkBlue), new Point((bitMapImage.Width / 2) - 45, bitMapImage.Height - 25));//new Point(bitMapImage.Width /2 - 45, 15));
        //    //graphicImage.DrawString(watermark, new Font("Arial", 16, FontStyle.Bold), new SolidBrush(Color.White), new Point((bitMapImage.Width / 2) + 20, bitMapImage.Height - 25 - 11));
        //    //graphicImage.DrawString(watermark, new Font("Arial", 16, FontStyle.Bold), new SolidBrush(Color.White), new Point((bitMapImage.Width - 150), bitMapImage.Height - 25 - 11));
        //    graphicImage.DrawString(watermark, new Font("Arial", 24, FontStyle.Bold), new SolidBrush(Color.White), new Point((bitMapImage.Width - 205), bitMapImage.Height - 63));

        //    byte[] bytes;

        //    using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
        //    {
        //        bitMapImage.Save(memory, bitMapImage.RawFormat);
        //        graphicImage.Dispose();
        //        bitMapImage.Dispose();
        //        bytes = memory.ToArray();
        //    }

        //    return File(bytes, "image/jpeg");
        //}

        [HttpPost]
        public JsonResult SearchSubmit(SearchViewModel model)
        {
            var output = GetResponse();
            //if (output.Result == false) return Json(output);
            if (model.CaptchaKey == null || model.CaptchaKey.Equals(Session[GlobalConstants.CaptchaKey]) == false)
            {
                output.AddError("SECURITY KEY DOES NOT MATCH");
                output.Result = false;
            }
            if (model.Company == null)
            {
                output.AddError("PLEASE SELECT TAKAFUL OPERATOR");
                output.Result = false;
            }
            if (String.IsNullOrEmpty(model.ICNumber) && String.IsNullOrEmpty(model.RegistrationNumber))
            {
                output.AddError("PLEASE SEARCH EITHER BY REGISTRATION NUMBER or NRIC NUMBER");
                output.Result = false;
            }
            if (!String.IsNullOrEmpty(model.ICNumber) && !String.IsNullOrEmpty(model.RegistrationNumber))
            {
                output.AddError("PLEASE SEARCH EITHER BY REGISTRATION NUMBER or NRIC NUMBER");
                output.Result = false;
            }
            return Json(output);
        }

        [HttpPost]
        public ActionResult Result(SearchViewModel model, LookupItem LookupCompany)
        {
            model.Company = LookupCompany;
            model.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
            var m = agencyService.Enquiry(model);
            m.ForEach(delegate (SearchResultViewModel agent)
            {
                agent.ICNumber = model.Company.ID.ToString();
                if (agent.ValidTo.HasValue)
                {
                    //agent.AgentName = agent.AgentName.ToProperCase();
                    //agent.Company = agent.Company.ToProperCase();
                    //agent.CorporateNominee = agent.CorporateNominee.ToProperCase();
                    QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                    QrCode qrCode = qrEncoder.Encode(string.Format(config.PhotoUrl.Replace("/images/", "/Enquiry/Result?code={0}&mta={1}"), agent.RegistrationNumber.Encrypt().Encode(), model.Company.ID.ToString().Encrypt().Encode()));
                    GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);

                    var agentPhotoFI = new FileInfo(config.UploadBaseDirectory + string.Format(@"\Photo\{0}.png", agent.RegistrationNumber));
                    if (!agentPhotoFI.Directory.Exists)
                        agentPhotoFI.Directory.Create();

                    using (FileStream stream = new FileStream(agentPhotoFI.FullName, FileMode.Create))
                    {
                        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                    }
                    agent.Qr = config.PhotoUrl + string.Format("{0}.png", agent.RegistrationNumber);
                }
                else
                {
                    agent.Qr = model.ICNumber;
                }
            });

            return View(m);
        }

        [HttpGet]
        public ActionResult Result(string code, string mta)
        {
            SearchViewModel model = new SearchViewModel();
            List<SearchResultViewModel> m = new List<SearchResultViewModel>();
            try
            {
                string decryptedagent = code.Decrypt();
                string decryptedcompany = mta.Decrypt();
                long id = 0;
                if (Int64.TryParse(decryptedcompany, out id))
                {
                    model.Company = new LookupItem() { ID = id };
                    model.RegistrationNumber = decryptedagent;
                    model.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                    m = agencyService.Enquiry(model);
                    m.ForEach(delegate (SearchResultViewModel agent)
                    {
                        agent.ICNumber = model.Company.ID.ToString();
                        if (agent.ValidTo.HasValue)
                        {
                            //agent.AgentName = agent.AgentName.ToProperCase();
                            //agent.Company = agent.Company.ToProperCase();
                            //agent.CorporateNominee = agent.CorporateNominee.ToProperCase();
                            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                            QrCode qrCode = qrEncoder.Encode(string.Format(config.PhotoUrl.Replace("/images/", "/Enquiry/Result?code={0}&mta={1}"), agent.RegistrationNumber.Encrypt().Encode(), model.Company.ID.ToString().Encrypt().Encode()));
                            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                            using (FileStream stream = new FileStream(config.UploadBaseDirectory + string.Format(@"\Photo\{0}.png", agent.RegistrationNumber), FileMode.Create))
                            {
                                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
                            }
                            agent.Qr = config.PhotoUrl + string.Format("{0}.png", agent.RegistrationNumber);
                        }
                        else
                        {
                            agent.Qr = model.ICNumber;
                        }
                    });
                }
                else
                {
                    return RedirectToAction("Search");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("Search");
            }

            return View(m);
        }

    }// class
}// namespace
