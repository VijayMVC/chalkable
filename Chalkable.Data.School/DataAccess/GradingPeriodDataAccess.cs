using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
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

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId)
        {
            var condition = new AndQueryCondition {{GradingPeriod.SCHOOL_YEAR_REF_FIELD, schoolYearId}};
            if(markingPeriodId.HasValue)
                condition.Add(GradingPeriod.MARKING_PERIOD_REF_FIELD, markingPeriodId.Value);
            var dbQuery = BuildGetGradingPeriodQuery(condition);
            return ReadMany<GradingPeriodDetails>(dbQuery, true);
        }

        public GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            var condition = new AndQueryCondition
                {
                    { GradingPeriod.SCHOOL_YEAR_REF_FIELD, schoolYearId },
                    { GradingPeriod.START_DATE_FIELD, date, ConditionRelation.LessEqual },
                    { GradingPeriod.END_DATE_FIELD, date, ConditionRelation.GreaterEqual },
                };
            return ReadOneOrNull<GradingPeriodDetails>(BuildGetGradingPeriodQuery(condition), true);
        }

        private DbQuery BuildGetGradingPeriodQuery(QueryCondition condition)
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> { typeof(GradingPeriod), typeof(MarkingPeriod) };
            dbQuery.Sql.Append(string.Format(@"select {0} from [{1}] join [{2}] on [{2}].[{3}] = [{1}].[{4}]"
                                             , Orm.ComplexResultSetQuery(types), types[0].Name, types[1].Name
                                             , MarkingPeriod.ID_FIELD, GradingPeriod.MARKING_PERIOD_REF_FIELD));
            condition.BuildSqlWhere(dbQuery, types[0].Name);
            return dbQuery;
        }
    }
}
