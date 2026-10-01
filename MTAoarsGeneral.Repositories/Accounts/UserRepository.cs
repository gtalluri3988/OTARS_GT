using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Accounts {
    public class UserRepository : GenericRepository<User>, IUserRepository {

        public UserRepository(EntityContext context)
            : base(context) {
        }

        public User Get(string userName) {
            return Context.Users.Where(p => p.UserName == userName).FirstOrDefault();
        }

        public User Get(long userID)
        {
            return Context.Users.SingleOrDefault(p => p.ID == userID);
        }
    }// class
}// namespace
