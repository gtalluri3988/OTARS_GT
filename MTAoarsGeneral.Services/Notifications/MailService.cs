using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.ViewModels.Notifications;
using AutoMapper;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.DomainModels;
using System.Transactions;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Services.Notifications {
    
    public class MailService :  BaseService, IMailService {

        IMailRepository mailRepository;
        ILookupRepository lookupRepository;
        Identity currentIdentity;
        IObjectCreator objectCreator;

        public MailService(IValidationProvider validationProvider, IMailRepository mailRepository, ILookupRepository lookupRepository, IScopeDataProvider dataProvider, IObjectCreator objectCreator)
            : base(validationProvider) {
                this.lookupRepository = lookupRepository;
                this.mailRepository = mailRepository;
                currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
                this.objectCreator = objectCreator;
        }

        public MailListViewModel GetReceivedMails(GridPageViewModel model) {
            var output = objectCreator.Create<MailListViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            var list = mailRepository.GetReceivedMails(model.Page, model.PageSize, search, orderBy, currentIdentity.CompanyID);
            foreach (var item in list) {
                output.Data.Add(Mapper.Map<MailViewModel>(item));
            }
            output.TotalMail = mailRepository.GetReceivedMailsCount(currentIdentity.CompanyID, search);
            return output;
        }

        public MailListViewModel GetSentMails(GridPageViewModel model) {
            var output = objectCreator.Create<MailListViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            var list = mailRepository.GetSentMails(model.Page, model.PageSize, search, orderBy, currentIdentity.CompanyID);
            foreach (var item in list) {
                output.Data.Add(Mapper.Map<MailViewModel>(item));
            }
            output.TotalMail = mailRepository.GetSentMailsCount(currentIdentity.CompanyID, search);
            return output;
        }

        public MailViewModel Read(long id) {
            var mail = mailRepository.Get(id);
            if (mail == null) return null;
            var statusId = lookupRepository.Get<LookupMailStatus>(LookupConstants.MailStatus.Read).ID;
            if (mail.FromCompanyID == currentIdentity.CompanyID) {
                mailRepository.ChangeFromStatus(id, statusId);
            } else {
                mailRepository.ChangeToStatus(id, statusId);
            }
            return Mapper.Map<MailViewModel>(mail);
        }

        public MailReplyViewModel GetReplyMail(long id) {
            var mail = mailRepository.Get(id);
            if (mail == null) return null;
            var output =  Mapper.Map<MailReplyViewModel>(mail);
            if (currentIdentity.CompanyID != output.FromCompanyID) {
                output.ToCompanyID = output.FromCompanyID;
                output.ToCompanyName = output.FromCompanyName; ;
            }
            output.FromCompanyID = currentIdentity.CompanyID;
            output.FromCompanyName = currentIdentity.CompanyName;
            output.SoureceMailID = id;
            return output;
        }

        public void Send(MailReplyViewModel model) {
            using (var scope = new TransactionScope()) {
                var mail = Mapper.Map<Mail>(model);
                mailRepository.Save(mail);
                var statusId = lookupRepository.Get<LookupMailStatus>(LookupConstants.MailStatus.Replied).ID;
                var sourceMail = mailRepository.Get(model.SoureceMailID);
                if (sourceMail.FromCompanyID == currentIdentity.CompanyID) {
                    mailRepository.ChangeFromStatus(model.SoureceMailID, statusId);
                } else {
                    mailRepository.ChangeToStatus(model.SoureceMailID, statusId);
                }
                mailRepository.SaveChanges();
                scope.Complete();
            }
        }

        public void Send(NewMailViewModel model) {
            var statusId =lookupRepository.Get<LookupMailStatus>(LookupConstants.MailStatus.New).ID;
            foreach (var companyId in model.CompanyIdentifiers) {
               var mail = new Mail {
                    FromCompanyID = currentIdentity.CompanyID, ToCompanyID = companyId, Body = model.Body, Subject = model.Subject,
                    CreatedOn = DateTime.Now, FromStatusID = statusId, ToStatusID = statusId
                };
               mailRepository.Save(mail);
            }
            mailRepository.SaveChanges();
        }

    }// class

}// namespace
