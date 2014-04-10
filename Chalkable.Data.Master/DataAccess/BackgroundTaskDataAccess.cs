using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class BackgroundTaskDataAccess : DataAccessBase<BackgroundTask, Guid>
    {
        public BackgroundTaskDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public BackgroundTask GetTaskForProcessing(DateTime now)
        {
            using (var reader = ExecuteStoredProcedureReader("spGetBackgroundTaskForProcessing", new Dictionary<string, object>{{"currentTime", now}}))
            {
                return reader.ReadOrNull<BackgroundTask>();
            }
        }

        public PaginatedList<BackgroundTask> GetTasks(int start, int count)
        {
            return GetPage(start, count, BackgroundTask.SCHEDULED_FIELD_NAME, Orm.OrderType.Desc);
        }

        public void Complete(Guid id, bool success)
        {
            var state = (int)(success ? BackgroundTaskStateEnum.Processed : BackgroundTaskStateEnum.Failed);
            var conds = new AndQueryCondition {{BackgroundTask.ID_FIELD_NAME, id}};
            var setParamsDic = new Dictionary<string, object>
                {
                    {BackgroundTask.STATE_FIELD_NAME, state},
                    {BackgroundTask.COMPLETED_FIELD_NAME, DateTime.UtcNow},
                };
            SimpleUpdate<BackgroundTask>(setParamsDic, conds);
        }

        public PaginatedList<BackgroundTask> Find(Guid? districtId, BackgroundTaskStateEnum? state, BackgroundTaskTypeEnum? type, bool allDistricts, int start, int count)
        {
            var q = new AndQueryCondition();
            if (state.HasValue)
                q.Add(BackgroundTask.STATE_FIELD_NAME, state);
            if (type.HasValue)
                q.Add(BackgroundTask.TYPE_FIELD_NAME, type);
            if (!allDistricts)
                q.Add(BackgroundTask.DISTRICT_REF_FIELD_NAME, districtId);
            return PaginatedSelect<BackgroundTask>(q, BackgroundTask.SCHEDULED_FIELD_NAME, start, count, Orm.OrderType.Desc);
            
        }
    }
}
