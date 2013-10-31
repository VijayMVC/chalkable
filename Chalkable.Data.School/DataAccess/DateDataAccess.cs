﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class DateDataAccess : BaseSchoolDataAccess<Date>
    {
        public DateDataAccess(UnitOfWork unitOfWork, int? schoolid) : base(unitOfWork, schoolid)
        {
        }

        public void Delete(DateQuery query)
        {
            var q = new DbQuery();
            q.Sql.Append("delete from [Date] ");
            q = BuildConditionQuery(q, query);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        private DbQuery BuildConditionQuery(DbQuery dbQuery, DateQuery query)
        {
            var condition = new AndQueryCondition {FilterBySchool(null)};
            if(query.SchoolYearId.HasValue)
                condition.Add(Date.SCHOOL_YEAR_REF, query.SchoolYearId, ConditionRelation.Equal);
            if(query.DayType.HasValue)
                condition.Add(Date.DATE_TYPE_REF_FIELD, query.DayType, ConditionRelation.Equal);
            if(query.FromDate.HasValue)
                condition.Add(Date.DATE_TIME_FIELD, "fromDate", query.FromDate, ConditionRelation.Greater);
            if(query.ToDate.HasValue)
                condition.Add(Date.DATE_TIME_FIELD, "toDate", query.ToDate, ConditionRelation.Less);
            if(query.SchoolDaysOnly)
                condition.Add(Date.IS_SCHOOL_DAY_FIELD, true, ConditionRelation.Equal);

            condition.BuildSqlWhere(dbQuery, "Date");
            if (query.MarkingPeriodId.HasValue)
            {
                var joinOperand = dbQuery.Parameters.Count > 0 ? "and" : "where";
                dbQuery.Parameters.Add("@markingPeriodId", query.MarkingPeriodId);
                dbQuery.Sql.AppendFormat(@" {4} exists(select * from MarkingPeriod where [MarkingPeriod].[{0}] = @markingPeriodId 
                                                          and [MarkingPeriod].[{1}] <= [Date].[{3}] and [MarkingPeriod].[{2}] >= [Date].[{3}])"
                    , MarkingPeriod.ID_FIELD, MarkingPeriod.START_DATE_FIELD, MarkingPeriod.END_DATE_FIELD, Date.DATE_TIME_FIELD, joinOperand);
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

        public Date GetDate(DateQuery query)
        {
            return GetDate(query, ReadOne<Date>);
        }
        public Date GetDateOrNull(DateQuery query)
        {
            return GetDate(query, ReadOneOrNull<Date>);
        }
        private Date GetDate(DateQuery query, Func<DbQuery, bool, Date> acion)
        {
            var q = new DbQuery();
            q.Sql.Append("select * from [Date]");
            q = BuildConditionQuery(q, query);
            return acion(q, false);
        }
        public IList<Date> GetDates(DateQuery query)
        {
            var q = new DbQuery();
            q.Sql.Append("select * from [Date]");
            q = BuildConditionQuery(q, query);
            q.Sql.AppendFormat(" order by {1} desc OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY", query.Count, Date.DATE_TIME_FIELD);

            q = new DbQuery(string.Format("select * from ({0})x order by x.{1}", q.Sql, Date.DATE_TIME_FIELD), q.Parameters);
            return ReadMany<Date>(q);
        }

        public IList<DateDetails> GetDatesDetails(DateQuery query)
        {
            var q = new DbQuery();
            q.Sql.AppendFormat(@"select [Date].*, {0} from [Date] 
                                left join DayType on DayType.Id = [Date].DayTypeRef"
                           , Orm.ComplexResultSetQuery(new List<Type> {typeof (DayType)}));
            q = BuildConditionQuery(q, query);
            q.Sql.AppendFormat(" order by Day desc OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY ", query.Count);
            q.Sql.Insert(0, " select * from (").Append(")x order by x.Day");
            return ReadDetailsDate(q);
        } 

        private IList<DateDetails> ReadDetailsDate(DbQuery query)
        {
            using (var reader = ExecuteReaderParametrized(query.Sql.ToString(), query.Parameters))
            {
                var res = new List<DateDetails>();
                while (reader.Read())
                {
                    var date = reader.Read<DateDetails>();
                    if(date.DayTypeRef.HasValue)
                        date.DayType = reader.Read<DayType>(true);
                    res.Add(date);
                }
                return res;
            }
        }
 
        public bool Exists(DateQuery query)
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append("select * from [Date] ");
            dbQuery = BuildConditionQuery(dbQuery, query);
            return Exists(dbQuery);
        }

        public bool Exists(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
            //var sql = @"select * from [Date] where MarkingPeriodRef in ({0})";
            //var mpIdsString = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            //sql = string.Format(sql, mpIdsString);
            //return Exists(new DbQuery(sql, new Dictionary<string, object>()));
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
