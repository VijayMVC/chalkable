using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.DataAccess
{
    public class PeriodDataAccess : DataAccessBase<Period>
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
