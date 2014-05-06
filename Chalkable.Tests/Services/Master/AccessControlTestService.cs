using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using WindowsAzure.Acs.Oauth2;

namespace Chalkable.Tests.Services.Master
{
    public class AccessControlTestService : MasterServiceBase, IAccessControlService
    {
        public AccessControlTestService(IServiceLocatorMaster serviceLocator) : base(serviceLocator)
        {
        }

        public string GetAccessToken(string accessTokenUrl, string redirectUrl, string clientId, string clientSecret, string userName, string scope)
        {
            return null;
        }

        public ApplicationRegistration GetApplication(string clientId)
        {
            return null;
        }

        public bool RegisterApplication(string clientId, string appSecretKey, string appUrl, string appName)
        {
            return true;
        }

        public void RemoveApplication(string clientId)
        {
            return;
        }


        public string GetAuthorizationCode(string clientId, string userName, string scope)
        {
            return null;
        }
    }
}
