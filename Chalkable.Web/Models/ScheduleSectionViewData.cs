using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ScheduleSectionViewData
    {
        public Guid Id { get; set; }
        public Guid MarkingPeriodId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        private ScheduleSectionViewData() { }

        public static ScheduleSectionViewData Create(ScheduleSection section)
        {
            return new ScheduleSectionViewData
            {
                Id = section.Id,
                MarkingPeriodId = section.MarkingPeriodRef,
                Name = section.Name,
                Number = section.Number
            };
        }

        public static IList<ScheduleSectionViewData> Create(IList<ScheduleSection> sections)
        {
            return sections.Select(Create).ToList();
        }
    }
}