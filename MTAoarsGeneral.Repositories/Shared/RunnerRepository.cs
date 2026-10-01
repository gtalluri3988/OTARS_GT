using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;

namespace MTAoarsGeneral.Repositories.Shared {
    public class RunnerRepository : IRunnerRepository {

        EntityContext context;
        object sync = new object();

        public RunnerRepository(EntityContext context) {
            this.context = context;
        }

        public string GetNext(string code) {
            string output = "";  
            lock (sync) {
                var runner = context.Runners.Single(p => p.Code == code);
                output = String.Format("{0}{1}", 
                    runner.Prefix,
                    runner.Value.ToString().PadLeft(runner.Length.GetValueOrDefault(), '0'));
                runner.Value += 1;
                context.SaveChanges();
                context.AcceptAllChanges();
            }
            return output;
        }

    }// class
}// namespace
