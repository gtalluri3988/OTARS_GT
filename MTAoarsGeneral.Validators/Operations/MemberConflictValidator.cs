using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations {
    public class MemberConflictValidator : IValidator<IMemberViewModel> {
        IMemberRepository memberRepository;

        public MemberConflictValidator(IMemberRepository memberRepository) {
            this.memberRepository = memberRepository;
        }

        public IEnumerable<ValidationMessage> Validate(IMemberViewModel item) {
            var result = memberRepository.IsMemberExistsInConflict(item.ICNumber);
            if (result == true) {
                yield return new ValidationMessage("", "The member is already in conflict and cannot be processed now.");
            }
        }

    }// class
}// namespace
