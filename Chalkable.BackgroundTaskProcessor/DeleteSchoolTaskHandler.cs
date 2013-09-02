using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class DeleteSchoolTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var id = Guid.Parse(task.Data);
            if (sl.SchoolService.GetByIdOrNull(id) != null)
            {
                sl.SchoolService.DeleteSchool(id);    
            }
            return true;
        }
    }
}