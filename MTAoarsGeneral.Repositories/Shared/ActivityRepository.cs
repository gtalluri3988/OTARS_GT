using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Shared {
    
    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository {

        public ActivityRepository(EntityContext context)
            : base(context) {
        }


        public Activity Get(string code) {
            return Context.Activities.SingleOrDefault(p => p.Code == code);
        }

    }// class

}// namespace
