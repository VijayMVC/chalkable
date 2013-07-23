using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.ClassesViewData
{
    public class ClassHoverBoxViewData<T> : HoverBoxesViewData<T>
    {
        public static ClassHoverBoxViewData<ClassAttendanceHoverViewData> Create(IList<ClassAttendanceComplex> attendances, int possibleAbsents)
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
        //public static ClassHoverBoxesViewData<ClassDisciplineHoveViewData> Create(IList<ClassDisciplineComplex> disciplineList)
        //{
        //    var res = new ClassHoverBoxesViewData<ClassDisciplineHoveViewData>();
        //    var disciplineTypeIds = disciplineList.OrderBy(x => x.DisciplineScore).GroupBy(x => x.DisciplineId)
        //                                          .Select(x => x.Key).Take(MAX_HOVER_LIST_NUMBER).ToList();
        //    res.Hover = ClassDisciplineHoveViewData.Create(disciplineTypeIds, disciplineList);
        //    res.Tile = disciplineList.Count;
        //    return res;
        //}

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
        public static IList<ClassAttendanceHoverViewData> Create(IList<ClassAttendanceComplex> attendances)
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

    //public class ClassDisciplineHoveViewData
    //{
    //    public string DisciplineName { get; set; }
    //    public int Count { get; set; }

    //    public static IList<ClassDisciplineHoveViewData> Create(IList<int> disciplineTypeIds, IList<ClassDisciplineComplex> disciplines)
    //    {
    //        var res = new List<ClassDisciplineHoveViewData>();
    //        foreach (var disciplineTypeId in disciplineTypeIds)
    //        {
    //            var currentTypeDisc = disciplines.Where(x => x.DisciplineId == disciplineTypeId).ToList();
    //            res.Add(new ClassDisciplineHoveViewData
    //            {
    //                DisciplineName = currentTypeDisc.Count > 0 ? currentTypeDisc[0].DisciplineName : "",
    //                Count = currentTypeDisc.Count
    //            });
    //        }
    //        return res;
    //    }
    //}
}