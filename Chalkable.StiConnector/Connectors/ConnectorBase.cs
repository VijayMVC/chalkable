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

        //private string mockResp = "{\"Date\":\"2013-11-18T00:00:00\",\"IsDailyAttendancePeriod\":true,\"IsPosted\":true,\"MergeRosters\":true,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"SectionId\":635,\"StudentAttendance\":[{\"AbsentPreviousDay\":false,\"Category\":\"Excused\",\"ClassroomLevel\":\"Absent\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":\"A\",\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":1,\"SectionId\":635,\"StudentId\":19},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":21},{\"AbsentPreviousDay\":true,\"Category\":\"Excused\",\"ClassroomLevel\":\"Absent\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":\"A\",\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":1,\"SectionId\":635,\"StudentId\":23},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":25},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":29},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":30},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":977},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1116},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1147},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1158},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1163},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1171},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1181},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1505},{\"AbsentPreviousDay\":true,\"Category\":\"Unexcused\",\"ClassroomLevel\":\"Tardy\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":\"T\",\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":10,\"SectionId\":635,\"StudentId\":1839},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1924},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":1951},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2152},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2285},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2293},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2498},{\"AbsentPreviousDay\":true,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2569},{\"AbsentPreviousDay\":false,\"Category\":null,\"ClassroomLevel\":\"Present\",\"Date\":\"2013-11-18T00:00:00\",\"Level\":null,\"ReadOnly\":false,\"ReadOnlyReason\":null,\"ReasonId\":null,\"SectionId\":635,\"StudentId\":2623}]}";

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
                //Thread.Sleep(500);
                //var data = Encoding.UTF8.GetBytes(mockResp);
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

    public class ActivityConnector : ConnectorBase
    {
        private string urlFormat;
        public ActivityConnector(ConnectorLocator locator) : base(locator)
        {
            urlFormat = BaseUrl + "Chalkable/sections/{0}/activities"; //"http://localhost/Api/chalkable/sections/{0}/activities"; //
        }

        public Activity GetActivity(int sectionId, int id)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            return Call<Activity>(url);
        }

        public IList<Activity> GetActivities(int sectionId, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(urlFormat, sectionId);
            var optionalParams = new NameValueCollection();
            if(endDate.HasValue)
                optionalParams.Add("endDate", endDate.Value.ToString());
            if(startDate.HasValue)
                optionalParams.Add("startDate", startDate.Value.ToString());
            return  Call<IList<Activity>>(url, optionalParams);
        } 

        public IList<Activity> GetTeacherActivities(int acadSessionId, int teacherId, int? start = null, int? end = null, DateTime? endDate = null, DateTime? startDate = null)
        {
            string url = string.Format(BaseUrl + "Chalkable/{0}/teachers/{1}/activities", acadSessionId, teacherId);
            var optinalParams = new NameValueCollection();
            if(start.HasValue)
                optinalParams.Add("start", start.Value.ToString());
            if(end.HasValue)
                optinalParams.Add("end", end.Value.ToString());
            if(startDate.HasValue)
                optinalParams.Add("startDate", startDate.Value.ToString());
            if(endDate.HasValue)
                optinalParams.Add("endDate", endDate.Value.ToString());
            return Call<IList<Activity>>(url, optinalParams);
        }

        public void DeleteActivity(int sectionId, int id)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            Post<Activity>(url, null, DELETE);
            
        }
        public Activity CreateActivity(int sectionId, Activity activity)
        {
            string url = string.Format(urlFormat, sectionId);
            return Post(url, activity);
        }
        public void UpdateActivity(int sectionId, int id, Activity activity)
        {
            string url = string.Format(urlFormat + "/{1}", sectionId, id);
            Post(url, activity, PUT);
        }
    }
}
