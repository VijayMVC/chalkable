using System;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.Master
{
    public class BackgrounTaskServiceTest : ServiceTestBase
    {
        [Test]
        public void CreateGetTest()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var scheduled = DateTime.Now.AddHours(1);
            var task = sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateEmptySchool, scheduled, null, "some data");

        }
        
    }
}