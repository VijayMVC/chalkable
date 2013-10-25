using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<Model.School>
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}