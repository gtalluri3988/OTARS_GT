using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Operations {

    public class MemberList<T> : List<T> where T : IMemberViewModel {

        RegistrationViewModel parent;
        public MemberList(RegistrationViewModel parent) {
            this.parent = parent;
            
        }

        public new void Add(T item) {
            item.AgencyType = parent.AgencyType;
            base.Add(item);
        }

    }// class

}// namespace
