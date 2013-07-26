using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.CalendarsViewData
{

    public class DisciplineTypeCalendarItemViewData : DisciplineTypeViewData
    {
        public int Count { get; set; }
        public int PeriodOrder { get; set; }
    }

    public class DisciplineMonthCalendarViewData : MonthCalendarViewData
    {
        public IList<DisciplineView> Disciplines { get; set; }
        public IList<DisciplineTypeCalendarItemViewData> DisciplineTypes { get; set; }
        public int MoreCount { get; set; }
        public const int DISCIPLINE_COUNT = 3;

        protected DisciplineMonthCalendarViewData(DateTime date, bool isCurrentMonth) : base(date, isCurrentMonth)
        {
        }

        public static DisciplineMonthCalendarViewData Create(DateTime date, bool isCurrentMonth, IList<ClassDetails> classes
                                                      , IList<ClassDisciplineDetails> disciplines)
        {
            var dscViewDataList = DisciplineView.Create(disciplines, classes);
            var dscTypesList = new List<DisciplineTypeViewData>();
            var moreCount = 0;
            foreach (var discipline in dscViewDataList)
            {
                dscTypesList.AddRange(discipline.DisciplineTypes);
            }
            var sortedDisciplineTypes = dscTypesList
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.ToList())
                    .OrderByDescending(x => x.Value.Count)
                    .Select(x => new DisciplineTypeCalendarItemViewData
                    {
                        Id = x.Key,
                        Name = x.Value.First().Name,
                        Count = x.Value.Count,
                    }).ToList();

            if (sortedDisciplineTypes.Count > DISCIPLINE_COUNT)
            {
                moreCount = sortedDisciplineTypes.Skip(DISCIPLINE_COUNT).Sum(x => x.Count);
                sortedDisciplineTypes = sortedDisciplineTypes.Take(DISCIPLINE_COUNT).ToList();
            }
            return new DisciplineMonthCalendarViewData(date, isCurrentMonth)
            {
                DisciplineTypes = sortedDisciplineTypes,
                Disciplines = dscViewDataList,
                MoreCount = moreCount,
            };
        }
    }
}