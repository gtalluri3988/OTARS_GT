using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.ViewModels.Maintenance;

namespace MTAoarsGeneral.Services.Interfaces {
    public interface IReferredService : IBaseService {

        long AddHeader(long companyId);

        long AddDetail(ReferredDetailViewModel model);

        void SubmitHeader(long headerId);

        void Approve(long detailId, string comments);

        ReferredHeaderViewModel GetHeader(long headerId);

        ReferredDetailViewModel GetDetail(long detailId);

        ReferredAttachmentViewModel GetAttachment(long attachmentId);

    }// interface
}// 
