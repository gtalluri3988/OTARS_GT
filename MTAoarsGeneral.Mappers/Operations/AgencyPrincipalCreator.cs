using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Extensions;
using AutoMapper;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations {
    public class AgencyPrincipalCreator {

        ILookupRepository lookupRepository;
        IRunnerRepository runnerRepository;
        IScopeDataProvider dataProvider;


        public AgencyPrincipalCreator(ILookupRepository lookupRepository, IRunnerRepository runnerRepository, IScopeDataProvider dataProvider) {
            this.lookupRepository = lookupRepository;
            this.runnerRepository = runnerRepository;
            this.dataProvider = dataProvider;
        }



        public AgencyPrincipal GetPrincipal(IExistableCompanyGuarantor source, long intermediaryTypeId,long typeID, bool isInclusion = false, long? memberID = null, string photoPath = null) {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var principal = new AgencyPrincipal();
            principal.CompanyID = source.CompanyID;
            principal.ValidFrom = DateTime.Now;
            principal.ValidTo = DateTime.Now.GetCurrentQuarterEndDate().AddYears(2);
            principal.IntermediaryTypeID = intermediaryTypeId;
            principal.AgencyNumber = GetAgencyNumber(intermediaryTypeId);
            principal.IsInclusion = isInclusion;
            principal.IsActive = true;
            principal.DateAppointed = source.DateAppointed;
            principal.TypeID = typeID;
            principal.IsBancaStaff = source.CorporateNominee.IsBancaStaff;
            principal.MemberID = memberID;
            principal.PhotoPath = photoPath;
            var guarantor = Mapper.Map<GuarantorViewModel, AgencyPrincipalGuarantor>(source.Guarantor);
            principal.AgencyPrincipalGuarantors.Add(guarantor);
            return principal;
        }

        public AgencyPrincipalConflict GetPrincipalConflict(RegistrationConflictViewModel source, long intermediaryTypeId, bool isInclusion = false) {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var conflict = Mapper.Map<AgencyPrincipalConflict>(source);
            
            conflict.IntermediaryTypeID = intermediaryTypeId;
            conflict.DestinationCompanyID = source.CompanyID;
            conflict.TypeID = source.AgencyType.ID;
            conflict.StatusID = lookupRepository.Get<LookupConflictStatus>(LookupConstants.ConflictStatus.Open).ID;
            foreach (var item in source.ConflictAttachments) {
                var attachment = Mapper.Map<ConflictAttachment>(item);
                conflict.ConflictAttachments.Add(attachment);
            }
            var guarantor = Mapper.Map<ConflictGuarantor>(source.Guarantor);
            conflict.ConflictGuarantors.Add(guarantor);
            return conflict;
        }

        public AgencyPrincipal GetPrincipal(AgencyPrincipalConflict source) {
            var principal = new AgencyPrincipal();
            principal.CompanyID = source.DestinationCompanyID;
            principal.ValidFrom = DateTime.Now;
            principal.ValidTo = DateTime.Now.GetCurrentQuarterEndDate().AddYears(2);
            var generalId = lookupRepository.Get<LookupIntermediaryType>(LookupConstants.IntermediaryType.General).ID;
            principal.IntermediaryTypeID = generalId;
            principal.TypeID = source.TypeID;
            principal.AgencyNumber = GetAgencyNumber(generalId);
            principal.IsInclusion = source.Agency.AgencyPrincipals.Count > 0;
            principal.IsActive = true;
            principal.DateAppointed = source.DateAppointed;
            principal.ModifiedBy = source.CreatedBy;
            principal.ModifiedDate = source.CreatedDate;
            if (source.ConflictGuarantors.Count > 0) {
                var guarantor = Mapper.Map<AgencyPrincipalGuarantor>(source.ConflictGuarantors.First());
                principal.AgencyPrincipalGuarantors.Add(guarantor);
            }
            return principal;
        }

        string GetAgencyNumber(long intermediaryTypeId) {
            var code = lookupRepository.Get<LookupIntermediaryType>(intermediaryTypeId).Code;
            return runnerRepository.GetNext(LookupConstants.Runner.AgencyRegistrationNumber);
        }

    }// class
}// namespace
