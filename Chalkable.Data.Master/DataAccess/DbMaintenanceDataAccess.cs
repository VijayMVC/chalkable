using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class DbMaintenanceDataAccess : DataAccessBase<RestoreLogItem>
    {
        public DbMaintenanceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void BeforeSisRestore(Guid districtId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId}
            };
            ExecuteStoredProcedure("spBeforeRestore", ps, Settings.DbUpdateTimeout);
        }

        public IList<RestoreLogItem> AfterSisRestore(Guid districtId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@districtId", districtId}
            };
            return ExecuteStoredProcedureList<RestoreLogItem>("spAfterRestore", ps, Settings.DbUpdateTimeout);
        }
    }
}