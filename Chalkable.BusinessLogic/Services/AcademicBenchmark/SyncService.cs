using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.DataAccess;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface ISyncService
    {
        void UpdateLastSyncDate(DateTime date);
        DateTime? GetLastSyncDateOrNull();
        void BeforeSync();
        void AfterSync();
    }

    public class SyncService : AcademicBenchmarkServiceBase<SyncLastDate, DateTime>, ISyncService
    {
        public SyncService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public DateTime? GetLastSyncDateOrNull()
        {
            return DoRead(u => new SyncDataAccess(u).GetAll().FirstOrDefault())?.Date;
        }

        public void UpdateLastSyncDate(DateTime date)
        {
            DoUpdate(u =>
            {
                new SyncDataAccess(u).Delete(1);
                new SyncDataAccess(u).Insert(SyncLastDate.Create(1, date));
            });
        }

        public void BeforeSync()
        {
            DoUpdate(u => new SyncDataAccess(u).BeforeSync());
        }

        public void AfterSync()
        {
            DoUpdate(u => new SyncDataAccess(u).AfterSync());
        }
    }
}
