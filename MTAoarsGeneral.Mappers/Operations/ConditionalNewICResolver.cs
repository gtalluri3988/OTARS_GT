using MTAoarsGeneral.DomainModels;
using AutoMapper;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Interfaces;


namespace MTAoarsGeneral.Mappers.Operations {

    class ConditionalNewICResolver : ValueResolver<IMemberViewModel, string> {
        ILookupRepository lookupRepository;

        public ConditionalNewICResolver(ILookupRepository lookupRepository) {
            this.lookupRepository = lookupRepository;
            
        }

        protected override string ResolveCore(IMemberViewModel source) {
            if (source.ICType.Code == LookupConstants.ICTypes.NewIc)
                return source.ICNumber.Trim();
            return null;
        }

    }

}
