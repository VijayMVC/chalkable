using Chalkable.BusinessLogic.Model;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Services.Master
{
    public interface IApplicationUploadService
    {
        Application Create(BaseApplicationInfo appInfo);
        Application UpdateDraft(int applicationId, BaseApplicationInfo appInfo);
        Application Submit(int applicationId, BaseApplicationInfo appInfo);
        bool ApproveReject(int applicationId, bool isApprove);
        bool GoLive(int applicationId);
        bool UnList(int applicationId);
        bool Exists(int? currentApplicationId, string name, string url);
        bool DeleteApplication(int id);
        void ChangeApplicationType(int applicationId, bool isInternal);
    }

    public class ApplicationUploadService : MasterServiceBase, IApplicationUploadService
    {
        public ApplicationUploadService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public Application Create(BaseApplicationInfo appInfo)
        {
            throw new System.NotImplementedException();
        }

        public Application UpdateDraft(int applicationId, BaseApplicationInfo appInfo)
        {
            throw new System.NotImplementedException();
        }

        public Application Submit(int applicationId, BaseApplicationInfo appInfo)
        {
            throw new System.NotImplementedException();
        }

        public bool ApproveReject(int applicationId, bool isApprove)
        {
            throw new System.NotImplementedException();
        }

        public bool GoLive(int applicationId)
        {
            throw new System.NotImplementedException();
        }

        public bool UnList(int applicationId)
        {
            throw new System.NotImplementedException();
        }

        public bool Exists(int? currentApplicationId, string name, string url)
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteApplication(int id)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeApplicationType(int applicationId, bool isInternal)
        {
            throw new System.NotImplementedException();
        }
    }
}