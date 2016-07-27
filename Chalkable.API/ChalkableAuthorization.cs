using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WindowsAzure.Acs.Oauth2.Client;
using WindowsAzure.Acs.Oauth2.Client.Protocol;
using Chalkable.API.Configuration;
using Chalkable.API.Exceptions;
using Chalkable.API.Helpers;
using Newtonsoft.Json;

namespace Chalkable.API
{
    internal class SimpleOAuth2ClientInternal : SimpleOAuth2Client
    {
        public AccessTokenResponse CurrentAccessTokenPublic
        {
            get { return CurrentAccessToken; }
            set { CurrentAccessToken = value; }
        }

        public DateTime LastAccessTokenRefreshPublic
        {
            get { return LastAccessTokenRefresh; }
            set { LastAccessTokenRefresh = value; }
        }

        public SimpleOAuth2ClientInternal(Uri authorizeUri, Uri accessTokenUri, string clientId, string clientSecret, string scope, Uri redirectUri, ClientMode mode = ClientMode.ThreeLegged) : base(authorizeUri, accessTokenUri, clientId, clientSecret, scope, redirectUri, mode)
        {            
        }
    }

    public class ChalkableAuthorization
    {
        public string ApiRoot { get; }
        private string Token { get; set; }
        public ApplicationEnvironment Configuration { get; }

        public ChalkableAuthorization(string apiRoot, ApplicationEnvironment configuration = null)
        {
            ApiRoot = apiRoot;
            Configuration = configuration ?? Settings.GetConfiguration(apiRoot);
        }


        public async Task AuthorizeAsync(string token)
        {
            Token = token;
            await Task.FromResult(true);
        }

        public void SignRequest(HttpWebRequest request)
        {
            
        }

        public void AuthorizeQueryRequest(string token, IList<string> identityParams)
        {
            var signatureMsg = identityParams.JoinString("|");
            var appSecret = Configuration.AppSecret;
            signatureMsg += "|" + HashHelper.HexOfCumputedHash(appSecret);
            var hash = HashHelper.HexOfCumputedHash(signatureMsg);
            if (token != hash)
                throw new ChalkableApiException($"Security error. Invalid token in query request to {Configuration.ApplicationRoot}");

        }


        public class ChalkableAuthorizationSerialized
        {
            public string ApiRoot { get; set; }
            public string Token { get; set; }
        }

        private ChalkableAuthorization(ChalkableAuthorizationSerialized data, ApplicationEnvironment configuration = null)
        {
            ApiRoot = data.ApiRoot;
            Configuration = configuration ?? Settings.GetConfiguration(ApiRoot);
            Token = data.Token;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(new ChalkableAuthorizationSerialized
            {
                ApiRoot = ApiRoot,
                Token = Token
            });
        }

        public static ChalkableAuthorization Deserialize(string json, ApplicationEnvironment configuration = null)
        {
            var data = JsonConvert.DeserializeObject<ChalkableAuthorizationSerialized>(json);
            return new ChalkableAuthorization(data, configuration);
        }
    }    
}
