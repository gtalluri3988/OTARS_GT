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

    class AgencyPrincipalGuarantorHistoryMappingAction : IMappingAction<AgencyPrincipal, AgencyPrincipalHistory> {

        public void Process(AgencyPrincipal source, AgencyPrincipalHistory destination) {
            foreach (var item in source.AgencyPrincipalGuarantors) {
                destination.AgencyPrincipalGuarantorHistories.Add(Mapper.Map<AgencyPrincipalGuarantorHistory>(item));
            }
        }

    }// class
}// namespace
