using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.DataAccess
{
    public class DbMaintenanceDataAccess : DataAccessBase<object>
    {
        public DbMaintenanceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void BeforeSisRestore()
        {
            IDictionary<string, object> ps = new Dictionary<string, object>{};
            ExecuteStoredProcedure("spBeforeRestore", ps, Settings.DbUpdateTimeout);
        }

        public void AfterSisRestore()
        {
            IDictionary<string, object> ps = new Dictionary<string, object> {};
            ExecuteStoredProcedure("spAfterRestore", ps, Settings.DbUpdateTimeout);
        }
    }
}