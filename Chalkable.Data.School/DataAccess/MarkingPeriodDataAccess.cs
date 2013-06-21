﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MarkingPeriodDataAccess : DataAccessBase
    {
        public MarkingPeriodDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Create(MarkingPeriod markingPeriod)
        {
            SimpleInsert(markingPeriod);
        }
        public void Update(MarkingPeriod markingPeriod)
        {
            SimpleUpdate(markingPeriod);
        }
        public void Delete(MarkingPeriod markingPeriod)
        {
            SimpleDelete(markingPeriod);
        }

        public MarkingPeriod GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"id", id}};
            return SelectOne<MarkingPeriod>(conds);
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

        public IList<MarkingPeriod> GetMarkingPeriods(Guid schoolYearId)
        {
            var conds = new Dictionary<string, object> {{"schoolYearRef", schoolYearId}};
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
