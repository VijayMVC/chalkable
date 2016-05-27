using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.Data.Common.Storage
{
    public class TableHelper<T> : BaseStorageHelper where T : TableEntity, new()
    {
        private const int MAX_BATCH_SIZE = 1000;
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
            var rangeQuery = new TableQuery<T>().Where(tableQuery);
            var res = t.ExecuteQuery(rangeQuery).ToList();
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

        private void SaveBatch(IEnumerable<T> rows)
        {
            var t = GetTable();
            var batchOperation = new TableBatchOperation();
            foreach (var row in rows)
            {
                batchOperation.Insert(row);
            }
            if (batchOperation.Count > MAX_BATCH_SIZE)
                throw new Exception($"Batch size should be not greater than {MAX_BATCH_SIZE}");
            t.ExecuteBatch(batchOperation);
        }

        public void Save(IList<T> rows)
        {
            int start = 0;
            while (start < rows.Count)
            {
                SaveBatch(rows.Skip(start).Take(MAX_BATCH_SIZE));
                start += MAX_BATCH_SIZE;
            }
        }
    }
}
