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
            var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/");
            //var cl = ConnectorLocator.Create("administrator", "qwertyui1234", "http://208.83.95.80:8210/");
        
            Debug.WriteLine(DateTime.Now.Ticks);
            var r = cl.SyncConnector.GetDiff(typeof(GradingPeriod), null) as SyncResult<GradingPeriod>;
            Debug.WriteLine(DateTime.Now.Ticks);
            foreach (var item in r.All)
            {

                //(id, code, name, description, markingperiodref, schoolyearref, startdate, enddate, endtime, schoolannouncement, allowgradeposting)
                var s = string.Format("({0}, '{1}', '{2}', '{3}', {4}, {5}, '{6}', '{7}', '{8}', '{9}', {10}),", 
                    item.GradingPeriodID, item.Code, item.Name, item.Description, item.TermID, item.AcadSessionID, item.StartDate, item.EndDate, item.EndTime, item.SchoolAnnouncement, (item.AllowGradePosting ? 1 : 0));

                Debug.WriteLine(s);
            }
            Assert.NotNull(r);
        }
    }
}
