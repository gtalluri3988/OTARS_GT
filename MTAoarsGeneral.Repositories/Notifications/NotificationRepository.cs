using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Notifications {

    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository {

        public NotificationRepository(EntityContext context)
            : base(context) {
        }

        public Notification Get(string code) {
            return Context.Notifications.SingleOrDefault(p => p.Code == code);
        }

    }// class

}// namespace
