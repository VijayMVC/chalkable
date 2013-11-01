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
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(FirstSchoolContext, FirstSchoolContext.NowDate.AddDays(-7));
//            var mp = MarkingPeriodServiceTest.CreateNextMp(FirstSchoolContext, sy.Id);
//            FirstSchoolContext.AdminGradeSl.DayTypeService.GenerateDefaultSections(mp.Id);
//            var cDate = FirstSchoolContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(FirstSchoolContext.NowDate);
//            var course = FirstSchoolContext.AdminGradeSl.CourseService.Add("001", "course1", null);
//            var room1 = FirstSchoolContext.AdminGradeSl.RoomService.AddRoom("001", "room1", "10X10", null, "333-444");
//            var room2 = FirstSchoolContext.AdminGradeSl.RoomService.AddRoom("002", "room2", "10X10", null, "333-555");
//            var c1 = FirstSchoolContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class1", "class1", FirstSchoolContext.FirstTeacher.Id,
//                                       FirstSchoolContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> {mp.Id});
//            var c2 = FirstSchoolContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class2", "class2", FirstSchoolContext.FirstTeacher.Id,
//                                       FirstSchoolContext.FirstStudent.StudentInfo.GradeLevelRef, new List<Guid> { mp.Id });
//            FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c1.Id, FirstSchoolContext.FirstStudent.Id);
           
//            var period1 = FirstSchoolContext.AdminGradeSl.PeriodService.Add(mp.Id, FirstSchoolContext.NowMinutes - 10,
//                                           FirstSchoolContext.NowMinutes + 30, cDate.ScheduleSectionRef.Value, 1);
//            AssertForDeny(sl=>sl.ClassPeriodService.Add(period1.Id, c1.Id, room1.Id), FirstSchoolContext, SchoolContextRoles.AdminViewer 
//                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            var cPeriodService = FirstSchoolContext.AdminGradeSl.ClassPeriodService;
//            var cPeriod1 = cPeriodService.Add(period1.Id, c1.Id, room1.Id);
//            Assert.AreEqual(cPeriod1.ClassRef, c1.Id);
//            Assert.AreEqual(cPeriod1.PeriodRef, period1.Id);
//            Assert.AreEqual(cPeriod1.RoomRef, room1.Id);
//            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstStudent.Id, FirstSchoolContext.NowTime));
//            AssertAreEqual(cPeriod1, cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstTeacher.Id, FirstSchoolContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.SecondStudent.Id, FirstSchoolContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.SecondTeahcer.Id, FirstSchoolContext.NowTime));
//            Assert.IsNull(cPeriodService.GetClassPeriodForSchoolPersonByDate(FirstSchoolContext.FirstStudent.Id, FirstSchoolContext.NowTime.AddHours(2)));
//            AssertAreEqual(cPeriod1, cPeriodService.GetNearestClassPeriod(c1.Id, FirstSchoolContext.NowTime));
//            Assert.IsNull(cPeriodService.GetNearestClassPeriod(c2.Id, FirstSchoolContext.NowTime));
//            var availableClasses = cPeriodService.GetAvailableClasses(period1.Id);
//            Assert.AreEqual(availableClasses.Count, 1);
//            AssertAreEqual(availableClasses[0], c1);
//            var availableRooms = cPeriodService.GetAvailableRooms(period1.Id);
//            Assert.AreEqual(availableRooms.Count, 1);
//            AssertAreEqual(availableRooms[0], room1);
            
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room1.Id));
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c1.Id, room2.Id));
//            FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c2.Id, FirstSchoolContext.FirstStudent.Id);
//            AssertException<Exception>(() => cPeriodService.Add(period1.Id, c2.Id, room2.Id));
//            FirstSchoolContext.AdminGradeSl.ClassService.DeleteStudent(c2.Id, FirstSchoolContext.FirstStudent.Id);
            
//            FirstSchoolContext.AdminGradeSl.ClassService.AddStudent(c2.Id, FirstSchoolContext.SecondStudent.Id);
//            cPeriodService.Add(period1.Id, c2.Id, room2.Id);
//            var period2 = FirstSchoolContext.AdminGradeSl.PeriodService.Add(mp.Id, period1.EndTime + 10, period1.EndTime + 50, cDate.ScheduleSectionRef.Value, 2);
//            var cDate2 = FirstSchoolContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(FirstSchoolContext.NowDate.AddDays(1).Date);
//            var period3 = FirstSchoolContext.AdminGradeSl.PeriodService.Add(mp.Id, period1.StartTime, period1.EndTime, cDate2.ScheduleSectionRef.Value, 1);
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

//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, null, FirstSchoolContext.FirstTeacher.Id).Count, 4);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, FirstSchoolContext.FirstStudent.Id).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, FirstSchoolContext.FirstStudent.Id, null
//                , FirstSchoolContext.NowMinutes).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null, null, null, FirstSchoolContext.NowMinutes).Count, 3);
            
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null,null,null).Count, 3);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, c2.Id, null, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, c1.Id, null, null, null).Count, 1);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, room1.Id, null, null).Count, 2);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, FirstSchoolContext.FirstStudent.Id, null).Count, 1);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, null, FirstSchoolContext.FirstTeacher.Id).Count, 3);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(cDate.DateTime, null, null, null, null, period1.StartTime + 10).Count, 2);
            
//            // test security for delete method 
//            AssertForDeny(sl => sl.ClassPeriodService.Delete(classPeriod1.Id), FirstSchoolContext, SchoolContextRoles.AdminViewer
//              | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            FirstSchoolContext.AdminGradeSl.ClassPeriodService.Delete(classPeriod1.Id);
//            Assert.AreEqual(cPeriodService.GetClassPeriods(mp.Id, null, null, null, null).Count, 3);
//        }
//    }
//}
