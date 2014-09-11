using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class SchoolUserDataAccess : DataAccessBase<SchoolUser, Guid>
    {
        public SchoolUserDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<SchoolUser> schoolUsers)
        {
            SimpleDelete<SchoolUser>(schoolUsers);
        }
    }
}