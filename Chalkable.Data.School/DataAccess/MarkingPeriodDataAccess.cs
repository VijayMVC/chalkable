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
    public class MarkingPeriodDataAccess : DataAccessBase<MarkingPeriod>
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void DeleteMarkingPeriods(IList<Guid> markingPeriodIds)
        {
            var b = new StringBuilder();
            var mpidsString = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            b.Append(@"delete from ClassPeriod where PeriodRef in (select Id from Period where MarkingPeriodRef in ({0})) ");
            b.Append(@"delete from Period where MarkingPeriodRef in ({0}) ");
            b.Append(@"delete from ScheduleSection where MarkingPeriodRef in ({0}) ");
            b.Append(@"delete from MarkingPeriod where Id in ({0}) ");
            var conds = new Dictionary<string, object> ();
            var sql = string.Format(b.ToString(), mpidsString);
            ExecuteNonQueryParametrized(sql, conds);
        }
        
        public void ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays)
        {
            var b = new StringBuilder();
            foreach (var markingPeriodId in markingPeriodIds)
            {
                b.AppendFormat(" update MarkingPeriod set WeekDays = @weekDays where Id = '{0}' ", markingPeriodId);
            }
            var conds = new Dictionary<string, object> {{"weekDays", weekDays}};
            ExecuteNonQueryParametrized(b.ToString(), conds);
        }

        private const string TILL_DATE_PARAM = "tillDate";

        public MarkingPeriod GetLast(DateTime tillDate)
        {
            var q = new DbQuery();
            var sqlCommand = @"select top 1 * from MarkingPeriod 
                               where StartDate <= @{0}
                               order by EndDate desc ";
            q.Parameters.Add(TILL_DATE_PARAM, tillDate);
            q.Sql.AppendFormat(sqlCommand, TILL_DATE_PARAM);
            return ReadOneOrNull<MarkingPeriod>(q);
        }

        public MarkingPeriod GetNextInYear(Guid markingPeriodId)
        {
            var sql = @"declare @schoolYearId uniqueidentifier, @startDate datetime2
                        select @schoolYearId = SchoolYearRef, @startDate = StartDate
                        from MarkingPeriod where Id = @markingPeriodId
                            
                        select top 1 * from MarkingPeriod 
                        where SchoolYearRef = @schoolYearId and StartDate > @startDate 
                        order by StartDate";

            var conds = new Dictionary<string, object>{{"markingPeriodId", markingPeriodId}};
            return ReadOneOrNull<MarkingPeriod>(new DbQuery (sql, conds));
        }

        public IList<MarkingPeriod> GetMarkingPeriods(Guid? schoolYearId)
        {
            var conds = new AndQueryCondition();
            if (schoolYearId.HasValue)
                conds.Add(MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId);
            return SelectMany<MarkingPeriod>(conds);
        } 

        public MarkingPeriod GetMarkingPeriod(DateTime date)
        {
            var conds = new AndQueryCondition
                {
                    {MarkingPeriod.START_DATE_FIELD, "date1", date, ConditionRelation.LessEqual},
                    {MarkingPeriod.END_DATE_FIELD, "date2", date, ConditionRelation.GreaterEqual}
                };
            return SelectOneOrNull<MarkingPeriod>(conds);   
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, Guid? currentMarkingPeriodId)
        {
            var conds = new AndQueryCondition
                {
                    {MarkingPeriod.START_DATE_FIELD, endDate, ConditionRelation.LessEqual},
                    {MarkingPeriod.END_DATE_FIELD, startDate, ConditionRelation.GreaterEqual}
                };
            if (currentMarkingPeriodId.HasValue)
                conds.Add(MarkingPeriod.ID_FIELD, currentMarkingPeriodId, ConditionRelation.NotEqual);
            return Exists<MarkingPeriod>(conds);
            
        } 

    }
}
