using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
        public StudyCenterEndpoint StudeCenterEndpoint => new StudyCenterEndpoint(this);
        public GradingEndpoint GradingEndpoint => new GradingEndpoint(this);

        
        public async Task<T> Get<T>(string endpoint)
        {
            return await Call<T>(endpoint);
        }

        public async Task<T> Put<T>(string endpoint, Stream stream)
        {
            return await Call<T>(endpoint, stream, WebRequestMethods.Http.Put);
        }

        public async Task<T> Post<T>(string endpoint, NameValueCollection postData)
        {
            var stream = new MemoryStream();
            if (postData != null)
            {
                var data = Encoding.ASCII.GetBytes(postData.ToString());
                stream.Write(data, 0, data.Length);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return await Call<T>(endpoint, stream, WebRequestMethods.Http.Post, "application/x-www-form-urlencoded");

        }

        private async Task<T> Call<T>(string endpoint, Stream stream, string method = null, string contentType = null)
        {
            return await Call<T>(endpoint,
                wr =>
                {
                    wr.Method = string.IsNullOrWhiteSpace(method) ? WebRequestMethods.Http.Get : method;
                    wr.KeepAlive = true;
                    wr.Credentials = CredentialCache.DefaultCredentials;
                    wr.ContentLength = stream.Length;
                    if (!string.IsNullOrWhiteSpace(contentType))
                        wr.ContentType = contentType;
                    stream.CopyTo(wr.GetRequestStream());
                    stream.Dispose();
                });
        }

        public class ResponseDto<T>
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("data")]
            public T Data { get; set; }
        }

        protected async Task<T> Call<T>(string endpoint, OnWebRequestIsCreated onCreated = null, string method = null)
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