using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public ApplicationEnvironment Configuration { get; }
        public SimpleOAuth2Client OauthClient { get; private set; }
        private string RefreshToken { get; set; }

        public ChalkableAuthorization(string apiRoot, ApplicationEnvironment configuration = null)
        {
            ApiRoot = apiRoot;
            Configuration = configuration ?? Settings.GetConfiguration(apiRoot);
        }


        public async Task AuthorizeAsync(string refreshToken)
        {
            if (RefreshToken == refreshToken)
                return;

            OauthClient = new SimpleOAuth2ClientInternal(
                authorizeUri: new Uri(ApiRoot + "/authorize/index"),
                accessTokenUri: new Uri(Configuration.AcsUri),
                clientId: Configuration.ClientId,
                clientSecret: Configuration.AppSecret,
                scope: Configuration.Scope,
                redirectUri: new Uri(Configuration.RedirectUri));

            await Task.Run(() => OauthClient.Authorize(refreshToken));

            RefreshToken = refreshToken;
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
            public IDictionary<string, string> AccessTokenParams { get; set; }
            public Uri AccessTokenBase { get; set; }
            public DateTime AccessTokenRefreshed { get; set; }
            public string RefreshToken { get; set; }
        }

        private ChalkableAuthorization(ChalkableAuthorizationSerialized data, ApplicationEnvironment configuration = null)
        {
            ApiRoot = data.ApiRoot;
            Configuration = configuration ?? Settings.GetConfiguration(ApiRoot);

            var client = new SimpleOAuth2ClientInternal(
                authorizeUri: new Uri(ApiRoot + "/authorize/index"),
                accessTokenUri: new Uri(Configuration.AcsUri),
                clientId: Configuration.ClientId,
                clientSecret: Configuration.AppSecret,
                scope: Configuration.Scope,
                redirectUri: new Uri(Configuration.RedirectUri))
            {
                LastAccessTokenRefreshPublic = data.AccessTokenRefreshed,
                CurrentAccessTokenPublic = new AccessTokenResponse(data.AccessTokenBase)
            };

            foreach (var pair in data.AccessTokenParams)
            {
                client.CurrentAccessTokenPublic.Parameters.Add(pair.Key, pair.Value);
            }

            OauthClient = client;

            RefreshToken = data.RefreshToken;
        }

        public string Serialize()
        {
            var client = OauthClient as SimpleOAuth2ClientInternal;

            if (client == null)
                throw new Exception("Serialization failed");

            var s = client.CurrentAccessTokenPublic.Parameters;
            var p = new Dictionary<string, string>();
            foreach (var key in s.AllKeys)
            {
                p[key] = s[key];
            }
            
            return JsonConvert.SerializeObject(new ChalkableAuthorizationSerialized
            {
                ApiRoot = ApiRoot,
                AccessTokenParams = p,
                AccessTokenBase = client.CurrentAccessTokenPublic.BaseUri,
                AccessTokenRefreshed = client.LastAccessTokenRefreshPublic,
                RefreshToken = RefreshToken
            });
        }

        public static ChalkableAuthorization Deserialize(string json, ApplicationEnvironment configuration = null)
        {
            var data = JsonConvert.DeserializeObject<ChalkableAuthorizationSerialized>(json);
            return new ChalkableAuthorization(data, configuration);
        }
    }    
}
