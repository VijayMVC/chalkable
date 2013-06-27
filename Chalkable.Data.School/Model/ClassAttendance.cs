﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class ClassAttendance
    {
        public Guid Id { get; set; }
        public Guid ClassPersonRef { get; set; }
        public Guid ClassPeriodRef { get; set; }
        public Guid AttendanceReasonRef { get; set; }
        public string Description { get; set; }
        public AttendanceTypeEnum Type { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastModified { get; set; }
        public int? SisId { get; set; }
    }


    public class ClassAttendanceComplex : ClassAttendance
    {
        public Person Student { get; set; }
        public ClassPerson ClassPerson { get; set; }
        public ClassPeriod ClassPeriod { get; set; }
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
