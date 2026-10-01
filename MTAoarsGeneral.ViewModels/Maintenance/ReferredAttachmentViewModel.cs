using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Maintenance
{
    [Serializable]
    public class ReferredAttachmentViewModel
    {
        public ReferredAttachmentViewModel()
        {
            IsActive = true;
        }

        public long ID { get; set; }

        public string UploadFileName { get; set; }

        public string FilePath { get; set; }

        public bool IsActive { get; set; }
    }// class

}// namespace

