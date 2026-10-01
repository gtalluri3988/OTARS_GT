using AutoMapper;
using MTAoarsGeneral.Builders.Interfaces;
using MTAoarsGeneral.Builders.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Administrative;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Builders.Administrative
{
    public class ReferredBuilder : BaseBuilder, IReferredBuilder
    {
        IReferredMemberRepository referredMemberRepository;

        public ReferredBuilder(IReferredMemberRepository referredMemberRepository)
        {
            this.referredMemberRepository = referredMemberRepository;
        }

        public ReferredSearchViewModel SearchReferredMember(string IcNumber)
        {
            var ViewModel = new ReferredSearchViewModel();

            var OutPut = new List<ReferredResponseViewModel>();

            var items = referredMemberRepository.GetMembers(IcNumber);

            foreach (var item in items)
            {
                var Map = Mapper.Map<ReferredResponseViewModel>(item);
                Map.ReasonDescription = referredMemberRepository.LookupReferredReason(Map.ReasonID);
                Map.CategoryDescription = referredMemberRepository.LookupReferredCategory(Map.CategoryID);

                OutPut.Add(Map);
            }


            ViewModel.ReferredMembers = OutPut;

            return ViewModel;
        }

        public ReferredSearchViewModel SearchReferredMemberDetail(int id)
        {
            var ViewModel = new ReferredSearchViewModel();
            //var Output = new ReferredResponseViewModel();

            var item = referredMemberRepository.GetMember(id);

            var Map = Mapper.Map<ReferredResponseViewModel>(item);

            ViewModel.ReferredMember = Map;

            return ViewModel;

        }

        public string GetCategoryDescription(string IcNumber)
        {
            return referredMemberRepository.GetCategoryDescription(IcNumber);
        }

        public string GetReferredMemberDateCreated(string IcNumber)
        {
            return referredMemberRepository.GetReferredMemberDateCreated(IcNumber);
        }
    }
}
