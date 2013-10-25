using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DeveloperDataAccess : DataAccessBase<Developer>
    {
        public DeveloperDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Developer GetDeveloper(Guid districtId)
        {
            var conds = new AndQueryCondition { { Developer.DISTRICT_REF_FIELD, districtId } };
            return ReadOneOrNull<Developer>(BuildGetDeveloperQuery(conds), true);
        }
        public override Developer GetById(Guid id)
        {
            var conds = new AndQueryCondition { { Developer.ID_FIELD, id } };
            return ReadOne<Developer>(BuildGetDeveloperQuery(conds), true);
        }
        public override Developer GetByIdOrNull(Guid id)
        {
            var conds = new AndQueryCondition { { Developer.ID_FIELD, id } };
            return ReadOne<Developer>(BuildGetDeveloperQuery(conds), true);
        }

        public static DbQuery BuildGetDeveloperQuery(QueryCondition conds)
        {
            var developertype = typeof (Developer);
            var resulSet = Orm.ComplexResultSetQuery(new List<Type> { developertype, typeof(User) });
            DbQuery query = new DbQuery();
            query.Sql.AppendFormat(@"select {0} 
                        from [Developer]
                        join [User] on [User].Id = [Developer].[Id] ", resulSet);
            conds.BuildSqlWhere(query, developertype.Name);
            return query;
        }
    }
}
