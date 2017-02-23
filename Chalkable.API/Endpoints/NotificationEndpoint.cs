using System.Threading.Tasks;
using System.Web;

namespace Chalkable.API.Endpoints
{
    public class NotificationEndpoint : Base
    {
        public NotificationEndpoint(IConnector connector) : base(connector)
        {
        }

        public async Task NotifyPerson(string html)
        {
            var @params = HttpUtility.ParseQueryString(string.Empty);
            @params.Add("htmlText", html);

            await Connector.Post<bool>($"/Notification/ApplicationNotification.json", @params);
        }
    }
}
