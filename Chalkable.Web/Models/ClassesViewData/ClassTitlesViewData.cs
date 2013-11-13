using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        public static ClassHoverBoxViewData<ClassAttendanceHoverViewData> Create(IList<ClassAttendanceDetails> attendances, int possibleAbsents)
        {
           return new ClassHoverBoxViewData<ClassAttendanceHoverViewData>
                {
                    Title = possibleAbsents > 0 
                        ? ((100 * attendances.Count(x => x.IsAbsent)) / possibleAbsents).ToString(CultureInfo.InvariantCulture) 
                        : "0",
                    Hover = ClassAttendanceHoverViewData.Create(attendances)
                };
        }
        public static ClassHoverBoxViewData<ClassAverageForMpHoverViewData> Create(IList<MarkingPeriodClassGradeAvg> classGradingStats)
        {
            classGradingStats = classGradingStats.OrderByDescending(x => x.MarkingPeriod.StartDate).ToList();
            var res = new ClassHoverBoxViewData<ClassAverageForMpHoverViewData>
                {
                    Title = classGradingStats.First().Avg.ToString()
                };
            classGradingStats.RemoveAt(0);
            res.Hover = ClassAverageForMpHoverViewData.Create(classGradingStats);
            return res;
        }

        public static ClassHoverBoxViewData<ClassDisciplineHoveViewData> Create(IList<DisciplineType> disciplineTypes, IList<ClassDisciplineDetails> disciplineList)
        {
            var res = new ClassHoverBoxViewData<ClassDisciplineHoveViewData>();
            res.Title = disciplineList.Sum(x => x.DisciplineTypes.Count).ToString();
            disciplineTypes = disciplineTypes.Take(MAX_HOVER_LIST_NUMBER).ToList();
            res.Hover = ClassDisciplineHoveViewData.Create(disciplineTypes, disciplineList);
            return res;
        }

    }

    public class ClassAverageForMpHoverViewData
    {
        public int MarkingPeriodId { get; set; }
        public string MarkingPeriodName { get; set; }
        public int? Avg { get; set; }
        public static IList<ClassAverageForMpHoverViewData> Create(IList<MarkingPeriodClassGradeAvg> classGradingStats)
        {
            var res = classGradingStats.Select(x => new ClassAverageForMpHoverViewData
            {
                MarkingPeriodId = x.MarkingPeriod.Id,
                MarkingPeriodName = x.MarkingPeriod.Name,
                Avg = x.Avg
            });
            return res.ToList();
        }
    }
    public class ClassAttendanceHoverViewData
    {
        private const string ABSENT = "Absent";
        private const string LATE = "Late";
        private const string EXCUSED = "Excused";
        private const string CLASSES = "Classes";
        public int Total { get; set; }
        public string Summary { get; set; }
        private ClassAttendanceHoverViewData(int total, string summary)
        {
            Total = total;
            Summary = summary;
        }
        public static IList<ClassAttendanceHoverViewData> Create(IList<ClassAttendanceDetails> attendances)
        {
            return new List<ClassAttendanceHoverViewData>
                {
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.IsAbsent), ABSENT),
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.IsLate),  LATE), 
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.IsExcused), EXCUSED),
                    new ClassAttendanceHoverViewData(attendances.GroupBy(x=>x.Date).Select(x=>x.Key).Count(), CLASSES)
                };
        }
    }
    public class ClassDisciplineHoveViewData
    {
        public string DisciplineName { get; set; }
        public int Count { get; set; }

        public static IList<ClassDisciplineHoveViewData> Create(IList<DisciplineType> disciplineTypes, IList<ClassDisciplineDetails> disciplines)
        {
            var res = new List<ClassDisciplineHoveViewData>();
            foreach (var disciplineType in disciplineTypes)
            {
                res.Add(new ClassDisciplineHoveViewData
                {
                    DisciplineName = disciplineType.Name,
                    Count = disciplines.Sum(x=>x.DisciplineTypes.Count(y=>y.DisciplineTypeRef == disciplineType.Id))
                });
            }
            return res;
        }
    }
}