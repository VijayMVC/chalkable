﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class StudentDailyAttendance
    {
        public Guid Id { get; set; }
        public Guid PersonRef { get; set; }
        public const string PERSON_REF_FIELD_NAME = "PersonRef";
        public DateTime Date { get; set; }
        public const string DATE_FIELD_NAME = "date";
        public int? Arrival { get; set; }
        public int? TimeIn { get; set; }
        public int? TimeOut { get; set; }
    }

    public class StudentDailyAttendanceDetails : StudentDailyAttendance
    {
        public Person Person { get; set; }
    }
}
