using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;

namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        [Test]
        public void ReportTest()
        {
            var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/");
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
            //var cl = ConnectorLocator.Create("Chalkable", "tN7nC9sI4", "http://sandbox.sti-k12.com/Chalkable/api/");
            var cl = ConnectorLocator.Create("Chalkable", "gT9qF5yV8", "http://208.83.95.80:8210/API/");
            //69E2AA38-EC70-45E8-AA99-4406CF6D4BA8	69e2aa38-ec70-45e8-aa99-4406cf6d4ba8			Chalkable	gT9qF5yV8	0	Central Standard Time	v8xjnk8au1.database.windows.net	69E2AA38-EC70-45E8-AA99-4406CF6D4BA8	http://208.83.95.80:8210/InformationNow/	NULL

            var r = cl.SyncConnector.GetDiff(typeof(Person), null) as SyncResult<Person>;
            var s = r.All.Where(x => x.PhotoModifiedDate.HasValue).First();
            Debug.WriteLine(s.PersonID);
            var photo = cl.UsersConnector.GetPhoto(s.PersonID);
            Assert.Null(photo);
        }
    }
}
