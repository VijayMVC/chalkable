using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class ScheduleSectionDataAccess : DataAccessBase
    {
        public ScheduleSectionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Create(ScheduleSection scheduleSection)
        {
            SimpleInsert(scheduleSection);
        }
        public void Update(ScheduleSection scheduleSection)
        {
            SimpleUpdate(scheduleSection);
        }

        public void Update(IList<ScheduleSection> scheduleSections)
        {
            var b = new StringBuilder();
            foreach (var scheduleSection in scheduleSections)
            {
                b.AppendFormat(@" update from ScheduleSection  set Number = {0}, Name = {1}, MarkingPeriodRef = {2}
                                  where Id = {3} ", scheduleSection.Number, scheduleSection.Name, scheduleSection.Id,
                               scheduleSection.MarkingPeriodRef);

            }
            ExecuteNonQueryParametrized(b.ToString(), new Dictionary<string, object> ());
        }
        
       
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
            ExecuteReaderParametrized(b.ToString(), query.Parameters as Dictionary<string, object>);
        }

        private const string REBUILD_SECTION_PROC = "spRebuildSections";
        private const string MARKING_PERIODS_IDS_PARAM = "markingPeriodIds";
        private const string SECTION_NAMES_PARAM = "newSectionNames";

        public void ReBuildSection(IList<Guid> markingPeriodIds, IList<string> sectionNames)
        {
            var parameters = new Dictionary<string, object>
                {
                    {MARKING_PERIODS_IDS_PARAM, markingPeriodIds},
                    {SECTION_NAMES_PARAM, sectionNames}
                };
            ExecuteStoredProcedureReader(REBUILD_SECTION_PROC, parameters);
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
                b.Append(" and Number => @fromNumber");
            }
            if (tillNumber.HasValue)
            {
                conds.Add("tillNumber", tillNumber);
                b.Append(" and Number <= @tillNumber ");
            }
            b.Append("  order by ScheduleSection.Number ");
            return ReadMany<ScheduleSection>(new DbQuery {Sql = b.ToString(), Parameters = conds});
        } 

        public ScheduleSection GetById(Guid id)
        {
            return SelectOne<ScheduleSection>(new Dictionary<string, object> {{"Id", id}});
        }
    }
}
