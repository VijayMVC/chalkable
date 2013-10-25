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
    public class ScheduleSectionDataAccess : DataAccessBase<DateType>
    {
        public ScheduleSectionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(DateType scheduleSection)
        {
            var b = new StringBuilder();
            b.Append(@"delete from ClassPeriod where PeriodRef in (select Id from Period where SectionRef = @sectionRef) ");
            var query = Orm.SimpleDelete<Period>(new AndQueryCondition {{Period.SECTION_REF, scheduleSection.Id}});
            b.Append(" " + query.Sql + " ");
            var deleteSectionQuery = Orm.SimpleDelete(scheduleSection);
            b.Append(" " + deleteSectionQuery.Sql + " ");
            foreach (var parameter in deleteSectionQuery.Parameters)
            {
              if(!query.Parameters.ContainsKey(parameter.Key))
                  query.Parameters.Add(parameter);
            }
            ExecuteNonQueryParametrized(b.ToString(), query.Parameters);
        }

        private const string REBUILD_SECTION_PROC = "spRebuildSections";
        private const string MARKING_PERIODS_IDS_PARAM = "markingPeriodIds";
        private const string SECTION_NAMES_PARAM = "newSectionNames";

        public void ReBuildSection(IList<Guid> markingPeriodIds, IList<string> sectionNames)
        {
            var parameters = new Dictionary<string, object>
                {
                    {MARKING_PERIODS_IDS_PARAM, markingPeriodIds.Select(x=>x.ToString()).JoinString(",")},
                    {SECTION_NAMES_PARAM, sectionNames.JoinString(",")}
                };
            ExecuteStoredProcedureReader(REBUILD_SECTION_PROC, parameters).Dispose();
        }
      
        public IList<DateType> GetSections(Guid markingPeriodId, int? fromNumber, int? tillNumber)
        {
            var conds = new AndQueryCondition { {DateType.MARKING_PERIOD_REF_FIELD, markingPeriodId} };
            if (fromNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (tillNumber.HasValue)
                conds.Add(DateType.NUMBER_FIELD, "tillNumber", tillNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<DateType>(conds);
            dbQuery.Sql.AppendFormat("  order by ScheduleSection.{0} ", DateType.NUMBER_FIELD);
            return ReadMany<DateType>(dbQuery);
        } 

        public IList<DateType> GetSections(IList<Guid> markingPeriodIds)
        {
            var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            var sql = string.Format("select * from ScheduleSection where MarkingPeriodRef in ({0})", mpIds);
            return ReadMany<DateType>(new DbQuery(sql, new Dictionary<string, object>()));
        }
    }
}
