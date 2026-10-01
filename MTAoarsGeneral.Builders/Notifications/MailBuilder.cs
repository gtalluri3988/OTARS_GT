using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Builders.Notifications {
    public class MailBuilder : IMailBuilder {

        ICompanyRepository companyRepository;
        IScopeDataProvider dataProvider;

        public MailBuilder(ICompanyRepository companyRepository, IScopeDataProvider dataProvider) {
            this.companyRepository = companyRepository;
            this.dataProvider = dataProvider;
        }

        public NewMailViewModel GetNewMail() {
            var model = new NewMailViewModel();
            model.Companies = companyRepository.GetAll().ToLookupItem();
            return model;
        }


      
    }// class
}// namespace
