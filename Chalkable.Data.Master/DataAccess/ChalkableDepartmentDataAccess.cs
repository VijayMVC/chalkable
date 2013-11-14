using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class ChalkableDepartmentDataAccess : DataAccessBase<ChalkableDepartment>
    {
        public ChalkableDepartmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}