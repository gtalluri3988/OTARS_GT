using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;

namespace MTAoarsGeneral.Repositories.Maintenance
{

    public class ReferredRepository : GenericRepository<ReferredHeader>, IReferredRepository
    {

        public ReferredRepository(EntityContext context)
            : base(context)
        {
        }

        public ReferredDetail GetDetail(string icNumber)
        {
            return Context.ReferredDetails.Where(p => p.ICNumber == icNumber).FirstOrDefault();
        }

        public ReferredDetail GetDetailByCompany(string icNumber, long referredHeaderID, long reasonId)
        {
            var referredHeader = Context.ReferredHeaders.FirstOrDefault(i => i.ID == referredHeaderID);
            return Context.ReferredDetails.FirstOrDefault(p => p.ICNumber == icNumber &&
                                                            p.ReferredHeader.CompanyID == referredHeader.CompanyID &&
                                                           p.ReasonID == reasonId);
        }
    }// class

}// namespace
