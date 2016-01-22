using System;
using System.Net.Http;

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
            var url = $"{BaseUrl}chalkable/linkstatus?LinkKey={key}";
            return Call<string>(url).Equals("active", StringComparison.OrdinalIgnoreCase);
        }

        
        public void CompleteSync(int schoolId)
        {
            var url = $"{BaseUrl}chalkable/school/{schoolId}";
            Post<object, object>(url, new { Id = schoolId, IsSyncComplete = true}, null, HttpMethod.Put);
        }
    }
}