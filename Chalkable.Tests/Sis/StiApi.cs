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

            var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/Api/");
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
