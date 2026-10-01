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
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.ViewModels.Interfaces;



namespace MTAoarsGeneral.Mappers.Operations {

    class AgencyPrincipalStatusHistoryMappingAction : IMappingAction<AgencyPrincipal, AgencyPrincipalHistory> {

        public void Process(AgencyPrincipal source, AgencyPrincipalHistory destination) {
            foreach (var item in source.AgencyPrincipalStatus) {
                destination.AgencyPrincipalStatusHistories.Add(Mapper.Map<AgencyPrincipalStatusHistory>(item));
            }
        }

    }// class
}// namespace
