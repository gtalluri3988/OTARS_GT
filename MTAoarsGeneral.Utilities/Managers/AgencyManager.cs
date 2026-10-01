using MTAoarsGeneral.Utilities.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Managers
{
    public static class AgencyManager
    {
        public static bool IsIndividual(string code)
        {
            return Array.Exists<string>(new string[]
            {
                LookupConstants.AgencyType.Individual,
                //LookupConstants.AgencyType.SoleProprietorship
            }, m => m == code);
        }
    }
}
