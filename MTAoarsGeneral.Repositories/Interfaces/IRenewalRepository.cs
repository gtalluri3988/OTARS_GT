using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;


namespace MTAoarsGeneral.Repositories.Interfaces {
    public interface IRenewalRepository : IRepository<RenewalHeader> {

        IEnumerable<long> GetRenewableCompanies(DateTime validToDate);

        IEnumerable<AgencyPrincipal> GetRenewableAgencies(long companyId, DateTime validToDate);

        IEnumerable<RenewalHeader> GetGeneratedRenewalHeaders(long companyId);

        IEnumerable<RenewalReminderViewDetail> GetReminderDetails(int year, int quarter);

        IEnumerable<RenewalHeader> GetSubmittedRenewalHeaders();

        IEnumerable<RenewalHeader> GetHeaders(int year, int quarter);

        RenewalHeader Get(long companyId, int year, int quarter);

        IEnumerable<RenewalViewDetail> GetRenwalDetails(int pageNo, int pageSize, string search, string orderBy, long headerId, bool isSubmitted);

        int GetRenewalDetailsCount(long headerId, string search, bool isSubmitted);

        int GetRenewalDetailsCount(bool isSubmitted, long? companyId = null);

        RenewalDetail GetInProcessDetail(long agencyId, long companyId, long intermediaryTypeId);

        //Remark new added
        List<Company> GetCompanyNames(string CompanyName);

        RenewalViewDetail RenewalViewDetail(long ID);
    }
}
