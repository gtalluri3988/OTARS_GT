using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.ViewModels.Maintenance {
    public class IbfimExcelViewModel : ExcelViewModel, IExcelViewModel {

        public IbfimExcelViewModel(string rawData)
            : base(rawData) {
        }

        public string No { get; set; }

        public DateTime ExamDate { get; set; }

        public string StudentName { get; set; }

        public string ICNO { get; set; }

        public string Age { get; set; }

        public string Race { get; set; }

        public string RaceCode { get; set; }

        public string Qualification { get; set; }

        public string Gender { get; set; }

        public string ExaminationType { get; set; }

        public string ExaminationTypeCode { get; set; }

        public string ExamCenter { get; set; }

        public string ExamCenterCode { get; set; }

        public string Result { get; set; }

        public string Grade { get; set; }

        public string Email { get; set; }

        public string TakafulOperatorCode { get; set; }
    }// class
}// namespace
