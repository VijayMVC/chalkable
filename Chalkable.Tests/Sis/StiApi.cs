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
            var cl = ConnectorLocator.Create("Chalkable", "tN7nC9sI4", "http://sandbox.sti-k12.com/Chalkable/api/");
            //var cl = ConnectorLocator.Create("Chalkable", "t7Nn2Jr9F", "http://208.83.95.80:8215/API/");
            
            var sss = cl.SyncConnector.GetDiff(typeof(SectionStaff), null) as SyncResult<SectionStaff>;
            var ss = sss.All.Where(x => x.SectionID == 725);
            foreach (var sectionStaff in ss)
            {
                Debug.WriteLine(sectionStaff.StaffID + " " + sectionStaff.IsPrimary);
            }
        }
    }
}
