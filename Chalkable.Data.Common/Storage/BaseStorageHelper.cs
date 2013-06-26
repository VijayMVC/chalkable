using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace Chalkable.Data.Common.Storage
{
    public class BaseStorageHelper
    {
        protected const string CONNECTION_STRING_NAME = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        protected CloudStorageAccount storageAccount;
        
        protected CloudStorageAccount GetStorageAccount()
        {
            if (storageAccount == null)
            {
                string connectionString = RoleEnvironment.GetConfigurationSettingValue(CONNECTION_STRING_NAME);
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return storageAccount;
        }

    }
}
