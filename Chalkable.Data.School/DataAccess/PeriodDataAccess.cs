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
    public class PeriodDataAccess : BaseSchoolDataAccess<Period>
    {
        public PeriodDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
        {
        }

        //public void Delete(IList<Guid> markingPeriodIds)
        //{
        //    var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
        //    var sql = string.Format("delete from Period where {0} in ({1})", Period.MARKING_PERIOD_REF_FIELD, mpIds);
        //    ExecuteNonQueryParametrized(sql, new Dictionary<string, object>());
        //}

        public void Delete(int? schoolYearId)
        {
            SimpleDelete<Period>(GetCondsBySchoolYear(schoolYearId));
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return SelectMany<Period>(GetCondsBySchoolYear(schoolYearId));
        }

        public Period GetPeriodOrNull(int time, int schoolYearId)
        {
            return SelectOneOrNull<Period>(new AndQueryCondition
                {
                    {Period.START_TIME_FIELD, time, ConditionRelation.LessEqual},
                    {Period.END_TIME_FIELD, time, ConditionRelation.GreaterEqual},
                    GetCondsBySchoolYear(schoolYearId)
                });
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
