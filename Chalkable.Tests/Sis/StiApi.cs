using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
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
            //var cl = ConnectorLocator.Create("Chalkable", "8Ha8At0Gp", "https://365970.stiinformationnow.com/API/");

            //var items2 = (cl.SyncConnector.GetDiff(typeof(TimeSlot), 26960989) as SyncResult<TimeSlot>);
            //var items = (cl.SyncConnector.GetDiff(typeof(Course), null) as SyncResult<Course>).All.Where(x=>x.CourseID == 1194 || x.CourseID == 3688).ToList();

            //1194


            //3688
            //ScheduledTimeSlot
            IList<District> districts;
            using (var muow =
                new UnitOfWork(
                    "Data Source=yqdubo97gg.database.windows.net;Initial Catalog=ChalkableMaster;UID=chalkableadmin;Pwd=Hellowebapps1!",
                    false))
            {
                var dda = new DistrictDataAccess(muow);
                districts = dda.GetAll();
            }

            foreach (var district in districts)
            {
                var cs =
                    string.Format("Data Source=yqdubo97gg.database.windows.net;Initial Catalog={0};UID=chalkableadmin;Pwd=Hellowebapps1!", district.Name);
                IList<Class> classes;
                using (var uow = new UnitOfWork(cs, false))
                {
                    var da = new ClassDataAccess(uow, null);
                    classes = da.GetAll();
                }

                var cl = ConnectorLocator.Create(district.SisUserName, district.SisPassword, district.SisUrl);
                var items = (cl.SyncConnector.GetDiff(typeof(Course), null) as SyncResult<Course>)
                    .All.ToList();
                if (classes.Count != items.Count)
                    Debug.WriteLine("{0} - {1} {2}", classes.Count, items.Count, district.Id);
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
