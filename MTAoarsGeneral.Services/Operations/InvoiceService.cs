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

namespace MTAoarsGeneral.Services.Operations
{
    public class InvoiceService : BaseService, IInvoiceService
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        IInvoiceRepository invoiceRepository;
        INotificaitonService notificationService;
        IRunnerRepository runnerRepository;
        IObjectCreator objectCreator;
        IScopeDataProvider dataProvider;
        IRepository<Receipt> receiptRepository;
        ILookupService lookupService;
        IAgencyRepository agencyRepository;

        public InvoiceService(IValidationProvider validationProvider, IInvoiceRepository invoiceRepository, IRunnerRepository runnerRepository, INotificaitonService notificationService, IObjectCreator objectCreator, IScopeDataProvider dataProvider, IRepository<Receipt> receiptRepository, ILookupService lookupService, IAgencyRepository agencyRepository)
            : base(validationProvider)
        {
            this.invoiceRepository = invoiceRepository;
            this.notificationService = notificationService;
            this.runnerRepository = runnerRepository;
            this.objectCreator = objectCreator;
            this.dataProvider = dataProvider;
            this.receiptRepository = receiptRepository;
            this.lookupService = lookupService;
            this.agencyRepository = agencyRepository;
        }

        public void Generate()
        {
            var date = DateTime.Now.AddMonths(-1); ;
            int year = date.Year;
            int month = date.Month;
            Generate(year, month);
        }

        public void Generate(int year, int month)
        {
            Generate(null, year, month);
        }

        public void Generate(long? companyID, int year, int month)
        {
            logger.Info("=======================================================================");
            logger.Info("InvoiceService.Generate");
            logger.Info("Year: " + year + ", Month: " + month);

            var dictionary = invoiceRepository.GetCompanies(year, month);

            if (companyID != null)
                dictionary = dictionary.Where(c => c.Key == companyID).ToDictionary(c => c.Key, c => c.Value);

            var statusID = lookupService.GetInvoiceStatuses().SingleOrDefault(i => i.Code == LookupConstants.InvoiceStatus.Generated).ID;
            using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan))
            {
                foreach (var company in dictionary.Keys)
                {
                    logger.Info("CompanyID: " + company + ", Amount: " + dictionary[company]);

                    var number = runnerRepository.GetNext(LookupConstants.Runner.InvoiceNumber);
                    logger.Info("Invoice Number: " + number);

                    var invoice = new Invoice
                    {
                        Amount = dictionary[company],
                        CompanyID = company,
                        Date = DateTime.Now,
                        Month = month,
                        Year = year,
                        Number = number,
                        StatusID = statusID
                    };
                    invoiceRepository.Save(invoice);
                    var invoiceVariableSource = new InvoiceVariableSource
                    {
                        Month = month,
                        Year = year,
                        Amount = dictionary[company]
                    };
                    notificationService.Notify(MTACompany.ID, company, LookupConstants.Notifications.Invoice, invoiceVariableSource);
                }
                invoiceRepository.SaveChanges();
                scope.Complete();
            }
        }
        public void Regenerate(long invoiceID, int year, int month)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (currentIdentity.IsISMOrMTA)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan))
                {

                    var invoice = invoiceRepository.Get(invoiceID);
                    Generate(invoice.CompanyID, invoice.Year, invoice.Month);
                    invoiceRepository.Delete(invoice);
                    invoiceRepository.SaveChanges();
                    scope.Complete();
                }
            }
        }

        public void Save(List<InvoiceViewModel> invoices)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (currentIdentity.IsISMOrMTA)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, GlobalConstants.HugeTransactionTimeSpan))
                {
                    foreach (var item in invoices)
                    {
                        var invoice = invoiceRepository.Get(item.ID);
                        invoice.StatusID = item.InvoiceStatus.ID;
                        invoice.Remarks = item.Remarks;

                    }
                    invoiceRepository.SaveChanges();
                    scope.Complete();
                }
            }
        }


        public InvoiceListViewModel GetInvoiceList(GridPageViewModel model)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var output = objectCreator.Create<InvoiceListViewModel>();
            var orderBy = GetSortOrder(model);
            var search = GetSearch(model);
            long? companyId = currentIdentity.CompanyID;
            if (currentIdentity.IsMTA) companyId = null;
            if (currentIdentity.IsISM) companyId = CompanyRepository.Get(GlobalConstants.MTACompanyCode).ID;
            var list = invoiceRepository.GetInvoiceList(model.Page, model.PageSize, search, orderBy, companyId);
            foreach (var item in list)
            {

                output.Data.Add(Mapper.Map<InvoiceViewModel>(item));
            }
            output.TotalInvoice = invoiceRepository.GetInvoiceListCount(companyId);
            return output;
        }

        public InvoiceListViewModel GetInvoiceList(long? companyId, int year, int month)
        {
            var currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            var output = objectCreator.Create<InvoiceListViewModel>();
            if (currentIdentity.IsMTA) companyId = null;
            if (currentIdentity.IsISM) companyId = CompanyRepository.Get(GlobalConstants.MTACompanyCode).ID;
            var list = invoiceRepository.GetInvoiceList(companyId, year, month);
            foreach (var item in list)
            {
                output.Data.Add(Mapper.Map<InvoiceViewModel>(item));
            }
            output.TotalInvoice = invoiceRepository.GetInvoiceListCount(companyId);
            return output;
        }


        public IEnumerable<InvoiceDetailViewModel> GetInvoiceDetails(long invoiceId)
        {
            var list = invoiceRepository.GetInvoiceDetails(invoiceId);
            foreach (var item in list)
            {
                yield return Mapper.Map<InvoiceDetailViewModel>(item);
            }
        }

        public void Pay(long invoiceId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var number = runnerRepository.GetNext(LookupConstants.Runner.ReceiptNumber);
                var receipt = new Receipt
                {
                    InvoiceId = invoiceId,
                    Date = DateTime.Now,
                    Number = number
                };
                receiptRepository.Save(receipt);
                receiptRepository.SaveChanges();
                scope.Complete();
            }
        }

        public IEnumerable<InvoiceDetailExportModel> GetInvoiceViewDetail(long invoiceId)
        {
            var list = invoiceRepository.GetInvoiceViewDetails(invoiceId);

            var models = new List<InvoiceDetailExportModel>();
            foreach (var item in list)
            {
                var model = new InvoiceDetailExportModel()
                {
                    CompanyName = item.CompanyName, //journal.Company.Name,
                    DateCreated = item.Date,
                    Description = item.Description,
                    IntermediaryTypeDescription = item.IntermediaryTypeDescription,
                    Amount = item.Amount.HasValue ? Convert.ToDecimal(item.Amount) : 0,
                    IsBancaStaff = !item.IsBancaStaff.HasValue ? "N" : Convert.ToBoolean(item.IsBancaStaff) ? "Y" : "N",
                    AgencyName = item.AgencyName,
                    AgencyNumber = item.AgencyNumber,
                    NewBusinessRegistrationNumber = item.NewBusinessRegistrationNumber,
                };

                /* Added AgencyNumber and IsHistorical to identity correct Agent record from Journal [20191121] */
                if (!Convert.ToBoolean(item.IsHistorical))
                {
                    //Agency Principal Records
                    //--------------------------------------------------------------------
                    var agencyPrincipal = agencyRepository.GetAgencyPrincipal(item.AgencyPrincipalID);
                    var isIndividual = agencyPrincipal.LookupAgencyType.Code == LookupConstants.AgencyType.Individual;

                    model.AgencyTypeDescription = agencyPrincipal.LookupAgencyType.Description;
                    /* 20200402 - Do not show BR No# if "Individual" */
                    model.BusinessRegistrationNumber = (isIndividual) ? "" : agencyPrincipal.Agency.BusinessRegistrationNumber;
                    model.ValidFrom = agencyPrincipal.ValidFrom;
                    model.ValidTo = agencyPrincipal.ValidTo;

                    var corporateNorminee = agencyPrincipal.Agency.AgencyMembers.FirstOrDefault(
                        i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                    if (corporateNorminee != null)
                    {
                        model.NomineeName = corporateNorminee.Member.Name;
                        model.NomineeICNumber = corporateNorminee.Member.NewICNumber;
                    }
                }
                else
                {
                    //Agency Principal Historical Records 
                    //--------------------------------------------------------------------
                    var agencyPrincipalHistory = agencyRepository.GetAgencyPrincipalHistory(item.AgencyPrincipalID);

                    if (agencyPrincipalHistory != null)
                    {
                        var isIndividual = agencyPrincipalHistory.LookupAgencyType.Code == LookupConstants.AgencyType.Individual;

                        model.AgencyTypeDescription = agencyPrincipalHistory.LookupAgencyType.Description;
                        /* 20200402 - Do not show BR No# if "Individual" */
                        model.BusinessRegistrationNumber = (isIndividual) ? "" : agencyPrincipalHistory.Agency.BusinessRegistrationNumber;
                        model.ValidFrom = agencyPrincipalHistory.ValidFrom;
                        model.ValidTo = agencyPrincipalHistory.ValidTo;

                        var corporateNorminee = agencyPrincipalHistory.Agency.AgencyMembers.FirstOrDefault(
                            i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
                        if (corporateNorminee != null)
                        {
                            model.NomineeName = corporateNorminee.Member.Name;
                            model.NomineeICNumber = corporateNorminee.Member.NewICNumber;
                        }
                    }
                }

                yield return model;

                //models.Add(model);
            }

            //return models;
        }

        //public IEnumerable<InvoiceDetailExportModel> GetInvoiceViewDetail2(long invoiceId)
        //{           
        //    var list = invoiceRepository.GetInvoiceDetails(invoiceId);
        //    var models = new List<InvoiceDetailExportModel>();
        //    foreach (var item in list)
        //    {
        //        var journal = item.Journal;
        //        var model = new InvoiceDetailExportModel()
        //        {
        //            CompanyName = journal.Company.Name,
        //            DateCreated = item.CreatedDate,
        //            Description = journal.Description,
        //            IntermediaryTypeDescription = journal.LookupIntermediaryType.Description,
        //            Amount = (item.Amount < 0) ? item.Amount * -1 : item.Amount
        //        };

        //        //Agency Principal Records
        //        //--------------------------------------------------------------------
        //        AgencyPrincipal agencyPrincipal;

        //        if (journal.Description == LookupConstants.Activities.Renewal)
        //        {
        //            var agencyPrincipalId = Convert.ToInt64(journal.Uri.Split('/').Last());
        //            agencyPrincipal = agencyRepository.GetAgencyPrincipal(agencyPrincipalId);
        //        }
        //        else
        //        {
        //            agencyPrincipal = journal.Agency.AgencyPrincipals.Where(
        //                i => i.CompanyID == journal.CompanyID && i.IntermediaryTypeID == journal.IntermediaryTypeID).FirstOrDefault();
        //        }

        //        if (agencyPrincipal != null)
        //        {
        //            model.IsBancaStaff = (!agencyPrincipal.IsBancaStaff.HasValue || agencyPrincipal.IsBancaStaff == false) ? "N" : "Y";
        //            model.AgencyName = agencyPrincipal.Agency.Name;
        //            model.AgencyNumber = agencyPrincipal.AgencyNumber;
        //            model.AgencyTypeDescription = agencyPrincipal.LookupAgencyType.Description;
        //            model.BusinessRegistrationNumber = agencyPrincipal.Company.BusinessRegistrationNumber;
        //            model.ValidFrom = agencyPrincipal.ValidFrom;
        //            model.ValidTo = agencyPrincipal.ValidTo;

        //            var corporateNorminee = agencyPrincipal.Agency.AgencyMembers.FirstOrDefault(
        //                i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
        //            if (corporateNorminee != null)
        //            {
        //                model.NomineeName = corporateNorminee.Member.Name;
        //                model.NomineeICNumber = corporateNorminee.Member.NewICNumber;
        //            }
        //        }

        //        //Agency Principal Historical Records 
        //        //--------------------------------------------------------------------
        //        if (agencyPrincipal == null)
        //        {
        //            AgencyPrincipalHistory agencyPrincipalHistory;
        //            if (journal.Description == LookupConstants.Activities.Renewal)
        //            {
        //                var agencyPrincipalHistoryId = Convert.ToInt64(journal.Uri.Split('/').Last());
        //                agencyPrincipalHistory = agencyRepository.GetAgencyPrincipalHistory(agencyPrincipalHistoryId);
        //            }
        //            else
        //            {
        //                agencyPrincipalHistory = journal.Agency.AgencyPrincipalHistories.Where(
        //                    i => i.CompanyID == journal.CompanyID && i.IntermediaryTypeID == journal.IntermediaryTypeID).FirstOrDefault();
        //            }

        //            if (agencyPrincipalHistory != null)
        //            {
        //                model.IsBancaStaff = (!agencyPrincipalHistory.IsBancaStaff.HasValue || agencyPrincipalHistory.IsBancaStaff == false) ? "N" : "Y";
        //                model.AgencyName = agencyPrincipalHistory.Agency.Name;
        //                model.AgencyNumber = agencyPrincipalHistory.AgencyNumber;
        //                model.AgencyTypeDescription = agencyPrincipalHistory.LookupAgencyType.Description;
        //                model.BusinessRegistrationNumber = agencyPrincipalHistory.Company.BusinessRegistrationNumber;
        //                model.ValidFrom = agencyPrincipalHistory.ValidFrom;
        //                model.ValidTo = agencyPrincipalHistory.ValidTo;

        //                var corporateNorminee = agencyPrincipalHistory.Agency.AgencyMembers.FirstOrDefault(
        //                    i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
        //                if (corporateNorminee != null)
        //                {
        //                    model.NomineeName = corporateNorminee.Member.Name;
        //                    model.NomineeICNumber = corporateNorminee.Member.NewICNumber;
        //                }
        //            }
        //        }

        //        models.Add(model);
        //    }

        //    return models;

        //    //foreach (var item in list)
        //    //{
        //    //    yield return Mapper.Map<InvoiceDetailExportModel>(item);
        //    //}
        //}

    }// class

}// namepsace
