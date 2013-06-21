using System;
using System.Linq;
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
            var now = DateTime.Now;
            var scheduled = now.AddHours(1);
            var task = sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateEmptySchool, scheduled, null, "some data");
            var tasks = sl.BackgroundTaskService.GetTasks(0, 10);
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(task.Id, tasks[0].Id);
            Assert.AreEqual(BackgroundTaskStateEnum.Created, tasks[0].State);
            
            var toProcess = sl.BackgroundTaskService.GetTaskToProcess(now);
            Assert.Null(toProcess);//1 hour before scheduled time
            tasks = sl.BackgroundTaskService.GetTasks(0, 10);
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(BackgroundTaskStateEnum.Created, tasks[0].State);

            toProcess = sl.BackgroundTaskService.GetTaskToProcess(scheduled);
            Assert.NotNull(toProcess);
            tasks = sl.BackgroundTaskService.GetTasks(0, 10);
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual(toProcess.Id, tasks[0].Id);
            Assert.AreEqual(BackgroundTaskStateEnum.Processing, tasks[0].State);

            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.CreateEmptySchool, scheduled, null, "some data");
            tasks = sl.BackgroundTaskService.GetTasks(0, 10);
            Assert.AreEqual(2, tasks.Count);
            var toProcess2 = sl.BackgroundTaskService.GetTaskToProcess(scheduled);
            Assert.Null(toProcess2);//there is another task with such school ref in processing state
            sl.BackgroundTaskService.Complete(toProcess.Id,true);
            toProcess2 = sl.BackgroundTaskService.GetTaskToProcess(scheduled);
            Assert.NotNull(toProcess2);
            sl.BackgroundTaskService.Complete(toProcess2.Id, false);
            tasks = sl.BackgroundTaskService.GetTasks(0, 10);
            Assert.AreEqual(2, tasks.Count);
            Assert.AreEqual(tasks.Count(x => x.State == BackgroundTaskStateEnum.Processed), 1);
            Assert.AreEqual(tasks.Count(x => x.State == BackgroundTaskStateEnum.Failed), 1);
        }
        
    }
}