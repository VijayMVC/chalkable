using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Common;

namespace Chalkable.AcademicBenchmarkImport
{
    public class ImportDbService : IDbService, IDisposable
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

        public UnitOfWork GetUowForUpdate(IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
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

        public void Dispose()
        {
            uow.Dispose();
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
