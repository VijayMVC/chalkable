using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolYearDataAccess : BaseSchoolDataAccess<SchoolYear>
    {
        public SchoolYearDataAccess(UnitOfWork unitOfWork, int? schoolId)
            : base(unitOfWork, schoolId)
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
        }

        public bool Exists(string name)
        {
            var conds = new AndQueryCondition { { SchoolYear.NAME_FIELD, name } };
            return Exists<SchoolYear>(conds);
        }

        public bool IsOverlaped(DateTime startDate, DateTime endDate, int? currentSchoolYearId)
        {
            var conds = new AndQueryCondition
                {
                    {SchoolYear.START_DATE_FIELD, SchoolYear.END_DATE_FIELD, endDate, ConditionRelation.LessEqual},
                    {SchoolYear.END_DATE_FIELD, SchoolYear.START_DATE_FIELD, startDate, ConditionRelation.GreaterEqual},
                };
            if(currentSchoolYearId.HasValue)
                conds.Add(SchoolYear.ID_FIELD, currentSchoolYearId.Value, ConditionRelation.NotEqual);
            
            return Exists<SchoolYear>(conds);
        }
    }
}
