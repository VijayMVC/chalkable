using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Model.StudentPanorama;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.PanoramaViewDatas
{
    public class StudentPanoramaViewData
    {
        public IList<StandardizedTestStatsViewData> StandardizedTestsStats { get; set; }
        public IList<StudentDailyAbsenceViewData> Absences { get; set; }

        public IList<DailyStatsViewData> AttendanceStats { get; set; }
        public IList<StudentInfractionViewData> DisciplineStats { get; set; }
        public IList<DailyStatsViewData> DailyDisciplineStats { get; set; }
        public StudentProfilePanoramaSetting FilterSettings { get; set; }
        public IList<StandardizedTestViewData> StandardizedTests { get; set; }

        public static StudentPanoramaViewData Create(int studentId, StudentPanoramaInfo panorama, StudentProfilePanoramaSetting settings, IList<StandardizedTestDetails> tests)
        {
            return new StudentPanoramaViewData
            {
                StandardizedTestsStats = StandardizedTestStatsViewData.CreateForStudent(studentId, panorama.StandardizedTests, tests),
                Absences = panorama.DailyAbsences?.Select(StudentDailyAbsenceViewData.Create).OrderBy(x => x.Date).ToList(),
                DisciplineStats = panorama.Infractions?.Select(StudentInfractionViewData.Create).OrderBy(x => x.OccurrenceDate).ToList(),
                AttendanceStats = BuildAttendanceStats(panorama.DailyAbsences, panorama.AllSchoolDays),
                DailyDisciplineStats = BuildDisciplineStats(panorama.Infractions, panorama.AllSchoolDays),
                FilterSettings = settings, 
                StandardizedTests = tests.Select( x => StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList()
            };
        }

        private static IList<DailyStatsViewData> BuildDisciplineStats(IList<StudentInfractionInfo> studentInfractions, IList<Date> allSchoolDays)
        {
            var res = new List<DailyStatsViewData>();
            foreach (var day in allSchoolDays.Select(d=>d.Day))
            {
                var infractionsCount = studentInfractions?.Count(x => x.OccurrenceDate == day.Date) ?? 0;
                res.Add(DailyStatsViewData.Create(day.Date, infractionsCount, "MMM yyyy"));
            }
            return res;
        } 

        private static IList<DailyStatsViewData> BuildAttendanceStats(IList<StudentAbsenceInfo> absences, IList<Date> allSchoolDays)
        {
            var res = new List<DailyStatsViewData>();
            foreach (var day in allSchoolDays.Select(x=>x.Day))
            {
                var absent = absences?.FirstOrDefault(x => x.Date == day.Date);
                decimal number = 1;
                if (absent?.AbsenceLevel == "All Day") number = 0;
                if (absent?.AbsenceLevel == "Half Day") number = 0.5m;
                res.Add(DailyStatsViewData.Create(day.Date, number, "MMM yyyy"));
            }
            return res;
        } 
    }

    public class StudentDailyAbsenceViewData
    {
        public int SchoolYearId { get; set; }
        public int StudentId { get; set; }
        public int AbsenceReasonId { get; set; }
        public string AbsenceCategory { get; set; }
        public string AbsenceLevel { get; set; }
        public string AbsenceReasonName { get; set; }
        public DateTime Date { get; set; }
        public IList<string> Periods { get; set; }

        public static StudentDailyAbsenceViewData Create(StudentAbsenceInfo model)
        {
            return new StudentDailyAbsenceViewData
            {
                SchoolYearId = model.SchoolYearId,
                Date = model.Date,
                StudentId = model.StudentId,
                AbsenceReasonId = model.AbsenceReasonId,
                AbsenceReasonName = model.AbsenceReasonName,
                AbsenceLevel = model.AbsenceLevel,
                AbsenceCategory = model.AbsenceCategory,
                Periods = model.Periods
            };
        }
    }

    public class StudentInfractionViewData
    {
        public int StudentId { get; set; }
        public DateTime OccurrenceDate { get; set; }
        public string InfractionName { get; set; }
        public string InfractionStateCode { get; set; }
        public byte DemeritsEarned { get; set; }
        public bool IsPrimary { get; set; }
        public string DispositionName { get; set; }
        public string DispositionNote { get; set; }
        public DateTime? DispositionStartDate { get; set; }

        public static StudentInfractionViewData Create(StudentInfractionInfo model)
        {
            return new StudentInfractionViewData
            {
                StudentId = model.StudentId,
                OccurrenceDate = model.OccurrenceDate,
                InfractionStateCode = model.InfractionStateCode,
                IsPrimary = model.IsPrimary,
                DemeritsEarned = model.DemeritsEarned,
                DispositionName = model.DispositionName,
                DispositionNote = model.DispositionNote,
                DispositionStartDate = model.DispositionStartDate,
                InfractionName = model.InfractionName
            };
        }
    }
}