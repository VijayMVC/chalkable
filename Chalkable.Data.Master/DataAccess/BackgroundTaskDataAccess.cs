using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class BackgroundTaskDataAccess : DataAccessBase<BackgroundTask>
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
            SimpleUpdate<BackgroundTask>(new Dictionary<string, object>{
                                                                            {BackgroundTask.STATE_FIELD_NAME, state},
                                                                            {BackgroundTask.COMPLETED_FIELD_NAME, DateTime.UtcNow},
                                                                        }
                , new Dictionary<string, object>{{BackgroundTask.ID_FIELD_NAME, id}});
        }

        public BackgroundTask Find(Guid? schoolId, BackgroundTaskStateEnum state, BackgroundTaskTypeEnum type)
        {
            return SelectOneOrNull<BackgroundTask>(new AndQueryCondition {
                                                                        {BackgroundTask.SCHOOL_REF_FIELD_NAME, schoolId},
                                                                        {BackgroundTask.STATE_FIELD_NAME, state},
                                                                        {BackgroundTask.TYPE_FIELD_NAME, type},
                                                                });
        }
    }
}
