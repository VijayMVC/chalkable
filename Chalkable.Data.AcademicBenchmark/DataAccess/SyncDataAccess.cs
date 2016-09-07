using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.AcademicBenchmark.Model;
using Chalkable.Data.Common;

namespace Chalkable.Data.AcademicBenchmark.DataAccess
{
    public class SyncDataAccess : DataAccessBase<SyncLastDate, int>
    {
        public SyncDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void BeforeSync()
        {
            ExecuteStoredProcedure("spBeforeSync", new Dictionary<string, object>(), 5 * 60);
        }

        public void AfterSync()
        {
            ExecuteStoredProcedure("spAfterSync", new Dictionary<string, object>(), 5 * 60);
        }
    }
}
