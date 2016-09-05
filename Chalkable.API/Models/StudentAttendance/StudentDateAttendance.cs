using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Chalkable.API.Models.StudentAttendance
{
    public class StudentDateAttendance
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("student")]
        public Student Student { get; set; }

        [JsonProperty("dailyattendance")]
        public StudentDailyAttendance DailyAttendance { get; set; }

        [JsonProperty("periodattendances")]
        public IList<StudentPeriodAttendance> PeriodAttendances { get; set; }

        [JsonProperty("checkincheckouts")]
        public IList<CheckInCheckOut> CheckInCheckOuts { get; set; }
    }
}
