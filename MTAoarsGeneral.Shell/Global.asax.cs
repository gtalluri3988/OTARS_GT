using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Services;
using MTAoarsGeneral.Shell.Controllers;
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


namespace MTAoarsGeneral.Shell
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ServiceLoader.Load();
            RegisterControllers();
            RegisterModelbinders();
            RegisterBuilders();
            DependencyResolver.SetResolver(new UnityContainerResolver());

            FluentValidationModelValidatorProvider.Configure(provider => {
                provider.ValidatorFactory = new UnityValidatorFactory(ObjectContainer.Container);
            });

            var oldProvider = FilterProviders.Providers.Single(p => p is FilterAttributeFilterProvider);
            FilterProviders.Providers.Remove(oldProvider);
            FilterProviders.Providers.Add(new UnityContainerFilterAttributeFilterProvider());

        }

        protected void Session_Start() {
            // The following code will be removed after login implementation
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = (ObjectContainer.Container.Resolve<IUserService>()).Get("817User");
            dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
        }

        void RegisterControllers() {
            var container = ObjectContainer.Container;
            container.RegisterType<AgencyController>();
            container.RegisterType<HomeController>();
            container.RegisterType<RegistrationController>();
            container.RegisterType<AccountController>();
            container.RegisterType<TerminationController>();
            container.RegisterType<RenewalController>();
            container.RegisterType<MailController>();
            container.RegisterType<InvoiceController>();
            container.RegisterType<IFilterProvider, UnityContainerFilterAttributeFilterProvider>();
            container.RegisterType<IViewEngine, RazorViewEngine>();
            container.RegisterType<MaintenanceController>();
        }

        void RegisterModelbinders() {
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();
            ModelBinders.Binders.Add(typeof(DateTime), new DateModelBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new DateModelBinder());
        }

        void RegisterBuilders() {
            var container = ObjectContainer.Container;
            container.RegisterType<IAgencyBuilder, AgencyBuilder>();
            container.RegisterType<IRegistrationBuilder, RegistrationBuilder>();
            container.RegisterType<IRenewalBuilder, RenewalBuilder>();
            container.RegisterType<IMailBuilder, MailBuilder>();
        }

       
    }// class
}// namespace