using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
using Chalkable.BusinessLogic.Services;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using User = Chalkable.Data.Master.Model.User;

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
            var cl = ConnectorLocator.Create("Chalkable", "r3Hp1Dm5Q", "http://208.83.95.80:8222/API/");
            //var cl = ConnectorLocator.Create("Chalkable", "Zs5Qb4Wz8", "http://sandbox.sti-k12.com/Chalkable_Large/api/");
            //var cl = ConnectorLocator.Create("Chalkable", "b1Yn9Rz2X", "http://qa-external.stiinformationnow.com:8220/API/");
            //var cl = ConnectorLocator.Create("Chalkable", "Fp6Gs0Ck7", "http://208.83.95.80:8216/api/");
        		


            var schools = (cl.SyncConnector.GetDiff(typeof(School), null) as SyncResult<School>).All;
            foreach (var school in schools)
            {
                Debug.WriteLine(school.SchoolID);
            }
        }

        [Test]
        public void TransactionScopeTest()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var id = Guid.Parse("53D7CBD7-CD3E-4A50-9258-8C1F6DAEDB61");
            var now = DateTime.Now;

            IList<User> users = new List<User>();
            for (int i = 0; i < 500000; i++)
            {
                var u = new User
                    {
                        LocalId = i,
                        Login = i.ToString() + "_" + Guid.NewGuid().ToString() + "@chalkable.com",
                        DistrictRef = id,
                        Password = "Qwerty1@",
                        SisUserName = Guid.NewGuid().ToString(),
                        Id = Guid.NewGuid()
                    };
                users.Add(u);
            }
            Debug.WriteLine("Generated");
            sl.UserService.CreateSchoolUsers(users);
            Debug.WriteLine("");
            Debug.WriteLine((DateTime.Now - now).TotalSeconds);
        }
    }
}
