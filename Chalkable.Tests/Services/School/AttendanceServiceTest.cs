using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class AttendanceServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void TestSetGetAttendance()
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id, true);

            SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(mp.StartDate);
            var student = SchoolTestContext.GetPerson(SchoolTestContext.FirstStudent);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("testCourse", "testCourse");
            var c = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "testClass", "testClass", SchoolTestContext.FirstTeacher.UserRef,
                                                                    student.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id });

            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, student.Id);

            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("testRoomNumber", "testDesc", null, null, "testPhone");
            var periods = SchoolTestContext.AdminGradeSl.PeriodService.GetPeriods(mp.Id, null);
            var currentPeriod = periods[0];
            //var 
            var clPeriod =  SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(currentPeriod.Id, c.Id, room.Id);
            var clPerson = SchoolTestContext.AdminGradeSl.ClassService.GetClassPerson(c.Id, student.Id);
            var nowDate = SchoolTestContext.AdminGradeSl.Context.NowSchoolTime;
            AssertForDeny(sl => sl.AttendanceService.SetClassAttendance(clPerson.Id, clPeriod.Id, nowDate, AttendanceTypeEnum.Absent)
                , SchoolTestContext, SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);


            var c2 = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "testClass2", "testClass2"
                     , SchoolTestContext.FirstTeacher.UserRef, student.StudentInfo.GradeLevelRef, new List<Guid>{mp.Id});
            var clPeriod2 = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(currentPeriod.Id, c2.Id, room.Id);
   
            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.AttendanceService
                .SetClassAttendance(clPerson.Id, clPeriod2.Id, nowDate, AttendanceTypeEnum.Absent));


            var reason = SchoolTestContext.AdminGradeSl.AttendanceReasonService.Add(AttendanceTypeEnum.Excused, "reason1", "reason1");

            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.AttendanceService
                .SetClassAttendance(clPerson.Id, clPeriod.Id, nowDate, AttendanceTypeEnum.Absent, reason.Id));



        }

        [Test]
        public void TestSwipeCard()
        {
            
        }

    }
}
