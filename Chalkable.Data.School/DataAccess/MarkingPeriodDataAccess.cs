using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodDataAccess : DataAccessBase<MarkingPeriod>
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        
        public void ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays)
        {
            var b = new StringBuilder();
            foreach (var markingPeriodId in markingPeriodIds)
            {
                b.AppendFormat(" update MarkingPeriod set WeekDays = @weekDays where Id ={0} ", markingPeriodId);
            }
            var conds = new Dictionary<string, object> {{"weekDays", weekDays}};
            ExecuteNonQueryParametrized(b.ToString(), conds);
        }

        private const string TILL_DATE_PARAM = "tillDate";

        public MarkingPeriod GetLast(DateTime tillDate)
        {
            var sqlCommand = @"select top 1 * from MarkingPeriod 
                               where StartDate <= @{0}
                               order by EndDate desc ";
            var conds = new Dictionary<string, object>{{TILL_DATE_PARAM, tillDate}};
            sqlCommand = string.Format(sqlCommand, TILL_DATE_PARAM);
            using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            {
                return reader.ReadOrNull<MarkingPeriod>();
            }
        }

        public IList<MarkingPeriod> GetMarkingPeriods(Guid? schoolYearId)
        {
            var conds = new Dictionary<string, object>();
            if (schoolYearId.HasValue)
                conds.Add("schoolYearRef", schoolYearId);
            return SelectMany<MarkingPeriod>(conds);
        } 

        public MarkingPeriod GetMarkingPeriod(DateTime date)
        {
            var b = new StringBuilder();
            b.Append(@"select * from MarkingPeriod where StartDate <= @date and EndDate >= @date");
            var conds = new Dictionary<string, object>();
            conds.Add("date", date);
            return ReadOneOrNull<MarkingPeriod>(new DbQuery {Sql = b.ToString(), Parameters = conds});
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, Guid? currentMarkingPeriodId)
        {
            var sqlCommand = "select * from SchoolYear where StartDate <= @endDate and EndDate >= @startDate";
            var conds = new Dictionary<string, object>
                {
                    {"startDate", startDate},
                    {"endDate", endDate}
                };
            if (currentMarkingPeriodId.HasValue)
            {
                sqlCommand += " and Id != @id";
                conds.Add("@id", currentMarkingPeriodId);
            }
            return Exists(new DbQuery {Sql = sqlCommand, Parameters = conds});
        }

    }
}
