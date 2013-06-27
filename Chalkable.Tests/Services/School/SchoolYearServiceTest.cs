using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.Model;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class SchoolYearServiceTest : BaseSchoolServiceTest
    {
        private const string SCHOOL_YEAR_NAME = "testYear";
        
        [Test]
        public void TestAddGet()
        {
            var nowTime = DateTime.UtcNow.Date; 
            AssertForDeny(sl => sl.SchoolYearService.Add(SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1)), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            var syService = SchoolTestContext.AdminGradeSl.SchoolYearService;
            AssertException<Exception>(() => syService.Add(SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime.AddYears(1), nowTime));

            var schoolYear = syService.Add(SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1));
            Assert.AreEqual(syService.GetSortedYears().Count, 1);
            Assert.AreEqual(schoolYear.Name, SCHOOL_YEAR_NAME);
            Assert.AreEqual(schoolYear.Description, SCHOOL_YEAR_NAME);
            Assert.AreEqual(schoolYear.StartDate, nowTime);
            Assert.AreEqual(schoolYear.EndDate, nowTime.AddYears(1));
            
            AssertException<Exception>(() => syService.Add(SCHOOL_YEAR_NAME + "_2",  SCHOOL_YEAR_NAME + "_2", nowTime.AddMonths(4), nowTime.AddYears(1)));
            AssertException<Exception>(() => syService.Add(SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME + "_2", nowTime.AddYears(1).AddDays(2), nowTime.AddYears(2)));

            var sy2 = CreateNextSchoolYear(SchoolTestContext);
            Assert.AreEqual(syService.GetSortedYears().Count, 2);
         
            var sy3 = SchoolTestContext.FirstStudentSl.SchoolYearService.GetSchoolYearById(schoolYear.Id);
            AssertAreEqual(schoolYear, sy3);
            sy3 = SchoolTestContext.FirstStudentSl.SchoolYearService.GetCurrentSchoolYear();
            AssertAreEqual(schoolYear, sy3);
            var schoolYears = syService.GetSchoolYears(0, 1);
            Assert.AreEqual(schoolYears.TotalCount, 2);
            AssertAreEqual(schoolYears[0], schoolYear);
            schoolYears = syService.GetSchoolYears(1, 2);
            AssertAreEqual(schoolYears[0], sy2);

        }
        [Test]
        public void TestEdit()
        {
            var nowTime = DateTime.UtcNow.Date;
            var syService = SchoolTestContext.AdminGradeSl.SchoolYearService;
            var sy = syService.Add(SCHOOL_YEAR_NAME, SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1));
            AssertForDeny(sl=>sl.SchoolYearService.Edit(sy.Id, SCHOOL_YEAR_NAME + "_2", SCHOOL_YEAR_NAME, nowTime, nowTime.AddYears(1)), SchoolTestContext
                , SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);

            var secondSyName = SCHOOL_YEAR_NAME + "_2";
            SchoolTestContext.AdminGradeSl.SchoolYearService.Add(secondSyName, secondSyName, nowTime.AddYears(1).AddDays(1), nowTime.AddYears(2));
            AssertException<Exception>(() => syService.Edit(sy.Id, secondSyName, secondSyName, nowTime, nowTime.AddYears(1)));
            AssertException<Exception>(() => syService.Edit(sy.Id, sy.Name, sy.Description, nowTime.AddMonths(1), sy.EndDate.AddMonths(1)));

            DateTime newStartDate = sy.StartDate.AddMonths(1), newEndDate = sy.EndDate;
            sy = syService.Edit(sy.Id, "testSchoolYear3", "testDesc3", newStartDate, newEndDate);
            Assert.AreEqual(sy.Name, "testSchoolYear3");
            Assert.AreEqual(sy.Description, "testDesc3");
            Assert.AreEqual(sy.StartDate, newStartDate);
            Assert.AreEqual(sy.EndDate, newEndDate);
        }
        [Test]
        public void TestDelete()
        {
            var sy = CreateNextSchoolYear(SchoolTestContext);
            AssertForDeny(sl=>sl.SchoolYearService.Delete(sy.Id), SchoolTestContext, SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent);
            SchoolTestContext.AdminGradeSl.SchoolYearService.Delete(sy.Id);
            Assert.AreEqual(SchoolTestContext.AdminGradeSl.SchoolYearService.GetSchoolYears().Count, 0);
        }

        public static SchoolYear CreateNextSchoolYear(SchoolTestContext context)
        {
            var syService = context.AdminGradeSl.SchoolYearService;
            var schoolYears = syService.GetSortedYears();
            var startDate = DateTime.UtcNow.Date;
            if (schoolYears.Count > 0)
            {
                var lastSy = schoolYears[schoolYears.Count - 1];
                startDate = lastSy.EndDate.AddDays(1);
            }
            string syName = string.Format("{0}_{1}", SCHOOL_YEAR_NAME, schoolYears.Count + 1);
            return syService.Add(syName, syName, startDate, startDate.AddYears(1));
        }

    }
}
