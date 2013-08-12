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
    public class DateDataAccess : DataAccessBase<Date>
    {
        public DateDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(DateQuery query)
        {
            var b = new StringBuilder();
            b.Append("delete from [Date] ");
            var  q = BuildConditionQuery(b, query);
            ExecuteNonQueryParametrized(q.Sql.ToString(), q.Parameters);
        }

        private DbQuery BuildConditionQuery(StringBuilder builder, DateQuery query)
        {
            var conds = new Dictionary<string, object>();
            builder.Append(" where 1 = 1 ");
            if (query.Id.HasValue)
            {
                conds.Add("@id", query.Id);
                builder.AppendFormat(" and [Date].Id = @id");
            }
            if (query.MarkingPeriodId.HasValue)
            {
                conds.Add("@markingPeriodId", query.MarkingPeriodId);
                builder.AppendFormat(" and [Date].markingPeriodRef =@markingPeriodId ");
            }
            if (query.SchoolYearId.HasValue)
            {
                conds.Add("@schoolYearId", query.SchoolYearId);
                builder.AppendFormat(" and [Date].MarkingPeriodRef in (select Id from MarkingPeriod where SchoolYearRef = @schoolYearId)");
            }
            if (query.FromDate.HasValue)
            {
                conds.Add("@fromDate", query.FromDate);
                builder.AppendFormat(" and [Date].DateTime >= @fromDate");
            }
            if (query.ToDate.HasValue)
            {
                conds.Add("@toDate", query.ToDate);
                builder.AppendFormat(" and [Date].DateTime <= @toDate ");
            }
            if (query.SectionRef.HasValue)
            {
                conds.Add("@sectionId", query.SectionRef);
                builder.AppendFormat(" and [Date].ScheduleSectionRef = @sectionId");
            }
            if (query.SchoolDaysOnly)
            {
                builder.AppendFormat(" and [Date].IsSchoolDay = 1 ");
            }
            return new DbQuery {Sql = builder, Parameters = conds};
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
            var b = new StringBuilder();
            b.Append("select * from [Date]");
            var q = BuildConditionQuery(b, query);
            return acion(q, false);
        }
        public IList<Date> GetDates(DateQuery query)
        {
            var b = new StringBuilder();
            b.Append("select * from [Date]");
            var q = BuildConditionQuery(b, query);
            b.AppendFormat(" order by DateTime desc OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY", query.Count);

            q = new DbQuery(string.Format("select * from ({0})x order by x.DateTime", b), q.Parameters);
            return ReadMany<Date>(q);
        }

        public IList<DateDetails> GetDatesDetails(DateQuery query)
        {
            var b = new StringBuilder();
            b.AppendFormat(@"select [Date].*, {0} from [Date] 
                             left join ScheduleSection on ScheduleSection.Id = [Date].ScheduleSectionRef"
                           , Orm.ComplexResultSetQuery(new List<Type> {typeof (ScheduleSection)}));
            var q = BuildConditionQuery(b, query);
            b.AppendFormat(" order by DateTime desc OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY", query.Count);
            q.Sql.Append(string.Format("select * from ({0})x order by x.DateTime", b));
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
                    if(date.ScheduleSectionRef.HasValue)
                        date.ScheduleSection = reader.Read<ScheduleSection>(true);
                    res.Add(date);
                }
                return res;
            }
        }
 
        public bool Exists(DateQuery query)
        {
            var b = new StringBuilder();
            var dbQuery = BuildConditionQuery(b, query);
            dbQuery.Sql.Insert(0, "select * from [Date] ");
            return Exists(dbQuery);
        }

        public bool Exists(IList<Guid> markingPeriodIds)
        {
            var sql = @"select * from [Date] where MarkingPeriodRef in ({0})";
            var mpIdsString = markingPeriodIds.Select(x => "'" + x.ToString() + "'").JoinString(",");
            sql = string.Format(sql, mpIdsString);
            return Exists(new DbQuery(sql, new Dictionary<string, object>()));
        }
    }

    public class DateQuery
    {
        public Guid? Id { get; set; }
        public Guid? MarkingPeriodId { get; set; }
        public Guid? SchoolYearId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Count { get; set; }
        public bool SchoolDaysOnly { get; set; }
        public Guid? SectionRef { get; set; }

        public DateQuery()
        {
            Count = int.MaxValue;
            SchoolDaysOnly = false;
        }
    }
}
