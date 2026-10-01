using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Mappers.Operations {

    class TerminationScheduleStatusResolver : ValueResolver<TerminationSearchResponseViewModel, long> {

        ILookupRepository lookupRepository;

        public TerminationScheduleStatusResolver(ILookupRepository lookupRepository) {
            this.lookupRepository = lookupRepository;
        }

        protected override long ResolveCore(TerminationSearchResponseViewModel source) {
            return lookupRepository.Get<LookupTerminationStatus>(LookupConstants.TerminationStatus.Scheduled).ID;
        }

    }// class

}// namespace
