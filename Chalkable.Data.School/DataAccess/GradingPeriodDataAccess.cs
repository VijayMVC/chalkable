using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using System.Linq;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingPeriodDataAccess : DataAccessBase<GradingPeriod, int>
    {
        public GradingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=> new GradingPeriod{Id = x}).ToList());
        }

        public IList<GradingPeriod> GetGradingPeriodsDetails(GradingPeriodQuery query)
        {
            var ps = new Dictionary<string, object>
            {
                {"@id", query.GradingPeriodId},
                {"@schoolYearid", query.SchoolYearId},
                {"@classId", query.ClassId},
            };
            return ExecuteStoredProcedureList<GradingPeriod>("spGetGradingPeriods", ps);
        }
    }

    public class GradingPeriodQuery
    {
        public int? GradingPeriodId { get; set; }
        public int? SchoolYearId { get; set; }
        public int? ClassId { get; set; }
    }
}
