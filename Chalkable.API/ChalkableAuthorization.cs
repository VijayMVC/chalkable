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
            var ts = DateTime.UtcNow.ToString("s");
            var appSecret = Configuration.AppSecret;

            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var keys = query.AllKeys
                .OrderBy(x => x)
                .Select(key => $"_{key}={query[key]}")
                .JoinString("_");

            var signatureBase = $"{request.Method.ToLowerInvariant()}_{request.RequestUri.AbsolutePath}_{Token}_{ts}_{keys}_{appSecret}";

            var signatureHash = HashHelper.HexOfCumputedHash(signatureBase);

            var signatureJson = JsonConvert.SerializeObject(new
            {
                token = Token,
                ts,
                signature = signatureHash
            });

            var signatureBytes = System.Text.Encoding.UTF8.GetBytes(signatureJson);
            var signature = Convert.ToBase64String(signatureBytes);

            request.Headers.Add("Authorization", "Signature:" + signature);
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
