using System.Diagnostics;
using System.IO;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
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

        [Test]
        public void ReportTest()
        {
            var cl = ConnectorLocator.Create("administrator", "qwertyui1234", "http://sandbox.sti-k12.com/chalkable/");
            var obj = new ReportConnector.ProgressReportParams
            {
                AcadSessionId = 18,
                GradingPeriodId = 31,
                IdToPrint = 1,
                SectionId = 576,
                StudentIds = new[] { 126 }
            };
            var r = cl.ReportConnector.ProgressReport(obj);
            Assert.NotNull(r);
        }

        [Test]
        public void SyncTest()
        {
            //var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/");
            var cl = ConnectorLocator.Create("administrator", "qwertyui1234", "http://sandbox.sti-k12.com/chalkable/");
            var r = cl.SyncConnector.GetDiff<School>("school", null);
            Assert.NotNull(r);
        }
    }
}
