using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Operations
{
    [Serializable()]
    public class ConflictAttachmentViewModel
    {
        public ConflictAttachmentViewModel()
        {
            IsActive = true;
        }

        public long ID { get; set; }

        public string UploadFileName { get; set; }

        public string FilePath { get; set; }

        public bool IsActive { get; set; }
    }// class

}// namespace

