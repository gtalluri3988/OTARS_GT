using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Repositories.Operations
{

    public class RenewalRepository : GenericRepository<RenewalHeader>, IRenewalRepository
    {

        public RenewalRepository(EntityContext context)
            : base(context)
        {
        }

        public IEnumerable<long> GetRenewableCompanies(DateTime validToDate)
        {
            return Context.AgencyPrincipals.Where(agencyPrincipal => agencyPrincipal.ValidTo == validToDate)
                .Select(agencyPrincipal => agencyPrincipal.CompanyID).Distinct();
        }

        public IEnumerable<AgencyPrincipal> GetRenewableAgencies(long companyId, DateTime validToDate)
        {
            return Context.AgencyPrincipals
                .Where(agencyPrincipal => agencyPrincipal.ValidTo == validToDate && agencyPrincipal.CompanyID == companyId);

        }

        public IEnumerable<RenewalReminderViewDetail> GetReminderDetails(int year, int quarter)
        {
            return Context.RenewalReminderViewDetails
                .Where(p => p.Year == year && p.Quarter == quarter);
        }


        //public IEnumerable<RenewalHeader> GetGeneratedRenewalHeaders(long companyId) {
        //    return Context.RenewalHeaders
        //        .Where(header => (header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Generated
        //            || header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Saved
        //            || header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Submitted)
        //            && header.CompanyID == companyId);
        //}

        public IEnumerable<RenewalHeader> GetGeneratedRenewalHeaders(long companyId)
        {

            int quater = DateTime.Now.GetQuarter();
            int year = DateTime.Now.Year;

            return Context.RenewalHeaders
                .Where(header => (header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Generated
                    || header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Saved
                    || header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Submitted)
                    && header.CompanyID == companyId
                    && (header.Year < year || (header.Year == year && header.Quarter <= quater))
                    );
        }

        public IEnumerable<RenewalHeader> GetSubmittedRenewalHeaders()
        {
            return Context.RenewalHeaders
                .Where(header => header.LookupRenewalHeaderStatus.Code == LookupConstants.RenewalHeaderStatus.Submitted);
        }

        public IEnumerable<RenewalHeader> GetHeaders(int year, int quarter)
        {
            return Context.RenewalHeaders
                .Where(p => p.Year == year && p.Quarter == quarter);
        }

        public RenewalHeader Get(long companyId, int year, int quarter)
        {
            return Context.RenewalHeaders.FirstOrDefault(p => p.CompanyID == companyId
                && p.Year == year
                && p.Quarter == quarter);
        }

        public IEnumerable<RenewalViewDetail> GetRenwalDetails(int pageNo, int pageSize, string search, string orderBy, long headerId, bool isSubmitted)
        {
            base.IsSlowQuery = true;
            return Context.RenewalViewDetails
                .Where(p => p.HeaderID == headerId && p.IsSubmitted == isSubmitted && p.IsProcessed == false
                //&& p.Agency.AgencyPrincipals.Any(ap => ap.CompanyID== p.RenewalHeader.CompanyID 
                //    && p.IntermediaryTypeID == ap.IntermediaryTypeID)
                )
                .Where(search)
                .OrderBy(orderBy)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize).ToList();
        }

        public int GetRenewalDetailsCount(long headerId, string search, bool isSubmitted)
        {
            base.IsSlowQuery = true;
            return Context.RenewalViewDetails
                .Where(p => p.HeaderID == headerId && p.IsSubmitted == isSubmitted && p.IsProcessed == false
                  // && p.Agency.AgencyPrincipals.Any(ap => ap.CompanyID == p.RenewalHeader.CompanyID
                  //  && p.IntermediaryTypeID == ap.IntermediaryTypeID)
                  )
                 .Where(search)
                 .Count();
        }

        public int GetRenewalDetailsCount(bool isSubmitted, long? companyId = null)
        {
            IEnumerable<RenewalDetail> query = Context.RenewalDetails.Where(p => p.IsProcessed == false && p.IsSubmitted == isSubmitted);
            if (companyId != null)
            {
                query = query.Where(p => p.RenewalHeader.CompanyID == companyId.Value).ToList();
            }
            return query.Count();
        }

        public RenewalDetail GetInProcessDetail(long agencyId, long companyId, long intermediaryTypeId)
        {
            var generalTypeId = Context.LookupIntermediaryTypes.Single(i => i.Code == LookupConstants.IntermediaryType.General).ID;
            return Context.RenewalDetails.OrderByDescending(r => r.CreatedDate).FirstOrDefault(
                p => p.AgencyID == agencyId
                    && p.RenewalHeader.CompanyID == companyId
                    && p.IntermediaryTypeID == generalTypeId
                    && p.IsProcessed == false
            );
        }


        //Remark new added
        public List<Company> GetCompanyNames(string CompanyName)
        {
            //var companyInRenewalHeder = Context.RenewalHeaders
            //    .Join(Context.Companies, RH => RH.CompanyID, CM => CM.ID, (RH, CM) => new { RH, CM })
            //    .GroupBy(x => new { x.RH.CompanyID })
            //    .Select(x => new { x.Key.CompanyID })
            //    .ToList();
            var companyInRenewalHeder = (from a in Context.Companies
                                         join b in Context.RenewalHeaders on a.ID equals b.ID
                                         group b by b.CompanyID into c
                                         let d = c.FirstOrDefault()
                                         where d.Company.Name.Contains(CompanyName)
                                         select new { company = d.Company })
                                         .ToList();

            var companies = companyInRenewalHeder.Select(x => new Company()
            {
                ID = x.company.ID,
                Name = x.company.Name
                ,
                BusinessRegistrationNumber = x.company.BusinessRegistrationNumber
                ,
                Abbreviation = x.company.Abbreviation
            })
                .ToList();

            return companies;
        }

        public RenewalViewDetail RenewalViewDetail(long ID)
        {
            return Context.RenewalViewDetails.Where(x => x.ID == ID).FirstOrDefault();
        }

    }// class

}// namespace
