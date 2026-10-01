using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {

    public interface IAgencyUnitOfWork : IUnitOfWork {

        IAgencyRepository AgencyRepository { get; }
        IRepository<AgencyPrincipal> AgencyPrincipalRepository { get; }
        IRepository<AgencyPrincipalGuarantor> AgencyPrincipalGuarantorRepository { get; }
        IRepository<AgencyPrincipalStatus> AgencyPrincipalStatusRepository { get; }
        IRepository<AgencyPrincipalHistory> AgencyPrincipalHistoryRepository { get; }
        IRepository<AgencyPrincipalGuarantorHistory> AgencyPrincipalGuarantorHistoryRepository { get; }
        IRepository<AgencyPrincipalStatusHistory> AgencyPrincipalStatusHistoryRepository { get; }
        IRepository<Journal> JournalRepository { get; }
        IConflictRepository ConflictRepository { get; }
        ILookupRepository LookupRepository { get; }
        IRepository<PhotoHistory> PhotoRepository { get; }
    }// interface

}// namespace
