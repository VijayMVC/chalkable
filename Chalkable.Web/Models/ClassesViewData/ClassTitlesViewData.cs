using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        public static ClassHoverBoxViewData<ClassAttendanceHoverViewData> Create(IList<ClassAttendanceDetails> attendances, int possibleAbsents)
        {
           return new ClassHoverBoxViewData<ClassAttendanceHoverViewData>
                {
                    Titel = ((100*attendances.Count(x => x.Type == AttendanceTypeEnum.Absent))/possibleAbsents).ToString(),
                    Hover = ClassAttendanceHoverViewData.Create(attendances)
                };
        }
        //public static ClassHoverBoxesViewData<ClassAverageForMpHoverViewData> Create(IList<PocoClassGradingStats> classGradingStats, IList<MarkingPeriod> previosMps)
        //{
        //    var res = new ClassHoverBoxesViewData<ClassAverageForMpHoverViewData>
        //        {
        //            Titel = classGradingStats.First(x => x.MarkingPeriodId == previosMps[0].Id).ClassAvg
        //        };
        //    var avgByMp = classGradingStats.GroupBy(x => x.MarkingPeriodId).ToDictionary(x => x.Key, x => x.First().ClassAvg);
        //    res.Hover = ClassAverageForMpHoverViewData.Create(avgByMp, previosMps);
        //    return res;
        //}

        public static ClassHoverBoxViewData<ClassDisciplineHoveViewData> Create(IList<DisciplineType> disciplineTypes, IList<ClassDisciplineDetails> disciplineList)
        {
            var res = new ClassHoverBoxViewData<ClassDisciplineHoveViewData>();
            res.Titel = disciplineList.Sum(x => x.DisciplineTypes.Count).ToString();
            disciplineTypes = disciplineTypes.Take(MAX_HOVER_LIST_NUMBER).ToList();
            res.Hover = ClassDisciplineHoveViewData.Create(disciplineTypes, disciplineList);
            return res;
        }

    }

    public class ClassAverageForMpHoverViewData
    {
        public Guid MarkingPeriodId { get; set; }
        public string MarkingPeriodName { get; set; }
        public int? Avg { get; set; }
        public static IList<ClassAverageForMpHoverViewData> Create(IDictionary<Guid, int?> avgByMp, IList<MarkingPeriod> previosMps)
        {
            var res = previosMps.Select(x => new ClassAverageForMpHoverViewData
            {
                MarkingPeriodId = x.Id,
                MarkingPeriodName = x.Name,
                Avg = avgByMp.ContainsKey(x.Id) ? avgByMp[x.Id] : default(int?)
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
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.Type == AttendanceTypeEnum.Absent), ABSENT),
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.Type == AttendanceTypeEnum.Late),  LATE), 
                    new ClassAttendanceHoverViewData(attendances.Count(x => x.Type == AttendanceTypeEnum.Excused), EXCUSED),
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