using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Attributes {
    
    public class ValueProviderAttribute : Attribute {

        Type valueProviderType;

        public ValueProviderAttribute(Type valueProviderType) {
            this.valueProviderType = valueProviderType;
        }

        public Type ValueProviderType {
            get { return valueProviderType; }
        }

       

    }// class

}// namespace
