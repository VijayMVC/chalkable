using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassAttendance
    {
        public Guid Id { get; set; }
        public Guid ClassPersonRef { get; set; }
        public const string CLASS_PERIOD_REF_FIELD = "ClassPeriodRef";
        public Guid ClassPeriodRef { get; set; }
        public Guid? AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public const string TYPE_FIELD = "Type";
        public AttendanceTypeEnum Type { get; set; }
        public const string DATE_FIELD = "Date";
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }
        public int? SisId { get; set; }
    }


    public class ClassAttendanceDetails : ClassAttendance
    {
        public Person Student { get; set; }

        private ClassPerson person;
        [DataEntityAttr]
        public ClassPerson ClassPerson
        {
            get { return person; }
            set
            {
                person = value;
                if (value != null && value.Id != Guid.Empty)
                    ClassPersonRef = value.Id;
            }
        }

        private ClassPeriod period;
        [DataEntityAttr]
        public ClassPeriod ClassPeriod
        {
            get { return period; }
            set
            {
                period = value;
                if (value != null && value.Id != Guid.Empty)
                    ClassPeriodRef = value.Id;
            }
        }
        
        [DataEntityAttr]
        public Class Class { get; set; }
        
    }

    public class AttendanceTotalPerType
    {
        public const string TOTAL_FIELD = "Total";
        public int Total { get; set; }
        public const string ATTENDANCE_TYPE_FIELD = "AttendanceType";
        public AttendanceTypeEnum AttendanceType { get; set; }     
    }
    public class PersonAttendanceTotalPerType : AttendanceTotalPerType
    {
        public Guid PersonId { get; set; }
    }
    public class StudentAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public Guid PersonId { get; set; }
        public int PeriodOrder { get; set; }
    }
    public class StudentCountAbsentFromPeriod
    {
        public DateTime Date { get; set; }
        public int StudentCount { get; set; }
        public int PeriodOrder { get; set; }
    }


    [Flags]
    public enum AttendanceTypeEnum
    {
        NotAssigned = 1,
        Present = 2,
        Excused = 4,
        Absent = 8,
        Late = 16
    }
}
