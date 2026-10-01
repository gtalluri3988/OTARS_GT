using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.IoC;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Repositories.Interfaces;

namespace MTAoarsGeneral.Mappers.Notifications {
    
    public class NotificationMapper : IMapper {

        public void Map() {
            MapRegistrationNotification();
            MapMail();
            MapReply();
        }

        void MapRegistrationNotification() {
            Mapper.CreateMap<Agency, RegistrationVariableSource>()
                .ConvertUsing(agency => {
                    var output = new RegistrationVariableSource();
                    var cn = agency.AgencyMembers.Single(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee).Member;
                    output.AgencyName = agency.Name;
                    output.CorporateNomineeName = cn.Name;
                    output.CorporateNomineeNewICNumber = cn.NewICNumber;
                    output.CorporateNomineeOldICNumber = cn.OldICNumber;
                    var generalPrincipals = agency.AgencyPrincipals.Where(ap => ap.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).ToList();
                    output.CompanyName = generalPrincipals.First().Company.Name;
                    output.AgencyNumbers = String.Join(",", generalPrincipals.Select(ap => ap.AgencyNumber).ToArray());
                    output.IntermediaryTypes = String.Join(",", generalPrincipals.Select(ap => ap.LookupIntermediaryType.Description).ToArray());
                    return output;
                });

            Mapper.CreateMap<RenewalReminderViewDetail, RenewalReminderVariableSource>();
        }

        void MapMail() {
            Mapper.CreateMap<Mail, MailViewModel>()
                .ForMember(dest => dest.FromCompanyName, opt => opt.MapFrom(src => src.FromCompany.Name))
                .ForMember(dest => dest.ToCompanyName, opt => opt.MapFrom(src => src.ToCompany.Name))
                .ForMember(dest => dest.FromStatusDescription, opt => opt.MapFrom(src => src.FromStatus.Description))
                .ForMember(dest => dest.ToStatusDescription, opt => opt.MapFrom(src => src.ToStatus.Description));
        }

        void MapReply() {
            var lookupRepository = ObjectContainer.Container.Resolve<ILookupRepository>();
            Mapper.CreateMap<Mail, MailReplyViewModel>()
                .ForMember(dest => dest.FromCompanyName, opt => opt.MapFrom(src => src.ToCompany.Name))
                .ForMember(dest => dest.ToCompanyName, opt => opt.MapFrom(src => src.FromCompany.Name))
                .ForMember(dest => dest.FromCompanyID, opt => opt.MapFrom(src => src.ToCompanyID))
                .ForMember(dest => dest.ToCompanyID, opt => opt.MapFrom(src => src.FromCompanyID))
                .ForMember(dest => dest.Subject, opt => opt.MapFrom(src=> String.Concat("Re:", src.Subject)))
                .ForMember(dest => dest.Body, opt => opt.Ignore());

            var statusId = lookupRepository.Get<LookupMailStatus>(LookupConstants.MailStatus.New).ID;
            Mapper.CreateMap<MailReplyViewModel, Mail>()
                .ForMember(dest => dest.CreatedOn, opt => opt.UseValue(DateTime.Now))
                .ForMember(dest => dest.FromStatusID, opt => opt.UseValue(statusId))
                .ForMember(dest => dest.ToStatusID, opt => opt.UseValue(statusId));

        }

    }// class

}// namespace
