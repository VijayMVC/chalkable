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
    public class MarkingPeriodDataAccess : BaseSchoolDataAccess<MarkingPeriod>
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork, int? schoolId) : base(unitOfWork, schoolId)
        {
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            if (markingPeriodIds.Count == 0)
                return;
            var b = new StringBuilder();
            var mpidsString = markingPeriodIds.Select(x => "'" + x + "'").JoinString(",");
            b.Append(@"delete from ClassPeriod where PeriodRef in (select Id from Period where MarkingPeriodRef in ({0})) ");
            b.Append(@"delete from Period where MarkingPeriodRef in ({0}) ");
            b.Append(@"delete from ScheduleSection where MarkingPeriodRef in ({0}) ");
            b.Append(@"delete from MarkingPeriod where Id in ({0}) ");
            var conds = new Dictionary<string, object> ();
            var sql = string.Format(b.ToString(), mpidsString);
            ExecuteNonQueryParametrized(sql, conds);
        }
        
        public void ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            var b = new StringBuilder();
            foreach (var markingPeriodId in markingPeriodIds)
            {
                b.AppendFormat(" update MarkingPeriod set WeekDays = @weekDays where Id = '{0}' ", markingPeriodId);
            }
            var conds = new Dictionary<string, object> {{"weekDays", weekDays}};
            ExecuteNonQueryParametrized(b.ToString(), conds);
        }

        public MarkingPeriod GetLast(DateTime tillDate, int? schoolYearId)
        {
            var conds = new AndQueryCondition {{MarkingPeriod.START_DATE_FIELD, tillDate, ConditionRelation.LessEqual}};
            if(schoolYearId.HasValue)
                conds.Add(MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId.Value);
            var q = Orm.SimpleSelect<MarkingPeriod>(FilterBySchool(conds));
            q.Sql.AppendFormat("order by {0}  desc", MarkingPeriod.END_DATE_FIELD);
            return ReadOneOrNull<MarkingPeriod>(q);
        }

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            var sql = @"declare @schoolYearId int, @startDate datetime2
                        select @schoolYearId = SchoolYearRef, @startDate = StartDate
                        from MarkingPeriod where Id = @markingPeriodId
                            
                        select top 1 * from MarkingPeriod 
                        where SchoolYearRef = @schoolYearId and StartDate > @startDate 
                        order by StartDate";

            var conds = new Dictionary<string, object>{{"markingPeriodId", markingPeriodId}};
            return ReadOneOrNull<MarkingPeriod>(new DbQuery (sql, conds));
            //todo filtering by school
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            QueryCondition conds = null;
            if (schoolYearId.HasValue)
                conds = new AndQueryCondition {{MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId}};
            return SelectMany<MarkingPeriod>(conds);
        }


        public MarkingPeriod GetMarkingPeriod(DateTime date, int? schoolYearId)
        {
            var conds = new AndQueryCondition
                {
                    {MarkingPeriod.START_DATE_FIELD, "date1", date, ConditionRelation.LessEqual},
                    {MarkingPeriod.END_DATE_FIELD, "date2", date, ConditionRelation.GreaterEqual}
                };
            if(schoolYearId.HasValue)
                conds.Add(MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId.Value);
            return SelectOneOrNull<MarkingPeriod>(conds);   
        }

        public bool IsOverlaped(int schoolYearId, DateTime startDate, DateTime endDate, int? currentMarkingPeriodId)
        {
            var conds = new AndQueryCondition
                {
                    {MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId},
                    {MarkingPeriod.START_DATE_FIELD, MarkingPeriod.END_DATE_FIELD, endDate, ConditionRelation.LessEqual},
                    {MarkingPeriod.END_DATE_FIELD, MarkingPeriod.START_DATE_FIELD, startDate, ConditionRelation.GreaterEqual}
                };
            if (currentMarkingPeriodId.HasValue)
                conds.Add(MarkingPeriod.ID_FIELD, currentMarkingPeriodId, ConditionRelation.NotEqual);
            return Exists<MarkingPeriod>(conds);
            
        } 

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=>new MarkingPeriod{Id = x}).ToList());
        }

    }
}
