using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;


namespace MTAoarsGeneral.Repositories.Interfaces
{
    public interface IAgencyRepository : IRepository<Agency>
    {

        IEnumerable<SearchAgencyResult> Search(string agencyNumber, string memberName, string icNumber, string businessRegistrationNumber, string newBusinessRegistrationNumber, long companyId, bool isGeneral, bool isFamily);

        IEnumerable<SearchAgencyArchiveResult> SearchArchive(string agencyNumber, string memberName, string icNumber, string businessRegistrationNumber, long companyId);

        //Agency SelectOtherTypeAgency(string businessRegistrationNumber, string currentTypeCode);

        AgencyPrincipalHistory GetFullReinstateHistory(string agencyNumber);

        AgencyPrincipalHistory GetReinstateHistory(string icNumber, long companyId, long intermediaryTypeId, long agencyTypeId);

        AgencyPrincipalHistory GetReinstateHistory(long agencyId, long companyId, long intermediaryTypeId, long agencyTypeId);

        AgencyPrincipalHistory GetCorporateNomineeReinstateHistory(string icNumber, long companyId, long intermediaryTypeId, long agencyTypeId);

        SearchAgencyResult Search(string agencyNumber);

        SearchAgencyResult SearchHistories(string agencyNumber);

        IEnumerable<AsciiReportDetail> Search(DateTime fromDate, DateTime toDate);

        Agency Select(string businessRegistrationNumber);
        Agency Select(string businessRegistrationNumber, bool isNew);

        List<Agency> SelectByRegistrationNumber(string businessRegistrationNumber, string newBusinessRegistrationNumber);

        List<AgencyPrincipal> Enquiry(String registrationNumber, String ICNumber, long companyId);

        AgencyPrincipal GetAgencyPrincipal(string agencyNumber);

        AgencyPrincipal GetAgencyPrincipal(long id);

        AgencyPrincipalHistory GetAgencyPrincipalHistory(long id);
    }
}
