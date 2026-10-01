using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;

namespace MTAoarsGeneral.ViewModels.Notifications {
    
    public class RenewedListValueProvider : INotificationValueProvider {


        public string GetValue(INotificationVariableSource source) {
            var input = source as IExistableRenewalList;
            var sb = new StringBuilder();
            sb.Append("<table width='100%'>");
            sb.Append("<tr><th>Agency Number</th><th>Agency Name</th><th>Intermediary Type</th></tr>");
            foreach (var item in input.RenewalList) {
                sb.Append("<tr>");
                sb.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", item.AgencyNumber, item.AgencyName, item.IntermediaryTypeDescription);
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

    }// class

}// namesapce
