using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IDbMaintenanceService
    {
        void BeforeSisRestore();
        void AfterSisRestore();
    }

    public class DbMaintenanceService : SchoolServiceBase, IDbMaintenanceService
    {
        public DbMaintenanceService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void BeforeSisRestore()
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(uow => new DbMaintenanceDataAccess(uow).BeforeSisRestore());
        }

        public void AfterSisRestore()
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(uow => new DbMaintenanceDataAccess(uow).AfterSisRestore());
        }
    }
}