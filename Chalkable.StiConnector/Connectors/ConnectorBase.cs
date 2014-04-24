using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Chalkable.Common;
using Chalkable.Common.Web;
using Chalkable.StiConnector.Connectors.Model;
using Newtonsoft.Json;
using System.Configuration;


namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorBase
    {
        private const string STI_APPLICATION_KEY = "sti.application.key";
        
        protected ConnectorLocator locator;
        public ConnectorBase(ConnectorLocator locator)
        {
            this.locator = locator;
        }

        protected WebClient InitWebClient()
        {
            var client = new WebClient();
            InitHeaders(client.Headers);
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/json");
            return client;
        }

        protected void InitHeaders(WebHeaderCollection headers)
        {
            headers[HttpRequestHeader.Authorization] = "Session " + locator.Token;
            headers["ApplicationKey"] = string.Format("chalkable {0}", ConfigurationManager.AppSettings[STI_APPLICATION_KEY]);
        }

        public T Call<T>(string url, NameValueCollection parameters = null)
        {

            var client = InitWebClient();
            MemoryStream stream = null;
            try
            {
                client.QueryString = parameters ?? new NameValueCollection();
                var data = client.DownloadData(url);
                Debug.WriteLine(Encoding.UTF8.GetString(data));
                stream = new MemoryStream(data);

                var serializer = new JsonSerializer();
                var reader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(reader);
                return serializer.Deserialize<T>(jsonReader);
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse &&
                    (ex.Response as HttpWebResponse).StatusCode == HttpStatusCode.NotFound)
                    return default(T);
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
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
                return client.UploadData(url, httpMethod.Method, stream.ToArray());
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
            finally
            {
                stream.Dispose();
            }
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
                , ThrowWebException, Derialize<T>, parameters, headers, method ?? HttpMethod.Post);
        }

        public void Delete(string url) 
        {
            Post<Object,Object>(url, null, null, HttpMethod.Delete);
        }
        public T Put<T>(string url, T obj)
        {
            return Post<T,T>(url, obj, null, HttpMethod.Put);
        }

        private static void ThrowWebException(WebException exception)
        {
            var reader = new StreamReader(exception.Response.GetResponseStream());
            var msg = reader.ReadToEnd();
            throw new Exception(msg);
        }
        private static T Derialize<T>(string response)
        {
            return (new JsonSerializer()).Deserialize<T>(new JsonTextReader(new StringReader(response)));
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

    public class UsersConnector: ConnectorBase
    {
        public UsersConnector(ConnectorLocator locator) : base(locator)
        {
        }
        public User GetMe()
        {
            var url = string.Format("{0}chalkable/{1}/me", BaseUrl, "users"); //"http://localhost/Api/chalkable/users/me"; //
            return Call<User>(url); 
        }

        public byte[] GetPhoto(int personId)
        {
            var url = string.Format("{0}person/{1}/photo", BaseUrl, personId); //"http://localhost/Api/chalkable/users/me"; //
            return Call<byte[]>(url); 
        }
    }

    public class AttendanceConnector : ConnectorBase
    {
        public AttendanceConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int sectionId)
        {
            return Call<SectionAttendance>(string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString(Constants.DATE_FORMAT)));
        }

        public void SetSectionAttendance(int acadSessionId, DateTime date, int sectionId, SectionAttendance sectionAttendance)
        {
            string url = string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString(Constants.DATE_FORMAT));
            Post(url, sectionAttendance);
                        
        }
    }
}
