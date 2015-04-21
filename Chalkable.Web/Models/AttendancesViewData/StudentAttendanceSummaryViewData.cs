using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Attendances;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceSummaryViewData : StudentViewData
    {
        public IList<MarkingPeriodViewData> MarkingPeriods { get; set; }
        public MarkingPeriodViewData CurrentMarkingPeriod { get; set; }
        public StudentAttendanceHoverBox Absences { get; set; }
        public StudentAttendanceHoverBox Lates { get; set; }
        public StudentAttendanceHoverBox Presents { get; set; }

        protected StudentAttendanceSummaryViewData(StudentDetails student) : base(student)
        {
        }

        public static StudentAttendanceSummaryViewData Create(StudentAttendanceSummary attendanceSummary, MarkingPeriod currentMarkingPeriod, IList<MarkingPeriod> markingPeriods)
        {
            var res = new StudentAttendanceSummaryViewData(attendanceSummary.Student)
                {
                    MarkingPeriods = MarkingPeriodViewData.Create(markingPeriods)
                };
            if (currentMarkingPeriod != null)
            {
                res.CurrentMarkingPeriod = MarkingPeriodViewData.Create(currentMarkingPeriod);
            }
            if (attendanceSummary.DailyAttendanceSummary != null)
            {
                if (attendanceSummary.DailyAttendanceSummary.Absences.HasValue)
                    res.Absences = PrepareAbsentBox(attendanceSummary);
                if (attendanceSummary.DailyAttendanceSummary.Tardies.HasValue)
                    res.Lates = PrepareLatesBox(attendanceSummary);
                if (attendanceSummary.DailyAttendanceSummary.Presents.HasValue)
                    res.Presents = PreparePresentBox(attendanceSummary);   
            }
            return res;
        }

        private static StudentAttendanceHoverBox PrepareAbsentBox(StudentAttendanceSummary attendanceSummary)
        {
            var res = new StudentAttendanceHoverBox();
            var absences = attendanceSummary.DailyAttendanceSummary.Absences.Value;
            var posibleAbsent = attendanceSummary.DailyAttendanceSummary.TotalAttendanceCount;
            res.Title = absences.ToString();
            res.IsPassing = posibleAbsent > 0 && (absences * 100) / posibleAbsent > 5;
            res.Hover = attendanceSummary.ClassAttendanceSummaries
                .Select(x => new StudentAttendnaceHoverBoxItemViewData
                {
                    AttendnaceCount = x.Absences.HasValue ? (int)x.Absences.Value : 0,
                    ClassName = x.Class.Name
                }).ToList();
            return res;
        }
        
        private static StudentAttendanceHoverBox PrepareLatesBox(StudentAttendanceSummary attendanceSummary)
        {
            var res = new StudentAttendanceHoverBox();
            var tardies = attendanceSummary.DailyAttendanceSummary.Tardies.Value;
            var posibleTardies = attendanceSummary.DailyAttendanceSummary.TotalAttendanceCount;
            res.Title = tardies.ToString();
            res.IsPassing = posibleTardies > 0 && (tardies * 100) / posibleTardies > 10;
            res.Hover = attendanceSummary.ClassAttendanceSummaries
                .Select(x => new StudentAttendnaceHoverBoxItemViewData
                {
                    AttendnaceCount = x.Tardies.HasValue ? x.Tardies.Value : 0,
                    ClassName = x.Class.Name
                }).ToList();
            return res;
        }

        private static StudentAttendanceHoverBox PreparePresentBox(StudentAttendanceSummary attendanceSummary)
        {
            var res = new StudentAttendanceHoverBox();
            var presentes = attendanceSummary.DailyAttendanceSummary.Presents.Value;
            res.Title = presentes.ToString();
            res.IsPassing = true;
            res.Hover = attendanceSummary.ClassAttendanceSummaries
                .Select(x => new StudentAttendnaceHoverBoxItemViewData
                {
                    AttendnaceCount = x.Presents.HasValue ? (int)x.Presents.Value : 0,
                    ClassName = x.Class.Name
                }).ToList();
            return res;
        }
    }

    public class StudentAttendanceHoverBox : HoverBoxesViewData<StudentAttendnaceHoverBoxItemViewData>
    {
        public bool IsPassing { get; set; }
    }

    public class StudentAttendnaceHoverBoxItemViewData
    {
        public int AttendnaceCount { get; set; }
        public string ClassName { get; set; }
    }
}