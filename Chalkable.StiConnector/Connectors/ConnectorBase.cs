using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
            client.Headers[HttpRequestHeader.Authorization] = "Session " + locator.Token;
            client.Headers.Add("ApplicationKey", string.Format("chalkable {0}", ConfigurationManager.AppSettings[STI_APPLICATION_KEY]));
            client.Headers.Add("Content-Type", "application/json");
            client.Encoding = Encoding.UTF8;
            return client;
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
