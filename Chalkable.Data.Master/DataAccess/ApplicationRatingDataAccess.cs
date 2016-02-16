using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ApplicationRatingDataAccess : DataAccessBase<ApplicationRating, Guid>
    {
        public ApplicationRatingDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override IList<ApplicationRating> GetAll(QueryCondition conditions = null)
        {
            return ReadMany<ApplicationRating>(BuildGetRaitingQuery(conditions), true);
        }
        public override ApplicationRating GetById(Guid key)
        {
            var conds = new AndQueryCondition {{nameof(ApplicationRating.Id), key}};
            return ReadOne<ApplicationRating>(BuildGetRaitingQuery(conds), true);
        }

        public override ApplicationRating GetByIdOrNull(Guid key)
        {
            var conds = new AndQueryCondition { { nameof(ApplicationRating.Id), key } };
            return ReadOneOrNull<ApplicationRating>(BuildGetRaitingQuery(conds), true);
        }
        
        public bool Exists(Guid applicationId, Guid userId)
        {
            var conds = new AndQueryCondition
                {
                    {nameof(ApplicationRating.ApplicationRef), applicationId},
                    {nameof(ApplicationRating.UserRef), userId}
                };
            return Exists<ApplicationRating>(conds);
        }

        private DbQuery BuildGetRaitingQuery(QueryCondition conditions)
        {
            var res = new DbQuery();
            var types = new List<Type> {typeof (ApplicationRating), typeof (User)};
            res.Sql.AppendFormat(@"select {0} from [{1}] 
                                   join [{2}] on [{2}].[{3}] = [{1}].[{4}] "
                                 , Orm.ComplexResultSetQuery(types), types[0].Name, types[1].Name
                                 , User.ID_FIELD, nameof(ApplicationRating.UserRef));
            if(conditions != null)
                conditions.BuildSqlWhere(res, types[0].Name);
            return res;
        }
    }
}