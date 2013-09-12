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
    public class ScheduleSectionDataAccess : DataAccessBase<ScheduleSection>
    {
        public ScheduleSectionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(ScheduleSection scheduleSection)
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
      
        public IList<ScheduleSection> GetSections(Guid markingPeriodId, int? fromNumber, int? tillNumber)
        {
            var conds = new AndQueryCondition { {ScheduleSection.MARKING_PERIOD_REF_FIELD, markingPeriodId} };
            if (fromNumber.HasValue)
                conds.Add(ScheduleSection.NUMBER_FIELD, "fromNumber", fromNumber.Value, ConditionRelation.GreaterEqual);
            if (tillNumber.HasValue)
                conds.Add(ScheduleSection.NUMBER_FIELD, "tillNumber", tillNumber.Value, ConditionRelation.LessEqual);
            var dbQuery = Orm.SimpleSelect<ScheduleSection>(conds);
            dbQuery.Sql.AppendFormat("  order by ScheduleSection.{0} ", ScheduleSection.NUMBER_FIELD);
            return ReadMany<ScheduleSection>(dbQuery);
        } 

        public IList<ScheduleSection> GetSections(IList<Guid> markingPeriodIds)
        {
            var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            var sql = string.Format("select * from ScheduleSection where MarkingPeriodRef in ({0})", mpIds);
            return ReadMany<ScheduleSection>(new DbQuery(sql, new Dictionary<string, object>()));
        }
    }
}
