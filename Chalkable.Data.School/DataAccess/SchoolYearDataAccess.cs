using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolYearDataAccess : DataAccessBase<SchoolYear, int>
    {
        public SchoolYearDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public SchoolYear GetByDate(DateTime date, int schoolId)
        {
            var conds = new AndQueryCondition
                {
                    {SchoolYear.START_DATE_FIELD, date, ConditionRelation.LessEqual},
                    {SchoolYear.END_DATE_FIELD, date, ConditionRelation.GreaterEqual},
                    {SchoolYear.SCHOOL_REF_FIELD, schoolId},
                };
            return SelectOneOrNull<SchoolYear>(conds);
        }

        public SchoolYear GetLast(DateTime tillDate, int schoolId)
        {
            var conds = new AndQueryCondition
            {
                { SchoolYear.START_DATE_FIELD, tillDate, ConditionRelation.LessEqual },
                { SchoolYear.SCHOOL_REF_FIELD, schoolId, ConditionRelation.Equal }
            };
            var q = Orm.SimpleSelect<SchoolYear>(conds);
            q.Sql.AppendFormat("order by {0}  desc", SchoolYear.END_DATE_FIELD);
            return ReadOneOrNull<SchoolYear>(q);
        }
        
        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new SchoolYear {Id = x}).ToList());
        }
    }
}
