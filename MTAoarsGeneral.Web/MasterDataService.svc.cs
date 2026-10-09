using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using MTAoarsGeneral.DomainModels;
using System.ServiceModel;
using MTAoarsGeneral.Utilities.Wcf;
using System.Linq.Expressions;

namespace MTAoarsGeneral.Web
{
    // Findings 2-4: do not send server exception details or stack traces in WCF faults.
    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [JSONPSupportBehavior]
    public class MasterDataService : DataService<EntityContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            // config.SetEntitySetAccessRule("MyEntityset", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            // Findings 2-4: retain error diagnostics in server logs only.
            config.UseVerboseErrors = false;
            config.SetEntitySetAccessRule("Users", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupRoles", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("TrainingViewDetails", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("TrainingDetails", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("CBCViewDetails", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("CBCDetails", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("UploadHistories", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;

            config.SetEntitySetAccessRule("LookupGuarantorTypes", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("Activities", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("Companies", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupAgencyTypes", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupCourseCategories", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupDesignations", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupEducationalQualifications", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupInsuranceQualifications", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupMemberLevels", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupRaces", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupReligions", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupMTAAwards", EntitySetRights.AllRead | EntitySetRights.AllWrite);

            config.SetEntitySetAccessRule("LookupStates", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("ReferredMembers", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("ReferredHeaders", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("ReferredDetails", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("TbeExemptions", EntitySetRights.AllRead | EntitySetRights.AllWrite);

            config.SetEntitySetAccessRule("ComplaintViewDetails", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("ConflictViewDetails", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("Complaints", EntitySetRights.AllRead | EntitySetRights.AllWrite);

            config.SetEntitySetAccessRule("InvoiceViewDetails", EntitySetRights.AllRead);
            config.SetEntitySetAccessRule("LookupReferredReasons", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupReferredCategories", EntitySetRights.AllRead | EntitySetRights.AllWrite);

            config.SetEntitySetAccessRule("ALCHeaders", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("ALCMembers", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("Addresses", EntitySetRights.AllRead | EntitySetRights.AllWrite);

            config.SetEntitySetAccessRule("LookupReferredActionTakens", EntitySetRights.AllRead | EntitySetRights.AllWrite);
            config.SetEntitySetAccessRule("LookupReferredPoliceReportLodgeds", EntitySetRights.AllRead | EntitySetRights.AllWrite);


            config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);

        }

        protected override EntityContext CreateDataSource()
        {
            /* [20191121] - Due to [InvoiceViewDetails] is taking up too long to get the count, so increase the CommandTimeout */
            var ds = base.CreateDataSource();
            ds.CommandTimeout = 300;
            return ds;
        }

        [ChangeInterceptor("TrainingDetails")]
        public void OnChangeTrainingDetails(TrainingDetail details, UpdateOperations operations)
        {
            if (operations != UpdateOperations.Delete)
            {
                details.StartDate = details.StartDate.Value.ToLocalTime();
                details.EndDate = details.EndDate.Value.ToLocalTime();
            }
        }

        [ChangeInterceptor("ReferredDetails")]
        public void OnChangeReferredDetails(ReferredDetail detail, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Delete)
            {
                foreach (var attachment in CurrentDataSource.ReferredAttachments.Where(p => p.DetailID == detail.ID))
                {
                    this.CurrentDataSource.ReferredAttachments.DeleteObject(attachment);
                }
            }

            CurrentDataSource.SaveChanges();
        }

        [ChangeInterceptor("ReferredMembers")]
        public void OnChangeReferredMembers(ReferredMember member, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Change)
            {
                /* 2018-12-14 - To Update ReferredDetails when information save on ReferredMember */
                var referredDetail = CurrentDataSource.ReferredDetails.FirstOrDefault(i => i.ICNumber.Equals(member.NewICNumber));
                referredDetail.AllowForRegistration = member.AllowForRegistration;
                referredDetail.IsActive = member.IsActive;

                CurrentDataSource.SaveChanges();
            }
        }

        [ChangeInterceptor("ALCHeaders")]
        public void OnChangeALCHeaders(ALCHeader header, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Delete)
            {
                foreach (var member in CurrentDataSource.ALCMembers.Where(p => p.HeaderID == header.ID))
                {
                    this.CurrentDataSource.ALCMembers.DeleteObject(member);
                }
            }
            //CurrentDataSource.SaveChanges();
        }

        [ChangeInterceptor("LookupRaces")]
        public void OnChangeLookupRace(LookupRace input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupRaces.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupGuarantorTypes")]
        public void OnChangeLookupGuarantorType(LookupGuarantorType input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupGuarantorTypes.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupAgencyTypes")]
        public void OnChangeLookupAgencyType(LookupAgencyType input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupAgencyTypes.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupCourseCategories")]
        public void OnChangeLookupCourseCategory(LookupCourseCategory input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupCourseCategories.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupDesignations")]
        public void OnChangeLookupDesignation(LookupDesignation input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupDesignations.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupEducationalQualifications")]
        public void OnChangeLookupEducationalQualification(LookupEducationalQualification input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupEducationalQualifications.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupInsuranceQualifications")]
        public void OnChangeLookupInsuranceQualification(LookupInsuranceQualification input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupInsuranceQualifications.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupMemberLevels")]
        public void OnChangeLookupMemberLevel(LookupMemberLevel input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupMemberLevels.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupReligions")]
        public void OnChangeLookupReligion(LookupReligion input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupReligions.Max(p => p.ID) + 1;
            }
        }
        [ChangeInterceptor("LookupMTAAwards")]
        public void OnChangeLookupMTAAwards(LookupMTAAward input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                var maxId = CurrentDataSource.LookupMTAAwards?.Max(p => p.ID);
                input.ID = (maxId ?? 0) + 1;
            }
        }

        [ChangeInterceptor("LookupStates")]
        public void OnChangeLookupState(LookupState input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupStates.Max(p => p.ID) + 1;
            }
        }

        [ChangeInterceptor("LookupReferredReasons")]
        public void OnChangeLookupReferredReason(LookupReferredReason input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupReferredReasons.Max(p => p.ID) + 1;
            }

        }

        [ChangeInterceptor("LookupReferredCategories")]
        public void OnChangeLookupReferredCategory(LookupReferredCategory input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {
                input.ID = CurrentDataSource.LookupReferredCategories.Max(p => p.ID) + 1;
            }

        }
        [ChangeInterceptor("LookupReferredActionTakens")]
        public void OnChangeLookupReferredActionTaken(LookupReferredActionTaken input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {

                var maxId = CurrentDataSource.LookupReferredActionTakens?.Max(p => p.ID);
                input.ID = (maxId ?? 0) + 1;
            }

        }

        [ChangeInterceptor("LookupReferredPoliceReportLodgeds")]
        public void OnChangeLookupReferredPoliceReportLodgeds(LookupReferredPoliceReportLodged input, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Add)
            {

                var maxId = CurrentDataSource.LookupReferredPoliceReportLodgeds?.Max(p => p.ID);
                input.ID = (maxId ?? 0) + 1;
            }

        }
        [WebGet]
        public IQueryable<ALCHeader> ALCHeaderWithMembers()
        {
            var query = this.CurrentDataSource.ALCHeaders.Include("Address").Include("ALCMembers").Include("Company");
            return query;
        }


    }// class
}// namespace
