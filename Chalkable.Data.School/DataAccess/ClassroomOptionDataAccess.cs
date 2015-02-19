using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class ClassroomOptionDataAccess : DataAccessBase<ClassroomOption, int>
    {
        public ClassroomOptionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x=> new ClassroomOption{Id = x}).ToList());
        } 
    }
}
