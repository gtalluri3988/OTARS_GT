using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Repositories.Interfaces;
using System.Transactions;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Interfaces;
using AutoMapper;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Services.Operations {
   /* public class ComplaintService : BaseService, IComplaintService {
        IInvoiceRepository invoiceRepository;
        INotificaitonService notificationService;
        IRunnerRepository runnerRepository;
        IObjectCreator objectCreator;
        IScopeDataProvider dataProvider;
        IRepository<Receipt> receiptRepository;
        

        public InvoiceService(IValidationProvider validationProvider, IInvoiceRepository invoiceRepository, IRunnerRepository runnerRepository, INotificaitonService notificationService, IObjectCreator objectCreator, IScopeDataProvider dataProvider, IRepository<Receipt> receiptRepository)
            : base(validationProvider) {
                this.invoiceRepository = invoiceRepository;
                this.notificationService = notificationService;
                this.runnerRepository = runnerRepository;
                this.objectCreator = objectCreator;
                this.dataProvider = dataProvider;
                this.receiptRepository = receiptRepository;
        }

        public void Generate() {
            var date = DateTime.Now.AddMonths(-1); ;
            int year = date.Year;
            int month = date.Month;
            var dictionary = invoiceRepository.GetCompanies(year, month);
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan)) {
                foreach (var company in dictionary.Keys) {
                    var number = runnerRepository.GetNext(LookupConstants.Runner.InvoiceNumber);
                    var invoice = new Invoice {
                        Amount = dictionary[company], CompanyID = company, Date= DateTime.Now,
                        Month = month, Year = year, Number = number
                    };
                    invoiceRepository.Save(invoice);
                    var invoiceVariableSource = new InvoiceVariableSource {
                        Month = month, Year = year, Amount = dictionary[company]
                    };
                    notificationService.Notify(MTACompany.ID, company, LookupConstants.Notifications.Invoice, invoiceVariableSource);
                }
                invoiceRepository.SaveChanges();
                scope.Complete();
            }
        }

        public InvoiceListViewModel GetInvoiceList(GridPageViewModel model) {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var output = objectCreator.Create<InvoiceListViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            var list = invoiceRepository.GetInvoiceList(model.Page, model.PageSize, search, orderBy, currentIdentity.CompanyID);
            foreach (var item in list) {
                output.Data.Add(Mapper.Map<InvoiceViewModel>(item));
            }
            output.TotalInvoice = invoiceRepository.GetInvoiceListCount(currentIdentity.CompanyID);
            return output;
        }

        public IEnumerable<InvoiceDetailViewModel> GetInvoiceDetails(long invoiceId) {
            var list = invoiceRepository.GetInvoiceDetails(invoiceId);
            foreach (var item in list) {
               yield return Mapper.Map<InvoiceDetailViewModel>(item);
            }
        }

        public void Pay(long invoiceId) {
            using (var scope = new TransactionScope(TransactionScopeOption.Required)) {
                var number = runnerRepository.GetNext(LookupConstants.Runner.ReceiptNumber);
                var receipt = new Receipt {
                    InvoiceId = invoiceId, Date = DateTime.Now, Number = number
                };
                receiptRepository.Save(receipt);
                receiptRepository.SaveChanges();
                scope.Complete();
            }
        }

    }// class*/

}// namepsace
