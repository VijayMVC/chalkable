using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class DateTypeViewData
    {
        public int Id { get; set; }
        public int SchoolYearId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        private DateTypeViewData() { }

        public static DateTypeViewData Create(DateType section)
        {
            return new DateTypeViewData
            {
                Id = section.Id,
                SchoolYearId = section.SchoolYearRef,
                Name = section.Name,
                Number = section.Number
            };
        }

        public static IList<DateTypeViewData> Create(IList<DateType> sections)
        {
            return sections.Select(Create).ToList();
        }
    }
}