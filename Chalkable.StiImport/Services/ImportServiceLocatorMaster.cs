using System;
using System.Transactions;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common;

namespace Chalkable.StiImport.Services
{
    public class ImportServiceLocatorMaster : ServiceLocatorMaster
    {
        public ImportServiceLocatorMaster(UserContext context) : base(context)
        {
            DbService = new ImportDbService(context.MasterConnectionString);
        }

        public override IServiceLocatorSchool SchoolServiceLocator(Guid districtId, Guid? schoolId)
        {
            var district = DistrictService.GetByIdOrNull(districtId);
            Context.SwitchSchool(null, district.Id, district.Name, district.TimeZone, null, district.ServerUrl, null);
            var serviceLocator = new ImportServiceLocatorSchool(this);
            return serviceLocator;
        }
    }

    public class ImportServiceLocatorSchool : ServiceLocatorSchool
    {
        public ImportServiceLocatorSchool(IServiceLocatorMaster serviceLocatorMaster) : base(serviceLocatorMaster)
        {
            SchoolDbService = new ImportDbService(this.Context.SchoolConnectionString);
        }
    }

    public class ImportDbService : DbService, IDisposable
    {
        private TransactionScope scope;
        public ImportDbService(string connectionString) : base(connectionString)
        {
            
        }

        public void CommitAll()
        {
            scope.Complete();
        }

        public void BeginTransaction()
        {
            scope = new TransactionScope(TransactionScopeOption.RequiresNew);
        }

        public void Rollback()
        {
            scope.Dispose();
        }

        public void Dispose()
        {
            if (scope != null)
                scope.Dispose();
        }
    }
    
    /*public class ImportUnitOfWork : UnitOfWork
    {
        public ImportUnitOfWork(string connectionString) : base(connectionString, false)
        {
        }

        public override void Commit()
        {
            //TODO: do nothing
        }

        public void CommitAll()
        {
            base.Commit();
        }

        public void BeginTransaction()
        {
            if (Transaction == null)
                Transaction = Connection.BeginTransaction();
            else
                throw new Exception("Transaction wasn't started");
        }

        public override void Dispose()
        {
            //DO nothing
        }
    }*/
}
