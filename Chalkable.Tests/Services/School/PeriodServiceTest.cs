//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class PeriodServiceTest : BaseSchoolServiceTest
//    {
//        [Test]
//        public void AddDeletePeriodTest()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-7));
//            var mp = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
//            SchoolTestContext.AdminGradeSl.DayTypeService.GenerateDefaultSections(mp.Id);

//            var cDate = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(SchoolTestContext.NowDate);
//            AssertForDeny(sl => sl.PeriodService.Add(mp.Id, 450, 500, cDate.ScheduleSectionRef.Value, 1), SchoolTestContext
//                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent 
//                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            var pService = SchoolTestContext.AdminGradeSl.PeriodService; 
//            AssertException<Exception>(() => pService.Add(mp.Id, 500, 450, cDate.ScheduleSectionRef.Value, 1));

//            var period1 = pService.Add(mp.Id, 450, 500, cDate.ScheduleSectionRef.Value, 1);
//            AssertException<Exception>(() => pService.Add(mp.Id, 480, 520, cDate.ScheduleSectionRef.Value, 1));

//            Assert.AreEqual(period1.StartTime, 450);
//            Assert.AreEqual(period1.EndTime, 500);
//            Assert.AreEqual(period1.MarkingPeriodRef, mp.Id);
//            Assert.AreEqual(period1.SectionRef, cDate.ScheduleSectionRef);

//            Assert.IsNull(pService.GetPeriod(440, cDate.DateTime));
//            Assert.IsNull(pService.GetPeriod(510, cDate.DateTime));
//            Assert.IsNull(pService.GetPeriod(480, cDate.DateTime.AddDays(1)));
//            AssertAreEqual(period1, pService.GetPeriod(480, cDate.DateTime));
//            var periods = pService.GetPeriods(mp.Id, cDate.ScheduleSectionRef.Value);
//            Assert.AreEqual(periods.Count, 1);
//            AssertAreEqual(period1, periods[0]);
//            var period2 = pService.Add(mp.Id, 510, 560, cDate.ScheduleSectionRef.Value, 2);
//            Assert.AreEqual(pService.GetPeriods(mp.Id, cDate.ScheduleSectionRef.Value).Count, 2);
            
//            AssertForDeny(sl => sl.PeriodService.Edit(period1.Id, 570, 620), SchoolTestContext
//                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            AssertException<Exception>(()=> pService.Edit(period1.Id, 520, 570));
//            period1 = pService.Edit(period1.Id, 570, 620);
//            Assert.AreEqual(period1.StartTime, 570);
//            Assert.AreEqual(period1.EndTime, 620);
//            AssertAreEqual(period1, pService.GetPeriod(580, cDate.DateTime));
            
//            AssertForDeny(sl => sl.PeriodService.Delete(period1.Id), SchoolTestContext
//                , SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
//                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

//            pService.Delete(period1.Id);
//            Assert.IsNull(pService.GetPeriod(580, cDate.DateTime));
//            Assert.AreEqual(pService.GetPeriods(mp.Id, cDate.ScheduleSectionRef.Value).Count, 1);
//        }

//        [Test]
//        public void ReGeneratePeriodsTest()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext, SchoolTestContext.NowDate.AddDays(-7));
//            var mp1 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
//            var mp2 = MarkingPeriodServiceTest.CreateNextMp(SchoolTestContext, sy.Id);
//            SchoolTestContext.AdminGradeSl.DayTypeService.GenerateDefaultSections(mp1.Id);
//            int startTime = 450, pLength = 50, notPLength = 10, pCount = 5;
//            var mpIds = new List<Guid> { mp1.Id};
            
//            AssertForDeny(sl => sl.PeriodService.ReGeneratePeriods(mpIds, startTime, pLength, notPLength, pCount)
//                , SchoolTestContext, SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent
//                  | SchoolContextRoles.AdminViewer | SchoolContextRoles.Checkin);

//            var sectionsNames = new List<string> {"A", "B", "C"};
//            SchoolTestContext.AdminGradeSl.DayTypeService.ReBuildSections(sectionsNames, new List<Guid> {mp2.Id});
//            mpIds.Add(mp2.Id);
//            var pService = SchoolTestContext.AdminGradeSl.PeriodService;
//            AssertException<Exception>(()=>pService.ReGeneratePeriods(mpIds, startTime, pLength, notPLength, pCount));
//            var sections = SchoolTestContext.AdminGradeSl.DayTypeService.GetSections(mp2.Id);
//            foreach (var section in sections)
//            {
//                SchoolTestContext.AdminGradeSl.DayTypeService.Delete(section.Id);
//            }
//            SchoolTestContext.AdminGradeSl.DayTypeService.GenerateDefaultSections(mp2.Id);
//            var course = SchoolTestContext.AdminGradeSl.CourseService.Add("01", "course1", null);
//            var room = SchoolTestContext.AdminGradeSl.RoomService.AddRoom("001", "room1", "10X10", null, "333-444");
//            var c = SchoolTestContext.AdminGradeSl.ClassService.Add(sy.Id, course.Id, "class1", "class1",
//                               SchoolTestContext.FirstTeacher.Id, SchoolTestContext.FirstStudent.StudentInfo.GradeLevelRef, mpIds);
//            var cDate = SchoolTestContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(SchoolTestContext.NowDate);
//            var period = pService.Add(mp1.Id, startTime, startTime + pLength, cDate.ScheduleSectionRef.Value, 1);
//            var cPeriod = SchoolTestContext.AdminGradeSl.ClassPeriodService.Add(period.Id, c.Id, room.Id);

//            AssertException<Exception>(() => pService.ReGeneratePeriods(mpIds, startTime, pLength, notPLength, pCount));
//            SchoolTestContext.AdminGradeSl.ClassPeriodService.Delete(cPeriod.Id);
//            pService.ReGeneratePeriods(mpIds, startTime, pLength, notPLength, pCount);

//            sections = SchoolTestContext.AdminGradeSl.DayTypeService.GetSections(mp1.Id); 
//            var mp1Periods = pService.GetPeriods(mp1.Id, null);
//            var mp2Periods = pService.GetPeriods(mp2.Id, null);
//            Assert.AreEqual(mp1Periods.Count, sections.Count * 5);
//            Assert.AreEqual(mp1Periods.Count, mp2Periods.Count);

//            for (int i = 0; i < mp1Periods.Count; i++)
//            {
//                Assert.AreEqual(mp1Periods[i].Order, mp2Periods[i].Order);
//                Assert.AreEqual(mp1Periods[i].StartTime, mp2Periods[i].StartTime);
//                Assert.AreEqual(mp1Periods[i].EndTime, mp2Periods[i].EndTime);
//                Assert.AreEqual(mp1Periods[i].Section.Number, mp2Periods[i].Section.Number);
//                Assert.AreEqual(mp1Periods[i].Section.Name, mp2Periods[i].Section.Name);
//            }
//            Assert.AreEqual(pService.GetPeriods(null, null).Count, mp2Periods.Count + mp1Periods.Count);
//        }
//    }
//}
