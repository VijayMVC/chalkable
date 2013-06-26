using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
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
            b.Append("delete from Date ");
            var  q = BuildConditionQuery(b, query);
            ExecuteNonQueryParametrized(q.Sql, q.Parameters);
        }

        private DbQuery BuildConditionQuery(StringBuilder builder, DateQuery query)
        {
            var conds = new Dictionary<string, object>();
            var where = "where";

            if (query.Id.HasValue)
            {
                conds.Add("@d", query.Id);
                builder.AppendFormat(" {0} Id = @id", where);
                where = " and ";
            }
            if (query.MarkingPeriodId.HasValue)
            {
                conds.Add("@markingPeriodId", query.MarkingPeriodId);
                builder.AppendFormat(" {0} markingPeriodRef =@markingPeriodId ", where);
                where = "and";
            }
            if (query.SchoolYearId.HasValue)
            {
                conds.Add("@schoolYearId", query.SchoolYearId);
                builder.AppendFormat(" {0} MarkingPeriodRef in (select Id from MarkingPeriod where SchoolYearRef = @schoolYearId)", where);
                where = "and";
            }
            if (query.FromDate.HasValue)
            {
                conds.Add("@fromDate", query.FromDate);
                builder.AppendFormat(" {0} DateTime >= @fromDate", where);
                where = "and";
            }
            if (query.ToDate.HasValue)
            {
                conds.Add("@toDate", query.ToDate);
                builder.AppendFormat(" {0} DateTime <= @toDate ", where);
                where = "and";
            }
            if (query.SchoolDaysOnly)
            {
                builder.AppendFormat(" {0} IsSchoolDay = 1", where);
            }
            return new DbQuery {Sql = builder.ToString(), Parameters = conds};
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
            b.Append("select * from Date");
            var q = BuildConditionQuery(b, query);
            return acion(q, false);
        }
        public IList<Date> GetDates(DateQuery query)
        {
            var b = new StringBuilder();
            b.Append("select * from Date");
            var q = BuildConditionQuery(b, query);
            b.AppendFormat(" order by DateTime desc OFFSET 0 ROWS FETCH NEXT {0} ROWS ONLY", query.Count);
            q.Sql = string.Format("select * from ({0})x order by x.DateTime", b);
            return ReadMany<Date>(q);
        }
 
        public bool Exists(DateQuery query)
        {
            var b = new StringBuilder();
            var dbQuery = BuildConditionQuery(b, query);
            dbQuery.Sql = "select * from Date " + dbQuery.Sql;
            return Exists(dbQuery);
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

        public DateQuery()
        {
            Count = int.MaxValue;
            SchoolDaysOnly = false;
        }
    }
}
