using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.IoC {

    public interface IObjectMapper {

        TDestination Map<TSource, TDestination>(TSource source);

    }// class

}// namespace
