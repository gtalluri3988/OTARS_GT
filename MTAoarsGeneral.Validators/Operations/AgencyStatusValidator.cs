using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Config;

namespace MTAoarsGeneral.Validators.Operations {
    public class AgencyStatusValidator : IValidator<IMemberViewModel> {
        IMemberRepository memberRepository;
        ILookupRepository lookupRepository;
        ConfigManager config;

        public AgencyStatusValidator(IMemberRepository memberRepository, ILookupRepository lookupRepository, ConfigManager config) {
            this.memberRepository = memberRepository;
            this.lookupRepository = lookupRepository;
            this.config = config;
        }

        public IEnumerable<ValidationMessage> Validate(IMemberViewModel item) {
            if (!config.CPDCheckingAgainstRegistration) yield break;
            var agencyStatuses = memberRepository.GetAgencies(item.ICNumber).SelectMany(p => p.AgencyStatus)
                .Where(p => (p.FromDate <= DateTime.Now && p.ToDate >= DateTime.Now || p.ToDate == null) && p.IsActive == true);
            var status = agencyStatuses.FirstOrDefault();
            if (status == null) yield break;
            yield return new ValidationMessage("", "This Agent / Corporate Nominee  \"{0}\" is currently in the status {1}", item.ICNumber, status.LookupAgencyStatu.Description);
        }

    }// class
}// namespace
