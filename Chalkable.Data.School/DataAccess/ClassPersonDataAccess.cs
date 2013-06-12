using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassPersonDataAccess : DataAccessBase
    {
        public ClassPersonDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(ClassPerson classPerson)
        {
            SimpleInsert(classPerson);
        }
        public void Delete(Guid classId)
        {
            var conds = new Dictionary<string, object> {{"classId", classId}};
            SimpleDelete<ClassPerson>(conds);
        }
    }
}
