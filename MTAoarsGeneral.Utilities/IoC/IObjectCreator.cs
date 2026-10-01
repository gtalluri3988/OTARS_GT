using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.IoC {

    public interface IObjectCreator {

        T Create<T>();

    }// interface

}// namespace
