using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BackgroundTaskProcessor
{
    public class PictureImportTaskHandler : ITaskHandler
    {
        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = task.GetData<PictureImportTaskData>();
            var district = sl.DistrictService.GetByIdOrNull(data.DistrictId);
            var connectorLocator = ConnectorLocator.Create(district.SisUserName, district.SisPassword, district.SisUrl);
            foreach (var person in data.PersonIds)
            {
                var content = connectorLocator.UsersConnector.GetPhoto(person);
                if (content != null)
                    sl.PersonPictureService.UploadPicture(data.DistrictId, person, content);
                else
                    sl.PersonPictureService.DeletePicture(data.DistrictId, person);
            }
            return true;
        }
    }
}