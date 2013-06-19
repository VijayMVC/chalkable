using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassDataAccess : DataAccessBase
    {
        public ClassDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Class cClass)
        {
            SimpleInsert(cClass);
        }
        public void Update(Class cClass)
        {
            SimpleUpdate(cClass);
        }
        public void Delete(Guid id)
        {
            SimpleDelete<Class>(id);
        }

        public Class GetById(Guid id)
        {
            var conds = new Dictionary<string, object> {{"Id", id}};
            return SelectOne<Class>(conds);
        }
    }
}
