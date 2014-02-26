using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingPeriodDataAccess : DataAccessBase<GradingPeriod, int>
    {
        public GradingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodDetails(int schoolYearId, int? markingPeriodId)
        {
            var dbQuery = new DbQuery();
            var types = new List<Type> {typeof (GradingPeriod), typeof (MarkingPeriod)};
            dbQuery.Sql.Append(string.Format(@"select {0} from [{1}] join [{2}] on [{2}].[{3}] = [{1}].[{4}]"
                                             , Orm.ComplexResultSetQuery(types), types[0].Name, types[1].Name
                                             , MarkingPeriod.ID_FIELD, GradingPeriod.MARKING_PERIOD_REF_FIELD));
            var condition = new AndQueryCondition {{GradingPeriod.SCHOOL_YEAR_REF_FIELD, schoolYearId}};
            if(markingPeriodId.HasValue)
                condition.Add(GradingPeriod.MARKING_PERIOD_REF_FIELD, markingPeriodId.Value);
            condition.BuildSqlWhere(dbQuery, types[0].Name);
            return ReadMany<GradingPeriodDetails>(dbQuery, true);
        }
    }
}
