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
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Validators.Operations
{
    public class ChangeCompanyValidator : IValidator<ChangeCompanyViewModel>
    {
        IAgencyRepository agencyRepository;
        IMemberRepository memberRepository;
        IRepository<AgencyPrincipal> principalRepository;

        public ChangeCompanyValidator(IAgencyRepository agencyRepository, IMemberRepository memberRepository, IRepository<AgencyPrincipal> principalRepository)
        {
            this.agencyRepository = agencyRepository;
            this.memberRepository = memberRepository;
            this.principalRepository = principalRepository;
        }

        public IEnumerable<ValidationMessage> Validate(ChangeCompanyViewModel item)
        {
            if (item.RegistrationModel.AgencyType.Code == LookupConstants.AgencyType.Individual) yield break;
            //var agency = agencyRepository.SelectOtherTypeAgency(item.BusinesssRegistrationNumber, item.TypeCode);

            //var agency = agencyRepository.Select(item.RegistrationModel.Agency.BusinessRegistrationNumber);
            //var principal = principalRepository.Get(item.PrincipalID);
            //var principalAgency = principal.Agency;
            //if (agency == null || agency.ID == principalAgency.ID) yield break;
            //if (principal.LookupAgencyType.Code != item.RegistrationModel.AgencyType.Code)
            //    yield return new ValidationMessage("", "Business registration number {0} already registered under {1}", item.RegistrationModel.Agency.BusinessRegistrationNumber, principal.LookupAgencyType.Description);

            //var cn = memberRepository.GetCorporateNominee(agency.ID);
            //var icNumber = cn.LookupICType.Code == LookupConstants.ICTypes.NewIc ? cn.NewICNumber : cn.PassportNumber;
            //var itemNominee = item.RegistrationModel.CorporateNominee;
            //var itemIcNumber = (itemNominee.ICType.Code == LookupConstants.ICTypes.NewIc)? itemNominee.NewICNumber : itemNominee.PassportNumber;
            //if (icNumber != itemIcNumber) 
            //    yield return new ValidationMessage("", "Business registration number {0} is not matching with the IC number", item.RegistrationModel.Agency.BusinessRegistrationNumber, principal.LookupAgencyType.Description);

            var principal = principalRepository.Get(item.PrincipalID);
            var principalAgency = principal.Agency;
            var itemNominee = item.RegistrationModel.CorporateNominee;
            var itemIcNumber = (itemNominee.ICType.Code == LookupConstants.ICTypes.NewIc) ? itemNominee.NewICNumber : itemNominee.PassportNumber;

            //Check by business registration number
            //----------------------------------------------------------------
            var agency = agencyRepository.Select(item.RegistrationModel.Agency.BusinessRegistrationNumber);
            if (agency != null && agency.ID != principalAgency.ID)
            {
                if (principal.LookupAgencyType.Code != item.RegistrationModel.AgencyType.Code)
                    yield return new ValidationMessage("", "Business registration number {0} already registered under {1}", item.RegistrationModel.Agency.BusinessRegistrationNumber, principal.LookupAgencyType.Description);

                var cn = memberRepository.GetCorporateNominee(agency.ID);
                var icNumber = cn.LookupICType.Code == LookupConstants.ICTypes.NewIc ? cn.NewICNumber : cn.PassportNumber;
                if (icNumber != itemIcNumber)
                    yield return new ValidationMessage("", "Business registration number {0} is not matching with the IC number", item.RegistrationModel.Agency.BusinessRegistrationNumber, principal.LookupAgencyType.Description);
            }

            //Check by new business registration number
            //----------------------------------------------------------------
            agency = agencyRepository.Select(item.RegistrationModel.Agency.NewBusinessRegistrationNumber, true);
            if (agency != null && agency.ID != principalAgency.ID)
            {
                if (principal.LookupAgencyType.Code != item.RegistrationModel.AgencyType.Code)
                    yield return new ValidationMessage("", "New Business registration number {0} already registered under {1}", item.RegistrationModel.Agency.NewBusinessRegistrationNumber, principal.LookupAgencyType.Description);

                var cn = memberRepository.GetCorporateNominee(agency.ID);
                var icNumber = cn.LookupICType.Code == LookupConstants.ICTypes.NewIc ? cn.NewICNumber : cn.PassportNumber;
                if (icNumber != itemIcNumber)
                    yield return new ValidationMessage("", "New Business registration number {0} is not matching with the IC number", item.RegistrationModel.Agency.NewBusinessRegistrationNumber, principal.LookupAgencyType.Description);
            }

        }

    }// class
}// namespace
