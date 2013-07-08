using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduleSectionDataAccess : DataAccessBase<ScheduleSection>
    {
        public ScheduleSectionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

//        public void Update(IList<ScheduleSection> scheduleSections)
//        {
//            var b = new StringBuilder();
//            var parameters = new Dictionary<string, object>();
//            var index = 0;
//            if (scheduleSections != null && scheduleSections.Count > 0)
//            {
//                foreach (var scheduleSection in scheduleSections)
//                {
//                    b.AppendFormat(@" update ScheduleSection  set Number = @{0}, Name = @{1}, MarkingPeriodRef = @{2}
//                                  where Id = @{3} ", "number_" + index, "name_" + index, "markingPeriodId_" + index, "Id_" + index);
                    
//                    parameters.Add("name_" + index, scheduleSection.Name);
//                    parameters.Add("number_" + index, scheduleSection.Number);
//                    parameters.Add("Id_" + index, scheduleSection.Id);
//                    parameters.Add("markingPeriodId_" + index, scheduleSection.MarkingPeriodRef);
//                    index++;
//                }
//                ExecuteNonQueryParametrized(b.ToString(), parameters);
//            }
//        }
        
       
        public void Delete(ScheduleSection scheduleSection)
        {
            var b = new StringBuilder();
            b.Append(@"delete from ClassPeriod where PeriodRef in (select Id from Period where SectionRef = @sectionRef) ");
            var query = Orm.SimpleDelete<Period>(new Dictionary<string, object> {{"sectionRef", scheduleSection.Id}});
            b.Append(" " + query.Sql + " ");
            var deleteSectionQuery = Orm.SimpleDelete(scheduleSection);
            b.Append(" " + deleteSectionQuery.Sql + " ");
            foreach (var parameter in deleteSectionQuery.Parameters)
            {
              if(!query.Parameters.ContainsKey(parameter.Key))
                  query.Parameters.Add(parameter);
            }
            ExecuteNonQueryParametrized(b.ToString(), query.Parameters as Dictionary<string, object>);
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
            var conds = new Dictionary<string, object> {{"@markingPeriodRef", markingPeriodId}};
            var b = new StringBuilder();
            b.Append(@"select * from ScheduleSection 
                       where MarkingPeriodRef = @markingPeriodRef");
            if (fromNumber.HasValue)
            {
                conds.Add("fromNumber", fromNumber);
                b.Append(" and Number >= @fromNumber");
            }
            if (tillNumber.HasValue)
            {
                conds.Add("tillNumber", tillNumber);
                b.Append(" and Number <= @tillNumber ");
            }
            b.Append("  order by ScheduleSection.Number ");
            return ReadMany<ScheduleSection>(new DbQuery {Sql = b.ToString(), Parameters = conds});
        } 

        public IList<ScheduleSection> GetSections(IList<Guid> markingPeriodIds)
        {
            var mpIds = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            var sql = string.Format("select * from ScheduleSection where MarkingPeriodRef in ({0})", mpIds);
            return ReadMany<ScheduleSection>(new DbQuery { Sql = sql, Parameters = new Dictionary<string, object>() });
        }
    }
}
