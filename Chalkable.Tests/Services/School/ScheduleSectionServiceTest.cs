using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Tests.Services.TestContext;
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

            Assert.IsFalse(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanDeleteSections(new List<Guid> {mp.Id}));
            AssertException<Exception>(()=> SchoolTestContext.AdminGradeSl.ScheduleSectionService.Delete(ss2.Id));
            SchoolTestContext.AdminGradeSl.ClassPeriodService.Delete(cPeriod.Id);
            SchoolTestContext.AdminGradeSl.ScheduleSectionService.Delete(ss2.Id);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(mp.Id).Count, 3);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSectionById(ss3.Id).Number, 2);
        }

        [Test]
        public void ReBuildGetSectionsTest()
        {
            var nowTime = SchoolTestContext.AdminGradeSl.Context.NowSchoolTime;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, nowTime.AddDays(-7));
            var mp1 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
            var mp2 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
            var mpIds = new List<Guid> {mp1.Id, mp2.Id};
            var sectionsNames1 = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            AssertForDeny(sl => sl.ScheduleSectionService.ReBuildSections(sectionsNames1, mpIds), SchoolTestContext
                    , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                    | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            SchoolTestContext.AdminGradeSl.ScheduleSectionService.ReBuildSections(sectionsNames1, new List<Guid> {mp1.Id});
            Assert.IsFalse(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanGetSection(mpIds));
            Assert.IsTrue(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanGetSection(new List<Guid> {mp1.Id}));
            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(mpIds));
            Assert.IsFalse(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanDeleteSections(mpIds));
            Assert.IsTrue(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanDeleteSections(new List<Guid>{mp1.Id}));
            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.ScheduleSectionService.ReBuildSections(sectionsNames1, mpIds));
            
            var sections = SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(new List<Guid> {mp1.Id});
            Assert.AreEqual(sections.Count, sectionsNames1.Count);
            for (int i = 0; i < sections.Count; i++)
            {
                Assert.AreEqual(sections[i].Name, sectionsNames1[i]);
            }
            foreach (var scheduleSection in sections)
            {
                SchoolTestContext.AdminGradeSl.ScheduleSectionService.Delete(scheduleSection.Id);
            }
            var sectionsNames2 = new List<string> {"A", "B", "C"};
            SchoolTestContext.AdminGradeSl.ScheduleSectionService.ReBuildSections(sectionsNames2, mpIds);
            Assert.IsTrue(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanGetSection(mpIds));
            Assert.IsTrue(SchoolTestContext.AdminGradeSl.ScheduleSectionService.CanDeleteSections(mpIds));
            sections = SchoolTestContext.AdminGradeSl.ScheduleSectionService.GetSections(mpIds);
            sections = sections.Where(x => x.MarkingPeriodRef == mp1.Id).ToList();
            Assert.AreEqual(sections.Count, sectionsNames2.Count);
            for (int i = 0; i < sections.Count; i++)
            {
                Assert.AreEqual(sections[i].Name, sectionsNames2[i]);
            }
        }
    }
}
