//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Chalkable.BusinessLogic.Services;
//using Chalkable.Data.Master.Model;
//using Chalkable.Data.School.Model;
//using Chalkable.Tests.Services.TestContext;
//using NUnit.Framework;

//namespace Chalkable.Tests.Services.School
//{
//    public class MarkingPeriodServiceTest : BaseSchoolServiceTest
//    {

//        private const string MP_NAME = "TestMarkingPeriod";
//        private const int DEFAULT_WEEK_DAYS = 127;
        
//        [Test]
//        public void TestAddGet()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
//            DateTime startDate = sy.StartDate.AddDays(1), endDate = startDate.AddMonths(2); 
//            AssertForDeny(sl=>sl.MarkingPeriodService.Add(sy.Id, startDate, endDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS)
//                , SchoolTestContext, SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

//            var mpService = SchoolTestContext.AdminGradeSl.MarkingPeriodService;
//            AssertException<Exception>(() => mpService.Add(sy.Id, endDate, startDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
//            AssertException<Exception>(() => mpService.Add(sy.Id, startDate.AddMonths(-3), endDate.AddMonths(-3), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));

//            var mp1 = mpService.Add(sy.Id, startDate, endDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS);
//            Assert.AreEqual(mp1.Name, MP_NAME);
//            Assert.AreEqual(mp1.Description, MP_NAME);
//            Assert.AreEqual(mp1.StartDate, startDate);
//            Assert.AreEqual(mp1.EndDate, endDate);
//            AssertAreEqual(mpService.GetLastMarkingPeriod(startDate.AddDays(1)), mp1);
//            AssertAreEqual(mpService.GetMarkingPeriodById(mp1.Id), mp1);
        
//            //check overloping 
//            AssertException<Exception>(() => mpService.Add(sy.Id, startDate.AddMonths(1), endDate.AddDays(4), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
//            var mp2 = CreateNextMp(SchoolTestContext, sy.Id);

//            //testing get methods 
//            Assert.AreEqual(mpService.GetMarkingPeriods(sy.Id).Count, 2);
//            Assert.IsNull(mpService.GetMarkingPeriodByDate(mp1.StartDate.AddDays(-1)));
//            AssertAreEqual(mpService.GetMarkingPeriodByDate(mp1.StartDate.AddDays(1)), mp1);
//            AssertAreEqual(mpService.GetMarkingPeriodByDate(mp2.StartDate.AddDays(1)), mp2);
            
//            AssertAreEqual(mpService.GetLastMarkingPeriod(mp2.EndDate.AddDays(-1)), mp2);
//            Assert.IsNull(mpService.GetLastMarkingPeriod(mp1.StartDate.AddDays(-1)));

//        }
//        [Test]
//        public void TestEdit()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
//            var mp1 = CreateNextMp(SchoolTestContext, sy.Id);
//            AssertForDeny(sl=>sl.MarkingPeriodService.Edit(mp1.Id, sy.Id, mp1.StartDate, mp1.EndDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS)
//                , SchoolTestContext, SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

//            var mpService = SchoolTestContext.AdminGradeSl.MarkingPeriodService;
//            AssertException<Exception>(() => mpService.Add(sy.Id, mp1.EndDate, mp1.StartDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
//            AssertException<Exception>(() => mpService.Add(sy.Id, sy.StartDate.AddMonths(-3), sy.EndDate.AddMonths(-3), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));

//            var mp2 = CreateNextMp(SchoolTestContext, sy.Id);
//            AssertException<Exception>(() => mpService.Edit(mp1.Id, sy.Id, mp2.StartDate.AddDays(-2), mp2.StartDate.AddDays(2), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
            
//            var newMp1Name = MP_NAME + "_3";
//            var sy2 = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
//            mp1 = mpService.Edit(mp1.Id, sy2.Id, sy2.StartDate.AddDays(1), sy2.StartDate.AddMonths(1), newMp1Name, newMp1Name, DEFAULT_WEEK_DAYS);

//            Assert.AreEqual(mp1.StartDate, sy2.StartDate.AddDays(1));
//            Assert.AreEqual(mp1.EndDate, sy2.StartDate.AddMonths(1));
//            Assert.AreEqual(mp1.Name, newMp1Name);
//            Assert.AreEqual(mp1.Description, newMp1Name);
//            Assert.AreEqual(mp1.SchoolYearRef, sy2.Id);
//            AssertAreEqual(mp1, mpService.GetMarkingPeriodById(mp1.Id));

//        }
//        [Test]
//        public void TestDelete()
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(SchoolTestContext);
//            var mp = CreateNextMp(SchoolTestContext, sy.Id, true);
//            AssertForDeny(sl => sl.MarkingPeriodService.Delete(mp.Id),SchoolTestContext
//                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

//            var adminSl = SchoolTestContext.AdminGradeSl;
//            adminSl.CalendarDateService.GetCalendarDateByDate(mp.StartDate);
//            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id));
//            adminSl.CalendarDateService.ClearCalendarDates(mp.Id);

//            var cl =  ClassServiceTest.CreateClass(SchoolTestContext, SchoolTestContext.FirstTeacher, 
//                SchoolTestContext.FirstStudent, null, "math", sy.Id);

//            AssertException<Exception>(() => SchoolTestContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id));
//            adminSl.ClassService.DeleteClassFromMarkingPeriod(cl.Id, mp.Id);
//            SchoolTestContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id);
//            Assert.AreEqual(0, SchoolTestContext.AdminGradeSl.MarkingPeriodService.GetMarkingPeriods(sy.Id).Count);
//        }

//        public static MarkingPeriod CreateNextMp(SchoolTestContext context, Guid? schoolYearId = null, bool generatePeriods = false, int mpInterval = 30
//            , int weekDays = DEFAULT_WEEK_DAYS)
//        {
//            var adminSl = context.AdminGradeSl;
//            //var sysAdminSl = ServiceLocatorFactory.CreateMasterSysAdmin();
//            //var school =  sysAdminSl.SchoolService.GetById(context.AdminGradeSl.Context.SchoolId.Value);
//            //school.Status = SchoolStatus.DataImported;
//            SchoolYear sy;
//            if (schoolYearId.HasValue)
//               sy = adminSl.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
//            else sy = adminSl.SchoolYearService.GetCurrentSchoolYear();

//            var mps = adminSl.MarkingPeriodService.GetMarkingPeriods(sy.Id);
//            DateTime startDate;
//            if (mps.Count > 0)
//                startDate = mps[mps.Count - 1].EndDate.AddDays(1);
//            else startDate = sy.StartDate.AddDays(1);

//            var count = mps.Count;
//            var newMpName = MP_NAME + "_" + count + 1;
//            return adminSl.MarkingPeriodService.Add(sy.Id, startDate, startDate.AddDays(mpInterval), newMpName, newMpName, weekDays, generatePeriods);
//        }

//        public static MarkingPeriod CreateSchoolYearWithMp(SchoolTestContext context, DateTime? date, bool buildSections = false, bool generatePeriods = false
//            , int mpInterval = 30, int weekDays = DEFAULT_WEEK_DAYS)
//        {
//            var sy = SchoolYearServiceTest.CreateNextSchoolYear(context, date);
//            var mp = CreateNextMp(context, sy.Id, generatePeriods, mpInterval, weekDays);
//            if (buildSections)
//            {
//                var sections = new List<string> { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
//                context.AdminGradeSl.DayTypeService.ReBuildSections(sections, new List<Guid>() {mp.Id});
//            }
//            return mp;
//        }
//    }
//}
