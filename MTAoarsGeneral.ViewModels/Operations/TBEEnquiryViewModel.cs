using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MTAoarsGeneral.Utilities.Attributes;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Mvc;
using Microsoft.Practices.Unity;

namespace MTAoarsGeneral.ViewModels.Operations
{
    [Serializable]
    public class TBEEnquirySearchViewModel
    {
        public string ICNumber { get; set; }
        public List<TBEEnquiryResponseViewModel> Results;
        public string Message { get; set; }
        
        public TBEEnquiryResponseViewModel Result { get; set; }
        
        //public SelectList SelectListGrade()
        //{
        //    var grade = (Grade)System.Enum.Parse(typeof(Grade), this.Result.Grade);
        //    var items = new List<DropDownList>();
        //    items.Add(new DropDownList() { ID = Grade.A.ToString(), Text = Grade.A.ToString(), Selected = (Grade.A == grade) });
        //    items.Add(new DropDownList() { ID = Grade.B.ToString(), Text = Grade.B.ToString(), Selected = (Grade.B == grade) });
        //    items.Add(new DropDownList() { ID = Grade.C.ToString(), Text = Grade.C.ToString(), Selected = (Grade.C == grade) });
        //    items.Add(new DropDownList() { ID = Grade.F.ToString(), Text = Grade.F.ToString(), Selected = (Grade.F == grade) });
        //    items.Add(new DropDownList() { ID = Grade.X.ToString(), Text = Grade.X.ToString(), Selected = (Grade.X == grade) });

        //    return new SelectList(items, "ID", "Text", items.Where(x => x.Selected == true).Select(x => x.ID).FirstOrDefault());
        //}

        //public SelectList SelectListResult()
        //{
        //    var result = (ExamResult)System.Enum.Parse(typeof(ExamResult), this.Result.Result ?? "none");
        //    var items = new List<DropDownList>();
        //    items.Add(new DropDownList() { ID = ExamResult.none.ToString(), Text = null, Selected = (ExamResult.none == result) });
        //    items.Add(new DropDownList() { ID = ExamResult.Fail.ToString(), Text = ExamResult.Fail.ToString(), Selected = (ExamResult.Fail == result) });
        //    items.Add(new DropDownList() { ID = ExamResult.Pass.ToString(), Text = ExamResult.Pass.ToString(), Selected = (ExamResult.Pass == result) });

        //    return new SelectList(items, "ID", "Text", items.Where(x => x.Selected == true).Select(x => x.ID).FirstOrDefault());
        //}
    }

    public class TBEEnquiryResponseViewModel
    {
        public DateTime ExamDate { get; set; }
        public string ICNumber { get; set; }
        public string Name { get; set; }
        public string Exam { get; set; }
        public string ExamType { get; set; }
        public string Result { get; set; }
        public string Grade { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public int ID { get; set; }

    }

}