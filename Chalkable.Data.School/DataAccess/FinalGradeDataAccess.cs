using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class FinalGradeDataAccess : DataAccessBase<FinalGrade>
    {
        public FinalGradeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
