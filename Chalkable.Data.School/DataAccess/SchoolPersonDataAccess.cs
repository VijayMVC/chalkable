using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolPersonDataAccess : DataAccessBase<SchoolPerson, int>
    {
        public SchoolPersonDataAccess(UnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}