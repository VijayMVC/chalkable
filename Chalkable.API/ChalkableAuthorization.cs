using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsAzure.Acs.Oauth2.Client;
using Chalkable.API.Configuration;

namespace Chalkable.API
{
    public class ChalkableAuthorization
    {
        public string ApiRoot { get; }
        public ApplicationEnvironment Configuration { get; }
        public SimpleOAuth2Client OauthClient { get; private set; }
        private string RefreshToken { get; set; }

        public ChalkableAuthorization(string apiRoot, ApplicationEnvironment configuration = null)
        {
            ApiRoot = apiRoot;
#if DEBUG
            apiRoot = "https://localhost";
#endif
            Configuration = configuration ?? Settings.GetConfiguration(apiRoot);
        }

        public async Task AuthorizeAsync(string refreshToken)
        {
            if (RefreshToken == refreshToken)
                return;

            OauthClient = new SimpleOAuth2Client(
                authorizeUri: new Uri(ApiRoot + "/authorize/index"), 
                accessTokenUri: new Uri(Configuration.AcsUri), 
                clientId: Configuration.ClientId, 
                clientSecret: Configuration.AppSecret, 
                scope: Configuration.Scope, 
                redirectUri: new Uri(Configuration.RedirectUri));

            await Task.Run(() => OauthClient.Authorize(refreshToken));

            RefreshToken = refreshToken;
        }
    }
}
