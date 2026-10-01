using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Operations;

namespace MTAoarsGeneral.Builders.Interfaces
{
    public interface IRegsitrationCreator
    {
         void Process(RegistrationViewModel model);
         RegistrationViewModel Create(List<string> parameters);
        
         Dictionary<string, string> ErrorHandler{get;set;}
    }
}
