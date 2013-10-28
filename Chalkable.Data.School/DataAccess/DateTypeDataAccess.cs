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
    public class DateTypeDataAccess : DataAccessBase<DateType, int>
    {
        public DateTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(DateType dateType)
        {
            var cpQuery = Orm.SimpleDelete<ClassPeriod>(new AndQueryCondition {{ClassPeriod.DATE_TYPE_REF_FIELD, dateType}});
            var updateParams = new Dictionary<string, object> {{Date.DATE_TIME_FIELD, null}};
            var updateDateQuery = Orm.SimpleUpdate<Date>(updateParams, new AndQueryCondition { { Date.DATE_TYPE_REF_FIELD, dateType } });
            var deleteSectionQuery = Orm.SimpleDelete(dateType);
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

        private const string REBUILD_SECTION_PROC = "spRebuildSections";
        private const string MARKING_PERIODS_IDS_PARAM = "markingPeriodIds";
        private const string SECTION_NAMES_PARAM = "newSectionNames";

        public void ReBuildSection(IList<int> markingPeriodIds, IList<string> sectionNames)
        {
            throw new NotImplementedException();
            var parameters = new Dictionary<string, object>
                {
                    {MARKING_PERIODS_IDS_PARAM, markingPeriodIds.Select(x=>x.ToString()).JoinString(",")},
                    {SECTION_NAMES_PARAM, sectionNames.JoinString(",")}
                };
            ExecuteStoredProcedureReader(REBUILD_SECTION_PROC, parameters).Dispose();
        }
      
        public IList<DateType> GetDateTypes(int schoolYearId, int? fromNumber = null, int? toNumber = null)
        {
            var conds = new AndQueryCondition { { DateType.SCHOOL_YEAR_REF, schoolYearId } };
            if (fromNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (toNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "tillNumber", toNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<DateType>(conds);
            dbQuery.Sql.AppendFormat("  order by [{0}].[{1}] ", "DateType", DateType.NUMBER_FIELD);
            return ReadMany<DateType>(dbQuery);
        } 

        public IList<DateType> GetSections(int markingPeriodId, int? fromNumber, int? tillNumber)
        {
            throw new NotImplementedException();
            var conds = new AndQueryCondition { {DateType.MARKING_PERIOD_REF_FIELD, markingPeriodId} };
            if (fromNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (tillNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "tillNumber", tillNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<DateType>(conds);
            dbQuery.Sql.AppendFormat("  order by ScheduleSection.{0} ", DateType.NUMBER_FIELD);
            return ReadMany<DateType>(dbQuery);
        }

        public IList<DateType> GetSections(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
            var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            var sql = string.Format("select * from ScheduleSection where MarkingPeriodRef in ({0})", mpIds);
            return ReadMany<DateType>(new DbQuery(sql, new Dictionary<string, object>()));
        }
    }
}
