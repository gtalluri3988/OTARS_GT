using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Services;
using MTAoarsGeneral.Web.Controllers;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Builders.Operations;
using MTAoarsGeneral.Services.Interfaces;
using FluentValidation.Mvc;
using FluentValidation;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Validators.Operations.Fluents;
using MTAoarsGeneral.Builders.Notifications;
using System.Web;

namespace MTAoarsGeneral.Web {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new NoCacheAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.RouteExistingFiles = true;
            //routes.IgnoreRoute("Upload/Registration/{file}");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Headers.Add("Pragma", "no-cache");
            Response.Headers.Add("Cache-Control", "no-store, no-cache, must-revalidate, max-age=0");
        }
        protected void Application_PostAcquireRequestState(Object sender, EventArgs e)
        {
            // PCI DSS idle (15 min) and absolute session timeouts; expired sessions are sent to SSOUrl.
            SessionTimeoutManager.Enforce(Context);
        }
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ServiceLoader.Load();
            RegisterControllers();
            RegisterModelbinders();
            DependencyResolver.SetResolver(new UnityContainerResolver());

            FluentValidationModelValidatorProvider.Configure(provider => {
                provider.ValidatorFactory = new UnityValidatorFactory(ObjectContainer.Container);     
            });

            var oldProvider = FilterProviders.Providers.Single(p => p is FilterAttributeFilterProvider);
            FilterProviders.Providers.Remove(oldProvider);
            FilterProviders.Providers.Add(new UnityContainerFilterAttributeFilterProvider());

            //Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
            //Response.Cache.AppendCacheExtension("no-store, must-revalidate");
            //Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            //Response.AppendHeader("Expires", "0"); // Proxies.
        }

        protected void Session_Start() {
            // The following code will be removed after login implementation
            //var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            //var identity = (ObjectContainer.Container.Resolve<IUserService>()).Get("up_817");
            //dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
        }

        void Session_End(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }
        void RegisterControllers()
        {
            var container = ObjectContainer.Container;

            container.RegisterType<HomeController>();
            container.RegisterType<RegistrationController>();
            container.RegisterType<LookupController>();
            container.RegisterType<TerminationController>();
            container.RegisterType<AgencyController>();
            container.RegisterType<MailController>();
            container.RegisterType<RenewalController>();
            container.RegisterType<InvoiceController>();
            container.RegisterType<AccountController>();
            container.RegisterType<MaintenanceController>();
            container.RegisterType<SetupController>();
            container.RegisterType<CPDController>();
            container.RegisterType<CBCController>();
            container.RegisterType<UploadController>();
            container.RegisterType<ComplaintController>();
            container.RegisterType<ReportController>();
            container.RegisterType<ReferredController>();
            container.RegisterType<ConflictController>();
            container.RegisterType<ALCController>();
            container.RegisterType<EnquiryController>();
            container.RegisterType<IFilterProvider, UnityContainerFilterAttributeFilterProvider>();
            container.RegisterType<IViewEngine, RazorViewEngine>();
            container.RegisterType<AdministrativeController>();
        }

        void RegisterModelbinders()
        {
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), new DateModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(Decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(long), new LongModelBinder());
            ModelBinders.Binders.Add(typeof(long?), new LongModelBinder());
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
                logger.Error(exception);
        }

    }// class
}// namespace