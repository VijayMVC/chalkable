using System.Linq;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class AttendanceNotificationTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            if (!task.DistrictRef.HasValue)
            {
                log.LogError(string.Format("attendance notification task {0} should contains school id", task.Id));
                return false;
            }
            var school = sl.SchoolService.GetSchools(task.DistrictRef.Value, 0, 1).First();
            var schoolSl = sl.SchoolServiceLocator(school.Id);
            schoolSl.AttendanceService.ProcessClassAttendance(schoolSl.Context.NowSchoolTime);
            return true;
        }
    }

    public class TeacherAttendanceNotificationTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            if (!task.DistrictRef.HasValue)
            {
                log.LogError(string.Format("teacher attendance notification task {0} should contains school id", task.Id));
                return false;
            }
            var schoolSl = sl.SchoolServiceLocator(task.DistrictRef.Value, null);
            schoolSl.AttendanceService.NotAssignedAttendanceProcess();
            return true;
        }
    }
}