using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class AboutConnector : ConnectorBase
    {
        public AboutConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public About GetApiVersion()
        {
            var res =  Call<About>($"{BaseUrl}about");
            VersionHelper.ValidateVersionFormat(res.Version);
            return res;
        }
    }
}
