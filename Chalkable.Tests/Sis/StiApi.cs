﻿using System;
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
            var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/");
            //var cl = ConnectorLocator.Create("administrator", "qwertyui1234", "http://208.83.95.80:8210/");
        
            Debug.WriteLine(DateTime.Now.Ticks);
            //var r = cl.SyncConnector.GetDiff(typeof(School), null) as SyncResult<School>;
            Debug.WriteLine(DateTime.Now.Ticks);
            cl.UsersConnector.GetPhoto(19);
        }
    }
}
