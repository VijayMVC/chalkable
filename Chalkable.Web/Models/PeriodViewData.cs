using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class PeriodViewData
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public int SchoolYearId { get; set; }
        public string Name { get; set; }

        protected PeriodViewData(Period period)
        {
            Id = period.Id;
            SchoolYearId = period.SchoolYearRef;
            Order = period.Order;
            Name = period.Name;
        }

        protected PeriodViewData(int id, int schoolYearId, int order, string name)
        {
            Id = id;
            SchoolYearId = schoolYearId;
            Order = order;
            Name = name;
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