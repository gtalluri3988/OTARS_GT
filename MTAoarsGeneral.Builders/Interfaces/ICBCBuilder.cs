using MTAoarsGeneral.ViewModels.Operations;
using System.Collections.Generic;
using System;

namespace MTAoarsGeneral.Builders.Interfaces {
    public interface ICBCBuilder : IBaseBuilder {

        CBCStartViewModel GetStartViewModel();

        CBCDetailViewModel Search(string agencyNumber, long companyId);

    }// interface
}// namespace
