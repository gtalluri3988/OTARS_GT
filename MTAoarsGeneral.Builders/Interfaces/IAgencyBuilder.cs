using MTAoarsGeneral.ViewModels.Operations;
using System.Collections.Generic;
using System;

namespace MTAoarsGeneral.Builders.Interfaces {
    public interface IAgencyBuilder : IBaseBuilder {

        AgencyDisplayViewModel GetAgency(long agencyId);
        AgencyDisplayViewModel GetAgencyByPrincipal(long principalId);
        AgencyDisplayViewModel GetAgency(RegistrationViewModel model);
        AgencyDisplayViewModel GetAgency(long agencyId, long agencyPrincipalID, long memberID);
        IEnumerable<T> Search<T>(AgencySearchRequestViewModel request);
        IEnumerable<T> Search<T>(AgencySearchRequestViewModel request,string maintenanceType) where T :MaintenanceSearchResponseViewModel;
        IEnumerable<T> SearchArchive<T>(AgencySearchRequestViewModel request);
        T Search<T>(string agencyNumber);
        T SearchHistory<T>(string agencyNumber);
        T Search<T>(string agencyNumber, long companyId) where T:class;
        TBEEnquirySearchViewModel Search(string icNumber);
        IEnumerable<T> Search<T>(DateTime fromDate, DateTime toDate);
        PhotoUploadEnquirySearchViewModel SearchPhotoUpload(string agencyNumber);
        TBEEnquiryResponseViewModel getResult(int id, string Exam);
        void SaveTBEResult(TBEEnquirySearchViewModel model,out string message);
    }// interface
}// namespace
