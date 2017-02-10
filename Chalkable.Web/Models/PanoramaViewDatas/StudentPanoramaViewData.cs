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

        public IList<StudentPanoramaCalendarViewData> Calendars { get; set; } 
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
                StandardizedTests = tests.Select( x => StandardizedTestViewData.Create(x, x.Components, x.ScoreTypes)).ToList(),
                Calendars = StudentPanoramaCalendarViewData.Create(panorama.DailyAbsences, panorama.Infractions, panorama.AllSchoolDays, panorama.SchoolYears)
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

    public class StudentPanoramaCalendarViewData
    {
        public int SchoolYearId { get; set; }
        public string SchoolYearName { get; set; }
        public int AcadYear { get; set; }
        public IList<StudentPanoramaCalendarItemViewData> CalendarItems { get; set; }

        public static IList<StudentPanoramaCalendarViewData> Create(IList<StudentAbsenceInfo> absences, IList<StudentInfractionInfo> studentInfractions
            , IList<Date> allSchoolDays, IEnumerable<SchoolYear> schoolYears)
        {
            var res = new List<StudentPanoramaCalendarViewData>();
            schoolYears = schoolYears.OrderByDescending(sy => sy.AcadYear);
            foreach (var schoolYear in schoolYears)
            {
                var syDays = allSchoolDays.Where(sy => sy.SchoolYearRef == schoolYear.Id);
                var items = syDays.Select(day =>
                {
                    var absence = absences?.FirstOrDefault(a => a.Date == day.Day);
                    var dayInfractions = studentInfractions?.Where(i => i.OccurrenceDate == day.Day).ToList();
                    return StudentPanoramaCalendarItemViewData.Create(absence, dayInfractions, day);
                }).ToList();
                res.Add(new StudentPanoramaCalendarViewData
                {
                    AcadYear = schoolYear.AcadYear,
                    SchoolYearName = schoolYear.Name,
                    SchoolYearId = schoolYear.Id,
                    CalendarItems = items
                });
            }
            return res;
        }
    }

    public class StudentPanoramaCalendarItemViewData
    {
        public DateTime Date { get; set; }
        public string AbsenceLevel { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsHalfAbsent { get; set; }
        public bool IsLate { get; set; }
        public IList<string> Disciplines { get; set; }

        public static StudentPanoramaCalendarItemViewData Create(StudentAbsenceInfo absence, IList<StudentInfractionInfo> infractions, Date date)
        {
            return new StudentPanoramaCalendarItemViewData
            {
                IsAbsent = absence?.AbsenceLevel == "All Day",
                IsHalfAbsent = absence?.AbsenceLevel == "Half Day",
                IsLate = absence?.AbsenceLevel == "Tardy",
                Disciplines = infractions?.Select(x => x.InfractionName).OrderBy(x=>x).ToList(),
                Date = date.Day
            };
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