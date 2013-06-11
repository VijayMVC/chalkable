using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void Delete(Class cClass)
        {
            SimpleDelete(cClass);
        }
    }
}
