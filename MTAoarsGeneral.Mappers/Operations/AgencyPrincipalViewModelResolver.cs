using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using System.Data.Objects.DataClasses;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Extensions;
using AutoMapper;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;


namespace MTAoarsGeneral.Mappers.Operations {

    class AgencyPrincipalViewModelResolver : ValueResolver<Agency, List<AgencyPrincipalDisplayViewModel>> {
       
        public AgencyPrincipalViewModelResolver() {
           
        }

        protected override List<AgencyPrincipalDisplayViewModel> ResolveCore(Agency source) {
            var output = new List<AgencyPrincipalDisplayViewModel>();
            foreach (var ap in source.AgencyPrincipals.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General)) {
                output.Add(Mapper.Map<AgencyPrincipalDisplayViewModel>(ap));
            }
            foreach (var aph in source.AgencyPrincipalHistories.Where(p => p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General)) {
                output.Add(Mapper.Map<AgencyPrincipalDisplayViewModel>(aph));
            }
            return output;
        }

        
    }

}
