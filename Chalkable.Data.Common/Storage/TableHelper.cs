using System.Collections.Generic;
using Chalkable.Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.Data.Common.Storage
{
    public class TableHelper<T> : BaseStorageHelper where T : TableEntity, new()
    {
        private string tableName;
        private CloudTable table;
        
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

        public TableHelper(string defaultProtocol, string accountName, string accountKey) : base(defaultProtocol, accountName, accountKey)
        {
            tableName = typeof(T).Name;
        }
        
        public PaginatedList<T> GetByPartKey(string partitionKey, int start, int count)
        {
            var t = GetTable();
            var tableQuery = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var rangeQuery = new TableQuery<T>().Where(tableQuery).Take(start + count);
            var res = t.ExecuteQuery(rangeQuery);
            return new PaginatedList<T>(res, start / count, count);
        }

        public IList<T> GetNext(ref TableContinuationToken token)
        {
            var t = GetTable();
            var res = t.ExecuteQuerySegmented(new TableQuery<T>(), token);
            token = res.ContinuationToken;
            return res.Results;
        }

        public T GetByKey(string partKey, string key)
        {
            var t = GetTable();
            return (T)t.Execute(TableOperation.Retrieve<T>(partKey, key)).Result;
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
