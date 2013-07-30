using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DeveloperDataAccess : DataAccessBase<Developer>
    {
        public DeveloperDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Developer GetDeveloper(Guid schoolId)
        {
            var conds = new Dictionary<string, object> {{Developer.SCHOOL_REF_FIELD, schoolId}};
            return ReadOneOrNull<Developer>(BuildGetDeveloperQuery(conds), true);
        }
        public override Developer GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{Developer.ID_FIELD, id}};
            return ReadOne<Developer>(BuildGetDeveloperQuery(conds), true);
        }
        public override Developer GetByIdOrNull(Guid id)
        {
            var conds = new Dictionary<string, object> { { Developer.ID_FIELD, id } };
            return ReadOne<Developer>(BuildGetDeveloperQuery(conds), true);
        }

        public DbQuery BuildGetDeveloperQuery(IDictionary<string, object> conds)
        {
            var developertype = typeof (Developer);
            var resulSet = Orm.ComplexResultSetQuery(new List<Type> { developertype, typeof(User) });
            var sql = new StringBuilder();
            sql.AppendFormat(@"select {0} 
                        from [Developer]
                        join [User] on [User].Id = [Developer].[Id] ", resulSet);
            sql = Orm.BuildSqlWhere(sql, developertype, conds);
            return new DbQuery {Sql = sql.ToString(), Parameters = conds};
        }
    }
}
