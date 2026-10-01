using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using MTAoarsGeneral.Repositories.Operations;
using MTAoarsGeneral.Repositories.Interfaces;

namespace MTAoarsGeneral.Mappers.Operations
{

    public class InvoiceMapper : IMapper
    {

        IAgencyRepository agencyRepository;
        public InvoiceMapper(IAgencyRepository agencyRepository)
        {
            this.agencyRepository = agencyRepository;
        }

        public void Map()
        {
            MapInvoice();
        }

        void MapInvoice()
        {
            Mapper.CreateMap<Invoice, InvoiceViewModel>()
                .ForMember(dest => dest.IsPaid, opt => opt.MapFrom(src => src.Receipts.Count > 0))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                 .ForMember(dest => dest.InvoiceStatus, opt => opt.MapFrom(src => Mapper.Map<LookupItem>(src.LookupInvoiceStatus)));

            Mapper.CreateMap<Posting, InvoiceDetailViewModel>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Journal.Description))
                .ForMember(dest => dest.JournalCreatedDate, opt => opt.MapFrom(src => src.Journal.CreatedDate))
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Journal.Agency.Name))
                .ForMember(dest => dest.Uri, opt => opt.MapFrom(src => src.Journal.Uri))
                .AfterMap((src, dest) =>
                {
                    var principal = src.Journal.Agency.AgencyPrincipals
                             .Where(p => p.CompanyID == src.Journal.CompanyID
                                 && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General).FirstOrDefault();
                    if (principal != null)
                    {
                        dest.AgencyNumber = principal.AgencyNumber;
                        dest.IntermediaryTypeDescription = principal.LookupIntermediaryType.Description;
                    }
                    dest.IsBancaStaff = src.Journal.Agency.AgencyMembers
                        .First(p => p.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee)
                        .Member.IsBancaStaff;
                });

            //Mapper.CreateMap<Posting, InvoiceDetailExportModel>()
            //    .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Journal.Company.Name))
            //    .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.CreatedDate))
            //    .ForMember(dest => dest.IntermediaryTypeDescription, opt => opt.MapFrom(src => src.Journal.LookupIntermediaryType.Description))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Journal.Description))
            //    .AfterMap((src, dest) =>
            //    {
            //        AgencyPrincipal agencyPrincipal;
            //        var journal = src.Journal;
            //        if (journal.Description == LookupConstants.Activities.Renewal)
            //        {
            //            var agencyPrincipalId = Convert.ToInt64(journal.Uri.Split('/').Last());
            //            agencyPrincipal = agencyRepository.GetAgencyPrincipal(agencyPrincipalId);
            //        }
            //        else
            //        {
            //            agencyPrincipal = journal.Agency.AgencyPrincipals.Where(
            //                i => i.CompanyID == journal.CompanyID && i.IntermediaryTypeID == journal.IntermediaryTypeID).FirstOrDefault();
            //        }

            //        if (agencyPrincipal != null)
            //        {
            //            dest.IsBancaStaff = (!agencyPrincipal.IsBancaStaff.HasValue || agencyPrincipal.IsBancaStaff == false) ? "N" : "Y";
            //            dest.AgencyName = agencyPrincipal.Agency.Name;
            //            dest.AgencyNumber = agencyPrincipal.AgencyNumber;
            //            dest.AgencyTypeDescription = agencyPrincipal.LookupAgencyType.Description;
            //            dest.BusinessRegistrationNumber = agencyPrincipal.Company.BusinessRegistrationNumber;
            //            dest.ValidFrom = agencyPrincipal.ValidFrom;
            //            dest.ValidTo = agencyPrincipal.ValidTo;
            //            dest.Amount = src.Amount;

            //            var corporateNorminee = agencyPrincipal.Agency.AgencyMembers.FirstOrDefault(
            //                i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
            //            if (corporateNorminee != null)
            //            {
            //                dest.NomineeName = corporateNorminee.Member.Name;
            //                dest.NomineeICNumber = corporateNorminee.Member.NewICNumber;
            //            }
            //        }
            //        else
            //        {
            //            AgencyPrincipalHistory agencyPrincipalHistory;
            //            if (journal.Description == LookupConstants.Activities.Renewal)
            //            {
            //                var agencyPrincipalHistoryId = Convert.ToInt64(journal.Uri.Split('/').Last());
            //                agencyPrincipalHistory = agencyRepository.GetAgencyPrincipalHistory(agencyPrincipalHistoryId);
            //            }
            //            else
            //            {
            //                agencyPrincipalHistory = journal.Agency.AgencyPrincipalHistories.Where(
            //                    i => i.CompanyID == journal.CompanyID && i.IntermediaryTypeID == journal.IntermediaryTypeID).FirstOrDefault();
            //            }

            //            if (agencyPrincipalHistory != null)
            //            {
            //                dest.IsBancaStaff = (!agencyPrincipalHistory.IsBancaStaff.HasValue || agencyPrincipalHistory.IsBancaStaff == false) ? "N" : "Y";
            //                dest.AgencyName = agencyPrincipalHistory.Agency.Name;
            //                dest.AgencyNumber = agencyPrincipalHistory.AgencyNumber;
            //                dest.AgencyTypeDescription = agencyPrincipalHistory.LookupAgencyType.Description;
            //                dest.BusinessRegistrationNumber = agencyPrincipalHistory.Company.BusinessRegistrationNumber;
            //                dest.ValidFrom = agencyPrincipalHistory.ValidFrom;
            //                dest.ValidTo = agencyPrincipalHistory.ValidTo;
            //                dest.Amount = src.Amount;

            //                var corporateNorminee = agencyPrincipalHistory.Agency.AgencyMembers.FirstOrDefault(
            //                    i => i.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee);
            //                if (corporateNorminee != null)
            //                {
            //                    dest.NomineeName = corporateNorminee.Member.Name;
            //                    dest.NomineeICNumber = corporateNorminee.Member.NewICNumber;
            //                }
            //            }
            //        }

            //    });
        }

    }// class

}// namespace
