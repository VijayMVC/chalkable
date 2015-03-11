using System;
using System.Collections.Generic;
using System.Linq;
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

        public SchoolYear GetLast(DateTime tillDate)
        {
            var conds = new AndQueryCondition { { SchoolYear.START_DATE_FIELD, tillDate, ConditionRelation.LessEqual } };
            var q = Orm.SimpleSelect<SchoolYear>(FilterBySchool(conds));
            q.Sql.AppendFormat("order by {0}  desc", SchoolYear.END_DATE_FIELD);
            return ReadOneOrNull<SchoolYear>(q);
        }
        
        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new SchoolYear {Id = x}).ToList());
        }
    }
}
