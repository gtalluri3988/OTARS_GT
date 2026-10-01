using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Config;

namespace MTAoarsGeneral.Repositories.Operations {

    public class MiiResultRepository : GenericRepository<MiiResult>, IMiiResultRepository {

        public MiiResultRepository(EntityContext context)
            : base(context) {
              
        }
       
        public IEnumerable<MiiResult> Search(string icNumber){
            return Context.MiiResults.Where(p => p.ICNumber == icNumber).AsEnumerable();
        }

        public MiiResult Search(int id)
        {
            return Context.MiiResults.Where(p => p.ID == id).FirstOrDefault();
        }

        public void Update(MiiResult source,out string message)
        {
            var Result = Context.MiiResults.Where(p => p.ID == source.ID).FirstOrDefault();

            Result.Name = source.Name;
            Result.ICNumber = source.ICNumber;
            Result.ExamDate = source.ExamDate;
            Result.Grade = source.Grade;
            //Result. = source.Result;

            try
            {
                SaveChanges();
                message = "Successfully Save";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
        }
    }// class

}// namespace
