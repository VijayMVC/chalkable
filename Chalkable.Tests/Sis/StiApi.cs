using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
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
            //var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/");
            //var cl = ConnectorLocator.Create("Chalkable", "tN7nC9sI4", "http://sandbox.sti-k12.com/Chalkable/api/");
            var cl = ConnectorLocator.Create("Chalkable", "2hV4gC3aO", "http://208.83.95.80:8255/api/");
            //var cl = ConnectorLocator.Create("Chalkable", "r3Hp1Dm5Q", "http://208.83.95.80:8222/API/");

            var items = (cl.SyncConnector.GetDiff(typeof(UserSchool), null) as SyncResult<UserSchool>).All.Where(x => x.UserID == 107);
                
            foreach (var s in items)
            {
                Debug.WriteLine(s.UserID + " " + s.SchoolID);
            }


        }

        [Test]
        public void Test3()
        {
            //var hash = UserService.PasswordMd5("Hellowebapps1!");
            //Debug.WriteLine(hash);
        }
    }
}
