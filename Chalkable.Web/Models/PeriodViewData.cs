using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PeriodViewData
    {
        public int Id { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int Order { get; set; }
        public int SchoolYearId { get; set; }

        [Obsolete]
        protected PeriodViewData(Period period)
        {
            //StartTime = period.StartTime;
            //EndTime = period.EndTime;
            Id = period.Id;
            SchoolYearId = period.SchoolYearRef;
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
}