using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;
using System.Data;
using MTAoarsGeneral.Utilities.Constants;
using Microsoft.Practices.Unity;
using AutoMapper;
using MTAoarsGeneral.Utilities.Extensions;

namespace MTAoarsGeneral.DomainModels
{

    public partial class SearchAgencyResult
    {
        public DateTime? TerminationDate { get; set; }

        public Boolean? IsResigned { get; set; }

        public Boolean? IsBancaStaff { get; set; }

        public string Designation { get; set; }

        public string ReferredCategory { get; set; }
        public DateTime? ReferredCreatedDate { get; set; }

        public long MemberID { get; set; } //To pull correct Member at enquiry details

        public string BusinessRegistrationNumber { get; set; }
        public string NewBusinessRegistrationNumber { get; set; }
        public string M2Exam { get; set; }
        public DateTime? M2ExamDate { get; set; }
    }

    public partial class SearchAgencyArchiveResult
    {
        public DateTime? TerminationDate { get; set; }

        public Boolean? IsResigned { get; set; }

        public Boolean? IsBancaStaff { get; set; }

        public string Designation { get; set; }

        public string BusinessRegistrationNumber { get; set; }

        public string TBECategory { get; set; }

        public bool IsHistorical { get; set; }
    }

    public partial class User : IEntity
    {
    }

    public partial class Company : IEntity
    {
    }

    public partial class Agency : IEntity
    {
    }

    public partial class Member : IEntity
    {
    }

    public partial class AgencyStatus : IEntity
    {
    }


    public partial class AgencyMember : IEntity
    {
    }

    public partial class AgencyPrincipal : IEntity
    {
        public bool IsIndividual
        {
            get
            {
                return Array.Exists<string>(new string[] { LookupConstants.AgencyType.Individual }, m => m == this.LookupAgencyType.Code);
            }

        }
    }

    public partial class AgencyPrincipalConflict : IEntity
    {
    }

    public partial class AgencyPrincipalStatus : IEntity
    {
    }

    public partial class AgencyPrincipalGuarantor : IEntity
    {
    }

    public partial class AgencyPrincipalHistory : IEntity
    {
    }

    public partial class AgencyPrincipalStatusHistory : IEntity
    {
    }

    public partial class AgencyPrincipalGuarantorHistory : IEntity
    {
    }

    public partial class MemberEducationalQualification : IEntity
    {
    }

    public partial class MemberInsuranceQualification : IEntity
    {
    }

    public partial class Address : IEntity
    {
    }

    public partial class AgencyMember : IEntity
    {
    }

    public partial class AgencyBanker : IEntity
    {
    }

    public partial class MemberExperience : IEntity
    {
    }

    public partial class Spouse : IEntity
    {
    }

    public partial class Journal : IEntity
    {
    }

    public partial class Posting : IEntity
    {
    }

    public partial class Activity : IEntity
    {
    }

    public partial class ActivityDetail : IEntity
    {
    }

    public partial class Notification : IEntity
    {
    }

    public partial class Mail : IEntity { }

    public partial class RenewalHeader : IEntity { }

    public partial class RenewalDetail : IEntity { }

    public partial class TerminationSchedule : IEntity { }

    public partial class Invoice : IEntity { }

    public partial class IbfimResult : IEntity { }

    public partial class MiiResult : IEntity { }

    public partial class TbeExemption : IEntity { }

    public partial class Menu : IEntity
    {
    }

    public partial class RoleMenu : IEntity
    {
    }

    public partial class UserLogin : IEntity
    {
    }

    public partial class TrainingDetail : IEntity
    {
    }

    public partial class CPDDetail : IEntity
    {
    }

    public partial class Receipt : IEntity
    {
    }

    public partial class Runner : IEntity
    {
    }

    public partial class CBCHeader : IEntity
    {
    }

    public partial class CBCDetail : IEntity
    {
    }

    public partial class UploadHistory : IEntity
    {
    }

    public partial class Complaint : IEntity
    {
    }

    public partial class ReferredMember : IEntity
    {
    }

    public partial class ReferredHeader : IEntity
    {
    }

    public partial class ReferredDetail : IEntity
    {
    }

    public partial class ReferredAttachment : IEntity
    {
    }

    public partial class ConflictAttachment : IEntity
    {
    }

    public partial class ConflictGuarantor : IEntity
    {
    }

    public partial class RegistrationUploadHistory : IEntity
    {
    }

    public partial class ALCHeader : IEntity
    {
    }

    public partial class ALCMember : IEntity
    {
    }

    public partial class EnquiryLog : IEntity
    {
    }
    public partial class PhotoHistory : IEntity
    {
    }

    public partial class AdministrativeAuditTrail : IEntity
    {
    }


    public partial class LookupAgencyStatus : ILookupEntity { }

    public partial class LookupIntermediaryType : ILookupEntity { }

    public partial class LookupAgencyPrincipalStatus : ILookupEntity { }

    public partial class LookupAgencyType : ILookupEntity
    {

        public bool IsIndividual
        {
            get
            {
                //return Array.Exists<string>(new string[] { LookupConstants.AgencyType.Individual, LookupConstants.AgencyType.SoleProprietorship }, m => m == this.Code);
                return Array.Exists<string>(new string[] { LookupConstants.AgencyType.Individual }, m => m == this.Code);
            }

        }
    }

    public partial class LookupReferredActionTaken : ILookupEntity { }

    public partial class LookupReferredPoliceReportLodged : ILookupEntity { }

    public partial class LookupGender : ILookupEntity { }

    public partial class LookupRace : ILookupEntity { }

    public partial class LookupEducationalQualification : ILookupEntity { }

    public partial class LookupInsuranceQualification : ILookupEntity { }

    public partial class LookupMTAAward : ILookupEntity { }

    public partial class LookupGuarantorType : ILookupEntity { }

    public partial class LookupState : ILookupEntity { }

    public partial class LookupDesignation : ILookupEntity { }

    public partial class LookupReligion : ILookupEntity { }

    public partial class LookupMaritalStatus : ILookupEntity { }

    public partial class LookupICType : ILookupEntity { }

    public partial class LookupMemberLevel : ILookupEntity { }

    public partial class LookupBank : ILookupEntity { }

    public partial class LookupBankAccountType : ILookupEntity { }

    public partial class LookupMailStatus : ILookupEntity { }

    public partial class LookupRenewalHeaderStatus : ILookupEntity { }

    public partial class LookupRenewalDetailStatus : ILookupEntity { }

    public partial class LookupTerminationStatus : ILookupEntity { }

    public partial class LookupTerminationAction : ILookupEntity { }

    public partial class LookupTbeCategory : ILookupEntity { }

    public partial class LookupRole : ILookupEntity { }

    public partial class LookupCourseCategory : ILookupEntity { }

    public partial class LookupCourse : ILookupEntity { }

    public partial class LookupTrainingStatus : ILookupEntity { }

    public partial class LookupCPDStatus : ILookupEntity { }

    public partial class LookupCBCHeaderStatus : ILookupEntity { }

    public partial class LookupCBCDetailStatus : ILookupEntity { }

    public partial class LookupReferredReason : ILookupEntity { }

    public partial class LookupReferredCategory : ILookupEntity { }

    public partial class LookupReferredHeaderStatus : ILookupEntity { }

    public partial class LookupReferredDetailStatus : ILookupEntity { }

    public partial class LookupConflictStatus : ILookupEntity { }
    public partial class LookupInvoiceStatus : ILookupEntity { }

    public partial class LookupTrainingType : ILookupEntity { }

    public partial class LookupTakafulExemptedExam : ILookupEntity { }

    public partial class LookupTakafulExemptionFor : ILookupEntity { }

    public partial class EntityContext
    {

        public override int SaveChanges(SaveOptions options)
        {
            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
            var identity = dataProvider.Get<Identity>(GlobalConstants.CurrentIdentity);
            if (identity != null)
            {
                foreach (var entry in ObjectStateManager.GetObjectStateEntries(EntityState.Added))
                {
                    var entity = entry.Entity as IEntity;
                    entity.CreatedBy = identity.UserID;
                    entity.CreatedDate = DateTime.Now;
                }
                foreach (var entry in ObjectStateManager.GetObjectStateEntries(EntityState.Modified))
                {
                    var entity = entry.Entity as IEntity;
                    entity.ModifiedBy = identity.UserID;
                    entity.ModifiedDate = DateTime.Now;
                }
            }
            return base.SaveChanges(options);
        }
    }// EntityContext

    public static class EntitiesExtension
    {

        public static IEnumerable<LookupItem> ToLookupItem(this IEnumerable<ILookupEntity> entities)
        {
            foreach (var entity in entities)
            {
                yield return Mapper.Map<LookupItem>(entity);
            }
        }

        public static IEnumerable<LookupItem> ToLookupItem(this IEnumerable<Company> companies)
        {
            foreach (var entity in companies)
            {
                yield return Mapper.Map<LookupItem>(entity);
            }
        }
    }// Entities extension

}
