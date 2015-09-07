using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

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
            //var schoolSl = sl.SchoolServiceLocator(task.DistrictRef.Value, null);
            //schoolSl.AttendanceService.ProcessClassAttendance(schoolSl.Context.NowSchoolTime);
            throw new NotImplementedException();
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
            //var schoolSl = sl.SchoolServiceLocator(task.DistrictRef.Value, null);
            //schoolSl.AttendanceService.NotAssignedAttendanceProcess();
            throw new NotImplementedException();
            return true;
        }
    }
}