using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public interface ITaskHandler
    {
        bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log);
    }
}