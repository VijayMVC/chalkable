using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using ScheduledTimeSlot = Chalkable.StiConnector.SyncModel.ScheduledTimeSlot;

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
            var cl = ConnectorLocator.Create("Chalkable", "qP8vI4rL2", "https://inowhome.madison.k12.al.us/api/");

            var items2 = (cl.SyncConnector.GetDiff(typeof(TimeSlot), 26960989) as SyncResult<TimeSlot>);
            var items = (cl.SyncConnector.GetDiff(typeof(ScheduledSection), 26960989) as SyncResult<ScheduledSection>);
            //26960989
            //26960987
            //ScheduledTimeSlot
            foreach (var ts in items2.Deleted)
            {
                Debug.WriteLine(ts.TimeSlotID);
            }
            Debug.WriteLine(items.Deleted != null ? "deleted" : "not deleted");

        }

        [Test]
        public void Test3()
        {
            var hash = UserService.PasswordMd5("tester");
            Debug.WriteLine(hash);
        }
    }
}
