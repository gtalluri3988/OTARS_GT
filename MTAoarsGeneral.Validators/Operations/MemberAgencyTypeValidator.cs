using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations
{
    public class MemberAgencyTypeValidator : IValidator<IMemberViewModel>
    {
        IMemberRepository memberRepository;
        ILookupRepository lookupRepository;

        public MemberAgencyTypeValidator(IMemberRepository memberRepository, ILookupRepository lookupRepository)
        {
            this.memberRepository = memberRepository;
            this.lookupRepository = lookupRepository;
        }

        public IEnumerable<ValidationMessage> Validate(IMemberViewModel item)
        {
            //[CR No# 20191118-01] - Remove checking on Director, Shareholder and partner register as nominee 
            //[CR No# 20191118-01, updates: 20200428] - Alvin told MTA dont want to implement this CR, need to enable back the checking 

            //yield break;

            //if (item.AgencyType.Code == LookupConstants.AgencyType.Individual) // New change request to allow individual from any status. only check other corporate status
            //    yield break;

            //var latestMember = memberRepository.Get(item.ICNumber);
            //if (latestMember == null)
            //    yield break;

            //var isBancaStaff = latestMember.IsBancaStaff.GetValueOrDefault();
            //var agencies = memberRepository.GetActiveAgencies(item.ICNumber);
            //foreach(var agency in agencies)
            //{
            //    var agp = agency.AgencyPrincipals.Where(ap => isBancaStaff ? 
            //        (ap.LookupAgencyType.Code == item.AgencyType.Code) :
            //        (!ap.IsIndividual && ap.LookupAgencyType.Code == item.AgencyType.Code)
            //    ).FirstOrDefault();

            //    if (agp != null)
            //    {
            //        yield return new ValidationMessage("", "This Agent / Corporate Nominee  \"{0}\" is currently registered as an {1} as {2}", 
            //            item.ICNumber,
            //            lookupRepository.Get<LookupAgencyType>(agp.TypeID).Description,
            //            (isBancaStaff) ? "Banca" : "Non Banca");
            //        yield break;
            //    }                    
            //}

            yield break;

            /*var agency = agencies.Where(
            a=>a.AgencyPrincipals.Where(
                p => member.IsBancaStaff.GetValueOrDefault() ? true : p.LookupAgencyType.Code != LookupConstants.AgencyType.Individual).Any(ap => ap.LookupAgencyType.Code != item.AgencyType.Code)
            ).FirstOrDefault();*/

            //var agency = agencies.FirstOrDefault();
            //if (agency == null)
            //    yield break;

            //var agp = agency.AgencyPrincipals.Where(ap => latestMember.IsBancaStaff.GetValueOrDefault() ? 
            //        ap.LookupAgencyType.Code != item.AgencyType.Code : 
            //        (!ap.IsIndividual && ap.LookupAgencyType.Code != item.AgencyType.Code)
            //).FirstOrDefault();
            //if (agp == null)
            //    yield break;

            //var type = lookupRepository.Get<LookupAgencyType>(agp.TypeID);
            //var bancaStatus = latestMember.IsBancaStaff.GetValueOrDefault() ? "Banca" : "Non Banca";
            //yield return new ValidationMessage("", "This Agent / Corporate Nominee  \"{0}\" is currently registered as an {1} as {2}", item.ICNumber, type.Description, bancaStatus);
        }

    }// class
}// namespace
