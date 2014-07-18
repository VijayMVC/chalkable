using System;
using System.Data;
using System.Threading;
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

    public class ImportDbService : IDbService
    {
        private ImportUnitOfWork uow;
        private object locker = new object();
        private AutoResetEvent e = new AutoResetEvent(true);
        
        public ImportDbService(string connectionString)
        {
            uow = new ImportUnitOfWork(connectionString);
            uow.OnDisposing += () =>
                {
                    e.Set();
                };
            
        }

        public UnitOfWork GetUowForRead()
        {
            lock (locker)
            {
                e.WaitOne();
                e.Reset();
            }
            return uow;
        }

        public UnitOfWork GetUowForUpdate()
        {
            lock (locker)
            {
                e.WaitOne();
                e.Reset();    
            }
            return uow;
        }

        public void CommitAll()
        {
            uow.CommitAll();
        }

        public void BeginTransaction()
        {
            uow.BeginTransaction();
        }

        public void Rollback()
        {
            uow.Rollback();
        }
    }
    
    public class ImportUnitOfWork : UnitOfWork
    {
        public event Action OnDisposing;
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
                Transaction = Connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            else
                throw new Exception("Transaction wasn't started");
        }

        public override void Dispose()
        {
            if (OnDisposing != null)
                OnDisposing();
        }
    }
}
