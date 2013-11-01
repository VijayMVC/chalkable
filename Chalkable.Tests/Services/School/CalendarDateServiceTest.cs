//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class CalendarDateServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddClearDateTest()
//        {
//            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(FirstSchoolContext, FirstSchoolContext.NowDate.AddDays(-7), true);
//            AssertForDeny(sl => sl.CalendarDateService.Add(FirstSchoolContext.NowDate, true, mp.Id, null, null), FirstSchoolContext
//                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);

//            var mp2 = MarkingPeriodServiceTest.CreateNextMp(FirstSchoolContext, mp.SchoolYearRef);
//            var dateService = FirstSchoolContext.AdminGradeSl.CalendarDateService;
//            var sections = FirstSchoolContext.AdminGradeSl.DayTypeService.GetSections(mp.Id);
//            AssertException<Exception>(() => dateService.Add(FirstSchoolContext.NowDate, false, mp.Id, sections[0].Id, null));
//            AssertException<Exception>(() => dateService.Add(FirstSchoolContext.NowDate, true, mp2.Id, sections[0].Id, null));

//            var cDate = dateService.Add(FirstSchoolContext.NowDate, true, mp.Id, sections[0].Id, null);
//            Assert.IsTrue(cDate.IsSchoolDay);
//            Assert.AreEqual(cDate.ScheduleSectionRef, sections[0].Id);
//            Assert.AreEqual(cDate.MarkingPeriodRef, mp.Id);
//            Assert.AreEqual(cDate.DateTime, FirstSchoolContext.NowDate);
//            AssertAreEqual(cDate, dateService.GetById(cDate.Id));
//            AssertForDeny(sl => sl.CalendarDateService.ClearCalendarDates(mp.Id), FirstSchoolContext
//                          , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                          | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);
//            dateService.ClearCalendarDates(mp.Id);
//            AssertException<Exception>(()=>dateService.GetById(cDate.Id));
//        }

//        [Test]
//        public void AssignDateTest()
//        {
//            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(FirstSchoolContext, FirstSchoolContext.NowDate.AddDays(-7), true);
//            var sections = FirstSchoolContext.AdminGradeSl.DayTypeService.GetSections(mp.Id);
//            var section1 = sections[0];
//            var cDate = FirstSchoolContext.AdminGradeSl.CalendarDateService.Add(FirstSchoolContext.NowDate, true, mp.Id, section1.Id, null);
           
//            AssertForDeny(sl => sl.CalendarDateService.CanAssignDate(cDate.Id, sections[1].Id), FirstSchoolContext
//                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);
//            AssertForDeny(sl => sl.CalendarDateService.AssignDate(cDate.Id, sections[1].Id), FirstSchoolContext
//                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);

//            var mp2 = MarkingPeriodServiceTest.CreateNextMp(FirstSchoolContext, mp.SchoolYearRef);
//            var sectionNames = new List<string>{"A","B","C"};
//            FirstSchoolContext.AdminGradeSl.DayTypeService.ReBuildSections(sectionNames, new List<Guid>{mp2.Id});
//            var sections2 = FirstSchoolContext.AdminGradeSl.DayTypeService.GetSections(mp2.Id);
            
//            AssertException<Exception>(() => FirstSchoolContext.AdminGradeSl.CalendarDateService.CanAssignDate(cDate.Id, sections2[0].Id));
//            AssertException<Exception>(() => FirstSchoolContext.AdminGradeSl.CalendarDateService.AssignDate(cDate.Id, sections2[0].Id));
//            var course1 = FirstSchoolContext.AdminGradeSl.CourseService.Add("01", "course1", null);
//            var room1 = FirstSchoolContext.AdminGradeSl.RoomService.AddRoom("001", "room1", "10X10", null, "333-444");
//            var c1 = FirstSchoolContext.AdminGradeSl.ClassService.Add(mp.SchoolYearRef, course1.Id, "class1", "class1"
//                         , FirstSchoolContext.FirstTeacher.Id, FirstSchoolContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> {mp.Id});
//            var period1 = FirstSchoolContext.AdminGradeSl.PeriodService.Add(mp.Id, 450, 500, section1.Id, 1);

//            var clPeriod = FirstSchoolContext.AdminGradeSl.ClassPeriodService.Add(period1.Id, c1.Id, room1.Id);
//            FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c1.Id, FirstSchoolContext.FirstStudent.Id);
//            FirstSchoolContext.AdminGradeSl.AttendanceService.SetAttendanceForClass(clPeriod.Id, FirstSchoolContext.NowDate, AttendanceTypeEnum.Present);

//            Assert.IsFalse(FirstSchoolContext.AdminGradeSl.CalendarDateService.CanAssignDate(cDate.Id, section1.Id));
//            AssertException<Exception>(() => FirstSchoolContext.AdminGradeSl.CalendarDateService.AssignDate(cDate.Id, section1.Id));
//            //TODO: test in case discipline exists
//            FirstSchoolContext.AdminGradeSl.ClassPeriodService.Delete(clPeriod.Id);
//            FirstSchoolContext.AdminGradeSl.CalendarDateService.AssignDate(cDate.Id, sections[1].Id);
//            cDate = FirstSchoolContext.AdminGradeSl.CalendarDateService.GetById(cDate.Id);
//            Assert.AreEqual(cDate.ScheduleSectionRef, sections[1].Id);
//        }

//        [Test]
//        public void GetCalendareDateTest()
//        {
//            var mpInterval = 30;
//            var mpStartDate = FirstSchoolContext.NowTime.AddDays(-5).Date;
//            var mp = MarkingPeriodServiceTest.CreateSchoolYearWithMp(FirstSchoolContext, mpStartDate, false, false, mpInterval, 62);
//            var section = FirstSchoolContext.AdminGradeSl.DayTypeService.Add(1, "1", mp.Id);
//            var date = mpStartDate.AddDays(1);
//            var dateService = FirstSchoolContext.AdminGradeSl.CalendarDateService;
//            while (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
//            {
//                date = date.AddDays(1);
//            }
//            var cDate = dateService.GetCalendarDateByDate(date);
//            Assert.AreEqual(cDate.MarkingPeriodRef, mp.Id);
//            Assert.AreEqual(cDate.DateTime, date);
//            Assert.IsFalse(cDate.IsSchoolDay);
//            Assert.IsFalse(cDate.ScheduleSectionRef.HasValue);
//            date = date.AddDays(2);
//            cDate = dateService.GetCalendarDateByDate(date);
//            Assert.AreEqual(cDate.DateTime, date);
//            Assert.AreEqual(cDate.IsSchoolDay, true);
//            Assert.AreEqual(cDate.ScheduleSectionRef, section.Id);
//            var cDate2 = dateService.GetCalendarDateByDate(mp.EndDate.AddDays(2));
//            Assert.IsFalse(cDate2.MarkingPeriodRef.HasValue);
//            Assert.IsFalse(cDate2.IsSchoolDay);
//            Assert.IsFalse(cDate2.ScheduleSectionRef.HasValue);

//            Assert.AreEqual(dateService.GetDays(mp.Id, false).Count, mpInterval + 1);
//            Assert.AreEqual(dateService.GetDays(mp.Id, false, FirstSchoolContext.NowDate, FirstSchoolContext.NowDate.AddDays(2)).Count, 3);
//            Assert.AreEqual(dateService.GetDays(mp.Id, true, mp.StartDate, mp.StartDate.AddDays(6)).Count, 5);
            
//            var mp2 = MarkingPeriodServiceTest.CreateNextMp(FirstSchoolContext, mp.SchoolYearRef);
//            AssertException<Exception>(() => dateService.GetCalendarDateByDate(mp2.StartDate));
//            var section2 = FirstSchoolContext.AdminGradeSl.DayTypeService.Add(1, "1", mp2.Id);
//            dateService.ClearCalendarDates(mp.Id);
//            dateService.Delete(cDate2.Id);
//            var dates = dateService.GetLastDays(mp.SchoolYearRef, false, null, null);
//            Assert.AreEqual(dates.Count, (int)(mp2.EndDate - mp.StartDate).TotalDays + 1);
//        }
//    }
//}
