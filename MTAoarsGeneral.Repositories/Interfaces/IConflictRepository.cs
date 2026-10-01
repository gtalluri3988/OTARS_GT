using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;


namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface IConflictRepository : IRepository<AgencyPrincipalConflict> {

        IEnumerable<AgencyPrincipalConflict> SelectOpenConflicts(int days);

        IEnumerable<AgencyPrincipalConflict> SelectConflicts(int days, String statusCode);
        
        AgencyPrincipalConflict GetOpenConflict(long agencyId, long companyId);

        AgencyPrincipalConflict GetNotClosedConflict(long agencyId, long companyId);

        AgencyPrincipalConflict GetNotClosedAndNotRejectedConflict(long agencyId, long companyId);

        AgencyPrincipal GetSourceCompanyPrincipal(long id);

        bool IsAgencyPrincipalExists(long agencyId, long companyId, long intermediaryTypeId);
    }
}
