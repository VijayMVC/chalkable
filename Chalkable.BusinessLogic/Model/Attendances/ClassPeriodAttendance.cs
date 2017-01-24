using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Attendances
{
    public class ClassPeriodAttendance
    {
        public DateTime Date { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public IList<StudentPeriodAttendance> StudentAttendances { get; set; }
    }
}
