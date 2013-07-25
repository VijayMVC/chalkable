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
        public Guid ClassPeriodRef { get; set; }
        public Guid? AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public AttendanceTypeEnum Type { get; set; }
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
