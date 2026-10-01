using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using AutoMapper;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Mappers.Operations
{

    class AddressResolver : ValueResolver<RenewalViewDetail, string>
    {
        IAgencyRepository agencyRepository;
        public AddressResolver(IAgencyRepository agencyRepository)
        {
            this.agencyRepository = agencyRepository;
        }

        protected override string ResolveCore(RenewalViewDetail src)
        {
            var agencyPrincipal = agencyRepository.GetAgencyPrincipal(src.AgencyNumber);
            var address = agencyPrincipal.Agency.Address;

            var parts = new List<string>();            
            parts.Add(address.Address1);
            parts.Add(address.Address2);
            parts.Add(address.PostalCode);
            parts.Add(address.City);
            parts.Add((address.StateID == null) ? "" : address.LookupState.Description);

            parts = parts.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            if (parts.Count > 0)
                return string.Join(", ", parts);
            return "";
        }

    }
}
