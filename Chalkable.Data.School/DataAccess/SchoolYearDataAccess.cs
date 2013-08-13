using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
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
            var conds = new AndQueryCondition
                {
                    {SchoolYear.START_DATE_FIELD, date, ConditionRelation.LessEqual},
                    {SchoolYear.END_DATE_FIELD, date, ConditionRelation.GreaterEqual}
                };
            return SelectOneOrNull<SchoolYear>(conds);
            //var conds = new Dictionary<string, object> {{"date", date}};
            //var sqlCommand = "select * from SchoolYear where StartDate <= @date and EndDate >= @date";
            //using (var reader = ExecuteReaderParametrized(sqlCommand, conds))
            //{
            //    return reader.ReadOrNull<SchoolYear>();
            //}
        }

        public IList<SchoolYear> GetSchoolYears()
        {
            return SelectMany<SchoolYear>();
        }
        public PaginatedList<SchoolYear> GetSchoolYears(int start, int count)
        {
            return PaginatedSelect<SchoolYear>(SchoolYear.ID_FIELD, start, count);
        }

        public bool Exists(string name)
        {
            var conds = new AndQueryCondition { { SchoolYear.NAME_FIELD, name } };
            return Exists<SchoolYear>(conds);
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, Guid? currentSchoolYearId)
        {
            //var sqlCommand = "select * from SchoolYear where StartDate <= @endDate and EndDate >= @startDate";

            var conds = new AndQueryCondition
                {
                    {SchoolYear.START_DATE_FIELD, SchoolYear.END_DATE_FIELD, endDate, ConditionRelation.LessEqual},
                    {SchoolYear.END_DATE_FIELD, SchoolYear.START_DATE_FIELD, startDate, ConditionRelation.GreaterEqual},
                };
            if(currentSchoolYearId.HasValue)
                conds.Add(SchoolYear.ID_FIELD, currentSchoolYearId.Value, ConditionRelation.NotEqual);
            
            //var conds = new Dictionary<string, object>
            //    {  
            //        {"startDate", startDate},
            //        {"endDate", endDate}
            //    };
            //if (currentSchoolYearId.HasValue)
            //{
            //    conds.Add("id", currentSchoolYearId);
            //    sqlCommand += " and Id != @id";
            //}
            //var query = new DbQuery(sqlCommand, conds);
            return Exists<SchoolYear>(conds);
        }
    }
}
