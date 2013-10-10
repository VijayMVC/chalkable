using System.Diagnostics;
using Chalkable.StiConnector.Connectors;
using NUnit.Framework;

namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void TestApi()
        {
            //

            /*string credentials = "chalkable:1234qwer";
            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string credentialsBase64 = Convert.ToBase64String(credentialsBytes);
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Basic " + credentialsBase64;
            client.Encoding = Encoding.UTF8;
            string url = "http://sti-onlinereg-api.sti-k12.com/api/schools";
            var s = client.DownloadString(url);*/

            var cl = new ConnectorLocator("chalkable", "1234qwer", "http://sti-onlinereg-api.sti-k12.com/api/");
            var schools = cl.SchoolConnector.GetSchools();
            foreach (var school in schools)
            {
                Debug.WriteLine(school.Name);
                var sc = cl.SchoolConnector.GetSchoolDetails(school.Id);
                var sessions = cl.AcadSessionConnector.GetSessions(school.Id);
                foreach (var acadSession in sessions)
                {
                    cl.StudentConnector.GetSessionStudents(acadSession.Id);
                }
            }

            //Debug.WriteLine(s);
        }
    }
}
