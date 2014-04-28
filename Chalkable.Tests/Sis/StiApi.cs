using System;
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
            var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/api/");
            //var cl = ConnectorLocator.Create("administrator", "qwertyui1234", "http://208.83.95.80:8210/");
        
            var r = cl.SyncConnector.GetDiff(typeof(ClassroomOption), null) as SyncResult<ClassroomOption>;
            foreach (var item in r.All)
            {
                Debug.WriteLine(item.SectionID+" " +item.StandardsCalculationMethod);
            }

            var r1 = cl.SyncConnector.GetDiff(typeof(GradingScaleRange), null) as SyncResult<GradingScaleRange>;
            foreach (var item in r1.All)
            {
                Debug.WriteLine(item.GradingScaleID + " " + item.AveragingEquivalent);
            }

            var r2 = cl.SyncConnector.GetDiff(typeof(GradingScale), null) as SyncResult<GradingScale>;
            foreach (var item in r2.All)
            {
                Debug.WriteLine(item.GradingScaleID + " " + item.Name);
            }

            var r3 = cl.SyncConnector.GetDiff(typeof(GradingComment), null) as SyncResult<GradingComment>;
            foreach (var item in r3.All)
            {
                Debug.WriteLine(item.GradingCommentID + " " + item.Code);
            }
            
            
        }
    }
}
