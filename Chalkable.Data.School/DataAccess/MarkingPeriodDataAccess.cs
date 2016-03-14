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
    public class MarkingPeriodDataAccess : DataAccessBase<MarkingPeriod, int>
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            SimpleDelete(markingPeriods);
        }
        
        public MarkingPeriod GetLast(DateTime tillDate, int schoolYearId)
        {
            var conds = new AndQueryCondition {{MarkingPeriod.START_DATE_FIELD, tillDate, ConditionRelation.LessEqual}};
            conds.Add(MarkingPeriod.SCHOOL_YEAR_REF, schoolYearId);
            var q = Orm.SimpleSelect<MarkingPeriod>(conds);
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

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=>new MarkingPeriod{Id = x}).ToList());
        }

        public MarkingPeriod GetLastClassMarkingPeriod(int classId, DateTime? date)
        {
            var @params = new Dictionary<string, object>
            {
                ["classId"] = classId,
                ["date"] = date
            };
            using (var reader = ExecuteStoredProcedureReader("spGetLastClassMarkingPeriod", @params))
            {
                return reader.ReadOrNull<MarkingPeriod>();
            }
        }
    }
}
