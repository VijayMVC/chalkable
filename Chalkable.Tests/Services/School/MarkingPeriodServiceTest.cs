using System;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class MarkingPeriodServiceTest : BaseSchoolServiceTest
    {

        private const string MP_NAME = "TestMarkingPeriod";
        private const int DEFAULT_WEEK_DAYS = 127;

        [Test]
        public void TestAddGet()
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            DateTime startDate = sy.StartDate.AddDays(1), endDate = startDate.AddMonths(2);
            var mpService = DistrictTestContext.DistrictLocatorFirstSchool.MarkingPeriodService;
            var newMpId = GetNewMpId(FirstSchoolContext.AdminGradeSl);
            AssertForDeny(sl => sl.MarkingPeriodService.Add(newMpId, sy.Id, startDate, endDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS)
                , FirstSchoolContext, SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor 
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            AssertException<Exception>(() => mpService.Add(newMpId, sy.Id, endDate, startDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
            AssertException<Exception>(() => mpService.Add(newMpId, sy.Id, startDate.AddMonths(-3), endDate.AddMonths(-3), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));

            var mp1 = mpService.Add(newMpId, sy.Id, startDate, endDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS);
            var admin1MpService = FirstSchoolContext.AdminGradeSl.MarkingPeriodService;
            Assert.AreEqual(mp1.Name, MP_NAME);
            Assert.AreEqual(mp1.Description, MP_NAME);
            Assert.AreEqual(mp1.StartDate, startDate);
            Assert.AreEqual(mp1.EndDate, endDate);
            AssertAreEqual(admin1MpService.GetLastMarkingPeriod(startDate.AddDays(1)), mp1);
            AssertAreEqual(admin1MpService.GetMarkingPeriodById(mp1.Id), mp1);
            
            AssertAreEqual(SecondSchoolContext.AdminGradeSl.MarkingPeriodService.GetMarkingPeriods(null).Count, 0);
            newMpId++;
            //check overloping 
            AssertException<Exception>(() => mpService.Add(newMpId, sy.Id, startDate.AddMonths(1), endDate.AddDays(4), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
            var mp2 = CreateNextMp(DistrictTestContext.DistrictLocatorFirstSchool, sy.Id);

            //testing get methods 
            Assert.AreEqual(mpService.GetMarkingPeriods(sy.Id).Count, 2);
            Assert.IsNull(mpService.GetMarkingPeriodByDate(mp1.StartDate.AddDays(-1)));
            AssertAreEqual(mpService.GetMarkingPeriodByDate(mp1.StartDate.AddDays(1)), mp1);
            AssertAreEqual(mpService.GetMarkingPeriodByDate(mp2.StartDate.AddDays(1)), mp2);

            AssertAreEqual(mpService.GetLastMarkingPeriod(mp2.EndDate.AddDays(-1)), mp2);
            Assert.IsNull(mpService.GetLastMarkingPeriod(mp1.StartDate.AddDays(-1)));

        }
        [Test]
        public void TestEdit()
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            var mp1 = CreateNextMp(DistrictTestContext.DistrictLocatorFirstSchool, sy.Id);
            AssertForDeny(sl => sl.MarkingPeriodService.Edit(mp1.Id, sy.Id, mp1.StartDate, mp1.EndDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS)
                , FirstSchoolContext, SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            var mpService = DistrictTestContext.DistrictLocatorFirstSchool.MarkingPeriodService;
            var newId = mp1.Id + 1;
            AssertException<Exception>(() => mpService.Add(newId, sy.Id, mp1.EndDate, mp1.StartDate, MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));
            AssertException<Exception>(() => mpService.Add(newId, sy.Id, sy.StartDate.AddMonths(-3), sy.EndDate.AddMonths(-3), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));

            var mp2 = CreateNextMp(DistrictTestContext.DistrictLocatorFirstSchool, sy.Id);
            AssertException<Exception>(() => mpService.Edit(mp1.Id, sy.Id, mp2.StartDate.AddDays(-2), mp2.StartDate.AddDays(2), MP_NAME, MP_NAME, DEFAULT_WEEK_DAYS));

            var newMp1Name = MP_NAME + "_3";
            var sy2 = SchoolYearServiceTest.CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            mp1 = mpService.Edit(mp1.Id, sy2.Id, sy2.StartDate.AddDays(1), sy2.StartDate.AddMonths(1), newMp1Name, newMp1Name, DEFAULT_WEEK_DAYS);

            Assert.AreEqual(mp1.StartDate, sy2.StartDate.AddDays(1));
            Assert.AreEqual(mp1.EndDate, sy2.StartDate.AddMonths(1));
            Assert.AreEqual(mp1.Name, newMp1Name);
            Assert.AreEqual(mp1.Description, newMp1Name);
            Assert.AreEqual(mp1.SchoolYearRef, sy2.Id);
            AssertAreEqual(mp1, mpService.GetMarkingPeriodById(mp1.Id));

        }
        [Test]
        public void TestDelete()
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            var mp = CreateNextMp(DistrictTestContext.DistrictLocatorFirstSchool, sy.Id, true);
            AssertForDeny(sl => sl.MarkingPeriodService.Delete(mp.Id), FirstSchoolContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            //var districtSchoolL = DistrictTestContext.DistrictLocatorFirstSchool;
            //districtSchoolL.CalendarDateService.GetCalendarDateByDate(mp.StartDate);
            //AssertException<Exception>(() => FirstSchoolContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id));
            //districtSchoolL.CalendarDateService.ClearCalendarDates(mp.Id);

            //var cl = ClassServiceTest.CreateClass(FirstSchoolContext, FirstSchoolContext.FirstTeacher,
            //    FirstSchoolContext.FirstStudent, null, "math", sy.Id);

            //AssertException<Exception>(() => FirstSchoolContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id));
//            adminSl.ClassService.UnassignClassFromMarkingPeriod(cl.Id, mp.Id);
            //FirstSchoolContext.AdminGradeSl.MarkingPeriodService.Delete(mp.Id);
            //Assert.AreEqual(0, FirstSchoolContext.AdminGradeSl.MarkingPeriodService.GetMarkingPeriods(sy.Id).Count);
        }

        public static MarkingPeriod CreateNextMp(IServiceLocatorSchool locator, int? schoolYearId = null, bool generatePeriods = false, int mpInterval = 30
            , int weekDays = DEFAULT_WEEK_DAYS)
        {
            //var sysAdminSl = ServiceLocatorFactory.CreateMasterSysAdmin();
            //var school =  sysAdminSl.SchoolService.GetById(context.AdminGradeSl.Context.SchoolId.Value);
            //school.Status = SchoolStatus.DataImported;
            SchoolYear sy;
            if (schoolYearId.HasValue)
                sy = locator.SchoolYearService.GetSchoolYearById(schoolYearId.Value);
            else sy = locator.SchoolYearService.GetCurrentSchoolYear();

            var mps = locator.MarkingPeriodService.GetMarkingPeriods(sy.Id);
            DateTime startDate;
            if (mps.Count > 0)
                startDate = mps[mps.Count - 1].EndDate.AddDays(1);
            else startDate = sy.StartDate.AddDays(1);

            var newId = GetNewMpId(locator);
            var newMpName = MP_NAME + "_" + newId + 1;
            return locator.MarkingPeriodService.Add(newId, sy.Id, startDate, startDate.AddDays(mpInterval), newMpName, newMpName, weekDays);
        }

        private static int GetNewMpId(IServiceLocatorSchool serviceLocator)
        {
            return GetNewId(serviceLocator, sl=> sl.MarkingPeriodService.GetMarkingPeriods(null), x => x.Id);
        }

        public static MarkingPeriod CreateSchoolYearWithMp(IServiceLocatorSchool locator, DateTime? date, bool buildSections = false, bool generatePeriods = false
            , int mpInterval = 30, int weekDays = DEFAULT_WEEK_DAYS)
        {
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(locator, date);
            return CreateNextMp(locator, sy.Id, generatePeriods, mpInterval, weekDays);
        }
    }
}
