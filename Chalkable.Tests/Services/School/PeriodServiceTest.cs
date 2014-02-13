using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class PeriodServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddDeletePeriodTest()
        {
            var district1SL = DistrictTestContext.DistrictLocatorFirstSchool;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(district1SL, FirstSchoolContext.NowDate.AddDays(-7));
            var newId = 1;
            AssertForDeny(sl => sl.PeriodService.Add(newId, sy.Id, 450, 500, 1), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer 
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            var pService = district1SL.PeriodService;
            AssertException<Exception>(() => pService.Add(newId, sy.Id, 500, 450, 1));

            var period1 = pService.Add(newId, sy.Id, 450, 500, 1);
            AssertException<Exception>(() => pService.Add(newId, sy.Id, 480, 520, 1));

            Assert.AreEqual(period1.StartTime, 450);
            Assert.AreEqual(period1.EndTime, 500);
            Assert.AreEqual(period1.SchoolYearRef, sy.Id);

            Assert.IsNull(pService.GetPeriod(440));
            Assert.IsNull(pService.GetPeriod(510));
            Assert.IsNull(pService.GetPeriod(480, sy.EndDate.AddDays(1)));
            Assert.IsNull(SecondSchoolContext.AdminGradeSl.PeriodService.GetPeriod(480));
            AssertAreEqual(period1, pService.GetPeriod(480));
            var periods = pService.GetPeriods(sy.Id);
            Assert.AreEqual(periods.Count, 1);
            AssertAreEqual(period1, periods[0]);
            newId++;
            var period2 = pService.Add(newId, sy.Id, 510, 560, 2);
            Assert.AreEqual(pService.GetPeriods(sy.Id).Count, 2);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.PeriodService.GetPeriods(sy.Id).Count, 2);
            Assert.AreEqual(SecondSchoolContext.AdminGradeSl.PeriodService.GetPeriods(sy.Id).Count, 0);

            AssertForDeny(sl => sl.PeriodService.Edit(period1.Id, 570, 620), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor 
                | SchoolContextRoles.AdminViewer | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            AssertException<Exception>(() => pService.Edit(period1.Id, 520, 570));
            period1 = pService.Edit(period1.Id, 570, 620);
            Assert.AreEqual(period1.StartTime, 570);
            Assert.AreEqual(period1.EndTime, 620);
            AssertAreEqual(period1, pService.GetPeriod(580));

            AssertForDeny(sl => sl.PeriodService.Delete(period1.Id), FirstSchoolContext
                , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.AdminViewer 
                | SchoolContextRoles.FirstTeacher | SchoolContextRoles.FirstStudent
                | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin);

            pService.Delete(period1.Id);
            Assert.IsNull(pService.GetPeriod(580));
            Assert.AreEqual(pService.GetPeriods(sy.Id).Count, 1);
        }
    }
}
