using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;

namespace Chalkable.Data.School
{
    public class BaseSchoolDataAccess<TEntity> : DataAccessBase<TEntity,int> where TEntity : new()
    {
        protected int? schoolId;
        private string schoolRefField = "SchoolRef";
        public BaseSchoolDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork)
        {
            schoolId = localSchoolId;
        }

        public override TEntity GetById(int id)
        {
            return SelectOne<TEntity>(FilterBySchool(new AndQueryCondition { { ID_FIELD, id } }));
        }

        public override IList<TEntity> GetAll(QueryCondition conditions = null)
        {
            return base.GetAll(FilterBySchool(conditions));
        }

        public override TEntity GetByIdOrNull(int id)
        {
            return SelectOneOrNull<TEntity>(FilterBySchool(new AndQueryCondition { { ID_FIELD, id } }));
        }

        public override void Delete(int id)
        {
            SimpleDelete<TEntity>(FilterBySchool(new AndQueryCondition { { ID_FIELD, id } }));
        }

        public override Chalkable.Common.PaginatedList<TEntity> GetPage(int start, int count, string orderBy = null, Orm.OrderType orderType = Orm.OrderType.Asc)
        {
            if (orderBy == null)
                orderBy = ID_FIELD;
            return PaginatedSelect<TEntity>(FilterBySchool(null), orderBy, start, count, orderType);
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
