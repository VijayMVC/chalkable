using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;

namespace Chalkable.BackgroundTaskProcessor
{
    public class ReportCardTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var data = task.GetData<ReportProcessingTaskData>();          
            var sl = ServiceLocatorFactory.CreateSchoolLocator(data.UserContext);
            sl.ReportService.GenerateReportCard(data.ReportInputModel);
            return true;
        }
    }
}
