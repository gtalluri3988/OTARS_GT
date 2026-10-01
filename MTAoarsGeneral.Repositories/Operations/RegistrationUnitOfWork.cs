using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Shared;
using System.Data.Objects;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Repositories.Operations
{

    public class RegistrationUnitOfWork : IRegistrationUnitOfWork
    {
        EntityContext context;

        public RegistrationUnitOfWork(EntityContext context, IObjectCreator objectCreator, IScopeDataProvider dataProvider)
        {
            AgencyRepository = new AgencyRepository(context, dataProvider, objectCreator.Create<ConfigManager>(), objectCreator.Create<IMemberRepository>(), objectCreator.Create<ILookupRepository>());
            MemberRepository = new MemberRepository(context);
            AgencyMemberRepository = new GenericRepository<AgencyMember>(context);
            AgencyPrincipalRepository = new GenericRepository<AgencyPrincipal>(context);
            AgencyPrincipalGuarantorRepository = new GenericRepository<AgencyPrincipalGuarantor>(context);
            AgencyPrincipalHistoryRepository = new GenericRepository<AgencyPrincipalHistory>(context);
            AgencyPrincipalGuarantorHistoryRepository = new GenericRepository<AgencyPrincipalGuarantorHistory>(context);
            AgencyPrincipalStatusHistoryRepository = new GenericRepository<AgencyPrincipalStatusHistory>(context);
            RunnerRepository = new RunnerRepository(context);
            LookupRepository = new LookupRepository(context);
            CompanyRepository = new GenericRepository<Company>(context);
            JournalRepository = new GenericRepository<Journal>(context);
            PhotoRepository = new GenericRepository<PhotoHistory>(context);
            this.context = context;
        }


        public IAgencyRepository AgencyRepository
        {
            get; private set;
        }

        public IMemberRepository MemberRepository
        {
            get; private set;
        }

        public IRepository<AgencyMember> AgencyMemberRepository
        {
            get; private set;
        }

        public IRepository<AgencyPrincipal> AgencyPrincipalRepository
        {
            get; private set;
        }

        public IRepository<AgencyPrincipalGuarantor> AgencyPrincipalGuarantorRepository
        {
            get; private set;
        }

        public IRepository<AgencyPrincipalHistory> AgencyPrincipalHistoryRepository
        {
            get;
            private set;
        }

        public IRepository<AgencyPrincipalGuarantorHistory> AgencyPrincipalGuarantorHistoryRepository
        {
            get;
            private set;
        }

        public IRepository<AgencyPrincipalStatusHistory> AgencyPrincipalStatusHistoryRepository
        {
            get;
            private set;
        }

        public IRepository<MemberExperience> MemberExperienceRepository
        {
            get;
            private set;
        }

        public IRepository<Spouse> SpouseRepository
        {
            get;
            private set;
        }

        public IRunnerRepository RunnerRepository
        {
            get;
            private set;
        }

        public ILookupRepository LookupRepository
        {
            get;
            private set;
        }

        public IRepository<Company> CompanyRepository
        {
            get;
            private set;
        }

        public IRepository<Journal> JournalRepository
        {
            get;
            private set;
        }

        public IRepository<PhotoHistory> PhotoRepository
        {
            get;
            private set;
        }

        public void Save()
        {
            context.SaveChanges();
            context.AcceptAllChanges();
        }


    }// class

}// namespace
