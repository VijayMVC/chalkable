using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class CourseDataAccess : DataAccessBase<Course>
    {
        public CourseDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
