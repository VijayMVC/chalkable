using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class AttendanceNotificationProducer : BaseProducer
    {
        private const string CONFIG_SECTION_NAME = "AttendanceNotificationProducer";
        public AttendanceNotificationProducer()
            : base(CONFIG_SECTION_NAME)
        {
        }

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var schools = sl.SchoolService.GetSchools(false, false);
            foreach (var school in schools)
            {
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.AttendanceNotification, currentTimeUtc, school.Id, string.Empty);
            }
        }
    }
}