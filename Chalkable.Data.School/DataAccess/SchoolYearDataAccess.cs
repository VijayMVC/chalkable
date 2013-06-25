using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolYearDataAccess : DataAccessBase<SchoolYear>
    {
        public SchoolYearDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public SchoolYear GetByDate(DateTime date)
        {
            var conds = new Dictionary<string, object> {{"date", date}};
            var sqlCommand = "select * from SchoolYear where StartDate <= @date and EndDate >= @date";
            using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            {
                return reader.ReadOrNull<SchoolYear>();
            }
        }

        public IList<SchoolYear> GetSchoolYears()
        {
            return SelectMany<SchoolYear>();
        }
        public PaginatedList<SchoolYear> GetSchoolYears(int start, int count)
        {
            return PaginatedSelect<SchoolYear>("Id", start, count);
        }

        public bool Exists(string name)
        {
            var conds = new Dictionary<string, object> {{"@name", name}};
            return Exists<SchoolYear>(conds);
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, Guid? currentSchoolYearId)
        {
            var sqlCommand = "select * from SchoolYear where StartDate <= @endDate and EndDate >= @startDate";
            var conds = new Dictionary<string, object>
                {  
                    {"startDate", startDate},
                    {"endDate", endDate}
                };
            if (currentSchoolYearId.HasValue)
            {
                conds.Add("id", currentSchoolYearId);
                sqlCommand += " and Id != @id";
            }
            return Exists(new DbQuery {Sql = sqlCommand, Parameters = conds});
        }
    }
}
