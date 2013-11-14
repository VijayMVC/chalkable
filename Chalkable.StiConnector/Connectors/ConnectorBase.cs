using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Chalkable.StiConnector.Connectors.Model;
using Newtonsoft.Json;

namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorBase
    {

        private ConnectorLocator locator;
        public ConnectorBase(ConnectorLocator locator)
        {
            this.locator = locator;
        }

        public T Call<T>(string url)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Session " + locator.Token;
            client.Encoding = Encoding.UTF8;
            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            var x = typeof (T);
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(client.DownloadData(url));

                var serializer = new JsonSerializer();
                //serializer.Converters.Add(new JavaScriptDateTimeConverter());
                //serializer.NullValueHandling = NullValueHandling.Ignore;
                var reader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(reader);
                return serializer.Deserialize<T>(jsonReader);
            }
            catch (WebException ex)
            {
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

        public void Post<T>(string url, T obj)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Session " + locator.Token;
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/json");

            Debug.WriteLine(ConnectorLocator.REQ_ON_FORMAT, url);
            var x = typeof(T);
            var stream = new MemoryStream();
            try
            {
                var serializer = new JsonSerializer();
                var writer = new StreamWriter(stream);
                serializer.Serialize(writer, obj);
                client.UploadData(url, stream.ToArray());
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

        protected string BaseUrl
        {
            get { return locator.BaseUrl; }
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

        public SectionAttendance GetSectionAttendance(int acadSessionId, DateTime date, int sectionId)
        {
            return Call<SectionAttendance>(string.Format("{0}Chalkable/{1}/sections/{2}/attendance/{3}", BaseUrl, acadSessionId, sectionId, date.ToString("yyyy-MM-dd")));
        }

        public void SetSectionAttendance(int acadSessionId, DateTime date, int sectionId, SectionAttendance sectionAttendance)
        {
            string url = string.Format("{0}Chalkable/{1}/sections/{2}/attendance/{3}", BaseUrl, acadSessionId, sectionId,
                                       date.ToString("yyyy-MM-dd"));

            Post(url, sectionAttendance);
                        
        }
    }
}
