using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class ScheduleSectionServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddEditScheduleSectionTest()
        {
            var nowTime = SchoolTestContext.AdminGradeSl.Context.NowSchoolTime;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, nowTime.AddDays(-7));
            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
            AssertForDeny(sl=>sl.ScheduleSectionService.Add(1, "section1", mp.Id), SchoolTestContext
                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            var ss = SchoolTestContext.AdminGradeSl.ScheduleSectionService.Add(1, "A", mp.Id);
            Assert.AreEqual(ss.Number, 0);
            Assert.AreEqual(ss.Name, "A");
            Assert.AreEqual(ss.MarkingPeriodRef, mp.Id);
            AssertAreEqual(ss, SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss.Id));
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(mp.Id).Count, 1);
            var ss2 = SchoolTestContext.AdminGradeSl.ScheduleSectionService.Add(3, "B", mp.Id);
            Assert.AreEqual(ss2.Number, 1);
            var ss3 = SchoolTestContext.AdminGradeSl.ScheduleSectionService.Add(2, "C", mp.Id);
            var ss4 = SchoolTestContext.AdminGradeSl.ScheduleSectionService.Add(3, "D", mp.Id);
            AssertForDeny(sl => sl.ScheduleSectionService.Edit(ss4.Id, 2, "M"), SchoolTestContext
                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            ss4 = SchoolTestContext.AdminGradeSl.ScheduleSectionService.Edit(ss4.Id, 1, "M");
            Assert.AreEqual(ss4.Number, 1);
            Assert.AreEqual(ss4.Name, "M");
            AssertAreEqual(ss4, SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss4.Id));
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss2.Id).Number, 2);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss3.Id).Number, 3);

            AssertForDeny(sl => sl.ScheduleSectionService.Delete(ss4.Id), SchoolTestContext
                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);


            var period = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, 450, 500, ss2.Id, 1);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("course1", "course1", null);
            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("001", "firrst room", "X", null, "333-444");
            var c = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class1", "first class", SchoolTestContext.FirstTeacher.Id
                , SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> {mp.Id});
            var cPeriod = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(period.Id, c.Id, room.Id);

            AssertException<Exception>(()=> SchoolTestContext.AdminGradeSl.ScheduleSectionService.Delete(ss2.Id));
            SchoolTestContext.AdminGradeSl.ClassPeriodService.Delete(cPeriod.Id);
            SchoolTestContext.AdminGradeSl.ScheduleSectionService.Delete(ss2.Id);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(mp.Id).Count, 3);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss3.Id).Number, 2);
        }
    }
}
