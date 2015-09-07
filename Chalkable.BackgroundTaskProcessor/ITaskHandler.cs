using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.BackgroundTaskProcessor
{
    public interface ITaskHandler
    {
        bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log);
    }
}