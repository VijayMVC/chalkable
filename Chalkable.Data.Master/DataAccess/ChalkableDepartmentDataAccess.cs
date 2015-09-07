using System;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class ChalkableDepartmentDataAccess : DataAccessBase<ChalkableDepartment, Guid>
    {
        public ChalkableDepartmentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}