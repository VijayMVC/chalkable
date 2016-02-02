using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using WindowsAzure.Acs.Oauth2.Client;
using Chalkable.API.Endpoints;
using Chalkable.API.Exceptions;
using Newtonsoft.Json;

namespace Chalkable.API
{
    public class ChalkableConnector: IConnector
    {
        public PersonEndpoint Person => new PersonEndpoint(this);
        public AnnouncementEndpoint Announcement => new AnnouncementEndpoint(this);

        public class ResponseDto<T>
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("data")]
            public T Data { get; set; }
        }

        public async Task<T> Call<T>(string endpoint, OnWebRequestIsCreated onCreated = null, string method = null)
        {
            var url = ApiRoot + endpoint;

            try
            {
                Debug.WriteLine("Request on: " + url);
                if (OauthClient != null)
                {
                    Debug.WriteLine("Request on: " + url);
                    var webRequest = (HttpWebRequest) WebRequest.Create(url);
                    webRequest.Method = string.IsNullOrWhiteSpace(method) ? WebRequestMethods.Http.Get : method;
                    webRequest.Accept = "application/json";
                    OauthClient.AppendAccessTokenTo(webRequest);
                    onCreated?.Invoke(webRequest);
                    var response = await webRequest.GetResponseAsync();
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream == null)
                            throw new ChalkableApiException("Error");

                        using (var sr = new StreamReader(stream)) {
                            var str = sr.ReadToEnd();
                            Debug.WriteLine(str);
                            var obj = JsonConvert.DeserializeObject<ResponseDto<T>>(str) ;
                            if (!obj.Success)
                                throw new ChalkableApiException(str);

                            return obj.Data;
                        };
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var strRe = new StreamReader(e.Response.GetResponseStream());
                    var rsp = strRe.ReadToEnd();
                    throw new ChalkableApiException($"call to remote server failed: {e.Message}\n{rsp}", e);
                }

                throw;
            }
            throw new ChalkableApiException("oauth client isn't initialized");
        }

        private SimpleOAuth2Client OauthClient { get; }
        private string ApiRoot { get; }

        public ChalkableConnector(ChalkableAuthorization authorization)
        {
            OauthClient = authorization.OauthClient;
            ApiRoot = authorization.ApiRoot;
        }
    }
}