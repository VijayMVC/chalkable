using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradeLevelDataAccess : DataAccessBase<GradeLevel, int>
    {
        public GradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class SchoolGradeLevelDataAccess: DataAccessBase<SchoolGradeLevel, int>
    {
        public SchoolGradeLevelDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
