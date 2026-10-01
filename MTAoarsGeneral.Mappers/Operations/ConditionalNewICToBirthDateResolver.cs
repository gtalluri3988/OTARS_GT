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
using System.Globalization;


namespace MTAoarsGeneral.Mappers.Operations {

    class ConditionalNewICToBirthDateResolver : ValueResolver<RegistrationIndexViewModel, DateTime?> {
        ILookupRepository lookupRepository;

        public ConditionalNewICToBirthDateResolver(ILookupRepository lookupRepository) {
            this.lookupRepository = lookupRepository;
            
        }

        protected override DateTime? ResolveCore(RegistrationIndexViewModel source) {
            if (source.ICType.Code != LookupConstants.ICTypes.NewIc) return null;
            if (String.IsNullOrEmpty(source.ICNumber)) return null;
            if (source.ICNumber.Length < 9) return null;
            DateTime result;
            if (!DateTime.TryParseExact(source.ICNumber.Substring(0, 6), "yyMMdd", null, DateTimeStyles.None, out result)) return null;
            return result;
        }

    }

}
