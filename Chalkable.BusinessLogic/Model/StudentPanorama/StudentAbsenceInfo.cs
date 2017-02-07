using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model.StudentPanorama;

namespace Chalkable.BusinessLogic.Model.StudentPanorama
{
    public class StudentAbsenceInfo
    {
        public int SchoolYearId { get; set; }
        public int StudentId { get; set; }
        public short AbsenceReasonId { get; set; }
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }      
        public string AbsenceReasonName { get; set; }
        public DateTime Date { get; set; }
        public IList<string> Periods { get; set; }

        public static IList<StudentAbsenceInfo> Create(IList<StudentDailyAbsence> dailyAbsences, IList<StudentPeriodAbsence> periodAbsences)
        {
            return dailyAbsences?.Select(absence => new StudentAbsenceInfo
            {
                SchoolYearId = absence.AcadSessionId,
                StudentId = absence.StudentId,
                AbsenceReasonId = absence.AbsenceReasonId,
                AbsenceReasonName = absence.AbsenceReasonName,
                AbsenceCategory = absence.AbsenceCategory,
                AbsenceLevel = absence.AbsenceLevel,
                Date = absence.Date,
                Periods = periodAbsences?.Where(x => x.Date == absence.Date).Select(x => x.TimeSlotName).ToList()
            }).ToList();
        }
    }
}
