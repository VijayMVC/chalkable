﻿using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;

namespace Chalkable.Data.Common.Storage
{
    public class BaseStorageHelper
    {
        protected const string CONNECTION_STRING_NAME = "ChalkableStorage.ConnectionString";
        private static CloudStorageAccount defaultStorageAccount;
        private CloudStorageAccount storageAccount;

        static BaseStorageHelper()
        {
            if (RoleEnvironment.IsAvailable)
            {
                string connectionString = RoleEnvironment.GetConfigurationSettingValue(CONNECTION_STRING_NAME);
                defaultStorageAccount = CloudStorageAccount.Parse(connectionString);    
            }
            
        }

        public BaseStorageHelper()
        {
            storageAccount = defaultStorageAccount;
        }

        public BaseStorageHelper(string defaultProtocol, string accountName, string accountKey)
        {
            string connectionString = string.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}", defaultProtocol, accountName, accountKey);
            storageAccount = CloudStorageAccount.Parse(connectionString);
        }
        
        protected static CloudStorageAccount GetDefaultStorageAccount()
        {
            return defaultStorageAccount;
        }

        protected CloudStorageAccount GetStorageAccount()
        {
            return storageAccount;
        }
    }
}
