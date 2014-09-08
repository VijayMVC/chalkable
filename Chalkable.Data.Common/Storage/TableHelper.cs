﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Microsoft.WindowsAzure.Storage.Table;

namespace Chalkable.Data.Common.Storage
{
    public class TableHelper<T> : BaseStorageHelper where T : TableEntity, new()
    {
        private string tableName;
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

        public IList<T> GetNext(string lastKey, int count)
        {
            var t = GetTable();
            TableQuery<T> rangeQuery;
            if (lastKey != null)
            {
                var tableQuery = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, lastKey);
                rangeQuery = new TableQuery<T>().Where(tableQuery).Take(count);    
            }
            else
                rangeQuery = new TableQuery<T>().Take(count);    
            var res = t.ExecuteQuery(rangeQuery);
            return res.ToList();
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
