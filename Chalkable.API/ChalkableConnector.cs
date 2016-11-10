using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            return await Call<T>(endpoint, method,
                onCreated: wr =>
                {
                    wr.KeepAlive = true;
                    wr.Credentials = CredentialCache.DefaultCredentials;
                    wr.ContentLength = stream.Length;
                    if (!string.IsNullOrWhiteSpace(contentType))
                        wr.ContentType = contentType;
                }, 
                onBeforeSend: wr =>
                {
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

        public class ErrorInfo
        {
            [JsonProperty("message")]
            public string Message { get; set; }
            [JsonProperty("exceptiontype")]
            public string ExceptionType { get; set; }
        }

        protected async Task<T> Call<T>(string endpoint, string method = null, OnWebRequestIsCreated onCreated = null, OnWebRequestIsSent onBeforeSend = null)
        {
            var url = ApiRoot + endpoint;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine("Request on: " + url);
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                webRequest.Method = string.IsNullOrWhiteSpace(method) ? WebRequestMethods.Http.Get : method;
                webRequest.Accept = "application/json";
                webRequest.AllowAutoRedirect = false;

                onCreated?.Invoke(webRequest);

                Authorization?.SignRequest(webRequest);

                onBeforeSend?.Invoke(webRequest);

                var response = await webRequest.GetResponseAsync();
                using (var stream = response.GetResponseStream())
                {
                    var statusCode = (response as HttpWebResponse)?.StatusCode;
                    if (stream == null)
                        throw new ChalkableApiException("Server failed to respond: " +
                                                        $"Status: {statusCode}, " +
                                                        $"Content-Type: {response.ContentType}, " +
                                                        $"Timeout: {webRequest.Timeout}/{webRequest.ReadWriteTimeout}/{webRequest.ContinueTimeout}, " +
                                                        $"Elasped: {stopwatch.ElapsedMilliseconds}, " +
                                                        "Response: null");

                    using (var sr = new StreamReader(stream))
                    {
                        var str = sr.ReadToEnd();
                        if (str.TrimStart().StartsWith("<") ||
                            (statusCode != null && statusCode.Value != HttpStatusCode.OK))
                        {
                            throw new ChalkableApiException("Server failed to respond with JSON: " +
                                                            $"Status: {statusCode}, " +
                                                            $"Content-Type: {response.ContentType}, " +
                                                            $"Timeout: {webRequest.Timeout}/{webRequest.ReadWriteTimeout}/{webRequest.ContinueTimeout}, " +
                                                            $"Elasped: {stopwatch.ElapsedMilliseconds}, " +
                                                            $"Response: {str.Substring(0, Math.Min(str.Length, 512))}",
                                body: str);
                        }

                        var status = JsonConvert.DeserializeObject<ResponseSuccessDto>(str);
                        if (!status.Success)
                        {
                            var details = JsonConvert.DeserializeObject<ResponseDataDto<ErrorInfo>>(str).Data;
                            throw new ChalkableApiException($"{details.ExceptionType}: {details.Message}", body: str);
                        }

                        return JsonConvert.DeserializeObject<ResponseDataDto<T>>(str).Data;
                    }
                }
            }
            catch (ChalkableApiException)
            {
                throw; // expected to go up
            }
            catch (WebException e)
            {
                var stream = e.Response?.GetResponseStream();
                var rsp = stream != null ? new StreamReader(stream).ReadToEnd() : null;

                throw new ChalkableApiException("Call to remote server failed: " +
                                                $"Status: {e.Status}, " +
                                                $"Message: {e.Message}, " +
                                                $"Timeout: {webRequest.Timeout}/{webRequest.ReadWriteTimeout}/{webRequest.ContinueTimeout}, " +
                                                $"Elasped: {stopwatch.ElapsedMilliseconds}, " +
                                                $"Response: {rsp?.Substring(0, Math.Min(rsp.Length, 512)) ?? "null"}", e, body: rsp);
            }
            catch (Exception e)
            {
                throw new ChalkableApiException("Call to remote server failed: " +
                                                $"Exception: {e.GetType().FullName}, " +
                                                $"Message: {e.Message}," +
                                                $"Elasped: {stopwatch.ElapsedMilliseconds}, " +
                                                "Response: null", e);
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private ChalkableAuthorization Authorization { get; set; }
        private string ApiRoot => Authorization?.ApiRoot;

        public ChalkableConnector(ChalkableAuthorization authorization)
        {
            Authorization = authorization;
        }
    }
}