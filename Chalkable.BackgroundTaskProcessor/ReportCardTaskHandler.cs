using System;
using System.Diagnostics;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BackgroundTaskProcessor
{
    public class ReportCardTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            
            var data = task.GetData<ReportProcessingTaskData>();
            //var sl = ServiceLocatorFactory.CreateMasterSysAdmin().SchoolServiceLocator(data.DistrictId, data.SchoolId);//ServiceLocatorFactory.CreateSchoolLocator(data.UserContext);
            //sl.Context.SchoolYearId = data.SchoolYearId;

            //var d = sl.ServiceLocatorMaster.DistrictService.GetByIdOrNull(data.DistrictId);
            //var connector = ConnectorLocator.Create(d.SisUserName, d.SisPassword, d.SisUrl);
            //sl.Context.SisToken = connector.Token;
            //sl.Context.SisApiVersion = connector.ApiVersion;
            //sl.Context.SisUrl = d.SisUrl;
            //sl.Context.SisTokenExpires = connector.TokenExpires;
            
            var sl = ServiceLocatorFactory.CreateSchoolLocator(data.UserContext);
            try
            {
                var content = sl.ReportService.GetReportCards(data.ReportInputModel, data.ReportInputModel.DefaultDataPath);
                var key = GenerateReportKey();
                sl.StorageBlobService.AddBlob("reports", key, content);
                sl.NotificationService.AddReportProcessedNotification(data.UserContext.PersonId.Value, data.UserContext.RoleId, "Report Cards", key, null, true);
                return true;
            }
            catch (Exception e)
            {
                sl.NotificationService.AddReportProcessedNotification(data.UserContext.PersonId.Value, data.UserContext.RoleId, "Report Cards", null, "We had an issue building your report. Please try again.", false);
                throw e;
            }
        }

        private string GenerateReportKey()
        {
            return "reportcard_" + Guid.NewGuid();
        }
    }
}
