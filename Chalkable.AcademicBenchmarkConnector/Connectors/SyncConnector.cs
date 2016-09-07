﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Models;
using Chalkable.Common;

namespace Chalkable.AcademicBenchmarkConnector.Connectors
{
    public interface ISyncConnector
    {
        Task<PaginatedList<SyncData>> GetStandardsSyncData(DateTime since, int start, int count);
        Task<PaginatedList<SyncData>> GetTopicsSyncData(DateTime since, int start, int count);
    }

    public class SyncConnector : ConnectorBase, ISyncConnector
    {
        public SyncConnector(IConnectorLocator connectorLocator) : base(connectorLocator)
        {
        }
        
        private async Task<PaginatedList<SyncData>> GetSyncDataForResource(string resource, DateTime since,
            int start, int count)
        {
            var url = "sync";
            var nvc = new NameValueCollection
            {
                {"since", since.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) },
                {"resource", resource },
                {"limit", count.ToString()},
                {"offset", start.ToString() }
            };

            var res = await CallAsync<PaginatedResponse<BaseResource<SyncData>>>(url, nvc);

            return new PaginatedList<SyncData>(res.Resources.Select(x => x.Data), start / count, count, res.Count);
        }

        public async Task<PaginatedList<SyncData>> GetStandardsSyncData(DateTime since, int start, int count)
        {
            return await GetSyncDataForResource("standards", since, start, count);
        }

        public async Task<PaginatedList<SyncData>> GetTopicsSyncData(DateTime since, int start, int count)
        {
            return await GetSyncDataForResource("topics", since, start, count);
        }
    }
}
