using MTAoarsGeneral.DomainModels;
using AutoMapper;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Interfaces;


namespace MTAoarsGeneral.Mappers.Operations {

    class ConditionalPassportNumberResolver : ValueResolver<IMemberViewModel, string> {
        ILookupRepository lookupRepository;

        public ConditionalPassportNumberResolver(ILookupRepository lookupRepository) {
            this.lookupRepository = lookupRepository;
            
        }

        protected override string ResolveCore(IMemberViewModel source) {
            if (source.ICType.Code == LookupConstants.ICTypes.PoliceArmyPassport)
                return source.ICNumber;
            return null;
        }

    }

}
