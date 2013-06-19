using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolUserDataAccess : DataAccessBase
    {
        public SchoolUserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(SchoolUser schoolUser)
        {
            SimpleInsert(schoolUser);
        }
    }
}