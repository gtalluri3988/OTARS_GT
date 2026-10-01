using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.IoC;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Interfaces;
using System.Text.RegularExpressions;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using System.Reflection;
using MTAoarsGeneral.Utilities.Attributes;

namespace MTAoarsGeneral.Services.Notifications {

    public class NotificationService : BaseService, INotificaitonService  {

        INotificationRepository notificationRepository;
        IRepository<Mail> mailRepository;
        IObjectCreator objectCreator;
        ILookupService lookupService;

        public NotificationService(IValidationProvider validationProvider, ILookupService lookupService, INotificationRepository notificationRepository, IRepository<Mail> mailRepository, IObjectCreator objectCreator)
            : base(validationProvider) {
                this.lookupService = lookupService; 
                this.notificationRepository = notificationRepository;
                this.mailRepository = mailRepository;
                this.objectCreator = objectCreator;
        }

        public bool Notify(long fromCompanyId, long toCompanyId, string notificationCode, INotificationVariableSource variableSource) {
            var notification = notificationRepository.Get(notificationCode);
            var subject = ReplaceVariables(notification.Subject, variableSource);
            var body = ReplaceVariables(notification.Body, variableSource);
            var statusId = lookupService.GetMailStatuses().Single(p => p.Code == LookupConstants.MailStatus.New).ID;
            var mail = new Mail {
                FromCompanyID = fromCompanyId, ToCompanyID = toCompanyId,
                Subject = subject, Body = body, FromStatusID = statusId, ToStatusID = statusId,
                CreatedOn = DateTime.Now
            };
            mailRepository.Save(mail);
            mailRepository.SaveChanges();
            return true;
        }

        string ReplaceVariables(string input, INotificationVariableSource source) {
            var type = source.GetType();
            var matches = Regex.Matches(input, @"\{(\w+)\}", RegexOptions.IgnoreCase);
            foreach (Match match in matches) {
                var pinfo = type.GetProperty(match.Groups[1].Value);
                var value = GetValue(pinfo, source);
                input = input.Replace(match.Value, value);
            }
            return input;
        }

        string GetValue(PropertyInfo pinfo, INotificationVariableSource source) {
            var attr = pinfo.GetCustomAttributes(typeof(ValueProviderAttribute), true).FirstOrDefault() as ValueProviderAttribute;
            if (attr == null) return Convert.ToString(pinfo.GetValue(source, null));
            var instance = Activator.CreateInstance(attr.ValueProviderType) as INotificationValueProvider;
            return instance.GetValue(source);

        }


    }// class

}// namespace
