using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase<GradeLevel>
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
