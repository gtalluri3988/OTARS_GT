using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces
{
    public interface IAdministrativeUnitOfWork : IUnitOfWork
    {

        IAgencyRepository AgencyRepository { get; }
        IMemberRepository MemberRepository { get; }
        IRepository<AgencyMember> AgencyMemberRepository { get; }
        IRepository<AgencyPrincipal> AgencyPrincipalRepository { get; }
        IRepository<AgencyPrincipalGuarantor> AgencyPrincipalGuarantorRepository { get; }
        IRepository<AgencyPrincipalHistory> AgencyPrincipalHistoryRepository { get; }
        IRepository<AgencyPrincipalGuarantorHistory> AgencyPrincipalGuarantorHistoryRepository { get; }
        IRepository<AgencyPrincipalStatusHistory> AgencyPrincipalStatusHistoryRepository { get; }
        IRepository<MemberExperience> MemberExperienceRepository { get; }
        IRepository<Spouse> SpouseRepository { get; }
        IRepository<Company> CompanyRepository { get; }
        IRepository<Journal> JournalRepository { get; }
        IRunnerRepository RunnerRepository { get; }
        ILookupRepository LookupRepository { get; }
        IRepository<PhotoHistory> PhotoRepository { get; }
        IRepository<AdministrativeAuditTrail> AdministrativeAuditTrailRepository { get; }
    }// interface
}
