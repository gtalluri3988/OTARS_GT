using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.Repositories.Shared;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Config;
using MTAoarsGeneral.Utilities.Constants;
using System.Data.Objects;
using MTAoarsGeneral.Utilities.Interfaces;

namespace MTAoarsGeneral.Repositories.Operations
{

    public class AgencyRepository : GenericRepository<Agency>, IAgencyRepository
    {
        ConfigManager config;
        IScopeDataProvider dataProvider;
        IMemberRepository memberRepository;
        ILookupRepository lookupRepository;
        public AgencyRepository(EntityContext context, IScopeDataProvider dataProvider, ConfigManager config, IMemberRepository memberRepository, ILookupRepository lookupRepository)
            : base(context)
        {
            this.config = config;
            this.dataProvider = dataProvider;
            this.memberRepository = memberRepository;
            this.lookupRepository = lookupRepository;
        }

        public SearchAgencyResult Search(string agencyNumber)
        {
            var query = from ap in Context.AgencyPrincipals
                        join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
                        where ap.AgencyNumber == agencyNumber
                        && am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                        select new { ap, am };
            var item = query.FirstOrDefault();
            if (item == null) return null;
            //Updated below code like to take agenvt type from agnecy principal
            return new SearchAgencyResult
            {
                AgencyID = item.ap.AgencyID,
                AgencyNumber = item.ap.AgencyNumber,
                AgencyPrincipalID = item.ap.ID,
                CompanyID = item.ap.CompanyID,
                CompanyName = item.ap.Company.Name,
                IntermediaryTypeID = item.ap.IntermediaryTypeID,
                IntermediaryTypeDescription = item.ap.LookupIntermediaryType.Description,
                NomineeID = item.am.MemberID,
                NomineeName = item.am.Member.Name,
                NomineeNewICNumber = item.am.Member.NewICNumber,
                NomineeOldICNumber = item.am.Member.OldICNumber,
                AgencyTypeDescription = item.ap.LookupAgencyType.Description,
                ValidFrom = item.ap.ValidFrom,
                ValidTo = item.ap.ValidTo
            };
        }

        public SearchAgencyResult SearchHistories(string agencyNumber)
        {
            var query = from ap in Context.AgencyPrincipalHistories
                        join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
                        where ap.AgencyNumber == agencyNumber
                        && am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                        select new { ap, am };
            var item = query.FirstOrDefault();
            if (item == null) return null;
            //Updated below code like to take agenvt type from agnecy principal
            return new SearchAgencyResult
            {
                AgencyID = item.ap.AgencyID,
                AgencyNumber = item.ap.AgencyNumber,
                AgencyPrincipalID = item.ap.ID,
                CompanyID = item.ap.CompanyID,
                CompanyName = item.ap.Company.Name,
                IntermediaryTypeID = item.ap.IntermediaryTypeID,
                IntermediaryTypeDescription = item.ap.LookupIntermediaryType.Description,
                NomineeID = item.am.MemberID,
                NomineeName = item.am.Member.Name,
                NomineeNewICNumber = item.am.Member.NewICNumber,
                NomineeOldICNumber = item.am.Member.OldICNumber,
                AgencyTypeDescription = item.ap.LookupAgencyType.Description,
                ValidFrom = item.ap.ValidFrom,
                ValidTo = item.ap.ValidTo
            };
        }

        public IEnumerable<SearchAgencyResult> Search(string agencyNumber, string memberName, string icNumber, string businessRegistrationNumber, string newBusinessRegistrationNumber, long companyId, bool isGeneral, bool isFamily)
        {
            var stringParams = new List<string> { agencyNumber, memberName, icNumber, businessRegistrationNumber, newBusinessRegistrationNumber };
            if (stringParams.All(i => string.IsNullOrWhiteSpace(i)))
                return new List<SearchAgencyResult>();

            var identity = dataProvider.Get<Utilities.Mvc.Identity>(GlobalConstants.CurrentIdentity);

            //return Context.SearchAgency(agencyNumber, memberName, icNumber, companyId, businessRegistrationNumber).AsEnumerable();
            var designations = new string[4]
            {
                LookupConstants.Designation.CorporateNominee,
                LookupConstants.Designation.Partner,
                LookupConstants.Designation.Director,
                LookupConstants.Designation.Shareholder
            };
            var lookupDesignations = lookupRepository.GetAll<LookupDesignation>();
            var designationIDs = lookupDesignations.Where(i => designations.Contains(i.Code)).Select(i => i.ID);

            Context.CommandTimeout = 5 * 60; //5 mins

            //var excludeIndividualDesignations = designations.Where(i => !string.Equals(i, LookupConstants.Designation.CorporateNominee)).ToArray();

            IQueryable<AgencyPrincipal> agencyPrincipals = Context.AgencyPrincipals.Where(x => x.Company.IsGeneral);
            IQueryable<AgencyPrincipalHistory> agencyPrincipalHistories = Context.AgencyPrincipalHistories.Where(x => x.Company.IsGeneral);

            if (agencyNumber != null)
            {
                agencyPrincipals = agencyPrincipals.Where(i => i.AgencyNumber == agencyNumber);
                agencyPrincipalHistories = agencyPrincipalHistories.Where(i => i.AgencyNumber == agencyNumber);
            }
            if (memberName != null)
            {
                agencyPrincipals = agencyPrincipals.Where(i =>
                    (i.MemberID != null && i.Member.Name == memberName) ||
                    (i.MemberID == null && i.Agency.AgencyMembers.Any(j => j.Member.Name == memberName))
                );
                agencyPrincipalHistories = agencyPrincipalHistories.Where(i =>
                    (i.MemberID != null && i.Member.Name == memberName) ||
                    (i.MemberID == null && i.Agency.AgencyMembers.Any(j => j.Member.Name == memberName))
                );
            }
            if (icNumber != null)
            {
                //agencyPrincipals = agencyPrincipals.Where(i =>
                //    (i.MemberID != null && (i.Member.NewICNumber == icNumber || i.Member.PassportNumber == icNumber)) ||
                //    (i.MemberID == null && (i.Agency.AgencyMembers.Any(j => j.DesignationID == 1 && (j.Member.NewICNumber == icNumber || j.Member.OldICNumber == icNumber))))); //take CorporateNominee only
                //agencyPrincipalHistories = agencyPrincipalHistories.Where(i =>
                //    (i.MemberID != null && (i.Member.NewICNumber == icNumber || i.Member.PassportNumber == icNumber)) ||
                //    (i.MemberID == null && (i.Agency.AgencyMembers.Any(j => j.DesignationID == 1 && (j.Member.NewICNumber == icNumber || j.Member.OldICNumber == icNumber))))); //take CorporateNominee only

                //[20200724][CR No# URS-IT-OTARS-020] - include enquiry by NRIC number for (Corporate Nominee, Partnership, Shareholder and Director)
                agencyPrincipals = agencyPrincipals.Where(i =>
                    (i.MemberID != null && (i.Member.NewICNumber == icNumber || i.Member.OldICNumber == icNumber)) ||
                    (i.MemberID == null && (i.Agency.AgencyMembers.Any(
                        j => designationIDs.Contains((Int64)j.DesignationID) && (j.Member.NewICNumber == icNumber || j.Member.OldICNumber == icNumber)))));
                agencyPrincipalHistories = agencyPrincipalHistories.Where(i =>
                    (i.MemberID != null && (i.Member.NewICNumber == icNumber || i.Member.OldICNumber == icNumber)) ||
                    (i.MemberID == null && (i.Agency.AgencyMembers.Any(
                        j => designationIDs.Contains((Int64)j.DesignationID) && (j.Member.NewICNumber == icNumber || j.Member.OldICNumber == icNumber)))));
            }
            if (businessRegistrationNumber != null)
            {
                agencyPrincipals = agencyPrincipals.Where(i => i.Agency.BusinessRegistrationNumber == businessRegistrationNumber);
                agencyPrincipalHistories = agencyPrincipalHistories.Where(i => i.Agency.BusinessRegistrationNumber == businessRegistrationNumber);
            }
            if (newBusinessRegistrationNumber != null)
            {
                agencyPrincipals = agencyPrincipals.Where(i => i.Agency.NewBusinessRegistrationNumber == newBusinessRegistrationNumber);
                agencyPrincipalHistories = agencyPrincipalHistories.Where(i => i.Agency.NewBusinessRegistrationNumber == newBusinessRegistrationNumber);
            }

            var apList = agencyPrincipals.ToList();
            logger.Info("Agency Principal List");
            foreach (var ap in apList)
            {
                logger.Info("AP.ID: {0}, AP.AgencyID: {1}, AP.CompanyID: {2}, AP.AgencyNumber: {3}, AP.MemberID: {4}, AP.TypeID: {5}",
                    ap.ID, ap.AgencyID, ap.CompanyID, ap.AgencyNumber, ap.MemberID, ap.TypeID);
            }
            var aphList = agencyPrincipalHistories.ToList();
            logger.Info("Agency Principal History List");
            foreach (var aph in aphList)
            {
                logger.Info("APH.ID: {0}, APH.AgencyID: {1}, APH.CompanyID: {2}, APH.AgencyNumber: {3}, APH.MemberID: {4}, APH.TypeID: {5}",
                    aph.ID, aph.AgencyID, aph.CompanyID, aph.AgencyNumber, aph.MemberID, aph.TypeID);
            }


            var agencyPrincipalPairAgencyMembers = new List<KeyValuePair<long, long>>();
            var agencyPrincipalHistoryPairAgencyMembers = new List<KeyValuePair<long, long>>();
            var apListagencyIds = apList.Select(x => x.AgencyID).ToList();
            var aphListagencyIds = aphList.Select(x => x.AgencyID).ToList();
            var agencyIds = apListagencyIds;
            agencyIds.AddRange(aphListagencyIds);

            var apListagencyMemberIds = apList.Where(a => a.MemberID != null).Select(x => x.MemberID).ToList();
            var aphListagencyMemberIds = aphList.Where(a => a.MemberID != null).Select(x => x.MemberID).ToList();
            var agencyMemberIds = apListagencyMemberIds;
            agencyMemberIds.AddRange(aphListagencyMemberIds);

            var agencyMembers = Context.AgencyMembers.Where(x => agencyIds.Contains(x.AgencyID) || agencyMemberIds.Contains(x.MemberID)).ToList();
            var agencyMembersId = agencyMembers.Select(a => a.MemberID).ToList();
            var members = Context.Members.Where(x => agencyMembersId.Contains(x.ID)).ToList();

            if (!string.IsNullOrWhiteSpace(icNumber))
            {


                if (apList.Count() > 0)
                {
                    var query = from ap in apList
                                join am in agencyMembers on ap.AgencyID equals am.AgencyID
                                join m in members on am.MemberID equals m.ID
                                where (m.NewICNumber == icNumber || m.OldICNumber == icNumber) &&
                                    ((ap.TypeID == 1 && am.DesignationID == 1) || (ap.TypeID > 1)) //Individual only show Corporate Nominee
                                select new KeyValuePair<long, long>(ap.ID, am.ID);
                    agencyPrincipalPairAgencyMembers.AddRange(
                        query.AsEnumerable().ToList()
                    );
                }

                if (aphList.Count() > 0)
                {
                    var hQuery = from ap in aphList
                                 join am in agencyMembers on ap.AgencyID equals am.AgencyID
                                 join m in members on am.MemberID equals m.ID
                                 where (m.NewICNumber == icNumber || m.OldICNumber == icNumber) &&
                                    ((ap.TypeID == 1 && am.DesignationID == 1) || (ap.TypeID > 1)) //Individual only show Corporate Nominee
                                 select new KeyValuePair<long, long>(ap.ID, am.ID);
                    agencyPrincipalHistoryPairAgencyMembers.AddRange(
                        hQuery.AsEnumerable().ToList()
                    );
                }
            }
            else
            {
                if (apList.Count() > 0)
                {
                    var query = from ap in apList
                                from am in agencyMembers
                                    .Where(i => ((ap.MemberID != null && i.MemberID == ap.MemberID) || (ap.MemberID == null && i.AgencyID == ap.AgencyID)) &&
                                        ((ap.TypeID == 1 && i.DesignationID == 1) || (ap.TypeID > 1))) //Individual only show Corporate Nominee
                                                                                                       // .OrderBy(i => new { ap.MemberID, i.DesignationID })
                                    .Take(1)
                                select new KeyValuePair<long, long>(ap.ID, am.ID);
                    agencyPrincipalPairAgencyMembers.AddRange(
                        query.AsEnumerable().ToList()
                    );
                }

                if (aphList.Count() > 0)
                {
                    var hQuery = from ap in aphList
                                 from am in agencyMembers
                                     .Where(i => ((ap.MemberID != null && i.MemberID == ap.MemberID) || (ap.MemberID == null && i.AgencyID == ap.AgencyID)) &&
                                        ((ap.TypeID == 1 && i.DesignationID == 1) || (ap.TypeID > 1))) //Individual only show Corporate Nominee
                                                                                                       //   .OrderBy(i => new { ap.MemberID, i.DesignationID })
                                     .Take(1)
                                 select new KeyValuePair<long, long>(ap.ID, am.ID);
                    agencyPrincipalHistoryPairAgencyMembers.AddRange(
                        hQuery.AsEnumerable().ToList()
                    );
                }
            }

            logger.Info("agencyPrincipalPairAgencyMembers List");
            foreach (var apam in agencyPrincipalPairAgencyMembers)
                logger.Info("AP.ID: {0}, AM.ID: {1}", apam.Key, apam.Value);

            logger.Info("agencyPrincipalHistoryPairAgencyMembers List");
            foreach (var apam in agencyPrincipalHistoryPairAgencyMembers)
                logger.Info("APH.ID: {0}, AM.ID: {1}", apam.Key, apam.Value);


            var notToRelaseStatus = lookupRepository.Get<LookupAgencyPrincipalStatus>(LookupConstants.AgencyPrincipalStatus.NotReleased);

            var currentQuery = from ap in apList
                               from am in agencyMembers
                               from apam in agencyPrincipalPairAgencyMembers
                                   .Where(i => i.Key == ap.ID && i.Value == am.ID)
                                   //.Where(i => (ap.MemberID != null && i.MemberID == ap.MemberID) || (ap.MemberID == null && i.AgencyID == ap.AgencyID))
                                   //.OrderBy(i => new { ap.MemberID, i.DesignationID })
                                   //.Take(1)
                               select new
                               {
                                   ap.ID,
                                   ap.AgencyID,
                                   Remarks = ap.Remarks,
                                   IsNotToRelease = ap.AgencyPrincipalStatus.Any(s => s.StatusID == notToRelaseStatus.ID),
                                   ap.AgencyNumber,
                                   ap.CompanyID,
                                   //ap.LookupIntermediaryType,
                                   IntermediaryTypeID = ap.LookupIntermediaryType.ID,
                                   IntermediaryTypeDescription = ap.LookupIntermediaryType.Description,
                                   //ap.LookupAgencyType,
                                   AgencyTypeID = ap.LookupAgencyType.ID,
                                   AgencyTypeDescription = ap.LookupAgencyType.Description,
                                   //ap.Agency,
                                   CompanName = ap.Company.Name, //ap.Company,
                                   ap.ValidFrom,
                                   ap.ValidTo,
                                   TerminationDate = new Nullable<DateTime>(),
                                   IsResigned = false,
                                   IsBancaStaff = ap.IsBancaStaff,
                                   //Member = am.Member,
                                   //am,
                                   ap.Agency.BusinessRegistrationNumber,
                                   ap.Agency.NewBusinessRegistrationNumber,
                                   TBECategory = am.Member?.LookupTbeCategory?.Description,
                                   IsHistorical = false,

                                   NomineeName = ap.LookupAgencyType.IsIndividual ? am.Member.Name : am.Agency.Name,
                                   NewICNumber = am.Member.NewICNumber,
                                   OldICNumber = am.Member.OldICNumber,
                                   Designation = am.LookupDesignation.Description,

                                   MemberID = am.Member.ID,
                                   M2Exam = ap.Agency.M2Exam,
                                   M2ExamDate = ap.Agency.DateOfExam
                               };

            var apQuery = currentQuery.ToList();

            logger.Info("apQuery List");
            foreach (var apq in apQuery)
                logger.Info("AP.ID: {0}, AP.AgencyID: {1}, AP.AgencyNumber: {2}, AP.CompanyID: {3}, am.Member.ID: {4}, " +
                    "AP.LookupAgencyType.ID: {5}, am.LookupDesignation.Description: {6}",
                    apq.ID, apq.AgencyID, apq.AgencyNumber, apq.CompanyID, apq.MemberID, apq.AgencyTypeID, apq.Designation);

            var terminationActionResign = lookupRepository.Get<LookupTerminationAction>(LookupConstants.TerminationAction.Resign);

            var historyQuery = from ap in aphList
                               from am in agencyMembers
                               from apam in agencyPrincipalHistoryPairAgencyMembers
                                   .Where(i => i.Key == ap.ID && i.Value == am.ID)
                                   //.Where(i => (ap.MemberID != null && i.MemberID == ap.MemberID) || (ap.MemberID == null && i.AgencyID == ap.AgencyID))
                                   //.OrderBy(i => new { ap.MemberID, i.DesignationID })
                                   //.Take(1)
                               select new
                               {
                                   ap.ID,
                                   ap.AgencyID,
                                   Remarks = ap.Remarks,
                                   IsNotToRelease = false,
                                   ap.AgencyNumber,
                                   ap.CompanyID,
                                   //ap.LookupIntermediaryType,
                                   IntermediaryTypeID = ap.LookupIntermediaryType.ID,
                                   IntermediaryTypeDescription = ap.LookupIntermediaryType.Description,
                                   //ap.LookupAgencyType,
                                   AgencyTypeID = ap.LookupAgencyType.ID,
                                   AgencyTypeDescription = ap.LookupAgencyType.Description,
                                   //ap.Agency,
                                   CompanName = ap.Company.Name, //ap.Company,
                                   ap.ValidFrom,
                                   ap.ValidTo,
                                   ap.TerminationDate,
                                   IsResigned = (ap.StatusID == terminationActionResign.ID), //Termination Action
                                   IsBancaStaff = ap.IsBancaStaff,
                                   //Member = am.Member,
                                   //am,
                                   ap.Agency.BusinessRegistrationNumber,
                                   ap.Agency.NewBusinessRegistrationNumber,
                                   TBECategory = am.Member?.LookupTbeCategory?.Description,
                                   IsHistorical = true,

                                   NomineeName = ap.LookupAgencyType.IsIndividual ? am.Member.Name : am.Agency.Name,
                                   NewICNumber = am.Member.NewICNumber,
                                   OldICNumber = am.Member.OldICNumber,
                                   Designation = am.LookupDesignation.Description,

                                   MemberID = am.Member.ID,
                                   M2Exam = ap.Agency.M2Exam,
                                   M2ExamDate = ap.Agency.DateOfExam
                               };

            var aphQuery = historyQuery.ToList();

            logger.Info("aphQuery List");
            foreach (var apq in aphQuery)
                logger.Info("AP.ID: {0}, AP.AgencyID: {1}, AP.AgencyNumber: {2}, AP.CompanyID: {3}, am.Member.ID: {4}, " +
                    "AP.LookupAgencyType.ID: {5}, am.LookupDesignation.Description: {6}",
                    apq.ID, apq.AgencyID, apq.AgencyNumber, apq.CompanyID, apq.MemberID, apq.AgencyTypeID, apq.Designation);

            //var historyQuery = (from ap in Context.AgencyPrincipalHistories
            //                   join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
            //                   where 
            //                        designations.Contains(am.LookupDesignation.Code) & 
            //                        (agencyNumber == null || ap.AgencyNumber == agencyNumber) &
            //                        (memberName == null || am.Member.Name == memberName) &
            //                        (icNumber == null || (ap.Member != null && (ap.Member.NewICNumber == icNumber || ap.Member.PassportNumber == icNumber)) || (ap.Member == null && (am.Member.NewICNumber == icNumber || am.Member.PassportNumber == icNumber))) &
            //                        (businessRegistrationNumber == null || ap.Agency.BusinessRegistrationNumber == businessRegistrationNumber) 
            //                        //am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
            //                   select new
            //                   {
            //                       ap.ID,
            //                       ap.AgencyID,
            //                       Remarks = ap.Remarks,
            //                       IsNotToRelease = false,
            //                       ap.AgencyNumber,
            //                       ap.CompanyID,
            //                       ap.LookupIntermediaryType,
            //                       ap.LookupAgencyType,
            //                       ap.Agency,
            //                       ap.Company,
            //                       ap.ValidFrom,
            //                       ap.ValidTo,
            //                       ap.TerminationDate,
            //                       IsResigned = (ap.LookupTerminationAction.Code == LookupConstants.TerminationAction.Resign),
            //                       IsBancaStaff = ap.IsBancaStaff,
            //                       Member = ap.Member,
            //                       am,
            //                       ap.Agency.BusinessRegistrationNumber,
            //                       TBECategory = am.Member.LookupTbeCategory.Description,
            //                       IsHistorical = true
            //                   }).ToList();



            //var currentQuery = (from ap in Context.AgencyPrincipals
            //                    join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
            //                    where 
            //                        designations.Contains(am.LookupDesignation.Code) & 
            //                        (agencyNumber == null || ap.AgencyNumber == agencyNumber) &
            //                        (memberName == null || am.Member.Name == memberName) &
            //                        (icNumber == null || (ap.Member != null && (ap.Member.NewICNumber == icNumber || ap.Member.PassportNumber == icNumber)) || (ap.Member == null && (am.Member.NewICNumber == icNumber || am.Member.PassportNumber == icNumber))) &
            //                        (businessRegistrationNumber == null || ap.Agency.BusinessRegistrationNumber == businessRegistrationNumber) 
            //                    //am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
            //                    select new
            //                    {
            //                        ap.ID,
            //                        ap.AgencyID,
            //                        Remarks = ap.Remarks,
            //                        IsNotToRelease = ap.AgencyPrincipalStatus.Any(s => s.StatusID == notToRelaseStatus.ID),
            //                        ap.AgencyNumber,
            //                        ap.CompanyID,
            //                        ap.LookupIntermediaryType,
            //                        ap.LookupAgencyType,
            //                        ap.Agency,
            //                        ap.Company,
            //                        ap.ValidFrom,
            //                        ap.ValidTo,
            //                        TerminationDate = new Nullable<DateTime>(),
            //                        IsResigned = false,
            //                        IsBancaStaff = ap.IsBancaStaff,
            //                        Member = ap.Member,
            //                        am,
            //                        ap.Agency.BusinessRegistrationNumber,
            //                        TBECategory = am.Member.LookupTbeCategory.Description,
            //                        IsHistorical = false
            //                    }).ToList();


            //var dbList = historyQuery.Union(currentQuery).ToList();
            var dbList = aphQuery.Union(apQuery).ToList();


            /* remove any individual exist in (Partner, Director, Shareholder) and keep only (CorporateNominee) */
            //var excludedList = dbList.Where(p => excludeIndividualDesignations.Contains(p.am.LookupDesignation.Code) && p.LookupAgencyType.Code == LookupConstants.AgencyType.Individual);

            //var excludedList = dbList.Where(p => p.LookupAgencyType.Code == LookupConstants.AgencyType.Individual);
            //var list = dbList.Except(excludedList);

            var list = dbList;

            logger.Info("list (after Union)");
            foreach (var apq in list)
                logger.Info("AP.ID: {0}, AP.AgencyID: {1}, AP.AgencyNumber: {2}, AP.CompanyID: {3}, am.Member.ID: {4}, " +
                    "AP.LookupAgencyType.ID: {5}, am.LookupDesignation.Description: {6}",
                    apq.ID, apq.AgencyID, apq.AgencyNumber, apq.CompanyID, apq.MemberID, apq.AgencyTypeID, apq.Designation);

            var ICNumbers = new List<string>();
            //ICNumbers.AddRange(list.Select(i => i.am.Member.NewICNumber).ToList());
            //ICNumbers.AddRange(list.Select(i => i.am.Member.OldICNumber).ToList());
            ICNumbers.AddRange(list.Select(i => i.NewICNumber).ToList());
            ICNumbers.AddRange(list.Select(i => i.OldICNumber).ToList());
            ICNumbers = ICNumbers.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct().ToList();

            var referredMembers = Context.ReferredMembers.Where(i => ICNumbers.Contains(i.OldICNumber) || ICNumbers.Contains(i.NewICNumber));

            var returnList = new List<SearchAgencyResult>();
            foreach (var item in list)
            {
                //var referredMember = referredMembers.FirstOrDefault(i => i.NewICNumber == item.am.Member.NewICNumber || i.OldICNumber == item.am.Member.OldICNumber);
                //var isReferredMember = memberRepository.IsMemberReferred(item.am.Member.NewICNumber) || memberRepository.IsMemberReferred(item.am.Member.OldICNumber);

                //[2021-02-18] - if current user is not ISM/MTA, then check Company intermediaryType with referred member listing
                var currentUserCompanyID = (identity.IsISMOrMTA) ? (long?)null : identity.CompanyID;

                var isReferredMember = memberRepository.IsMemberReferred(item.NewICNumber, currentUserCompanyID);
                var referredMember = new ReferredMember();
                if (isReferredMember)
                {
                    if (currentUserCompanyID.HasValue)
                    {
                        var company = Context.Companies.Where(i => i.ID == currentUserCompanyID).FirstOrDefault();
                        referredMember = referredMembers.FirstOrDefault(i => (i.NewICNumber == icNumber || i.OldICNumber == icNumber) &&
                            i.IsGeneral && company.IsGeneral);
                    }
                    if (referredMember == null || referredMember.ID == 0)
                        referredMember = referredMembers.FirstOrDefault(i => i.NewICNumber == item.NewICNumber || i.OldICNumber == item.OldICNumber);
                }

                //var referredMember = referredMembers.FirstOrDefault(i => i.NewICNumber == item.NewICNumber || i.OldICNumber == item.OldICNumber);
                //var isReferredMember = memberRepository.IsMemberReferred(item.NewICNumber) || memberRepository.IsMemberReferred(item.OldICNumber);

                //Update to take agency type from agency principal with company id
                returnList.Add(new SearchAgencyResult
                {
                    AgencyID = item.AgencyID,
                    AgencyNumber = item.AgencyNumber,
                    AgencyPrincipalID = item.ID,
                    CompanyID = item.CompanyID,
                    CompanyName = item.CompanName, //item.Company.Name,
                    IntermediaryTypeID = item.IntermediaryTypeID, //item.LookupIntermediaryType.ID,
                    IntermediaryTypeDescription = item.IntermediaryTypeDescription, //item.LookupIntermediaryType.Description,
                    NomineeName = item.NomineeName, //item.LookupAgencyType.IsIndividual ? item.am.Member.Name : item.am.Agency.Name,
                    NomineeNewICNumber = item.NewICNumber, //item.am.Member.NewICNumber,
                    NomineeOldICNumber = item.OldICNumber, //item.am.Member.OldICNumber,
                    AgencyTypeDescription = item.AgencyTypeDescription, //item.LookupAgencyType.Description,
                    ValidFrom = item.ValidFrom,
                    ValidTo = item.ValidTo,
                    IsNotToRelease = item.IsNotToRelease,
                    Remarks = item.Remarks,
                    IsReferredMember = isReferredMember,
                    TerminationDate = item.TerminationDate,
                    IsResigned = item.IsResigned,
                    IsBancaStaff = item.IsBancaStaff,
                    Designation = item.Designation, //item.am.LookupDesignation.Description,

                    ReferredCategory = isReferredMember ? referredMember.LookupReferredCategory.Description : "",
                    ReferredCreatedDate = isReferredMember ? referredMember.CreatedDate : null,

                    MemberID = item.MemberID, //item.Member.ID //To pull correct Member at enquiry details

                    BusinessRegistrationNumber = item.BusinessRegistrationNumber,
                    NewBusinessRegistrationNumber = item.NewBusinessRegistrationNumber,
                    M2Exam = item.M2Exam,
                    M2ExamDate = item.M2ExamDate
                });
            }

            return returnList;
        }

        public IEnumerable<SearchAgencyArchiveResult> SearchArchive(string agencyNumber, string memberName, string icNumber, string businessRegistrationNumber, long companyId)
        {
            //return Context.SearchAgency(agencyNumber, memberName, icNumber, companyId, businessRegistrationNumber).AsEnumerable();
            string[] designations = new string[4] {
                LookupConstants.Designation.CorporateNominee,
                LookupConstants.Designation.Partner,
                LookupConstants.Designation.Director,
                LookupConstants.Designation.Shareholder
            };
            var historyQuery = from ap in Context.AgencyPrincipalHistoryArchives
                               join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
                               where designations.Contains(am.LookupDesignation.Code)
                               //am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                               select new
                               {
                                   ap.ID,
                                   ap.AgencyID,
                                   Remarks = ap.Remarks,
                                   IsNotToRelease = false,
                                   ap.AgencyNumber,
                                   ap.CompanyID,
                                   ap.LookupIntermediaryType,
                                   ap.LookupAgencyType,
                                   ap.Agency,
                                   ap.CompanyArchive,
                                   ap.ValidFrom,
                                   ap.ValidTo,
                                   ap.TerminationDate,
                                   IsResigned = (ap.LookupTerminationAction.Code == LookupConstants.TerminationAction.Resign),
                                   IsBancaStaff = ap.IsBancaStaff,
                                   Member = ap.Member,
                                   am,
                                   ap.Agency.BusinessRegistrationNumber,
                                   TBECategory = am.Member.LookupTbeCategory.Description,
                                   IsHistorical = true
                               };
            var notToRelaseStatus = Context.LookupAgencyPrincipalStatuses.FirstOrDefault(a => a.Code == LookupConstants.AgencyPrincipalStatus.NotReleased);
            var query = historyQuery.Union(from ap in Context.AgencyPrincipalArchives
                                           join am in Context.AgencyMembers on ap.AgencyID equals am.AgencyID
                                           where designations.Contains(am.LookupDesignation.Code)
                                           //am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee
                                           select new
                                           {
                                               ap.ID,
                                               ap.AgencyID,
                                               Remarks = ap.Remarks,
                                               IsNotToRelease = false, //ap.AgencyPrincipalStatus.Any(s => s.StatusID == notToRelaseStatus.ID),
                                               ap.AgencyNumber,
                                               ap.CompanyID,
                                               ap.LookupIntermediaryType,
                                               ap.LookupAgencyType,
                                               ap.Agency,
                                               ap.CompanyArchive,
                                               ap.ValidFrom,
                                               ap.ValidTo,
                                               TerminationDate = new Nullable<DateTime>(),
                                               IsResigned = false,
                                               IsBancaStaff = ap.IsBancaStaff,
                                               Member = ap.Member,
                                               am,
                                               ap.Agency.BusinessRegistrationNumber,
                                               TBECategory = am.Member.LookupTbeCategory.Description,
                                               IsHistorical = false
                                           });
            if (agencyNumber == null && memberName == null && icNumber == null && businessRegistrationNumber == null) return new List<SearchAgencyArchiveResult>();
            if (agencyNumber != null) query = query.Where(p => p.AgencyNumber == agencyNumber);
            if (memberName != null) query = query.Where(p => p.am.Member.Name.Contains(memberName));
            if (icNumber != null) query = query.Where(p => (p.Member != null && (p.Member.NewICNumber == icNumber || p.Member.PassportNumber == icNumber)) || (p.Member == null && (p.am.Member.NewICNumber == icNumber || p.am.Member.PassportNumber == icNumber)));//  (p.Member != null && (p.Member.NewICNumber == icNumber || p.Member.PassportNumber == icNumber)) || (p.Member == null && (p.am.Member.NewICNumber == icNumber || p.am.Member.PassportNumber == icNumber))
            if (businessRegistrationNumber != null) query = query.Where(p => p.Agency.BusinessRegistrationNumber == businessRegistrationNumber);

            var companyArchive = Context.CompanyArchives.ToList();

            //Update to take agency type from agency principal with company id
            return query.ToList().Select(item => new SearchAgencyArchiveResult
            {
                AgencyID = item.AgencyID,
                AgencyNumber = item.AgencyNumber,
                AgencyPrincipalID = item.ID,
                CompanyID = item.CompanyID,
                CompanyName = item.CompanyArchive.Name, //companyArchive.SingleOrDefault(i => i.ID == item.CompanyID).Name, 
                IntermediaryTypeID = item.LookupIntermediaryType.ID,
                IntermediaryTypeDescription = item.LookupIntermediaryType.Description,
                NomineeName = item.am.Member.Name,
                NomineeNewICNumber = item.am.Member.NewICNumber,
                NomineeOldICNumber = item.am.Member.OldICNumber,
                AgencyTypeDescription = item.LookupAgencyType.Description,
                ValidFrom = item.ValidFrom,
                ValidTo = item.ValidTo,
                //IsNotToRelease = item.IsNotToRelease,
                //Remarks = item.Remarks,
                //IsReferredMember = memberRepository.IsMemberReferred(item.am.Member.NewICNumber) || memberRepository.IsMemberReferred(item.am.Member.OldICNumber),
                TerminationDate = item.TerminationDate,
                IsResigned = item.IsResigned,
                IsBancaStaff = item.IsBancaStaff,
                Designation = item.am.LookupDesignation.Description,
                BusinessRegistrationNumber = item.BusinessRegistrationNumber,
                TBECategory = item.TBECategory,
                IsHistorical = item.IsHistorical
            });
        }

        //Remove unused  code while agency type migratrion
        //public Agency SelectOtherTypeAgency(string businessRegistrationNumber, string currentTypeCode) {
        //    return Context.Agencies
        //        .Where(ag => ag.BusinessRegistrationNumber == businessRegistrationNumber
        //            && ag.LookupAgencyType.Code != currentTypeCode)
        //            .FirstOrDefault();
        //}

        public AgencyPrincipalHistory GetFullReinstateHistory(string agencyNumber)
        {
            //Update to use Agency type from agency principal history
            var list = Context.AgencyPrincipalHistories.Where(aph => aph.AgencyNumber == agencyNumber
                 ).ToList();
            return list.OrderByDescending(p => p.TerminationDate).FirstOrDefault();
        }

        public AgencyPrincipalHistory GetReinstateHistory(long agencyId, long companyId, long intermediaryTypeId, long agencyTypeId)
        {
            //Update to use Agency type from agency principal history
            var list = Context.AgencyPrincipalHistories.Where(aph =>
                aph.AgencyID == agencyId
                && aph.CompanyID == companyId
                && aph.IntermediaryTypeID == intermediaryTypeId
                && aph.TypeID == agencyTypeId
                && aph.ValidTo > DateTime.Now
            ).ToList();
            return list.FirstOrDefault();
            //[CR 20190719-01] - Remove checking on 3 months at reinstatement module
            //return list.FirstOrDefault(p => p.TerminationDate > DateTime.Now.AddMonths(config.CoolingOffPeriod * -1));
        }

        public AgencyPrincipalHistory GetReinstateHistory(string icNumber, long companyId, long intermediaryTypeId, long agencyTypeId)
        {
            //Update to use Agency type from agency principal history
            var list = Context.AgencyPrincipalHistories.Where(aph => aph.Agency.AgencyMembers.Any(am => am.Member.NewICNumber == icNumber || am.Member.PassportNumber == icNumber)
                 && aph.CompanyID == companyId && aph.IntermediaryTypeID == intermediaryTypeId && aph.TypeID == agencyTypeId
                 && aph.ValidTo > DateTime.Now
                 ).ToList();
            return list.FirstOrDefault(p => p.TerminationDate > DateTime.Now.AddMonths(config.CoolingOffPeriod * -1));
        }

        public AgencyPrincipalHistory GetCorporateNomineeReinstateHistory(string icNumber, long companyId, long intermediaryTypeId, long agencyTypeId)
        {
            var list = Context.AgencyPrincipalHistories.Where(aph =>
                aph.Agency.AgencyMembers.Any(am => (am.Member.NewICNumber == icNumber || am.Member.PassportNumber == icNumber) && am.DesignationID == 1)
                && aph.CompanyID == companyId && aph.IntermediaryTypeID == intermediaryTypeId && aph.TypeID == agencyTypeId
                && aph.ValidTo > DateTime.Now
            ).ToList();
            return list.FirstOrDefault(p => p.TerminationDate > DateTime.Now.AddMonths(config.CoolingOffPeriod * -1));
        }

        public Agency Select(string businessRegistrationNumber)
        {
            //return Context.Agencies
            //   .Where(ag => ag.BusinessRegistrationNumber == businessRegistrationNumber && ag.AgencyPrincipals.Any())
            //   .FirstOrDefault();
            return Select(businessRegistrationNumber, false);
        }
        public Agency Select(string businessRegistrationNumber, bool isNew)
        {
            if (isNew)
            {
                return Context.Agencies
                   .Where(ag => ag.NewBusinessRegistrationNumber == businessRegistrationNumber && ag.AgencyPrincipals.Any())
                   .FirstOrDefault();
            }
            return Context.Agencies
               .Where(ag => ag.BusinessRegistrationNumber == businessRegistrationNumber && ag.AgencyPrincipals.Any())
               .FirstOrDefault();
        }

        public IEnumerable<AsciiReportDetail> Search(DateTime fromDate, DateTime toDate)
        {
            return Context.GetAsciiReport(fromDate, toDate, null).AsEnumerable();
        }

        public List<AgencyPrincipal> Enquiry(String registrationNumber, String ICNumber, long companyId)
        {
            var agencyPrincipals = Context.AgencyPrincipals.Where(p =>
                p.CompanyID == companyId &&
                !p.AgencyPrincipalStatus.Any(aps => aps.LookupAgencyPrincipalStatu.Code == LookupConstants.AgencyPrincipalStatus.NotReleased)
            );

            if (!String.IsNullOrEmpty(ICNumber))
            {
                agencyPrincipals = agencyPrincipals.Where(p =>

                    p.Agency.AgencyMembers.Any(am =>
                        (am.Member.NewICNumber == ICNumber || am.Member.OldICNumber == ICNumber || am.Member.PassportNumber == ICNumber) &&
                        am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee  //Only show CorporateNominee [20190627] 
                    )
                );
            }

            if (registrationNumber != null)
            {
                agencyPrincipals = agencyPrincipals.Where(p => p.AgencyNumber == registrationNumber &&
                    p.Agency.AgencyMembers.Any(am => am.LookupDesignation.Code == LookupConstants.Designation.CorporateNominee)); //Only show CorporateNominee [20190627] 
            }


            return agencyPrincipals.ToList();
        }

        public AgencyPrincipal GetAgencyPrincipal(string agencyNumber)
        {
            return Context.AgencyPrincipals
               .Where(ag => ag.AgencyNumber == agencyNumber)
               .FirstOrDefault();
        }

        public AgencyPrincipal GetAgencyPrincipal(long id)
        {
            return Context.AgencyPrincipals.Where(ag => ag.ID == id).FirstOrDefault();
        }

        public AgencyPrincipalHistory GetAgencyPrincipalHistory(long id)
        {
            return Context.AgencyPrincipalHistories.Where(ag => ag.ID == id).FirstOrDefault();
        }

    }// class

}// namespace
