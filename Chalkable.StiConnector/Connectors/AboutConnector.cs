using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class AboutConnector : ConnectorBase
    {
        public AboutConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public async Task<About> GetApiVersion()
        {
            return await CallAsync<About>($"{BaseUrl}about");
        }
    }
}
