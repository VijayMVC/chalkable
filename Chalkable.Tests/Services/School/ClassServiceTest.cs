using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class ClassServiceTest : BaseSchoolServiceTest 
    {
        [Test]
        public void AddDeleteClassTest()
        {
            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-5), true);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("01", "course1", null);
            AssertForDeny(sl=>sl.ClassService.Add(mp.SchoolYearRef, course.Id, "class1", "first class", SchoolTestContext.FirstTeacher.Id
                , SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid>{mp.Id}), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent 
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var cService = SchoolTestContext.AdminGradeSl.ClassService;
            var gradeLevelId = SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef;
            var c = cService.Add(mp.SchoolYearRef, course.Id, "class1", "first class", SchoolTestContext.FirstTeacher.Id
                                 , gradeLevelId, new List<Guid> { mp.Id });

            Assert.AreEqual(c.SchoolYearRef, mp.SchoolYearRef);
            Assert.AreEqual(c.CourseRef, course.Id);
            Assert.AreEqual(c.Name, "class1");
            Assert.AreEqual(c.Description, "first class");
            Assert.AreEqual(c.TeacherRef, SchoolTestContext.FirstTeacher.Id);
            Assert.AreEqual(c.StudentsCount, 0);
            Assert.AreEqual(c.GradeLevelRef, gradeLevelId);
            AssertAreEqual(c, cService.GetClassById(c.Id));
            Assert.AreEqual(cService.GetClasses(mp.SchoolYearRef, null, null).Count, 1);
            Assert.AreEqual(cService.GetClasses(null, mp.Id, null).Count, 1);

            var sy2 = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
            var mp2 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy2.Id);
            AssertException<Exception>(() => cService.Add(mp.SchoolYearRef, course.Id, "class2", "second class"
                                  , SchoolTestContext.FirstTeacher.Id, gradeLevelId, new List<Guid> {mp2.Id}));
            var c2 = cService.Add(sy2.Id, course.Id, "class2", "second class"
                                  , SchoolTestContext.FirstTeacher.Id, gradeLevelId, new List<Guid> {mp2.Id});
            
            Assert.AreEqual(cService.GetClasses(null, null, null).Count, 2);
            Assert.AreEqual(cService.GetClasses(null, null, SchoolTestContext.FirstTeacher.Id).Count, 2);

            AssertForDeny(sl => sl.ClassService.Edit(c.Id, course.Id, "class3", "third class", SchoolTestContext.FirstTeacher.Id
                    , SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id }), SchoolTestContext
                    , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                    | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => cService.Edit(c.Id, course.Id, "class3", "third class"
                           , SchoolTestContext.FirstTeacher.Id, gradeLevelId, new List<Guid> {mp2.Id}));

            var mp3 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, mp.SchoolYearRef);
            c = cService.Edit(c.Id, course.Id, "class3", "third class", SchoolTestContext.FirstTeacher.Id, gradeLevelId,
                              new List<Guid> {mp3.Id});
            Assert.AreEqual(c.SchoolYearRef, mp.SchoolYearRef);
            Assert.AreEqual(c.CourseRef, course.Id);
            Assert.AreEqual(c.Name, "class3");
            Assert.AreEqual(c.Description, "third class");
            Assert.AreEqual(c.TeacherRef, SchoolTestContext.FirstTeacher.Id);
            Assert.AreEqual(c.StudentsCount, 0);
            Assert.AreEqual(c.GradeLevelRef, gradeLevelId);
            Assert.AreEqual(c.MarkingPeriodClass[0].MarkingPeriodRef, mp3.Id);
            Assert.AreEqual(cService.GetClasses(null, mp3.Id, null).Count, 1);

            AssertForDeny(sl => sl.ClassService.Delete(c.Id), SchoolTestContext
                    , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                    | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, SchoolTestContext.FirstStudent.Id);
            cService.Delete(c.Id);
            Assert.AreEqual(cService.GetClasses(null, mp3.Id, null).Count, 0);
        }

        [Test]
        public void AddDeleteStudentTest()
        {
            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-5), true);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("01", "course1", null);
            var cService = SchoolTestContext.AdminGradeSl.ClassService;
            var gradeLevelId = SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef;
            var c = cService.Add(mp.SchoolYearRef, course.Id, "class1", "first class", SchoolTestContext.FirstTeacher.Id
                                 , gradeLevelId, new List<Guid> { mp.Id });

            AssertForDeny(sl=>sl.ClassService.AddStudent(c.Id, SchoolTestContext.FirstStudent.Id), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(()=> cService.AddStudent(c.Id, SchoolTestContext.FirstTeacher.Id));
            c = cService.AddStudent(c.Id, SchoolTestContext.FirstStudent.Id);
            var clPerson = cService.GetClassPerson(c.Id, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(clPerson.ClassRef, c.Id);
            Assert.AreEqual(clPerson.PersonRef, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(c.StudentsCount, 1);
            var classes = cService.GetClasses(null, null, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(classes.Count, 1);
            AssertAreEqual(classes[0], c);
            c = cService.AddStudent(c.Id, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(c.StudentsCount, 1);

            var cDate = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(SchoolTestContext.NowDate);
            var period = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, 450, 500, cDate.ScheduleSectionRef.Value, 1);
            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("001", "first room", "10X10", null, "333-444");
            var cPeriod = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(period.Id, c.Id, room.Id);
            var c2 = cService.Add(mp.SchoolYearRef, course.Id, "class2", "second class", SchoolTestContext.FirstTeacher.Id
                                 , gradeLevelId, new List<Guid> { mp.Id });
            var room2 = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("002", "second room", "10X10", null, "333-444");
            SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(period.Id, c2.Id, room2.Id);
            AssertException<Exception>(() => cService.AddStudent(c2.Id, SchoolTestContext.FirstStudent.Id));


            AssertForDeny(sl => sl.ClassService.DeleteStudent(c.Id, SchoolTestContext.FirstStudent.Id), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            c = cService.DeleteStudent(c.Id, SchoolTestContext.FirstStudent.Id);
            Assert.AreEqual(cService.GetClasses(null, null, SchoolTestContext.FirstStudent.Id).Count, 0);
            Assert.AreEqual(c.StudentsCount, 0);

        }

        [Test]
        public void AddDeleteClassFromMarkingPeriodTest()
        {
            var mp1 = MarkingPeriodServiceTest.CreateSchoolYearWithMp(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-5), true);
            var mp2 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, mp1.SchoolYearRef);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("01", "course1", null);
            var cService = SchoolTestContext.AdminGradeSl.ClassService;
            var gradeLevelId = SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef;
            var c = cService.Add(mp1.SchoolYearRef, course.Id, "class1", "first class", SchoolTestContext.FirstTeacher.Id
                                 , gradeLevelId, new List<Guid> { mp1.Id });
            var mp3 = MarkingPeriodServiceTest.CreateSchoolYearWithMp(SchoolTestContext, null);

            //Security check 
            AssertForDeny(sl=>sl.ClassService.AddMarkingPeriod(c.Id, mp2.Id), SchoolTestContext, SchoolContextRoles.FirstTeacher 
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);
            AssertForDeny(sl => sl.ClassService.DeleteClassFromMarkingPeriod(c.Id, mp1.Id), SchoolTestContext, SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => cService.AddMarkingPeriod(c.Id, mp3.Id));

            c = cService.AddMarkingPeriod(c.Id, mp2.Id);
            Assert.AreEqual(c.MarkingPeriodClass.Count, 2);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp3.Id, null).Count, 0);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp1.Id, null).Count, 1);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp2.Id, null).Count, 1);

            c = cService.DeleteClassFromMarkingPeriod(c.Id, mp2.Id);
            Assert.AreEqual(c.MarkingPeriodClass.Count, 1);
            Assert.AreEqual(cService.GetClasses(mp1.SchoolYearRef, mp2.Id, null).Count, 0);

        }
    }
}
