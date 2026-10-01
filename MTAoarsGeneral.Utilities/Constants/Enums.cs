using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.Utilities.Constants {

    public enum Actions {
        None = 0,
        Add,
        Update,
        Delete,
        Inclusion
    }

    public enum RegistrationCheckOptions {
        New,
        Include,
        Conflict,
        Reinstate,
        Referred,
        Error
    }

    public enum ExamResult
    {
        none = 0,
        [Description("Fail")]
        Fail = 1,
        [Description("Pass")]
        Pass = 2,
        [Description("Absent")]
        Absent = 3,
        [Description("Applied")]
        Applied = 4
    }

    public enum Grade
    {
        [Description("A")]
        A = 1,
        [Description("B")]
        B = 2,
        [Description("C")]
        C = 3,
        [Description("F")]
        F = 4,
        [Description("X")]
        X = 5,
        [Description("Y")]
        Y = 6
    }

}
