using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.BusinessLogic.Services.AcademicBenchmark
{
    public interface ISyncService
    {
        void UpdateLastSyncDate(DateTime date);
        DateTime? GetLastSyncDateOrNull();
    }

    public class SyncService : AcademicBenchmarkServiceBase<SyncLastDate, DateTime>, ISyncService
    {
        public SyncService(IAcademicBenchmarkServiceLocator locator) : base(locator)
        {
        }

        public DateTime? GetLastSyncDateOrNull()
        {
            return DoRead(u => new DataAccessBase<SyncLastDate>(u).GetAll().FirstOrDefault())?.Date;
        }

        public void UpdateLastSyncDate(DateTime date)
        {
            DoUpdate(u =>
            {
                new DataAccessBase<SyncLastDate, int>(u).Delete(1);
                new DataAccessBase<SyncLastDate>(u).Insert(SyncLastDate.Create(1, date));
            });
        }
    }
}
