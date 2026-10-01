using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Interfaces {

    public interface IActivityRepository : IRepository<Activity> {

        Activity Get(string code);

    }// interface

}// class
