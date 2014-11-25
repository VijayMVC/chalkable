using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class DeleteSchoolTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            log.LogError("This operation is not supported");
            return false;
        }
    }
}