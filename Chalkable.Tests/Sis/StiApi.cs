using System;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;

namespace Chalkable.Tests.Sis
{
    public class StiApi : TestBase
    {
        //[Test]
        //public void ReportTest()
        //{
        //    var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/");
        //    var obj = new ReportConnector.ProgressReportParams
        //    {
        //        AcadSessionId = 18,
        //        GradingPeriodId = 31,
        //        IdToPrint = 1,
        //        SectionId = 576,
        //        StudentIds = new[] { 126 }
        //    };
        //    var r = cl.ReportConnector.ProgressReport(obj);
        //    Assert.NotNull(r);
        //}

        [Test]
        public void SyncTest()
        {
            var cl = ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/Chalkable/api/");

            var items = (cl.SyncConnector.GetDiff(typeof(ScheduledTimeSlotVariation), null) as SyncResult<ScheduledTimeSlotVariation>).All.ToList();
            items = items.Where(x => x.BellScheduleId == 156 && x.TimeSlotId == 565).ToList();
            foreach (var item in items)
            {
                Debug.WriteLine(item.Name);
            }

        }

        [Test]
        public void Test3()
        {
            var hash = UserService.PasswordMd5("tester");
            Debug.WriteLine(hash);
        }
    }
}
