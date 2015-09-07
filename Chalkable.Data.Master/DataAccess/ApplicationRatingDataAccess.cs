using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

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
            var conds = new AndQueryCondition {{ApplicationRating.ID_FIELD, key}};
            return ReadOne<ApplicationRating>(BuildGetRaitingQuery(conds), true);
        }

        public override ApplicationRating GetByIdOrNull(Guid key)
        {
            var conds = new AndQueryCondition { { ApplicationRating.ID_FIELD, key } };
            return ReadOneOrNull<ApplicationRating>(BuildGetRaitingQuery(conds), true);
        }
        
        public bool Exists(Guid applicationId, Guid userId)
        {
            var conds = new AndQueryCondition
                {
                    {ApplicationRating.APPLICATION_REF_FIELD, applicationId},
                    {ApplicationRating.USER_REF_FIELD, userId}
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
                                 , User.ID_FIELD, ApplicationRating.USER_REF_FIELD);
            conditions?.BuildSqlWhere(res, Orm.TableName(types[0]));
            return res;
        }
    }
}