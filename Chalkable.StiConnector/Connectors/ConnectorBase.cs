using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.StiConnector.Connectors
{
    public class ConnectorBase
    {
        private const string REQ_ON_FORMAT = "Request on: {0}";
        private string userName;
        private string password;
        protected string BaseUrl { get; private set; }

        public ConnectorBase(string userName, string password, string baseUrl)
        {
            this.userName = userName;
            this.password = password;
            BaseUrl = baseUrl;
        }

        public T Call<T>(string url)
        {
            string credentials = string.Format("{0}:{1}", userName, password);
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string credentialsBase64 = Convert.ToBase64String(credentialsBytes);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentialsBase64;
            client.Encoding = Encoding.UTF8;
            Debug.WriteLine(REQ_ON_FORMAT, url);
            var x = typeof (T);
            var ser = new DataContractJsonSerializer(x);

            using (var stream = new MemoryStream(client.DownloadData(url)))
            {
                return (T) ser.ReadObject(stream);
            }
        }
    }

    public class SchoolConnector : ConnectorBase
    {
        public SchoolConnector(string userName, string password, string baseUrl) : base(userName, password, baseUrl)
        {
        }

        public List<School> GetSchools()
        {
            return Call<School[]>(BaseUrl + "schools").ToList();
        }

        public School GetSchoolDetails(int id)
        {
            return Call<School>(BaseUrl + "schools/" + id);
        }
    }

    public class AcadSessionConnector : ConnectorBase
    {
        public AcadSessionConnector(string userName, string password, string baseUrl)
            : base(userName, password, baseUrl)
        {
        }

        public IList<AcadSession> GetSessions(int schoolId)
        {
            return Call<AcadSession[]>(string.Format("{0}{1}/{2}/acadSessions", BaseUrl, "schools", schoolId)).ToList();
        }
    }

    public class StudentConnector : ConnectorBase
    {
        public StudentConnector(string userName, string password, string baseUrl) : base(userName, password, baseUrl)
        {
        }

        public IList<AcadSessionStudent> GetSessionStudents(int sessionId)
        {
            return Call<AcadSessionStudent[]>(string.Format("{0}{1}/students", BaseUrl, sessionId)).ToList();
        }
    }

    public class GradeLevelConnector : ConnectorBase
    {
        public GradeLevelConnector(string userName, string password, string baseUrl) : base(userName, password, baseUrl)
        {
        }

        public IList<GradeLevel> GetGradeLevels()
        {
            return Call<GradeLevel[]>(BaseUrl + "gradeLevels").ToList();
        }
    }

    public class GenderConnector : ConnectorBase
    {
        public GenderConnector(string userName, string password, string baseUrl) : base(userName, password, baseUrl)
        {
        }

        public IList<Gender> GetGenders()
        {
            return Call<Gender[]>(BaseUrl + "genders").ToList();
        }
    }

    public class ContactConnector : ConnectorBase
    {
        public ContactConnector(string userName, string password, string baseUrl) : base(userName, password, baseUrl)
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
}
