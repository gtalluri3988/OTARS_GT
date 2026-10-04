using System.Collections.Generic;
using System.Linq;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Validators.Operations
{
    public class RegistrationInclusionViewModelMemberCompanyValidator : IValidator<RegistrationInclusionViewModel>
    {
        static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        readonly IMemberRepository memberRepository;

        public RegistrationInclusionViewModelMemberCompanyValidator(IMemberRepository memberRepository)
        {
            this.memberRepository = memberRepository;
        }

        public IEnumerable<ValidationMessage> Validate(RegistrationInclusionViewModel item)
        {
            logger.Info("[RegistrationValidation][General][Inclusion] Start. AgencyID={0}, CompanyID={1}",
                item == null ? 0 : item.AgencyID,
                item == null ? 0 : item.CompanyID);
            if (item == null)
            {
                logger.Warn("[RegistrationValidation][General][Inclusion] STOPPED: model is null.");
                yield break;
            }

            var corporateNominee = memberRepository.GetCorporateNominee(item.AgencyID);
            logger.Info("[RegistrationValidation][General][Inclusion] Corporate nominee found={0}", corporateNominee != null);
            if (corporateNominee == null) yield break;

            if (!item.IsGeneral || corporateNominee.IsBancaStaff.GetValueOrDefault())
            {
                logger.Info("[RegistrationValidation][General][Inclusion] Non-Banca General principal rules do not apply.");
                yield break;
            }

            var icNumber = string.IsNullOrEmpty(corporateNominee.NewICNumber)
                ? corporateNominee.PassportNumber
                : corporateNominee.NewICNumber;

            logger.Info("[RegistrationValidation][General][Inclusion] Corporate nominee IC available={0}", !string.IsNullOrEmpty(icNumber));
            if (string.IsNullOrEmpty(icNumber)) yield break;

            var existingPrincipals = RegisteredPrincipalFilter.RegisteredTo(memberRepository.GetPrincipals(icNumber), icNumber);
            var isIndividual = item.AgencyType != null && item.AgencyType.Code == LookupConstants.AgencyType.Individual;
            var messages = MemberIntermediaryValidator.ValidateNonBancaGeneralPrincipals(existingPrincipals, icNumber, isIndividual).ToList();
            logger.Info("[RegistrationValidation][General][Inclusion] Shared General non-Banca rule message count={0}", messages.Count);
            foreach (var message in messages)
                yield return message;
            logger.Info("[RegistrationValidation][General][Inclusion] Completed.");
        }
    }
}
