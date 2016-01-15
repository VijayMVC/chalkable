using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Models.CalendarsViewData
{

    

    public class DisciplineMonthCalendarViewData : MonthCalendarViewData
    {
        private class DisciplineTypeCalendarItemViewData : DisciplineTypeViewData
        {
            public int Count { get; set; }
        }
        
        public IList<DisciplineView> Disciplines { get; set; }
        public int MoreCount { get; set; }
        public const int DISCIPLINE_COUNT = 3;
        public bool HasDisciplineIssues { get; set; }

        protected DisciplineMonthCalendarViewData(DateTime date, bool isCurrentMonth) : base(date, isCurrentMonth)
        {
        }

        public static DisciplineMonthCalendarViewData Create(DateTime date, bool isCurrentMonth, int currentPersonId, IList<ClassDisciplineDetails> disciplines)
        {
            disciplines = disciplines.Where(x => x.Date.Date == date.Date).ToList();
            var dscViewDataList = DisciplineView.Create(disciplines, currentPersonId);
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
            }
            return new DisciplineMonthCalendarViewData(date, isCurrentMonth)
            {
                HasDisciplineIssues = sortedDisciplineTypes.Count > 1,
                Disciplines = dscViewDataList,
                MoreCount = moreCount,
            };
        }
    }
}