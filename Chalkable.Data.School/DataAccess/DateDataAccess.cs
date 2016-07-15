using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class DateDataAccess : DataAccessBase<Date, int>
    {
        public DateDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private DbQuery BuildConditionQuery(DbQuery dbQuery, DateQuery query)
        {
            var condition = new AndQueryCondition ();
            if(query.SchoolYearId.HasValue)
                condition.Add(Date.SCHOOL_YEAR_REF, query.SchoolYearId, ConditionRelation.Equal);
            if(query.DayType.HasValue)
                condition.Add(Date.DATE_TYPE_REF_FIELD, query.DayType, ConditionRelation.Equal);
            if(query.FromDate.HasValue)
                condition.Add(Date.DATE_TIME_FIELD, "fromDate", query.FromDate, ConditionRelation.GreaterEqual);
            if(query.ToDate.HasValue)
                condition.Add(Date.DATE_TIME_FIELD, "toDate", query.ToDate, ConditionRelation.LessEqual);
            if(query.SchoolDaysOnly)
                condition.Add(Date.IS_SCHOOL_DAY_FIELD, true, ConditionRelation.Equal);

            condition.BuildSqlWhere(dbQuery, "Date");
            if (query.MarkingPeriodId.HasValue)
            {
                dbQuery.Parameters.Add("@markingPeriodId", query.MarkingPeriodId);
                dbQuery.Sql.AppendFormat(@" and exists(select * from MarkingPeriod where [MarkingPeriod].[{0}] = @markingPeriodId 
                                                          and [MarkingPeriod].[{1}] <= [Date].[{3}] and [MarkingPeriod].[{2}] >= [Date].[{3}])"
                    , MarkingPeriod.ID_FIELD, MarkingPeriod.START_DATE_FIELD, MarkingPeriod.END_DATE_FIELD, Date.DATE_TIME_FIELD);
            }
            return dbQuery;
        }


        public DateTime GetDbDateTime()
        {
            using (var reader = ExecuteReaderParametrized("select GETDATE() as DateTime", new Dictionary<string, object>()))
            {
                return SqlTools.ReadDateTime(reader, "DateTime");
            }
        }

        public IList<Date> GetDates(DateQuery query)
        {
            var q = new DbQuery();
            q.Sql.Append("select * from [Date]");
            q = BuildConditionQuery(q, query);
            q.Sql.Append($" order by {Date.DATE_TIME_FIELD} desc OFFSET 0 ROWS FETCH NEXT {query.Count} ROWS ONLY");

            q = new DbQuery($"select * from ({q.Sql}) x order by x.{Date.DATE_TIME_FIELD}", q.Parameters);
            return ReadMany<Date>(q);
        }
    }

    public class DateQuery
    {
        public int? SchoolYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Count { get; set; }
        public bool SchoolDaysOnly { get; set; }

        public int? MarkingPeriodId { get; set; }
        public int? DayType { get; set; }

        public DateQuery()
        {
            Count = int.MaxValue;
            SchoolDaysOnly = false;
        }
    }
}
