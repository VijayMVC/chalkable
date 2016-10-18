using System;
using System.IO;
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
            string appRoot = Environment.GetEnvironmentVariable("RoleRoot");
            data.ReportInputModel.DefaultDataPath = Path.Combine(appRoot + @"\", @"approot\");
            sl.ReportService.GenerateReportCard(data.ReportInputModel);

            return true;
        }
    }
}
