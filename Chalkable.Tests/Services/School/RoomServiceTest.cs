using System;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class RoomServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddEditRoomTest()
        {
            //Security check
            var newId = 1;
            var school1Id = FirstSchoolContext.School.LocalId;
            AssertForDeny(sl => sl.RoomService.AddRoom(newId, school1Id, "002", "second room", "10x10", null, "333-444"), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor
                 | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                 | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var roomService = DistrictTestContext.DistrictLocatorFirstSchool.RoomService;

            var r1 = roomService.AddRoom(newId, school1Id, "001", "first room", "10x10", null, "333-444");

            Assert.AreEqual(r1.RoomNumber, "001");
            Assert.AreEqual(r1.Description, "first room");
            Assert.AreEqual(r1.Size, "10x10");
            Assert.AreEqual(r1.PhoneNumber, "333-444");
            Assert.IsNull(r1.Capacity);
            Assert.AreEqual(1, roomService.GetRooms(0, 10).Count);
            Assert.AreEqual(1, FirstSchoolContext.AdminGradeSl.RoomService.GetRooms().Count);
            Assert.AreEqual(0, SecondSchoolContext.AdminGradeSl.RoomService.GetRooms().Count);
            AssertAreEqual(roomService.GetRoomById(r1.Id), r1);

            //Security check
            AssertForDeny(sl => sl.RoomService.EditRoom(r1.Id, "003", "third room", "12x12", null, "333-444-2"), FirstSchoolContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                 | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            r1 = roomService.EditRoom(r1.Id, "003", "third room", "12x12", null, "333-444-2");
            Assert.AreEqual(r1.RoomNumber, "003");
            Assert.AreEqual(r1.Description, "third room");
            Assert.AreEqual(r1.Size, "12x12");
            Assert.AreEqual(r1.PhoneNumber, "333-444-2");

        }

        [Test]
        public void DeleteRoomWhereIsPersonTest()
        {
            var newId = 1;
            var school1Id = FirstSchoolContext.School.LocalId;
            var roomService = DistrictTestContext.DistrictLocatorFirstSchool.RoomService;
            var r1 = roomService.AddRoom(newId, school1Id,"001", "first room", "10x10", null, "333-444");

            AssertForDeny(sl => sl.RoomService.DeleteRoom(r1.Id), FirstSchoolContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                 | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var nowDate = FirstSchoolContext.AdminGradeSl.Context.NowSchoolTime;
            var nowTime = (int)(nowDate - nowDate.Date).TotalMinutes;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool, nowDate.AddDays(-7));
            var mp = MarkingPeriodServiceTest.CreateNextMp(DistrictTestContext.DistrictLocatorFirstSchool, sy.Id);
            var newDayId = 1;
            var dayType = DistrictTestContext.DistrictLocatorFirstSchool.DayTypeService.Add(newDayId, 1, "1", sy.Id);
            var cDate = DistrictTestContext.DistrictLocatorFirstSchool.CalendarDateService.Add(nowDate.Date, true, sy.Id, dayType.Id);

            var currentPeriod = DistrictTestContext.DistrictLocatorFirstSchool.PeriodService.Add(1, sy.Id, nowTime - 10, nowTime + 30, 1);
            DistrictTestContext.DistrictLocatorFirstSchool.GradeLevelService.AddGradeLevel(1, "1", 1);
            var gradeLevel = DistrictTestContext.DistrictLocatorFirstSchool.GradeLevelService.GetGradeLevels()[0];
            var cl = DistrictTestContext.DistrictLocatorFirstSchool.ClassService
                .Add(1, sy.Id, null, "test", "test", FirstSchoolContext.FirstTeacher.Id, gradeLevel.Id);

            var clPeriod = DistrictTestContext.DistrictLocatorFirstSchool.ClassPeriodService.Add(currentPeriod.Id, cl.Id, r1.Id, dayType.Id);

            DistrictTestContext.DistrictLocatorFirstSchool.ClassService.AddStudent(cl.Id, FirstSchoolContext.FirstStudent.Id, mp.Id);
            var r2 = FirstSchoolContext.AdminGradeSl.RoomService.WhereIsPerson(FirstSchoolContext.FirstTeacher.Id, nowDate);
            AssertAreEqual(r1, r2);
            r2 = FirstSchoolContext.AdminGradeSl.RoomService.WhereIsPerson(FirstSchoolContext.FirstStudent.Id, nowDate);
            AssertAreEqual(r1, r2);
            Assert.IsNull(FirstSchoolContext.AdminGradeSl.RoomService.WhereIsPerson(FirstSchoolContext.SecondStudent.Id, nowDate));
            Assert.IsNull(FirstSchoolContext.AdminGradeSl.RoomService.WhereIsPerson(FirstSchoolContext.FirstStudent.Id, nowDate.AddHours(3)));

            AssertException<Exception>(() => DistrictTestContext.DistrictLocatorFirstSchool.RoomService.DeleteRoom(r1.Id));
            DistrictTestContext.DistrictLocatorFirstSchool.RoomService.DeleteRoom(r1.Id);

        }
    }
}
