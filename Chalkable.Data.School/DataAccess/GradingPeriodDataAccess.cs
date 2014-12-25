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

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(GradingPeriodQuery query)
        {
            return ReadMany<GradingPeriodDetails>(BuildGetGradingPeriodQuery(BuildCondition(query)), true);
        }

        private QueryCondition BuildCondition(GradingPeriodQuery query)
        {
            var res = new AndQueryCondition();
            if(query.GradingPeriodId.HasValue)
                res.Add(GradingPeriod.ID_FIELD, query.GradingPeriodId);
            if(query.MarkingPeriodId.HasValue)
                res.Add(GradingPeriod.MARKING_PERIOD_REF_FIELD, query.MarkingPeriodId);
            if(query.SchoolYearId.HasValue)
                res.Add(GradingPeriod.SCHOOL_YEAR_REF_FIELD, query.SchoolYearId);
            if (query.FromDate.HasValue)
                res.Add(GradingPeriod.START_DATE_FIELD, query.FromDate, ConditionRelation.LessEqual);
            if(query.ToDate.HasValue)
                res.Add(GradingPeriod.END_DATE_FIELD, query.ToDate, ConditionRelation.GreaterEqual);
            return res;
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

    public class GradingPeriodQuery
    {
        public int? GradingPeriodId { get; set; }
        public int? MarkingPeriodId { get; set; }
        public int? SchoolYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
