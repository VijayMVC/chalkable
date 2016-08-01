using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Chalkable.API.Configuration;
using Chalkable.API.Exceptions;
using Chalkable.API.Helpers;
using Newtonsoft.Json;

namespace Chalkable.API
{
    public class TokenAuthenticationInfo
    {
        [JsonProperty("token")]
        public string AppToken { get; set; }
        [JsonProperty("ts")]
        public long Timestamp { get; set; }
        [JsonProperty("sig")]
        public string Signature { get; set; }
    }

    public class ChalkableAuthorization
    {
        public const string AuthenticationHeaderName = "Authorization";
        public const string AuthenticationSignature = "Signature";

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

        public static string ComputeSignature(string method, Uri requestUri, long contentLength, long ts, string token, string appSecret)
        {
            var query = HttpUtility.ParseQueryString(requestUri.Query);
            var keys = query.AllKeys
                .OrderBy(x => x)
                .Select(key => $"_{key}={query[key]}")
                .JoinString("_");

            var signatureBase = $"{method.ToLowerInvariant()}_{requestUri.AbsolutePath}_{token}_{ts}_{Math.Max(0, contentLength)}_{keys}_{appSecret}";

            return HashHelper.HexOfCumputedHash(signatureBase);
        }
        
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public void SignRequest(HttpWebRequest request)
        {
            var ts = (long)DateTimeToUnixTimestamp(DateTime.UtcNow);
            var appSecret = Configuration.AppSecret;

            var signatureJson = JsonConvert.SerializeObject(new TokenAuthenticationInfo
            {
                AppToken = Token,
                Timestamp = ts,
                Signature = ComputeSignature(request.Method, request.RequestUri, request.ContentLength, ts, Token, appSecret)
            });

            var signatureBytes = System.Text.Encoding.UTF8.GetBytes(signatureJson);
            var signature = Convert.ToBase64String(signatureBytes);

            request.Headers.Add(AuthenticationHeaderName, $"{AuthenticationSignature}:{signature}");
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
