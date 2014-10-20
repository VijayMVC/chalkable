using System;
using System.Net.Http;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class LinkConnector : ConnectorBase
    {
        public LinkConnector(ConnectorLocator locator)
            : base(locator)
        {
        }
        public bool Link(Guid key)
        {
            var url = string.Format("{0}chalkable/linkstatus?LinkKey={1}", BaseUrl, key);
            return Call<string>(url).Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        public void CompleteSync(int schoolId)
        {
            var url = string.Format("{0}chalkable/school/{1}", BaseUrl, schoolId);
            Post<object, object>(url, new { Id = schoolId, IsSyncComplete = true}, null, HttpMethod.Put);
        }
    }
}