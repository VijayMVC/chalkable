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
        public int Order { get; set; }
        public Guid MarkingPeriodId { get; set; }

        protected PeriodViewData(Period period)
        {
            StartTime = period.StartTime;
            EndTime = period.EndTime;
            Id = period.Id;
            MarkingPeriodId = period.MarkingPeriodRef;
            Order = period.Order;
        }

        public static PeriodViewData Create(Period period)
        {
            return new PeriodViewData(period);
        }
        public static IList<PeriodViewData> Create(IList<Period> periods)
        {
            return periods.Select(Create).ToList();
        } 
    }

    public class PeriodDetailedViewData : PeriodViewData
    {
        public ScheduleSectionViewData Section { get; set; }
        
        protected PeriodDetailedViewData(Period period) : base(period)
        {
            if(period.Section != null)
               Section = ScheduleSectionViewData.Create(period.Section);
        }
        public new static PeriodViewData Create(Period period)
        {
            return new PeriodDetailedViewData(period);
        }
        public new static IList<PeriodViewData> Create(IList<Period> periods)
        {
            return periods.Select(Create).ToList();
        } 

    }
}