using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PeriodViewData
    {
        public Guid Id { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public ScheduleSectionViewData Section { get; set; }
        public Guid MarkingPeriodId { get; set; }

        public static PeriodViewData Create(Period period)
        {
            var res = new PeriodViewData
            {
                StartTime = period.StartTime,
                EndTime = period.EndTime,
                Id = period.Id,
                MarkingPeriodId = period.MarkingPeriodRef,
                Section = ScheduleSectionViewData.Create(period.Section)
            };
            return res;
        }
        public static IList<PeriodViewData> Create(IList<Period> periods)
        {
            return periods.Select(Create).ToList();
        } 
    }
}