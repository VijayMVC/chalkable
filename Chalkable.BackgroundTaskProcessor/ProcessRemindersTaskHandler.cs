using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class ProcessRemindersTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            if (!task.DistrictRef.HasValue)
            {
                log.LogError(string.Format("process reminders task {0} should contains school id", task.Id));
                return false;
            }
            var schoolSl = sl.SchoolServiceLocator(task.DistrictRef.Value, null);
            schoolSl.AnnouncementReminderService.ProcessReminders(int.MaxValue);
            return true;
        }
    }
}