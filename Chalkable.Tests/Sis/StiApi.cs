using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Transactions;
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
            var cl = ConnectorLocator.Create("Chalkable", "Zs5Qb4Wz8", "http://sandbox.sti-k12.com/Chalkable_Large/api/");
            //var cl = ConnectorLocator.Create("Chalkable", "b1Yn9Rz2X", "http://qa-external.stiinformationnow.com:8220/API/");
            //var cl = ConnectorLocator.Create("Chalkable", "Fp6Gs0Ck7", "http://208.83.95.80:8216/api/");


            var schools = (cl.SyncConnector.GetDiff(typeof(User), null) as SyncResult<User>).All;
            Debug.WriteLine(schools.Count());
        }

        [Test]
        public void TransactionScopeTest()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                var cs = @"Data Source=.\sqlexpress;Initial Catalog=xxx;UID=sa;Pwd=mc_z631";
                using (SqlConnection connection1 = new SqlConnection(cs))
                {
                    connection1.Open();

                    SqlCommand command1 = new SqlCommand("insert into test (id) values (1)", connection1);
                    command1.ExecuteNonQuery();
                }
                using (SqlConnection connection1 = new SqlConnection(cs))
                {
                    connection1.Open();

                    SqlCommand command1 = new SqlCommand("insert into test (id) values (2)", connection1);
                    command1.ExecuteNonQuery();
                }

                scope.Complete();
            }
        }
    }
}
