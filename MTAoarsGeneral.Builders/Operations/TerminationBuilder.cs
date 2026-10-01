using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Builders.Shared;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.DomainModels;
using System.IO;
using AutoMapper;
using MTAoarsGeneral.Utilities.CSV;

namespace MTAoarsGeneral.Builders.Operations {
    public class TerminationBuilder : BaseBuilder, ITerminationBuilder {
        IAgencyBuilder agencyBuilder;
        IScopeDataProvider dataProvider;

        public TerminationBuilder(IAgencyBuilder agencyBuilder, IScopeDataProvider dataProvider) {
            this.agencyBuilder = agencyBuilder;
            this.dataProvider = dataProvider;
        }

        public TerminationSearchResponseViewModel Search(string agencyNumber) {
            var output = agencyBuilder.Search<TerminationSearchResponseViewModel>(agencyNumber);
            if (output == null) {
                CurrentContext.ValidationMessages.Add(new ValidationMessage("AgencySearch.AgencyNumber",  "The agent is not available"));
                return null;
            }
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity.IsMTA == true) return output;
            if (output.CompanyID != identity.CompanyID) CurrentContext.ValidationMessages.Add(new ValidationMessage("AgencySearch.AgencyNumber", "This agent does not belong to your company"));
            return output;
        }

        public IEnumerable<TerminationSearchResponseViewModel> Search(Stream stream)
        {
            return GetAgencies(stream);
        }

        public IEnumerable<TerminationSearchResponseViewModel> GetAgencies(Stream stream)
        {
            CsvFileReader reader = new CsvFileReader(stream, EmptyLineBehavior.NoColumns);           
            TerminationSearchResponseViewModel agency = null;
            var parameters = new List<string>();
            var output = new List<TerminationSearchResponseViewModel>();
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
                var terminationDate = new DateTime();
                string remarks = string.Empty;
                if (parameters.Count > 0)
                {
                    agencyNo = parameters[0];
                  
                }
                if (parameters.Count > 1)
                {
                    try
                    {
                        terminationDate = Convert.ToDateTime(parameters[1]);
                    }
                    catch
                    {
                        //log invalid date 
                    }
                }
                if (parameters.Count > 2)
                {
                    try
                    {
                        remarks = parameters[2];
                    }
                    catch
                    {
                        
                    }
                }
                agency = new TerminationSearchResponseViewModel();
                parameters = new List<string>();
                if (!string.IsNullOrEmpty( agencyNo))
                {
                    agency = agencyBuilder.Search<TerminationSearchResponseViewModel>(agencyNo);
                    if (agency != null)
                    {
                        isAgencyExist = true;
                    }
                }
                if (isAgencyExist)
                {
                    
                    var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
                    if (terminationDate != DateTime.MinValue)
                    {
                        agency.ScheduledOn = terminationDate;
                    }
                    if(!string.IsNullOrEmpty(remarks))
                    {
                        agency.Remarks = remarks;
                    }
                    if (identity.IsMTA || agency.CompanyID == identity.CompanyID)
                    {
                        output.Add(agency);
                        continue;
                    }
                    if (agency.CompanyID != identity.CompanyID)
                    {
                        agency = new TerminationSearchResponseViewModel();
                        agency.AgencyID = 0;
                        agency.IsValid = false;
                        agency.AgencyNumber = "This agent does not belong to your company";
                        output.Add(agency);
                    }
                   
                }
                else
                {
                    agency = new TerminationSearchResponseViewModel();
                    agency.AgencyID = 0;
                    agency.IsValid = false;
                    agency.AgencyNumber = "The agent '" + agencyNo + "' not available ";
                    output.Add(Mapper.Map<TerminationSearchResponseViewModel>(agency));
                }

            }

            return output;


        }

        public TerminationSearchResponseViewModel SearchRecovery(string agencyNumber)
        {
            var output = agencyBuilder.SearchHistory<TerminationSearchResponseViewModel>(agencyNumber);
            if (output == null)
            {
                CurrentContext.ValidationMessages.Add(new ValidationMessage("AgencySearch.AgencyNumber", "The agent is not available"));
                return null;
            }
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity.IsMTA == true) return output;
            if (output.CompanyID != identity.CompanyID) CurrentContext.ValidationMessages.Add(new ValidationMessage("AgencySearch.AgencyNumber", "This agent does not belong to your company"));
            return output;
        }

        public IEnumerable<TerminationSearchResponseViewModel> SearchRecovery(Stream stream)
        {
            CsvFileReader reader = new CsvFileReader(stream, EmptyLineBehavior.NoColumns);
            TerminationSearchResponseViewModel agency = null;
            var parameters = new List<string>();
            var output = new List<TerminationSearchResponseViewModel>();
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

                if (parameters.Count > 0) agencyNo = parameters[0];

                agency = new TerminationSearchResponseViewModel();
                parameters = new List<string>();
                if (!string.IsNullOrEmpty(agencyNo))
                {
                    agency = agencyBuilder.SearchHistory<TerminationSearchResponseViewModel>(agencyNo);
                    if (agency != null)
                    {
                        isAgencyExist = true;
                    }
                }
                if (isAgencyExist)
                {

                    var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
                    
                    if (identity.IsMTA || agency.CompanyID == identity.CompanyID)
                    {
                        output.Add(agency);
                        continue;
                    }
                    if (agency.CompanyID != identity.CompanyID)
                    {
                        agency = new TerminationSearchResponseViewModel();
                        agency.AgencyID = 0;
                        agency.IsValid = false;
                        agency.AgencyNumber = "This agent does not belong to your company";
                        output.Add(agency);
                    }

                }
                else
                {
                    agency = new TerminationSearchResponseViewModel();
                    agency.AgencyID = 0;
                    agency.IsValid = false;
                    agency.AgencyNumber = "The agent '" + agencyNo + "' not available ";
                    output.Add(Mapper.Map<TerminationSearchResponseViewModel>(agency));
                }

            }

            return output;

        }

    }// class
}// namespace
