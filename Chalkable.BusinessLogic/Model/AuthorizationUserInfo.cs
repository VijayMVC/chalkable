using System;
using System.Text;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Model
{
    public class AuthorizationUserInfo
    {
        public string SessionKey { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid RandomGuid { get; set; }

        public static AuthorizationUserInfo Create(string sessionKey, Guid applicationId)
        {
            return new AuthorizationUserInfo
            {
                ApplicationId = applicationId,
                SessionKey = sessionKey,
                RandomGuid = Guid.NewGuid()
            };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AuthorizationUserInfo FromString(string json)
        {
            return JsonConvert.DeserializeObject<AuthorizationUserInfo>(json);
        }
    }
}
