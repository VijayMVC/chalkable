using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Chalkable.Common.Web;
using Chalkable.StiConnector.Connectors.Model;
using Newtonsoft.Json;
using System.Configuration;

namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorBase
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";

        private const string STI_APPLICATION_KEY = "sti.application.key";


        private ConnectorLocator locator;
        public ConnectorBase(ConnectorLocator locator)
        {
            this.locator = locator;
        }

        private WebClient InitWebClient()
        {
            var client = new WebClient();
            InitHeaders(client.Headers);
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/json");
            return client;
        }

        private void InitHeaders(WebHeaderCollection headers)
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

        public T Post<T>(string url, T obj, string method = POST)
        {
            var client = InitWebClient();           
            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            var stream = new MemoryStream();
            MemoryStream stream2 = null;
            try
            {
                var serializer = new JsonSerializer();
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, obj);
                writer.Flush();
                var data = client.UploadData(url, method, stream.ToArray());
                if (data != null && data.Length > 0)
                {
                    stream2 = new MemoryStream(data);
                    return serializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream2)));
                }
                return default(T);
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
                if(stream2 != null)
                    stream2.Dispose();
            }
        }

        //public T UploadFile<T>(string url, byte[] fileContent, string method = POST)
        //{
        //    var client = InitWebClient();
        //    Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
        //    var stream = new MemoryStream();
        //    try
        //    {
        //        client.UploadFileTaskAsync( new Uri(url), method,)
        //    }
        //    catch (Exception)
        //    {
                
        //        throw;
        //    }
        //}


        protected HttpWebRequest InitWebRequest(string url, string fileName, byte[] fileContent, string method = POST)
        {
            string boundary = "----" + DateTime.UtcNow.Ticks.ToString("x");
            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url); //sVal is id for the webService
            InitHeaders(wr.Headers);
            wr.KeepAlive = true;
            wr.Method = method;
            wr.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            wr.Credentials = CredentialCache.DefaultCredentials;
            Stream rs = wr.GetRequestStream();

            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, "file", fileName, MimeHelper.GetContentTypeByName(fileName));
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            using (var fileStream = new MemoryStream(fileContent))
            {
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
            }
            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();
            return wr;
        }

        public T PostWithFile<T>(string url, string fileName, byte[] fileContent, string method = POST) 
        {
            try
            {
                var wr = InitWebRequest(url, fileName, fileContent, method);
                WebResponse wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                if (stream2 != null)
                {
                    var reader2 = new StreamReader(stream2);
                    string resp = reader2.ReadToEnd();
                    if(!string.IsNullOrEmpty(resp))
                        return new JsonSerializer().Deserialize<T>(new JsonTextReader(new StringReader(resp)));
                }
                return default(T);
            }
            catch (WebException ex)
            {
                var reader = new StreamReader(ex.Response.GetResponseStream());
                var msg = reader.ReadToEnd();
                throw new Exception(msg);
            }
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
    }

    public class AcadSessionConnector : ConnectorBase
    {
        public AcadSessionConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<AcadSession> GetSessions(int schoolId)
        {
            return Call<AcadSession[]>(string.Format("{0}{1}/{2}/acadSessions", BaseUrl, "schools", schoolId)).ToList();
        }
    }

    public class StudentConnector : ConnectorBase
    {
        public StudentConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<AcadSessionStudent> GetSessionStudents(int sessionId)
        {
            return Call<AcadSessionStudent[]>(string.Format("{0}{1}/students", BaseUrl, sessionId)).ToList();
        }
    }

    public class GradeLevelConnector : ConnectorBase
    {
        public GradeLevelConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return Call<GradeLevel[]>(BaseUrl + "gradeLevels").ToList();
        }
    }

    public class GenderConnector : ConnectorBase
    {
        public GenderConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<Gender> GetGenders()
        {
            return Call<Gender[]>(BaseUrl + "genders").ToList();
        }
    }

    public class ContactConnector : ConnectorBase
    {
        public ContactConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public IList<PersonTelephone> GetPhones(int personId)
        {
            return Call<PersonTelephone[]>(string.Format("{0}persons/{1}/telephoneNumbers", BaseUrl, personId)).ToList();
        }

        public IList<PersonAddresses> GetAddresses(int personId)
        {
            return Call<PersonAddresses[]>(string.Format("{0}persons/{1}/addresses", BaseUrl, personId)).ToList();
        }
    }

    public class AttendanceConnector : ConnectorBase
    {
        public AttendanceConnector(ConnectorLocator locator) : base(locator)
        {
        }

        public SectionAttendance GetSectionAttendance(DateTime date, int sectionId)
        {
            return Call<SectionAttendance>(string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString("yyyy-MM-dd")));
        }

        public void SetSectionAttendance(int acadSessionId, DateTime date, int sectionId, SectionAttendance sectionAttendance)
        {
            string url = string.Format("{0}Chalkable/sections/{1}/attendance/{2}", BaseUrl, sectionId, date.ToString("yyyy-MM-dd"));
            Post(url, sectionAttendance);
                        
        }
    }
}
