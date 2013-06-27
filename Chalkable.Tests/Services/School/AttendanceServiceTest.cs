using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
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
            var c = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "testClass", "testClass", SchoolTestContext.FirstTeacher.User.Id,
                                                                    student.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id });

            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, student.Id);
            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, SchoolTestContext.SecondStudent.User.Id);

            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("testRoomNumber", "testDesc", null, null, "testPhone");
            var room2 = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("testRoomNUmer2", "testDesc2", null, null, "testPhone2");
            var dates = SchoolTestContext.AdminGradeSl.CalendarDateService.GetDays(mp.Id, false);
            var schoolDates = dates.Where(x => x.IsSchoolDay).ToList();
            var schoolDate = schoolDates[0];
            var periods = SchoolTestContext.AdminGradeSl.PeriodService.GetPeriods(mp.Id, schoolDate.ScheduleSectionRef);
            var currentPeriod = periods[0];

            var clPeriod =  SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(currentPeriod.Id, c.Id, room.Id);
            var clPerson = SchoolTestContext.AdminGradeSl.ClassService.GetClassPerson(c.Id, student.Id);
            
            AssertForDeny(sl => sl.AttendanceService.SetClassAttendance(clPerson.Id, clPeriod.Id, schoolDate.DateTime, AttendanceTypeEnum.Absent)
                , SchoolTestContext, SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
                | SchoolContextRoles.SecondTeacher);


            var c2 = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "testClass2", "testClass2"
                     , SchoolTestContext.FirstTeacher.UserRef, student.StudentInfo.GradeLevelRef, new List<Guid>{mp.Id});
            var clPeriod2 = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(currentPeriod.Id, c2.Id, room2.Id);
   
            // same class check 

            var attService = SchoolTestContext.AdminGradeSl.AttendanceService;
            AssertException<Exception>(() => attService.SetClassAttendance(clPerson.Id, clPeriod2.Id, schoolDate.DateTime, AttendanceTypeEnum.Absent));

            var reason = SchoolTestContext.AdminGradeSl.AttendanceReasonService.Add(AttendanceTypeEnum.Excused, "reason1", "reason1");
            AssertException<Exception>(() => attService.SetClassAttendance(clPerson.Id, clPeriod.Id, schoolDate.DateTime, AttendanceTypeEnum.Absent, reason.Id));
            
            var date2 = dates.First(x => x.ScheduleSectionRef.HasValue && x.ScheduleSectionRef != schoolDate.ScheduleSectionRef);
            AssertException<Exception>(() => attService.SetClassAttendance(clPerson.Id, clPeriod.Id, date2.DateTime, AttendanceTypeEnum.Absent));

            var attendace = attService.SetClassAttendance(clPerson.Id, clPeriod.Id, schoolDate.DateTime, AttendanceTypeEnum.Excused, reason.Id);
            Assert.AreEqual(attendace.ClassPeriodRef, clPeriod.Id);
            Assert.AreEqual(attendace.ClassPersonRef, clPerson.Id);
            Assert.AreEqual(attendace.Date, schoolDate.DateTime);
            Assert.AreEqual(attendace.AttendanceReasonRef, reason.Id);
            var attendances = attService.GetClassAttendanceComplex(new ClassAttendanceQuery {Id = attendace.Id}, null);
            AssertAreEqual(attendace, attendances[0]);

            AssertForDeny(sl=> sl.AttendanceService.SetAttendanceForClass(clPeriod.Id, schoolDate.DateTime, AttendanceTypeEnum.Present), SchoolTestContext
                , SchoolContextRoles.FirstParent | SchoolContextRoles.SecondTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.AdminViewer);

            AssertException<Exception>(() => attService.SetAttendanceForClass(clPerson.Id, date2.DateTime, AttendanceTypeEnum.Absent));
            
            attService.SetAttendanceForClass(clPeriod.Id, schoolDate.DateTime, AttendanceTypeEnum.Present);

            Assert.AreEqual(attService.GetClassAttendanceComplex(new ClassAttendanceQuery { Type = AttendanceTypeEnum.Late}, null).Count, 0);
            Assert.AreEqual(attService.GetClassAttendanceComplex(null, null, null, null, null, schoolDate.DateTime.AddDays(-1)).Count, 0);
            Assert.AreEqual(attService.GetClassAttendanceComplex(null, null, c2.Id, null, null, schoolDate.DateTime).Count, 0);
            attendances = attService.GetClassAttendanceComplex(null, null, c.Id, null, null, schoolDate.DateTime);
            Assert.AreEqual(attendances.Count, 2);
            Assert.AreEqual(attService.GetClassAttendanceComplex(null, null, c.Id, student.Id, null, schoolDate.DateTime).Count, 1);
            Assert.AreEqual(attendances[0].Type, AttendanceTypeEnum.Present);
            Assert.AreEqual(attendances[1].Type, AttendanceTypeEnum.Present);


        }

        [Test]
        public void TestGetSetDailAttendance()
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id, true);

            SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(mp.StartDate);
            var student = SchoolTestContext.GetPerson(SchoolTestContext.FirstStudent);
            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("testCourse", "testCourse");
            var c = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "testClass", "testClass", SchoolTestContext.FirstTeacher.User.Id,
                                                                    student.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id });
            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c.Id, student.Id);
            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("testRoomNumber", "testDesc", null, null, "testPhone");
            var room2 = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("testRoomNUmer2", "testDesc2", null, null, "testPhone2");
            var dates = SchoolTestContext.AdminGradeSl.CalendarDateService.GetDays(mp.Id, false);
            var schoolDates = dates.Where(x => x.IsSchoolDay).ToList();
            var schoolDate = schoolDates[0];
            
        }

        [Test]
        public void TestSwipeCard()
        {
            
        }

    }
}
