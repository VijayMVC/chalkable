using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class UsersConnector : ConnectorBase
    {
        public UsersConnector(ConnectorLocator locator)
            : base(locator)
        {
        }
        public User GetMe()
        {
            var url = string.Format("{0}chalkable/{1}/me", BaseUrl, "users");
            return Call<User>(url);
        }

        public byte[] GetPhoto(int personId)
        {
            var url = string.Format("{0}persons/{1}/photo", BaseUrl, personId);
            return Download(url);
        }

        public int[] GetUserAcadSessionsIds()
        {
            return Call<int[]>(string.Format("{0}users/me/acadsessions", BaseUrl));
        } 
    }
}
