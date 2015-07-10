using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Attendances
{

    public class ClassAttendance
    {
        public DateTime Date { get; set; }
        public bool IsDailyAttendancePeriod { get; set; }
        public bool IsPosted { get; set; }
        public bool MergeRosters { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyReason { get; set; }
        public int ClassId { get; set; }
        public IList<StudentClassAttendance> StudentAttendances { get; set; }
    }

    public class ClassAttendanceDetails : ClassAttendance
    {
        public Class Class { get; set; }
    }
}
