using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School.DataAccess
{
    public class BaseSchoolDataAccess<TEntity> : DataAccessBase<TEntity, int> where TEntity : new()
    {
        protected int? schoolId;
        private string schoolRefField = "SchoolRef";
        public BaseSchoolDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork)
        {
            schoolId = localSchoolId;
        }

        protected override QueryCondition FilterConditions(QueryCondition condition)
        {
            return FilterBySchool(base.FilterConditions(condition));
        }

        protected virtual QueryCondition FilterBySchool(QueryCondition queryCondition)
        {
            if (!schoolId.HasValue) return queryCondition;
            var res = new AndQueryCondition{{schoolRefField, schoolId}};
            if(queryCondition != null)
                res.Add(queryCondition);
            return res;
        }
    }
}
