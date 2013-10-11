using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolUserDataAccess : DataAccessBase<SchoolUser>
    {
        public SchoolUserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}