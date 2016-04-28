using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class PeriodDataAccess : DataAccessBase<Period,int>
    {
        public PeriodDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new Period {Id = x}).ToList());
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return SelectMany<Period>(GetCondsBySchoolYear(schoolYearId));
        }

        private QueryCondition GetCondsBySchoolYear(int? schoolYearId)
        {
            var res = new AndQueryCondition();
            if (schoolYearId.HasValue)
                res.Add(Period.SCHOOL_YEAR_REF, schoolYearId);
            return res;
        }
    }
}
