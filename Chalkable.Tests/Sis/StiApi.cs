using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Common;
using Chalkable.Data.Master.DataAccess;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.SyncModel;
using NUnit.Framework;
using AttendanceMonth = Chalkable.StiConnector.SyncModel.AttendanceMonth;
using ContactRelationship = Chalkable.StiConnector.SyncModel.ContactRelationship;
using GradedItem = Chalkable.StiConnector.SyncModel.GradedItem;
using StudentContact = Chalkable.StiConnector.SyncModel.StudentContact;
using StudentSchool = Chalkable.StiConnector.SyncModel.StudentSchool;


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
        
        //[Test]
        //public void test4()
        //{
        //    var name = "CCSS.Math.Content.K.NBT";
        //    var world = "CCSS".ToLower();
        //    //var b = 
        //}

        [Test]
        public void SyncTest2()
        {
            //var cl = ConnectorLocator.Create("Chalkable", "Bg5Zd7Ke3", "https://sis-sumner.tnk12.gov/api/");

            var cl = ConnectorLocator.Create("Chalkable", "h1Bx4Xr5S", "https://inow.mtnbrook.k12.al.us/Api/");


            //var version1 = 62883812;
            var version1 = 6980584;
            var disctrictId = "F76407F1-5AD1-4B92-BE5F-659DC3E15BF1(2)";

            var items3 = (cl.SyncConnector.GetDiff(typeof(StudentScheduleTerm), version1 - 1) as SyncResult<StudentScheduleTerm>);
            var items1 = (cl.SyncConnector.GetDiff(typeof(StudentScheduleTerm), version1) as SyncResult<StudentScheduleTerm>);
            
            var items2 =
                items1.Inserted.Where(
                    x =>
                    items3.All.Any(y => y.StudentID == x.StudentID && y.SectionID == x.SectionID && y.TermID == x.TermID))
                      .ToList();

            Debug.WriteLine(items2.Count);

            
            var fileS = new FileStream(string.Format(@"E:\Private\New folder (2)\{0}_delete.txt", disctrictId), 
                                        FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            var fileS2 = new FileStream(string.Format(@"E:\Private\New folder (2)\{0}_insert.txt", disctrictId), 
                                        FileMode.OpenOrCreate, FileAccess.ReadWrite,
                           FileShare.ReadWrite);
            using (var outfile = new StreamWriter(fileS))
            {
                foreach (var item in items2)
                {
                    var s = string.Format("delete from ClassPerson where PersonRef = {0} and ClassRef = {1} and MarkingPeriodRef = {2}"
                        , item.StudentID, item.SectionID, item.TermID);
                    outfile.WriteLine(s);
                    outfile.WriteLine("Go");
                    Debug.WriteLine(s);
                }
            }
            using (var outfile = new StreamWriter(fileS2))
            {
                outfile.WriteLine("insert into ClassPerson(PersonRef, ClassRef, MarkingPeriodRef, [IsEnrolled])");
                outfile.WriteLine("values ");
                foreach (var item in items2)
                {
                    outfile.WriteLine("({0}, {1}, {2}, {3}),", item.StudentID, item.SectionID, item.TermID, item.IsEnrolled ? 1 : 0);
                }
            }
        }

        [Test]
        public void Test4()
        {
            var cl = ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/Chalkable/api/");
           // var cl = ConnectorLocator.Create("Chalkable", "h1Bx4Xr5S", "https://inow.mtnbrook.k12.al.us/Api/");
            int? version1 = null;
            var items1 = (cl.SyncConnector.GetDiff(typeof(StudentContact), null) as SyncResult<StudentSchool>);
         //   var items3 = (cl.SyncConnector.GetDiff(typeof(ContactRelationship), version1) as SyncResult<ContactRelationship>);
            var items2 = (cl.SyncConnector.GetDiff(typeof(GradedItem), version1) as SyncResult<GradedItem>);

            //foreach (var item in items1.All)
            //{
            //    Debug.WriteLine("({0},{1},{2},{3},{4}, {5}, {6}, {7})",item.AttendanceMonthID.ToString(), item.AcadSessionID.ToString(), item.Name, item.StartDate.Date, item.EndDate.Date, item.EndTime
            //        , item.IsLockedAttendance, item.IsLockedDiscipline);
            //}
        }


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
                    var da = new ClassDataAccess(uow);
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
        public void TestNewApi()
        {
            var cl = ConnectorLocator.Create("Chalkable", "8nA4qU4yG", "http://sandbox.sti-k12.com/Chalkable/api/");
            //var sections = new List<int> {13770, 13771, 13772, 13806, 13861, 13862, 13950, 14011, 14436};
            //var res = cl.SectionCommentConnector.GetComments(179, 8502);
            var res = cl.SectionCommentConnector.GetCommentById(179, 8502, 13);
            if(res == null)
                Debug.WriteLine("not found");
            else Debug.WriteLine("found");
            //var cl = ConnectorLocator.Create("Chalkable", "h1Bx4Xr5S", "https://inow.mtnbrook.k12.al.us/Api/");
            int? version1 = null;
            //var items1 = (cl.SyncConnector.GetDiff(typeof(StudentContact), version1) as SyncResult<StudentSchool>);
            //var items3 = (cl.SyncConnector.GetDiff(typeof(ContactRelationship), version1) as SyncResult<ContactRelationship>);
            // var items2 = (cl.SyncConnector.GetDiff(typeof(ActivityAttribute), version1) as SyncResult<ActivityAttribute>);

            //foreach (var item in items1.All)
            //{
            //    Debug.WriteLine("({0},{1},{2},{3},{4}, {5}, {6}, {7})",item.AttendanceMonthID.ToString(), item.AcadSessionID.ToString(), item.Name, item.StartDate.Date, item.EndDate.Date, item.EndTime
            //        , item.IsLockedAttendance, item.IsLockedDiscipline);
            //}
        }


        [Test]
        public void Test3()
        {
            Debug.WriteLine(DateTime.Now.Month);
        }

    }
}
