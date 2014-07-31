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
            //var cl = ConnectorLocator.Create("Chalkable", "Zs5Qb4Wz8", "http://sandbox.sti-k12.com/Chalkable_Large/api/");
            var cl = ConnectorLocator.Create("Chalkable", "r3Hp1Dm5Q", "http://208.83.95.80:8222/API/");
            //var cl = ConnectorLocator.Create("Chalkable", "q1Jn6Oq6N", "http://208.83.95.80/API/");


            var acadSessions = (cl.SyncConnector.GetDiff(typeof(StudentAcadSession), null) as SyncResult<StudentAcadSession>).All
                .Where(x => x.StudentID == 3705);
            foreach (var s in acadSessions)
            {
                //IsChalkableEnabled
                Debug.WriteLine(s.CurrentEnrollmentStatus);
            }

            var ssT = (cl.SyncConnector.GetDiff(typeof(StudentScheduleTerm), null) as SyncResult<StudentScheduleTerm>).All
                .Where(x => x.StudentID == 3705 && x.SectionID == 2100);
            foreach (var s in ssT)
            {
                Debug.WriteLine(s.TermID + " " +  s.IsEnrolled);
            }
        }

        [Test]
        public void Test2()
        {
            //var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/");
            //var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/api/");
            var cl = ConnectorLocator.Create("administrator", "qqqq1111", "http://208.83.95.80:8215/API/");

            Debug.WriteLine(DateTime.Now.Ticks);
            var r = cl.SyncConnector.GetDiff(typeof(Term), null) as SyncResult<Term>;
            Debug.WriteLine(DateTime.Now.Ticks);
            var terms = r.All.Where(x => x.TermID == 54).ToList();
            foreach (var term in terms)
            {
                Debug.WriteLine("term = ({0}, {1}),", term.Name, term.AcadSessionID);
                
                var r2 = cl.SyncConnector.GetDiff(typeof(AcadSession), null) as SyncResult<AcadSession>;
                var acads = r2.All.Where(x => x.AcadSessionID == term.AcadSessionID).ToList();
                foreach (var acadSession in acads)
                {
                    Debug.WriteLine("acad = ({0}, {1}),", acadSession.AcadSessionID, acadSession.SchoolID);
                }
            }

        }

        [Test]
        public void AlternateScoreTest()
        {
            //var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/");
            //var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/api/");
            var cl = ConnectorLocator.Create("administrator", "qqqq1111", "http://208.83.95.80:8215/API/");

            Debug.WriteLine(DateTime.Now.Ticks);
            var r = cl.SyncConnector.GetDiff(typeof(StudentScheduleTerm), null) as SyncResult<StudentScheduleTerm>;
            var r3 = cl.SyncConnector.GetDiff(typeof(Student), null) as SyncResult<Student>;
            //var r2 = cl.SyncConnector.GetDiff(typeof (SectionStaff), null) as SyncResult<SectionStaff>;
            Debug.WriteLine(DateTime.Now.Ticks);

            //var items = r.All.Where(x => x.AcadSessionID== 9 &&  r2.All.Any(y => y.SectionID == x.CourseID && y.StaffID == 3919) && x.SectionOfCourseID.HasValue).ToList();

            var items = r.All.Where(x => x.SectionID == 11396 && x.TermID == 54).ToList();
            var students = r3.All.Where(x => x.StudentID == 6980).ToList();
            foreach (var student in students)
            {
                Debug.WriteLine("student = ({0}, {1}),", student.UserID, student.StudentID);
                var r2 = cl.SyncConnector.GetDiff(typeof(UserSchool), null) as SyncResult<UserSchool>;
                var users = r2.All.Where(x => x.UserID == student.UserID).ToList();
                foreach (var userSchool in users)
                {
                    Debug.WriteLine("userschool = ({0}, {1}),", userSchool.UserID, userSchool.SchoolID);
                }
            }

            foreach (var item in items)
            {
                //Debug.WriteLine("({0}, {1}),", item.CourseID, item.GradingScaleID.HasValue ? item.GradingScaleID.Value.ToString() : "null");
                //Debug.WriteLine("({0}, {1}, {2}),"
                //    , item.UserID, item.SchoolID, item.DistrictGuid);
                Debug.WriteLine("({0}, {1}),", item.TermID, item.StudentID);
            }
        }

        [Test]
        public void Test3()
        {
            //var cl = ConnectorLocator.Create("administrator", "1234qwer", "http://localhost/");
            var cl = ConnectorLocator.Create("administrator", "Ee9E(#UQe/5(G$U", "http://sandbox.sti-k12.com/chalkable/api/");
            //var cl = ConnectorLocator.Create("administrator", "qqqq1111", "https://qa-external.stiinformationnow.com/API/");

            Debug.WriteLine(DateTime.Now.Ticks);
            var r = cl.SyncConnector.GetDiff(typeof(GradingPeriod), null) as SyncResult<GradingPeriod>;
            Debug.WriteLine(DateTime.Now.Ticks);

            var gradingPeriods = r.All.Where(x => x.AcadSessionID == 12).ToList();

            foreach (var gradingPeriod in gradingPeriods)
            {
                Debug.WriteLine("gradingPeriod = ({0}, {1}, {2} ,{3}),", gradingPeriod.GradingPeriodID, gradingPeriod.Name, gradingPeriod.StartDate, gradingPeriod.EndDate);

            }
        }
    }
}
