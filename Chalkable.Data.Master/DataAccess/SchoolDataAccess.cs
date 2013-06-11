using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolDataAccess : DataAccessBase
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(School school)
        {
            SimpleInsert(school);
        }
    }
}