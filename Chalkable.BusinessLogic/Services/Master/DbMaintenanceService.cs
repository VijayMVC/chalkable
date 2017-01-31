using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IDbMaintenanceService
    {
        void BeforeSisRestore(Guid districtId);
        IList<RestoreLogItem> AfterSisRestore(Guid districtId);
    }

    public class DbMaintenanceService : MasterServiceBase, IDbMaintenanceService
    {
        public DbMaintenanceService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public void BeforeSisRestore(Guid districtId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(uow => new DbMaintenanceDataAccess(uow).BeforeSisRestore(districtId));
        }

        public IList<RestoreLogItem> AfterSisRestore(Guid districtId)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            return DoRead(uow => new DbMaintenanceDataAccess(uow).AfterSisRestore(districtId));
        }
    }
}