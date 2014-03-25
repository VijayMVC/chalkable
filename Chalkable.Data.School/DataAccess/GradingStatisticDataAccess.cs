using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{

    public class GradingStatisticQuery
    {
        public IList<Guid> MarkingPeriodIds { get; set; }
        public Guid? SchoolYearId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? StudentId { get; set; }
        public Guid? ClassId { get; set; }
        public Guid CallerId { get; set; }
        public int Role { get; set; }

        public DateTime Date { get; set; }

        public GradingStatisticQuery()
        {
            Date = DateTime.UtcNow;
        }
    }
}
