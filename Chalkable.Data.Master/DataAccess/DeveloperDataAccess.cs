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
            var resulSet = Orm.ComplexResultSetQuery(new List<Type> {typeof (Developer), typeof (User)});
            var sql = @"select {0} 
                        from [Developer]
                        join [User] on [User].Id = [Developer].[Id]
                        where [SchoolRef] =@schoolId";
            sql = string.Format(sql, resulSet);
            var conds = new Dictionary<string, object> {{"schoolId", schoolId}};
            return ReadOneOrNull<Developer>(new DbQuery {Sql = sql, Parameters = conds}, true);
        }
    }
}
