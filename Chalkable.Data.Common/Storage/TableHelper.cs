using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.Data.Common.Storage
{
    public class TableHelper<T> where T : TableEntity, new()
    {
        private const string CONNECTION_STRING_NAME = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private CloudStorageAccount storageAccount;
        private string tableName;

        private CloudStorageAccount GetStorageAccount()
        {
            if (storageAccount == null)
            {
                string connectionString = RoleEnvironment.GetConfigurationSettingValue(CONNECTION_STRING_NAME);
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            return storageAccount;
        }

        private CloudTable table = null;
        protected CloudTable GetTable()
        {
            if (table == null)
            {
                var tableClient = GetStorageAccount().CreateCloudTableClient();
                table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
            }
            return table;
        }

        public TableHelper()
        {
            tableName = typeof (T).Name;
        }

        /*
         * TableQuery<T> rangeQuery = new TableQuery<T>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, "E")));
         */

        public PaginatedList<T> GetByPartKey(string key, int start, int count)
        {
            var t = GetTable();
            var rangeQuery = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, key)).Take(start + count);
            var res = t.ExecuteQuery(rangeQuery).Skip(count).ToList();
            return new PaginatedList<T>(res, start / count, count);
        }

        public void Save(IList<T> rows)
        {
            if (rows.Count > 0)
            {
                var t = GetTable();
                var batchOperation = new TableBatchOperation();
                foreach (var row in rows)
                {
                    batchOperation.Insert(row);
                }
                t.ExecuteBatch(batchOperation);    
            }
            
        }
    }
}
