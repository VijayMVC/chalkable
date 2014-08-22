using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<Model.School, int>
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(IList<int> ids)
        {
            SimpleDelete<Model.School>(ids.Select(x => new Model.School {Id = x}).ToList());
        }
    }
}