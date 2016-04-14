using System;
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
            var url = $"{BaseUrl}chalkable/{"users"}/me";
            return Call<User>(url);
        }
        
        public byte[] GetPhoto(int personId)
        {
            var url = $"{BaseUrl}persons/{personId}/photo";
            return Download(url);
        }
        
        public StiPersonEmail GetPrimaryPersonEmail(int personId)
        {
            var url = $"{BaseUrl}persons/{personId}/emailaddresses/primary";
            return Call<StiPersonEmail>(url);
        }
        
        public void UpdatePrimaryPersonEmail(int personId, StiPersonEmail personEmail)
        {
            var url = $"{BaseUrl}persons/{personId}/emailaddresses/primary";
            Put<Object, StiPersonEmail>(url, personEmail);
        }
        
        public int[] GetUserAcadSessionsIds()
        {
            return Call<int[]>($"{BaseUrl}users/me/acadsessions");
        } 
    }
}
