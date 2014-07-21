using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;
using Chalkable.StiConnector.Mapping;
using Newtonsoft.Json;
using System.Configuration;


namespace Chalkable.StiConnector.Connectors
{
    class WebClientGZip : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }

    public class ConnectorBase
    {
        private const string STI_APPLICATION_KEY = "sti.application.key";
        
        private const string ERROR_FORMAT = "Error calling : '{0}' ;\n ErrorMessage : {1}";
        private const string REQUEST_TIME_MSG_FORMAT = "Request on : '{0}' \n Time : {1}";
        protected ConnectorLocator locator;
        private ErrorMapper errorMapper;
        public ConnectorBase(ConnectorLocator locator)
        {
            this.locator = locator;
            this.errorMapper = new ErrorMapper();
        }

        protected WebClient InitWebClient()
        {
            var client = new WebClientGZip();
            InitHeaders(client.Headers);
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/json");
            return client;
        }

        protected void InitHeaders(WebHeaderCollection headers)
        {
            headers[HttpRequestHeader.Authorization] = "Session " + locator.Token;
            headers["ApplicationKey"] = string.Format("chalkable {0}", ConfigurationManager.AppSettings[STI_APPLICATION_KEY]);
            headers["Accept-Encoding"] = "gzip, deflate";
        }

        public T Call<T>(string url, NameValueCollection parameters = null)
        {
            
            var client = InitWebClient();
            MemoryStream stream = null;
            try
            {
                client.QueryString = parameters ?? new NameValueCollection();
                var startTime = DateTime.Now;
                var data = client.DownloadData(url);
                var endTime = DateTime.Now;
                var time = endTime - startTime;
                var timeString = string.Format("{0}:{1}.{2}", time.Minutes, time.Seconds, time.Milliseconds);
                Trace.TraceInformation(REQUEST_TIME_MSG_FORMAT, url, timeString);

                Debug.WriteLine(Encoding.UTF8.GetString(data));
                stream = new MemoryStream(data);
                return JsonConvert.DeserializeObject<T>((new StreamReader(stream)).ReadToEnd());

            }
            catch (WebException ex)
            {
                return HandleInowException<T>(ex);
            }
            finally
            {
                if (stream != null)
                    stream.Dispose();
            }
        }

        public byte[] Download<TPostObj>(string url, TPostObj obj, NameValueCollection optionalParams = null,
                                     HttpMethod httpMethod = null)
        {
            httpMethod = httpMethod ?? HttpMethod.Post;
            var client = InitWebClient();
            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            var stream = new MemoryStream();
            try
            {
                client.QueryString = optionalParams ?? new NameValueCollection();
                var serializer = new JsonSerializer();
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, obj);
                
                writer.Flush();
                
                var startTime = DateTime.Now;
                var res = client.UploadData(url, httpMethod.Method, stream.ToArray());
                var time = DateTime.Now - startTime; 
                var timeString = string.Format("{0}:{1}.{2}", time.Minutes, time.Seconds, time.Milliseconds);
                Trace.TraceInformation(REQUEST_TIME_MSG_FORMAT, url, timeString);

                return res;
            }
            catch (WebException ex)
            {
                return HandleInowException<byte[]>(ex);
            }
            finally
            {
                stream.Dispose();
            }
        }

        public byte[] Download(string url, NameValueCollection optionalParams = null)
        {
            
            var client = InitWebClient();
            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            try
            {
                client.QueryString = optionalParams ?? new NameValueCollection();
                var startTime = DateTime.Now;
                var res = client.DownloadData(url);
                var time = DateTime.Now - startTime;
                var timeString = string.Format("{0}:{1}.{2}", time.Minutes, time.Seconds, time.Milliseconds);
                Trace.TraceInformation(REQUEST_TIME_MSG_FORMAT, url, timeString);

                var type = client.ResponseHeaders[HttpResponseHeader.ContentType];
                Debug.WriteLine(type);
                return res;
            }
            catch (WebException ex)
            {
                return HandleInowException<byte[]>(ex);
            }
        }

        private T HandleInowException<T>(WebException ex)
        {
            var reader = new StreamReader(ex.Response.GetResponseStream());
            var msg = reader.ReadToEnd();
            Trace.TraceError(string.Format(ERROR_FORMAT, ex.Response.ResponseUri, msg));
            if (ex.Response is HttpWebResponse)
            {
                if ((ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                    return default(T);
                else
                {
                    HttpStatusCode status = (ex.Response as HttpWebResponse).StatusCode;
                    if (status == HttpStatusCode.BadRequest)
                    {
                        var inowErrorModel = JsonConvert.DeserializeObject<InowErrorMessageModel>(msg);
                        if (inowErrorModel.ModelStates != null && inowErrorModel.ModelStates.Count > 0)
                        {
                            var chlkMessages = new List<string>();
                            foreach (var modelState in inowErrorModel.ModelStates)
                            {
                                if(string.IsNullOrEmpty(modelState)) continue;
                                var chlkMessage = errorMapper.Map(modelState);
                                chlkMessages.Add(chlkMessage ?? modelState);
                            }
                            throw new ChalkableSisException(chlkMessages);
                        }
                        throw new ChalkableSisException(msg);
                        
                    }
                    throw new HttpException((int)status, ex.Message + Environment.NewLine + msg);
                }
            }
            throw new ChalkableException(msg);
        }

        public TReturn Post<TReturn, TPostObj>(string url, TPostObj obj, NameValueCollection optionalParams = null, HttpMethod httpMethod = null)
        {
            var data = Download(url, obj, optionalParams, httpMethod);
            if (data != null && data.Length > 0)
            {
                using (var stream2 = new MemoryStream(data))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<TReturn>(new JsonTextReader(new StreamReader(stream2)));
                }
            }
            return default(TReturn);
        }

        public T Post<T>(string url, T obj, NameValueCollection optionalParams = null, HttpMethod httpMethod = null)
        {
            return Post<T, T>(url, obj, optionalParams, httpMethod);
        }


        public T PostWithFile<T>(string url, string fileName, byte[] fileContent, NameValueCollection parameters, HttpMethod method = null)
        {
            var headers = InitHeaders();
            var fileType = MimeHelper.GetContentTypeByName(fileName);
            return ChalkableHttpFileLoader.HttpUploadFile(url, fileName, fileContent, fileType
                , ThrowWebException, JsonConvert.DeserializeObject<T>, parameters, headers, method ?? HttpMethod.Post);
        }

        public void Delete(string url) 
        {
            Post<Object,Object>(url, null, null, HttpMethod.Delete);
        }

        public T Put<T>(string url, T obj)
        {
            return Put<T, T>(url, obj);
        }

        public TReturn Put<TReturn, TPutObj>(string url, TPutObj obj)
        {
            return Post<TReturn, TPutObj>(url, obj, null, HttpMethod.Put);
        }

        private static void ThrowWebException(WebException exception)
        {
            var reader = new StreamReader(exception.Response.GetResponseStream());
            var msg = reader.ReadToEnd();
            throw new ChalkableException(msg);
        }
        private IDictionary<string, string> InitHeaders()
        {
            return new Dictionary<string, string>
                {
                    {HttpRequestHeader.Authorization.ToString(), "Session " + locator.Token},
                    {"ApplicationKey", string.Format("chalkable {0}", ConfigurationManager.AppSettings[STI_APPLICATION_KEY])}
                };
        }

        protected string BaseUrl
        {
            get { return locator.BaseUrl; }
        }
    }


    public class InowErrorMessageModel
    {
        public class InowErrorModelState
        {
            [JsonProperty(PropertyName = "")]
            public IList<string> States { get; set; } 
        }
        [JsonProperty(PropertyName = "Message")]
        public string ErrorMessage { get; set; }
        public InowErrorModelState ModelState { get; set; }

        public IList<string> ModelStates
        {
            get { return ModelState != null ? ModelState.States : new List<string>(); }
        }
    }
}
