using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Mappers;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Operations;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Services.Operations;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Accounts;
using MTAoarsGeneral.Services.Accounts;
using MTAoarsGeneral.Repositories.Notifications;
using MTAoarsGeneral.Services.Notifications;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Validators.Operations.Fluents;
using MTAoarsGeneral.Services.Reports;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Builders.Operations;
using MTAoarsGeneral.Builders.Notifications;
using MTAoarsGeneral.Repositories.Maintenance;
using MTAoarsGeneral.Services.Maintenance;
using MTAoarsGeneral.Repositories.Administrative;
using MTAoarsGeneral.Builders.Administrative;

namespace MTAoarsGeneral.Services
{

    public static class ServiceLoader
    {

        public static void Load()
        {
            var container = ObjectContainer.Container;
            RegisterGlobalSettings(container);
            RegisterRepositories(container);
            RegisterServices(container);
            RegisterViewModel(container);
            RegisterMaps(container);
            RegisterValidators(container);
            RegisterBuilders(container);
            /*container.AddNewExtension<Interception>();

            container.Configure<Interception>()
               .SetInterceptorFor<ILookupRepository>(new InterfaceInterceptor());

            container.RegisterType<ILookupRepository, LookupRepository>(new InterceptionBehavior<PolicyInjectionBehavior>(),
            new Interceptor<TransparentProxyInterceptor>());*/
        }

        static void RegisterGlobalSettings(IUnityContainer container)
        {
            container.RegisterType<ConfigManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IObjectCreator, UnityObjectCreator>();
            container.RegisterType<IValidationProvider, ValidationProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IScopeDataProvider, SessionScopeDataProvider>();
        }

        static void RegisterRepositories(IUnityContainer container)
        {
            var connectionString = container.Resolve<ConfigManager>().EntityConextConnectionString;
            container.RegisterType<EntityContext>(new InjectionConstructor(connectionString));
            //container.RegisterType<EntityContext>(new PerThreadLifetimeManager(), new InjectionConstructor(connectionString));
            container.RegisterType<IIbfimResultRepository, IbfimResultRepository>();
            container.RegisterType<IMiiResultRepository, MiiResultRepository>();
            container.RegisterType<ITbeExemptionRepository, TbeExemptionRepository>();
            container.RegisterType<ILookupRepository, LookupRepository>();
            container.RegisterType<IRegistrationUnitOfWork, RegistrationUnitOfWork>();
            container.RegisterType<IRunnerRepository, RunnerRepository>();
            container.RegisterType<IMemberRepository, MemberRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IAgencyRepository, AgencyRepository>();
            container.RegisterType(typeof(IRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IAgencyUnitOfWork, AgencyUnitOfWork>();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IActivityRepository, ActivityRepository>();
            container.RegisterType<INotificationRepository, NotificationRepository>();
            container.RegisterType<IRenewalRepository, RenewalRepository>();
            container.RegisterType<IMailRepository, MailRepository>();
            container.RegisterType<IInvoiceRepository, InvoiceRepository>();
            container.RegisterType<IMenuRepository, MenuRepository>();
            container.RegisterType<ICPDRepository, CPDRepository>();
            container.RegisterType<ICBCRepository, CBCRepository>();
            container.RegisterType<IReferredRepository, ReferredRepository>();
            container.RegisterType<IConflictRepository, ConflictRepository>();
            container.RegisterType<IUploadRepository, UploadRepository>();
            container.RegisterType<IALCRepository, ALCRepository>();
            container.RegisterType<IPhotoRepository, PhotoRepository>();
            container.RegisterType<IReferredMemberRepository, ReferredMemberRepository>();
            container.RegisterType<IAdministrativeUnitOfWork, AdministrativeUnitOfWork>();
        }

        static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IRegistrationService, RegistrationService>();
            container.RegisterType<ILookupService, LookupService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IAgencyService, AgencyService>();
            container.RegisterType<INotificaitonService, NotificationService>();
            container.RegisterType<IRenewalService, RenewalService>();
            container.RegisterType<ITerminationService, TerminationService>();
            container.RegisterType<IMailService, MailService>();
            container.RegisterType<IHomeService, HomeService>();
            container.RegisterType<IInvoiceService, InvoiceService>();
            container.RegisterType<IMenuService, MenuService>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IActivityService, ActivityService>();
            container.RegisterType<ICPDService, CPDService>();
            container.RegisterType<ICBCService, CBCService>();
            container.RegisterType<IUploadService, UploadService>();
            container.RegisterType<IReferredService, ReferredService>();
            container.RegisterType<IConflictService, ConflictService>();
            container.RegisterType<IALCService, ALCService>();
            container.RegisterType<IAdministrativeService, AdministrativeService>();
        }

        static void RegisterBuilders(IUnityContainer container)
        {
            container.RegisterType<IAgencyBuilder, AgencyBuilder>();
            container.RegisterType<IRegistrationBuilder, RegistrationBuilder>();
            container.RegisterType<IRenewalBuilder, RenewalBuilder>();
            container.RegisterType<IMailBuilder, MailBuilder>();
            container.RegisterType<ITerminationBuilder, TerminationBuilder>();
            container.RegisterType<ICBCBuilder, CBCBuilder>();
            container.RegisterType<IReferredBuilder, ReferredBuilder>();
        }

        static void RegisterViewModel(IUnityContainer container)
        {
            container.RegisterType(typeof(List<>), new InjectionConstructor());
        }

        static void RegisterMaps(IUnityContainer container)
        {
            Mapper.Initialize(cnfg => {
                cnfg.ConstructServicesUsing(type => container.Resolve(type));
            });
            var assembly = Assembly.Load(new AssemblyName("MTAoarsGeneral.Mappers"));
            var list = assembly.GetTypes()
                         .Where(type => type.GetInterfaces().Any(intr => intr == typeof(IMapper)));
            Parallel.ForEach(list,
                     item => (container.Resolve(item) as IMapper).Map()
                     );
        }

        static void RegisterValidators(IUnityContainer container)
        {
            var assembly = Assembly.Load(new AssemblyName("MTAoarsGeneral.Validators"));
            var list = assembly.GetTypes()
                        .Where(type => type.GetCustomAttributes(typeof(FluentRegisterableAttribute), true).Any());
            Parallel.ForEach(list,
                item => {
                    var genericArgument = item.BaseType.GetGenericArguments().First();
                    var sourceType = typeof(FluentValidation.IValidator<>);
                    container.RegisterType(sourceType.MakeGenericType(genericArgument), item);
                });

            container.RegisterType<FluentValidation.IValidator<CorporateRegistrationViewModel>, AgencyBoardMembersValidator<CorporateRegistrationViewModel>>();
        }
    }// class

}// namespace
