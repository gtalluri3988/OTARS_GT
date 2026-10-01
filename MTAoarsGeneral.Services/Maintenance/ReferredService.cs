using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.ViewModels.Operations;
using AutoMapper;
using MTAoarsGeneral.ViewModels.Maintenance;
using System.Transactions;

namespace MTAoarsGeneral.Services.Maintenance {
    
    public class ReferredService : BaseService, IReferredService{
        IReferredRepository headerRepository;
        IRepository<ReferredDetail> detailRepository;
        IRepository<ReferredAttachment> attachmentRepository;
        ILookupRepository lookupRepository;
        IRepository<ReferredMember> memberRepository;
        long partialyProcessed, processed;

        public ReferredService(IValidationProvider validationProvider,IReferredRepository headerRepository,
            IRepository<ReferredDetail> detailRepository, ILookupRepository lookupRepository, 
            IRepository<ReferredAttachment> attachmentRepository, IRepository<ReferredMember> memberRepository)
            : base(validationProvider) {
                this.headerRepository = headerRepository;
                this.detailRepository = detailRepository;
                this.attachmentRepository = attachmentRepository;
                this.lookupRepository = lookupRepository;
                this.memberRepository = memberRepository;
                partialyProcessed = lookupRepository.Get<LookupReferredHeaderStatus>(LookupConstants.ReferredHeaderStatus.PartialyProcessed).ID;
                processed = lookupRepository.Get<LookupReferredHeaderStatus>(LookupConstants.ReferredHeaderStatus.Processed).ID;
        }

        public long AddHeader(long companyId) {
            var statusId = lookupRepository.Get<LookupReferredHeaderStatus>(LookupConstants.ReferredHeaderStatus.Saved).ID;
            var header = new ReferredHeader() {
                CompanyID = companyId, StatusID = statusId, IsActive = true
            };
            headerRepository.Save(header);
            headerRepository.SaveChanges();
            return header.ID;
        }


        public long AddDetail(ReferredDetailViewModel model) {
            if (Validate(model) == false) return 0;
            var statusId = lookupRepository.Get<LookupReferredDetailStatus>(LookupConstants.RenewalDetailStatus.Generated).ID;
            var detail = Mapper.Map<ReferredDetail>(model);
            detail.StatusID = statusId;
            foreach (var attachment in model.Attachments) {
                detail.ReferredAttachments.Add(Mapper.Map<ReferredAttachment>(attachment));
            }
            detailRepository.Save(detail);
            detailRepository.SaveChanges();
            return detail.ID;
        }

        public void SubmitHeader(long headerId) {
            var header = headerRepository.Get(headerId);
            header.StatusID = lookupRepository.Get<LookupReferredHeaderStatus>(LookupConstants.ReferredHeaderStatus.Submitted).ID;
            headerRepository.SaveChanges();
        }

        public void Approve(long detailId, string comments) {
            using (var scope = new TransactionScope()) {
                var detail = detailRepository.Get(detailId);
                detail.StatusID = lookupRepository.Get<LookupReferredDetailStatus>(LookupConstants.ReferredDetailStatus.Approved).ID;
                detail.ApproverComments = comments;
                detail.ReferredHeader.StatusID = detail.ReferredHeader.ReferredDetails
                        .Where(p => p.LookupReferredDetailStatu.Code == LookupConstants.RenewalDetailStatus.Generated)
                        .Count() > 0 ? partialyProcessed : processed;
                detailRepository.SaveChanges();
                var member = Mapper.Map<ReferredMember>(detail);
                memberRepository.Save(member);
                memberRepository.SaveChanges();
                scope.Complete();
            }
        }

        public ReferredDetailViewModel GetDetail(long detailId) {
            var detail = detailRepository.Get(detailId);
            var output = Mapper.Map<ReferredDetailViewModel>(detail);
            foreach (var attachment in detail.ReferredAttachments) {
                output.Attachments.Add(Mapper.Map<ReferredAttachmentViewModel>(attachment));
            }
            return output;
        }

        public ReferredHeaderViewModel GetHeader(long headerId) {
            var header = headerRepository.Get(headerId);
            if (header == null) return null;
            return Mapper.Map<ReferredHeaderViewModel>(header);
        }

        public ReferredAttachmentViewModel GetAttachment(long attachmentId) {
            var attachment = attachmentRepository.Get(attachmentId);
            return Mapper.Map<ReferredAttachmentViewModel>(attachment);
        }

    }// class

}// namespace
