using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Data.School.DataAccess
{
    public class SyncVersionDataAccess : DataAccessBase<SyncVersion, int>
    {
        public SyncVersionDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void DeleteAll()
        {
            ExecuteNonQueryParametrized("Delete from SyncVersion", new Dictionary<string, object>());
        }
    }
}