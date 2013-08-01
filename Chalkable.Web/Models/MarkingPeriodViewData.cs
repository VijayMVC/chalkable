using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class MarkingPeriodViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public int WeekDays { get; set; }

        public static MarkingPeriodViewData Create(MarkingPeriod markingPeriod)
        {
            var res = new MarkingPeriodViewData
            {
                Id = markingPeriod.Id,
                Name = markingPeriod.Name,
                StartDate = markingPeriod.StartDate,
                EndDate = markingPeriod.EndDate,
                Description = markingPeriod.Description,
                WeekDays = markingPeriod.WeekDays
            };
            return res;
        }
        public static IList<MarkingPeriodViewData> Create(IList<MarkingPeriod> markingPeriods)
        {
            return markingPeriods.Select(Create).ToList();
        } 
    }
}