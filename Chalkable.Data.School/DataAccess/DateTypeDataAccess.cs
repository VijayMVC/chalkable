using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class DateTypeDataAccess : DataAccessBase<DayType, int>
    {
        public DateTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(DayType dayType)
        {
            var cpQuery = Orm.SimpleDelete<ClassPeriod>(new AndQueryCondition {{ClassPeriod.DATE_TYPE_REF_FIELD, dayType}});
            var updateParams = new Dictionary<string, object> {{Date.DATE_TIME_FIELD, null}};
            var updateDateQuery = Orm.SimpleUpdate<Date>(updateParams, new AndQueryCondition { { Date.DATE_TYPE_REF_FIELD, dayType } });
            var deleteSectionQuery = Orm.SimpleDelete(dayType);
            var query = JoinQueries(new List<DbQuery> { cpQuery, updateDateQuery, deleteSectionQuery });
            ExecuteNonQueryParametrized(query.ToString(), query.Parameters);
        }

        private DbQuery JoinQueries(IList<DbQuery> dbQueries)
        {
            var res = new DbQuery();
            foreach (var dbQuery in dbQueries)
            {
                res.Sql.Append(dbQuery.Sql).Append("  ");
                foreach (var param in dbQuery.Parameters)
                {
                    if (!res.Parameters.ContainsKey(param.Key))
                        res.Parameters.Add(param);
                }
            }
            return res;
        }

        public IList<DayType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var conds = new AndQueryCondition { { DayType.SCHOOL_YEAR_REF, schoolYearId } };
            if (fromNumber.HasValue)
                conds.Add(DayType.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (toNumber.HasValue)
                conds.Add(DayType.NUMBER_FIELD, "tillNumber", toNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<DayType>(conds);
            dbQuery.Sql.AppendFormat("  order by [{0}].[{1}] ", "DayType", DayType.NUMBER_FIELD);
            return ReadMany<DayType>(dbQuery);
        } 
    }
}
