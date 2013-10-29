//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class ClassPeriodServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddDeleteClassPeriodTest()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-7));
//            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
//            SchoolTestContext.AdminGradeSl.ScheduleSectionService.GenerateDefaultSections(mp.Id);
//            var cDate = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(SchoolTestContext.NowDate);
//            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("001", "course1", null);
//            var room1 = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("001", "room1", "10X10", null, "333-444");
//            var room2 = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("002", "room2", "10X10", null, "333-555");
//            var c1 = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class1", "class1", SchoolTestContext.FirstTeacher.Id,
//                                       SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> {mp.Id});
//            var c2 = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class2", "class2", SchoolTestContext.FirstTeacher.Id,
//                                       SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id });
//            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c1.Id, SchoolTestContext.FirstStudent.Id);
           
//            var period1 = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, SchoolTestContext.NowMinutes - 10,
//                                           SchoolTestContext.NowMinutes + 30, cDate.ScheduleSectionRef.Value, 1);
//            AssertForDeny(sl=>sl.ClassPeriodService.Add(period1.Id, c1.Id, room1.Id), SchoolTestContext, SchoolContextRoles.AdminViewer 
//                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            var cPeriodService = SchoolTestContext.AdminGradeSl.ClassPeriodService;
//            var cPeriod1 = cPeriodService.Add(period1.Id, c1.Id, room1.Id);
//            Assert.AreEqual(cPeriod1.ClassRef, c1.Id);
//            Assert.AreEqual(cPeriod1.PeriodRef, period1.Id);
//            Assert.AreEqual(cPeriod1.RoomRef, room1.Id);
//            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(SchoolTestContext.FirstStudent.Id, SchoolTestContext.NowTime));
//            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(SchoolTestContext.FirstTeacher.Id, SchoolTestContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(SchoolTestContext.SecondStudent.Id, SchoolTestContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(SchoolTestContext.SecondTeahcer.Id, SchoolTestContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(SchoolTestContext.FirstStudent.Id, SchoolTestContext.NowTime.AddHours(2)));
//            AssertAreEqual(cPeriod1, cPeriodService.GetNearestClassPeriod(c1.Id, SchoolTestContext.NowTime));
//            Assert.IsNull(cPeriodService.GetNearestClassPeriod(c2.Id, SchoolTestContext.NowTime));
//            var availableClasses = cPeriodService.GetAvailableClasses(period1.Id);
//            Assert.AreEqual(availableClasses.Count, 1);
//            AssertAreEqual(availableClasses[0], c1);
//            var availableRooms = cPeriodService.GetAvailableRooms(period1.Id);
//            Assert.AreEqual(availableRooms.Count, 1);
//            AssertAreEqual(availableRooms[0], room1);
            
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room1.Id));
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c1.Id, room2.Id));
//            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c2.Id, SchoolTestContext.FirstStudent.Id);
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room2.Id));
//            SchoolTestContext.AdminGradeSl.ClassService.DeleteStudent(c2.Id, SchoolTestContext.FirstStudent.Id);
            
//            SchoolTestContext.AdminGradeSl.ClassService.AddStudent(c2.Id, SchoolTestContext.SecondStudent.Id);
//            cPeriodService.Add(period1.Id, c2.Id, room2.Id);
//            var period2 = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, period1.EndTime + 10, period1.EndTime + 50, cDate.ScheduleSectionRef.Value, 2);
//            var cDate2 = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(SchoolTestContext.NowDate.AddDays(1).Date);
//            var period3 = SchoolTestContext.AdminGradeSl.PeriodService.Add(mp.Id, period1.StartTime, period1.EndTime, cDate2.ScheduleSectionRef.Value, 1);
//            cPeriodService.Add(period2.Id, c2.Id, room1.Id);
//            Assert.AreEqual(cPeriodService.GetAvailableRooms(period1.Id).Count, 2);
//            Assert.AreEqual(cPeriodService.GetAvailableClasses(period1.Id).Count, 2);

//            var classPeriod1 = cPeriodService.Add(period3.Id, c1.Id, room1.Id);
            
//            //GetClassPeriods testing
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null).Count, 4);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, c2.Id, null, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, c1.Id, room1.Id, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, room1.Id, null, null).Count, 3);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, period1.Id, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, c1.Id, room1.Id, null, cDate.ScheduleSectionRef.Value).Count, 1);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, c2.Id, null, null, cDate.ScheduleSectionRef.Value).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, c2.Id, room1.Id, null, cDate.ScheduleSectionRef.Value).Count, 1);

//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, null, SchoolTestContext.FirstTeacher.Id).Count, 4);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, SchoolTestContext.FirstStudent.Id).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, SchoolTestContext.FirstStudent.Id, null
//                , SchoolTestContext.NowMinutes).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, null, null, SchoolTestContext.NowMinutes).Count, 3);
            
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null,null,null).Count, 3);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, c2.Id, null, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, c1.Id, null, null, null).Count, 1);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, room1.Id, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, SchoolTestContext.FirstStudent.Id, null).Count, 1);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, null, SchoolTestContext.FirstTeacher.Id).Count, 3);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, null, null, period1.StartTime + 10).Count, 2);
            
//            // test security for delete method 
//            AssertForDeny(sl => sl.ClassPeriodService.Delete(classPeriod1.Id), SchoolTestContext, SchoolContextRoles.AdminViewer
//              | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            SchoolTestContext.AdminGradeSl.ClassPeriodService.Delete(classPeriod1.Id);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null).Count, 3);
//        }
//    }
//}
