using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Utilities.Constants;
using AutoMapper;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.DomainModels;
using System.IO;
using MTAoarsGeneral.Utilities.CSV;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.Builders.Shared;

namespace MTAoarsGeneral.Builders.Operations {
    public class RenewalBuilder : BaseBuilder, IRenewalBuilder
    {
        ILookupRepository lookupRepository;
        IRenewalRepository renewalRepository;
        Identity currentIdentity;
        

        public RenewalBuilder(IRenewalRepository renewalRepository, IScopeDataProvider dataProvider, ILookupRepository lookupRepository) {
            this.renewalRepository = renewalRepository;
            this.lookupRepository = lookupRepository;
            currentIdentity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
        }

        public IEnumerable<RenewalListViewModel> GetRenewalHeaderList() {
            var headers = renewalRepository.GetGeneratedRenewalHeaders(currentIdentity.CompanyID);
            foreach (var header in headers) {
                var modelList = Mapper.Map<RenewalListViewModel>(header);
                yield return modelList;
            }
        }

        public RenewalHeaderViewModel GetRenewlHeader(long id) {
            var header = renewalRepository.Get(id);
            var modelHeader = Mapper.Map<RenewalHeaderViewModel>(header);
           /* foreach (var detail in header.RenewalDetails) {
                modelHeader.Details.Add(Mapper.Map<RenewalDetailViewModel>(detail));
            }*/
            modelHeader.RenewalDetailStatuses = lookupRepository.GetAll<LookupRenewalDetailStatus>().Cast<ILookupEntity>().ToLookupItem();
                                                                                                    
            
            return modelHeader;
        }

        public RenewalHeaderViewModel GetRenewlHeader(RenewalApproveHeaderViewModel approveHeader) {
            var header = renewalRepository.Get(approveHeader.Company.ID, Convert.ToInt32(approveHeader.Year.ID), Convert.ToInt32(approveHeader.Quarter.ID));
            if (header == null) //To prevent search no record hit error - [20191107]
                header = new RenewalHeader(); 
            var modelHeader = Mapper.Map<RenewalHeaderViewModel>(header);
            return modelHeader;
        }
        public RenewalHeaderViewModel GetRenewalDetails(Stream stream, int year, int quarter)
        {
            var modelHeader = new RenewalHeaderViewModel();
            var header = renewalRepository.Get(currentIdentity.CompanyID, year, quarter);
            
            if (header == null)
                return modelHeader;

            modelHeader = Mapper.Map<RenewalHeaderViewModel>(header);           
            CsvFileReader reader = new CsvFileReader(stream, EmptyLineBehavior.NoColumns);
            RenewalDetailViewModel detail = null;
            var parameters = new List<string>();
            var output = new List<RenewalDetailViewModel>();
            var agencies = new List<string>();
            var isFirstRow = true;
            while (reader.ReadRow(parameters))
            {
                if (isFirstRow)
                {
                    isFirstRow = false;
                    continue;
                }
                var agencyNo = string.Empty;
                bool isAgencyExist = false;                

                if (parameters.Count > 1)
                {
                    agencyNo = parameters[1];
                    if (modelHeader.Details.Exists(i => i.AgencyNumber == agencyNo))
                    {
                        logger.Info(string.Format("Duplicate Agency Number: {0}", agencyNo));
                        continue;
                    }
                }                
              
                if (!string.IsNullOrEmpty(agencyNo))
                {
                    try
                    {
                        if (header.RenewalDetails.Any(d => d.Agency != null && d.Agency.AgencyPrincipals != null && d.Agency.AgencyPrincipals.Count > 0 && d.Agency.AgencyPrincipals.FirstOrDefault().AgencyNumber == agencyNo))
                        {
                            var renewalDetail = Mapper.Map<RenewalDetailViewModel>(
                                header.RenewalDetails.FirstOrDefault(d => d.Agency != null && d.Agency.AgencyPrincipals != null && d.Agency.AgencyPrincipals.Count > 0 && d.Agency.AgencyPrincipals.FirstOrDefault().AgencyNumber == agencyNo)
                            );                           

                            isAgencyExist = true;
                            modelHeader.Details.Add(renewalDetail);
                            if(parameters.Count > 6)
                                renewalDetail.Remarks = parameters[6];
                            if(parameters.Count>5)
                            {
                                var status = lookupRepository.GetAll<LookupRenewalDetailStatus>().SingleOrDefault(s => s.Description==parameters[5]);
                                if (status != null)
                                {
                                    renewalDetail.StatusID = status.ID;
                                    renewalDetail.RenewalStatus = Mapper.Map<LookupItem>(status);
                                    renewalDetail.StatusDescription = status.Description;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                parameters = new List<string>();
                if (!isAgencyExist)
                {
                    detail = new RenewalDetailViewModel();
                    detail.AgencyName = "Then agent '" + agencyNo + "' not available";
                    modelHeader.Details.Add(detail);
                } 
            }            
            modelHeader.Details.ForEach((d) => {
                if (d.StatusID <= 0) {
                    var status=lookupRepository.Get<LookupRenewalDetailStatus>(LookupConstants.RenewalDetailStatus.Renew);
                    d.StatusID = status.ID;
                    d.StatusDescription = status.Description;
                    d.RenewalStatus = Mapper.Map<LookupItem>(status);
                }
            });
            return modelHeader;
        }
        
        public RenewalDetailsViewModel GetRenewalHeaderByCompany(long companyID)
        {
            var headers = renewalRepository.GetGeneratedRenewalHeaders(companyID);
            var headersMaxID = headers?.Max(x => x?.ID);
            headers = headers.Where(x => x.ID == headersMaxID);
            var viewModel = new RenewalDetailsViewModel();
            var listHeaders = new List<RenewalHearderViewModel>();
            foreach (var header in headers)
            {
                listHeaders.Add(Mapper.Map<RenewalHearderViewModel>(header));
            }
            viewModel.CompanyName = headers?.FirstOrDefault()?.Company?.Name;
            viewModel.RenewalHeader = listHeaders;

            return viewModel;
        }

        public RenewalDetailsViewModel GetRenewalHeaderRef(long id)
        {
            var ViewModel = new RenewalDetailsViewModel();
            var header = renewalRepository.Get(id);
            var model = Mapper.Map<RenewalHearderViewModel>(header);
            model.RenewalDetailStatuses = lookupRepository.GetAll<LookupRenewalDetailStatus>().Cast<ILookupEntity>().ToLookupItem();
            ViewModel.Header = model;
            return ViewModel;
        }
    }// class
}// namespace
