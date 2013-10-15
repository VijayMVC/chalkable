using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            AssertForDeny(sl => sl.RoomService.AddRoom("002", "second room", "10x10", null, "333-444"), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                 | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var roomService = SchoolTestContext.AdminGradeSl.RoomService;
            var r1 = roomService.AddRoom("001", "first room", "10x10", null, "333-444");
            
            Assert.AreEqual(r1.RoomNumber, "001");
            Assert.AreEqual(r1.Description, "first room");
            Assert.AreEqual(r1.Size, "10x10");
            Assert.AreEqual(r1.PhoneNumber, "333-444");
            Assert.IsNull(r1.Capacity);
            Assert.AreEqual(1, roomService.GetRooms(0, 10).Count);
            AssertAreEqual(roomService.GetRoomById(r1.Id), r1);
           
            //Security check
            AssertForDeny(sl => sl.RoomService.EditRoom(r1.Id, "003", "third room", "12x12", null, "333-444-2"), SchoolTestContext
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
            var roomService = SchoolTestContext.AdminGradeSl.RoomService;
            var r1 = roomService.AddRoom("001", "first room", "10x10", null, "333-444");

            AssertForDeny(sl => sl.RoomService.DeleteRoom(r1.Id), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                 | SchoolContextRoles.FirstParent | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

            var nowDate = SchoolTestContext.AdminGradeSl.Context.NowSchoolTime;
            var nowTime = (int) (nowDate - nowDate.Date).TotalMinutes;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, nowDate.AddDays(-7));
            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
            SchoolTestContext.AdminGradeSl.ScheduleSectionService.GenerateDefaultSections(mp.Id);
            var cDate = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(nowDate.Date);
            var currentPeriod = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, nowTime - 10, nowTime + 30, cDate.ScheduleSectionRef.Value, 0);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("test", "test", null);
            var cl = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "test", "test", SchoolTestContext.FirstTeacher.Id,
                                      SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> {mp.Id});
            var clPeriod = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(currentPeriod.Id, cl.Id, r1.Id);

            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(cl.Id, SchoolTestContext.FirstStudent.Id);
            var r2 = SchoolTestContext.AdminGradeSl.RoomService.WhereIsPerson(SchoolTestContext.FirstTeacher.Id, nowDate);
            AssertAreEqual(r1, r2);
            r2 = SchoolTestContext.AdminGradeSl.RoomService.WhereIsPerson(SchoolTestContext.FirstStudent.Id, nowDate);
            AssertAreEqual(r1, r2);
            Assert.IsNull(SchoolTestContext.AdminGradeSl.RoomService.WhereIsPerson(SchoolTestContext.SecondStudent.Id, nowDate));
            Assert.IsNull(SchoolTestContext.AdminGradeSl.RoomService.WhereIsPerson(SchoolTestContext.FirstStudent.Id, nowDate.AddHours(3)));
            
            AssertException<Exception>(()=> SchoolTestContext.AdminGradeSl.RoomService.DeleteRoom(r1.Id));
            SchoolTestContext.AdminGradeSl.ClassPeriodService.Delete(clPeriod.Id);
            SchoolTestContext.AdminGradeSl.RoomService.DeleteRoom(r1.Id);

        }
    }
}
