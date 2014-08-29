using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace Chalkable.Data.Common.Storage
{
    public class BaseStorageHelper
    {
        protected const string CONNECTION_STRING_NAME = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private static CloudStorageAccount storageAccount;

        static BaseStorageHelper()
        {
            string connectionString = RoleEnvironment.GetConfigurationSettingValue(CONNECTION_STRING_NAME);
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }
        
        protected static CloudStorageAccount GetStorageAccount()
        {
            return storageAccount;
        }

    }
}
