using MTAoarsGeneral.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Administrative;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.Repositories.Administrative
{
    public class ReferredMemberRepository : GenericRepository<ReferredMember>, IReferredMemberRepository
    {
        public ReferredMemberRepository(EntityContext context)
            : base(context)
        {
        }

        public List<ReferredMember> GetMembers(string IcNumber)
        {
            return Context.ReferredMembers.Where(x => x.NewICNumber == IcNumber).ToList();
        }

        public ReferredMember GetMember(int ID)
        {
            return Context.ReferredMembers.Where(x => x.ID == ID).FirstOrDefault();
        }

        public List<LookupReferredReason> GetLookupReasons(int CategoryID)
        {
            return Context.LookupReferredReasons.Where(x => x.ReferredCategoryID == CategoryID && x.IsActive).ToList();
        }

        public string LookupReferredReason(int ID)
        {
            return Context.LookupReferredReasons.Where(x => x.ID == ID).Select(x => x.Description).FirstOrDefault();
        }

        public string LookupReferredCategory(int ID)
        {
            return Context.LookupReferredCategories.Where(x => x.ID == ID).Select(x => x.Description).FirstOrDefault();
        }

        public void UpdateReferred(ReferredSearchViewModel model, out string message)
        {
            var Member = Context.ReferredMembers.Where(x => x.ID == model.ReferredMember.ID).FirstOrDefault();
            var Details = Context.ReferredDetails.Where(x => x.ICNumber == Member.NewICNumber
                            && x.CategoryID == Member.CategoryID
                            && x.ReasonID == Member.ReasonID
                            && x.ModifiedBy == Member.ModifiedBy
                            && x.ModifiedDate == Member.ModifiedDate).FirstOrDefault();

            //Table ReferredMember
            Member.NewICNumber = model.ReferredMember.ICNumber;
            Member.Name = model.ReferredMember.Name;
            Member.ReasonID = model.ReferredMember.ReasonID;

            if (Details != null)
            {
                //Table ReferredDetails
                Details.ICNumber = model.ReferredMember.ICNumber;
                Details.Name = model.ReferredMember.Name;
                Details.ReasonID = model.ReferredMember.ReasonID;
            }

            try
            {
                SaveChanges();
                message = "Successfully Save";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            //if (Member == null || Details == null)
            //{
            //    message = "Referred Member or Details not found";
            //}
            //else
            //{

            //}
        }
        public List<LookupReferredActionTaken> GetLookupReferredActionTaken(int ActionId)
        {
            return Context.LookupReferredActionTakens.Where(x => x.ID == ActionId && x.IsActive).ToList();
        }

        public string LookupReferredActionTaken(int ID)
        {
            return Context.LookupReferredActionTakens.Where(x => x.ID == ID).Select(x => x.Description).FirstOrDefault();
        }
        public ReferredDetail ReferredDetail(ReferredMember ReferredMember)
        {
            var Member = Context.ReferredMembers.Where(x => x.ID == ReferredMember.ID).FirstOrDefault();
            var Details = Context.ReferredDetails.Where(x => x.ICNumber == Member.NewICNumber
                            && x.CategoryID == Member.CategoryID
                            && x.ReasonID == Member.ReasonID
                            && x.ModifiedBy == Member.ModifiedBy
                            && x.ModifiedDate == Member.ModifiedDate).FirstOrDefault();

            return Details;
        }

        public string GetCategoryDescription(string IcNumber)
        {
            var referredMembers = this.GetMembers(IcNumber);
            if (referredMembers.IsNullOrEmpty())
                return string.Empty;
            return string.Join(", ", referredMembers.Select(i => i.LookupReferredCategory.Description));
        }

        public string GetReferredMemberDateCreated(string IcNumber)
        {
            var referredMembers = this.GetMembers(IcNumber);
            if (referredMembers.IsNullOrEmpty())
                return string.Empty;
            var dt = referredMembers.Max(i => i.CreatedDate);
            if (dt == null)
                return string.Empty;
            return dt.ToDateFormat();
        }
    }
}
