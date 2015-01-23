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
            var cl = ConnectorLocator.Create("Chalkable", "n8Ad8Rg7Y", "http://208.83.95.80:8285/API/");

            var items = (cl.SyncConnector.GetDiff(typeof(CalendarDay), null) as SyncResult<CalendarDay>).All;

            var ds = items.Where(x => x.AcadSessionID == 176 && x.Date == new DateTime(2015, 1, 22)).ToList();
            foreach (var calendarDay in ds)
            {
                Debug.WriteLine(calendarDay.BellScheduleID);
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
