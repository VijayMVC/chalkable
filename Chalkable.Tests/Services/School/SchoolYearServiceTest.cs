using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class SchoolYearServiceTest : BaseSchoolServiceTest
    {
        private const string SCHOOL_YEAR_NAME = "testYear";

        [Test]
        public void TestAddGet()
        {
            var nowTime = FirstSchoolContext.AdminGradeSl.Context.NowSchoolTime.Date;
            int newschoolYearId = 1;
            var schoolId1 = FirstSchoolContext.School.LocalId;
            AssertForDeny(sl => sl.SchoolYearService.Add(newschoolYearId, schoolId1, SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME,
                nowTime, nowTime.AddYears(1)), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor 
                |  SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            var districtSyService = DistrictTestContext.DistrictLocatorFirstSchool.SchoolYearService;
            AssertException<Exception>(() => districtSyService.Add(newschoolYearId, schoolId1, SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime.AddYears(1), nowTime));

            var schoolYear = districtSyService.Add(newschoolYearId, schoolId1, SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1));
            var adminSyService = FirstSchoolContext.AdminGradeSl.SchoolYearService;
            Assert.AreEqual(adminSyService.GetSortedYears().Count, 1);
            Assert.AreEqual(schoolYear.Name, SCHOOL_YEAR_NAME);
            Assert.AreEqual(schoolYear.Description, SCHOOL_YEAR_NAME);
            Assert.AreEqual(schoolYear.StartDate, nowTime);
            Assert.AreEqual(schoolYear.EndDate, nowTime.AddYears(1));
            Assert.AreEqual(SecondSchoolContext.AdminGradeSl.SchoolYearService.GetSchoolYears().Count, 0);
            newschoolYearId++;
            AssertException<Exception>(() => districtSyService.Add(newschoolYearId, schoolId1, SCHOOL_YEAR_NAME + "_2", SCHOOL_YEAR_NAME + "_2", nowTime.AddMonths(4), nowTime.AddYears(1)));
            AssertException<Exception>(() => districtSyService.Add(newschoolYearId, schoolId1, SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME + "_2", nowTime.AddYears(1).AddDays(2), nowTime.AddYears(2)));

            var sy2 = CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            Assert.AreEqual(districtSyService.GetSortedYears().Count, 2);

            var sy3 = FirstSchoolContext.FirstStudentSl.SchoolYearService.GetSchoolYearById(schoolYear.Id);
            AssertAreEqual(schoolYear, sy3);
            sy3 = FirstSchoolContext.FirstStudentSl.SchoolYearService.GetCurrentSchoolYear();
            AssertAreEqual(schoolYear, sy3);
            var schoolYears = districtSyService.GetSchoolYears(0, 1);
            Assert.AreEqual(schoolYears.TotalCount, 2);
            AssertAreEqual(schoolYears[0], schoolYear);
            schoolYears = districtSyService.GetSchoolYears(1, 2);
            AssertAreEqual(schoolYears[0], sy2);

        }
        [Test]
        public void TestEdit()
        {
            var nowTime = DateTime.UtcNow.Date;
            var districtSyService = DistrictTestContext.DistrictLocatorFirstSchool.SchoolYearService;
            var newId = 1;
            var schoolId1 = FirstSchoolContext.School.LocalId;
            var sy = districtSyService.Add(newId, schoolId1, SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1));
           
            AssertForDeny(sl => sl.SchoolYearService.Edit(sy.Id, SCHOOL_YEAR_NAME + "_2", SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1)), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor  
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            var secondSyName = SCHOOL_YEAR_NAME + "_2";
            districtSyService.Add(newId + 1, schoolId1, secondSyName, secondSyName, nowTime.AddYears(1).AddDays(1), nowTime.AddYears(2));
            AssertException<Exception>(() => districtSyService.Edit(sy.Id, secondSyName, secondSyName, nowTime, nowTime.AddYears(1)));
            AssertException<Exception>(() => districtSyService.Edit(sy.Id, sy.Name, sy.Description, nowTime.AddMonths(1), sy.EndDate.AddMonths(1)));

            DateTime newStartDate = sy.StartDate.AddMonths(1), newEndDate = sy.EndDate;
            sy = districtSyService.Edit(sy.Id, "testSchoolYear3", "testDesc3", newStartDate, newEndDate);
            Assert.AreEqual(sy.Name, "testSchoolYear3");
            Assert.AreEqual(sy.Description, "testDesc3");
            Assert.AreEqual(sy.StartDate, newStartDate);
            Assert.AreEqual(sy.EndDate, newEndDate);
        }
        [Test]
        public void TestDelete()
        {
            var sy = CreateNextSchoolYear(DistrictTestContext.DistrictLocatorFirstSchool);
            AssertForDeny(sl => sl.SchoolYearService.Delete(sy.Id), FirstSchoolContext,
                 SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor
                 | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);
            DistrictTestContext.DistrictLocatorFirstSchool.SchoolYearService.Delete(sy.Id);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.SchoolYearService.GetSchoolYears().Count, 0);
        }

        public static SchoolYear CreateNextSchoolYear(IServiceLocatorSchool locator, DateTime? date = null)
        {
            var syService = locator.SchoolYearService;
            var schoolYears = syService.GetSortedYears();
            var startDate = date ?? locator.Context.NowSchoolTime;
            if (schoolYears.Count > 0)
            {
                var lastSy = schoolYears[schoolYears.Count - 1];
                startDate = lastSy.EndDate.AddDays(1);
            }
            var newId = GetNewId(locator, sl => sl.SchoolYearService.GetSortedYears(), x => x.Id);
            string syName = string.Format("{0}_{1}", SCHOOL_YEAR_NAME, newId);
            return syService.Add(newId, locator.Context.SchoolLocalId.Value, syName, syName, startDate, startDate.AddYears(1));
        }

    }
}
