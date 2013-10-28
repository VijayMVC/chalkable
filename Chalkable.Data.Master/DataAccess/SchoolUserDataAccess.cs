using System;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolUserDataAccess : DataAccessBase<SchoolUser, Guid>
    {
        public SchoolUserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}