using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public class AcademicBenchmarkImportProducer : BaseProducer
    {
        public AcademicBenchmarkImportProducer(string configSectionName) : base(configSectionName)
        {
        }

        private const int FREQUENCY = 24;

        protected override void ProduceInternal(DateTime currentTimeUtc)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            
            var tasks = sl.BackgroundTaskService.Find(null, null, BackgroundTaskTypeEnum.AcademicBenchmarkImport);

            if(!tasks.Any())
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.AcademicBenchmarkImport, DateTime.UtcNow, null, "", BackgroundTask.GLOBAL_DOMAIN);

            var inProgress = tasks.Where(x => x.State == BackgroundTaskStateEnum.Processing || x.State == BackgroundTaskStateEnum.Created);
            if (inProgress.Any())
                return;

            var processedLastDate = tasks.Max(x => x.Completed);
            var now = DateTime.UtcNow;

            if ((now - processedLastDate).Value.Hours >= FREQUENCY)
                sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.AcademicBenchmarkImport, DateTime.UtcNow, null, "", BackgroundTask.GLOBAL_DOMAIN);
        }
    }
}
