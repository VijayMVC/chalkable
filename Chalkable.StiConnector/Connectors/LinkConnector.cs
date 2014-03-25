using System;
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
            //http://sandbox.sti-k12.com/chalkable/api/chalkale/linkstatus?LinkKey=[LinkKeyGuid]
            var url = string.Format("{0}chalkale/linkstatus?LinkKey={1}", BaseUrl, key);
            return Call<LinkStatus>(url).Active;
        }
    }
}