using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Repositories.Interfaces;

namespace MTAoarsGeneral.Repositories.Shared
{
    public class PhotoRepository : GenericRepository<PhotoHistory>, IPhotoRepository 
    {
        public PhotoRepository(EntityContext context)
            : base(context) {
        }

        public List<PhotoHistory> Enquiry(string agencyNumber)
        {
            return Context.PhotoHistories.Where(p => p.AgencyNumber == agencyNumber).ToList();
        }
    }
}
