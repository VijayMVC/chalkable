using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class BackgroundTaskDataAccess : DataAccessBase
    {
        public BackgroundTaskDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(BackgroundTask task)
        {
            SimpleInsert(task);
        }

        public BackgroundTask GetTaskForProcessing()
        {
            using (var reader = ExecuteStoredProcedureReader("spGetBackgroundTaskForProcessing", new Dictionary<string, object>()))
            {
                return reader.ReadOrNull<BackgroundTask>();
            }
        }
    }
}
