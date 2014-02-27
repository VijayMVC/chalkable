using System;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class ClassPeriodServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeleteClassPeriodTest()
        {
            var district1Sl = DistrictTestContext.DistrictLocatorFirstSchool;
            var school1Id = FirstSchoolContext.School.LocalId;
            district1Sl.GradeLevelService.AddGradeLevel(1, "1th", 1);
            var gl = district1Sl.GradeLevelService.AddSchoolGradeLevel(1, school1Id);
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(district1Sl, FirstSchoolContext.NowDate.AddDays(-7));
            var mp = MarkingPeriodServiceTest.CreateNextMp(district1Sl, sy.Id);
            var room1 = district1Sl.RoomService.AddRoom(1, school1Id, "001", "room1", "10X10", null, "333-444");
            var room2 = district1Sl.RoomService.AddRoom(2, school1Id, "002", "room2", "10X10", null, "333-555");
            var c1 = district1Sl.ClassService.Add(1, sy.Id, null, "class1", "class1", FirstSchoolContext.FirstTeacher.Id, gl.Id);
            var c2 = district1Sl.ClassService.Add(2, sy.Id, null, "class2", "class2", FirstSchoolContext.FirstTeacher.Id, gl.Id);
            district1Sl.ClassService.AddStudent(c1.Id, FirstSchoolContext.FirstStudent.Id, mp.Id);

            var dt1 = district1Sl.DayTypeService.Add(1, 1, "A", sy.Id);
            var dt2 = district1Sl.DayTypeService.Add(2, 2, "B", sy.Id);

            var period1 = district1Sl.PeriodService.Add(1, sy.Id, FirstSchoolContext.NowMinutes - 10
                , FirstSchoolContext.NowMinutes + 30, 1);
            
            AssertForDeny(sl => sl.ClassPeriodService.Add(period1.Id, c1.Id, room1.Id, dt1.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            var cPeriodService = district1Sl.ClassPeriodService;
            var cPeriod1 = cPeriodService.Add(period1.Id, c1.Id, room1.Id, dt1.Id);
            var cDate = district1Sl.CalendarDateService.Add(FirstSchoolContext.NowTime.Date, true, sy.Id, dt1.Id);

            Assert.AreEqual(cPeriod1.ClassRef, c1.Id);
            Assert.AreEqual(cPeriod1.PeriodRef, period1.Id);
            Assert.AreEqual(cPeriod1.RoomRef, room1.Id);
            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstStudent.Id, FirstSchoolContext.NowTime));
            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstTeacher.Id, FirstSchoolContext.NowTime));
            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.SecondStudent.Id, FirstSchoolContext.NowTime));
            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.SecondTeahcer.Id, FirstSchoolContext.NowTime));
            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstStudent.Id, FirstSchoolContext.NowTime.AddHours(2)));
            AssertAreEqual(cPeriod1, cPeriodService.GetNearestClassPeriod(c1.Id, FirstSchoolContext.NowTime));
            Assert.IsNull(cPeriodService.GetNearestClassPeriod(c2.Id, FirstSchoolContext.NowTime));
            var availableClasses = cPeriodService.GetAvailableClasses(period1.Id);
            Assert.AreEqual(availableClasses.Count, 1);
            AssertAreEqual(availableClasses[0], c1);
            var availableRooms = cPeriodService.GetAvailableRooms(period1.Id);
            Assert.AreEqual(availableRooms.Count, 1);
            AssertAreEqual(availableRooms[0], room1);
            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room1.Id, dt1.Id));
            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c1.Id, room2.Id, dt1.Id));
            district1Sl.ClassService.AddStudent(c2.Id, FirstSchoolContext.FirstStudent.Id, mp.Id);
            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room2.Id, dt1.Id));
            district1Sl.ClassService.DeleteStudent(c2.Id, FirstSchoolContext.FirstStudent.Id);

            district1Sl.ClassService.AddStudent(c2.Id, FirstSchoolContext.SecondStudent.Id, mp.Id);
            cPeriodService.Add(period1.Id, c2.Id, room2.Id, dt1.Id);
            var period2 = district1Sl.PeriodService.Add(2, sy.Id, period1.EndTime + 10, period1.EndTime + 50, 2);
            cPeriodService.Add(period2.Id, c2.Id, room1.Id, dt1.Id);
            Assert.AreEqual(cPeriodService.GetAvailableRooms(period1.Id).Count, 2);
            Assert.AreEqual(cPeriodService.GetAvailableClasses(period1.Id).Count, 2);

            var dt3 = district1Sl.DayTypeService.Add(3, 3, "3rd", sy.Id);
            var classPeriod1 = cPeriodService.Add(period1.Id, c1.Id, room1.Id, dt3.Id);

            //GetClassPeriods testing
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null).Count, 4);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, c2.Id, null, null, null).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, c1.Id, room1.Id, null, null).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, room1.Id, null, null).Count, 3);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, period1.Id, dt1.Id).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, period1.Id, null).Count, 3);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, c1.Id, room1.Id, null, dt1.Id).Count, 1);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, c2.Id, null, null, dt1.Id).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, c2.Id, room1.Id, null, dt1.Id).Count, 1);

            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null, null, FirstSchoolContext.FirstTeacher.Id).Count, 4);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null, FirstSchoolContext.FirstStudent.Id).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null, FirstSchoolContext.FirstStudent.Id, null
                , FirstSchoolContext.NowMinutes).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null, null, null, FirstSchoolContext.NowMinutes).Count, 3);

            
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, null, null, null, null).Count, 3);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, c2.Id, null, null, null).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, c1.Id, null, null, null).Count, 1);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, null, room1.Id, null, null).Count, 2);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, null, null, FirstSchoolContext.FirstStudent.Id, null).Count, 1);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, null, null, null, FirstSchoolContext.FirstTeacher.Id).Count, 3);
            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.Day.Date, null, null, null, null, period1.StartTime + 10).Count, 2);

            // test security for delete method 
            AssertForDeny(sl => sl.ClassPeriodService.Delete(classPeriod1.PeriodRef, classPeriod1.ClassRef, classPeriod1.DayTypeRef), FirstSchoolContext,
              SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer
              | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            district1Sl.ClassPeriodService.Delete(classPeriod1.PeriodRef, classPeriod1.ClassRef, classPeriod1.DayTypeRef);
            Assert.AreEqual(cPeriodService.GetClassPeriods(sy.Id, null, null, null, null, null).Count, 3);
        }
    }
}
