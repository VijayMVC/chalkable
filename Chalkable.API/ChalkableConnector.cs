using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
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
        public StudyCenterEndpoint StudyCenter => new StudyCenterEndpoint(this);
        public GradingEndpoint Grading => new GradingEndpoint(this);
        public StandardsEndpoint Standards => new StandardsEndpoint(this);
        public CalendarEndpoint Calendar => new CalendarEndpoint(this);
        public AttendanceEndpoint Attendance => new AttendanceEndpoint(this);

        public async Task<T> Get<T>(string endpoint)
        {
            try
            {
                return await Call<T>(endpoint);
            }
            catch (Exception e)
            {
                throw new ChalkableApiException($"Chalkable API GET {endpoint} failed", e);
            }
        }

        public async Task<T> Put<T>(string endpoint, Stream stream)
        {
            try
            {
                return await Call<T>(endpoint, stream, WebRequestMethods.Http.Put);
            }
            catch (Exception e)
            {
                throw new ChalkableApiException($"Chalkable API PUT {endpoint} failed", e);
            }            
        }

        public async Task<T> Post<T>(string endpoint, NameValueCollection postData)
        {
            try
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
            catch (Exception e)
            {
                throw new ChalkableApiException($"Chalkable API POST {endpoint} failed", e);
            }
        }

        protected async Task<T> Call<T>(string endpoint, Stream stream, string method = null, string contentType = null)
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

        public class ResponseDataDto<T>
        {
            [JsonProperty("data")]
            public T Data { get; set; }
        }

        public class ResponseSuccessDto
        {
            [JsonProperty("success")]
            public bool Success { get; set; }
            [JsonProperty("data")]
            public object Data { get; set; }
        }

        protected async Task<T> Call<T>(string endpoint, OnWebRequestIsCreated onCreated = null, string method = null)
        {
            var url = ApiRoot + endpoint;

            try
            {
                Debug.WriteLine("Request on: " + url);
                Debug.WriteLine("Request on: " + url);
                var webRequest = (HttpWebRequest) WebRequest.Create(url);
                webRequest.Method = string.IsNullOrWhiteSpace(method) ? WebRequestMethods.Http.Get : method;
                webRequest.Accept = "application/json";
                OauthClient?.AppendAccessTokenTo(webRequest);
                onCreated?.Invoke(webRequest);
                var response = await webRequest.GetResponseAsync();
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                        throw new ChalkableApiException("Error");

                    using (var sr = new StreamReader(stream)) {
                        var str = sr.ReadToEnd();
                        Debug.WriteLine(str);
                        var statusCode = (response as HttpWebResponse)?.StatusCode;
                        if (str.TrimStart().StartsWith("<") ||
                            (statusCode != null && statusCode.Value != HttpStatusCode.OK))
                        {
                            throw new ChalkableApiException("Server failed to respond with JSON: " +
                                                            $"Status: {statusCode}, " +
                                                            $"Content-Type: {response.ContentType}, " +
                                                            $"Body: {str.Substring(0, Math.Min(str.Length, 1024))}");
                        }

                        var status = JsonConvert.DeserializeObject<ResponseSuccessDto>(str) ;
                        if (!status.Success)
                            throw new ChalkableApiException(JsonConvert.SerializeObject(status.Data));

                        var data = JsonConvert.DeserializeObject<ResponseDataDto<T>>(str);

                        return data.Data;
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var strRe = new StreamReader(e.Response.GetResponseStream());
                    var rsp = strRe.ReadToEnd();
                    throw new ChalkableApiException("Call to remote server failed: " +
                                                    $"Status: {e.Status}" +
                                                    $"Message: {e.Message}," +
                                                    $"Body: {rsp}", e);
                }

                throw;
            }
            //throw new ChalkableApiException("oauth client isn't initialized");
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