using System;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class DayTypeServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddEditTest()
        {
            var nowTime = FirstSchoolContext.AdminGradeSl.Context.NowSchoolTime;
            var district1Sl = DistrictTestContext.DistrictLocatorFirstSchool;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(district1Sl, nowTime.AddDays(-7));
            var newId = 1;
            AssertForDeny(sl => sl.DayTypeService.Add(newId, 1, "section1", sy.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            var ss = district1Sl.DayTypeService.Add(newId, 1, "A", sy.Id);
            Assert.AreEqual(ss.Number, 0);
            Assert.AreEqual(ss.Name, "A");
            Assert.AreEqual(ss.SchoolYearRef, sy.Id);
            AssertAreEqual(ss, FirstSchoolContext.AdminGradeSl.DayTypeService.GetSectionById(ss.Id));
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.DayTypeService.GetSections(sy.Id).Count, 1);
            newId++;
            var ss2 = district1Sl.DayTypeService.Add(newId, 3, "B", sy.Id);
            Assert.AreEqual(ss2.Number, 1);
            newId++;
            var ss3 = district1Sl.DayTypeService.Add(newId, 2, "C", sy.Id);
            newId++;
            var ss4 = district1Sl.DayTypeService.Add(newId, 3, "D", sy.Id);
            AssertForDeny(sl => sl.DayTypeService.Edit(ss4.Id, 2, "M"), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            ss4 = district1Sl.DayTypeService.Edit(ss4.Id, 1, "M");
            Assert.AreEqual(ss4.Number, 1);
            Assert.AreEqual(ss4.Name, "M");
            AssertAreEqual(ss4, FirstSchoolContext.AdminGradeSl.DayTypeService.GetSectionById(ss4.Id));
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.DayTypeService.GetSectionById(ss2.Id).Number, 2);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.DayTypeService.GetSectionById(ss3.Id).Number, 3);

            AssertForDeny(sl => sl.DayTypeService.Delete(ss4.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);


            var period = district1Sl.PeriodService.Add(1,sy.Id, 450, 500, 1);
            var room = district1Sl.RoomService.AddRoom(1, FirstSchoolContext.School.LocalId, "001", "firrst room", "X", null, "333-444");
            district1Sl.GradeLevelService.AddGradeLevel(1, "1th", 1);
            var gradeLevel = district1Sl.GradeLevelService.GetGradeLevels()[0];
            var c = district1Sl.ClassService.Add(1, sy.Id, null, "class1", "first class", FirstSchoolContext.FirstTeacher.Id, gradeLevel.Id);
            var cPeriod = district1Sl.ClassPeriodService.Add(period.Id, c.Id, room.Id, ss2.Id);

            Assert.IsFalse(district1Sl.DayTypeService.CanDeleteSections(sy.Id));
            AssertException<Exception>(() => district1Sl.DayTypeService.Delete(ss2.Id));
            district1Sl.ClassPeriodService.Delete(cPeriod.PeriodRef, cPeriod.ClassRef, cPeriod.DayTypeRef);
            district1Sl.DayTypeService.Delete(ss2.Id);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.DayTypeService.GetSections(sy.Id).Count, 3);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.DayTypeService.GetSectionById(ss3.Id).Number, 2);
        }
    }
}
