using System;
using Chalkable.Tests.Services.TestContext;
using NUnit.Framework;

namespace Chalkable.Tests.Services.School
{
    public class CalendarDateServiceTest : BaseSchoolServiceTest
    {
        [Test]
        public void AddClearDateTest()
        {
            var districtSl = DistrictTestContext.DistrictLocatorFirstSchool;
            var sy = SchoolYearServiceTest.CreateNextSchoolYear(districtSl, FirstSchoolContext.NowDate.AddDays(-7));
            var sy2 = SchoolYearServiceTest.CreateNextSchoolYear(districtSl, sy.EndDate.Value.AddDays(1));
            AssertForDeny(sl => sl.CalendarDateService.Add(FirstSchoolContext.NowDate.Date, true, sy.Id, null), FirstSchoolContext
                ,SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor |  SchoolContextRoles.FirstTeacher
                | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);

            var dateService =districtSl.CalendarDateService;
            var dayType = districtSl.DayTypeService.Add(1, 1, "A", sy.Id);
            AssertException<Exception>(() => dateService.Add(FirstSchoolContext.NowDate, false, sy.Id, dayType.Id));
            AssertException<Exception>(() => dateService.Add(FirstSchoolContext.NowDate, true, sy2.Id, dayType.Id));


            var cDate = dateService.Add(FirstSchoolContext.NowDate.Date, true, sy.Id, dayType.Id);
            var cDate2 = dateService.Add(FirstSchoolContext.NowDate.Date.AddDays(1), false, sy.Id, null);
            Assert.IsTrue(cDate.IsSchoolDay);
            Assert.AreEqual(cDate.DayTypeRef, dayType.Id);
            Assert.AreEqual(cDate.SchoolYearRef, sy.Id);
            Assert.AreEqual(cDate.Day, FirstSchoolContext.NowDate.Date);
            AssertAreEqual(cDate, dateService.GetCalendarDateByDate(cDate.Day));
            Assert.IsNull(SecondSchoolContext.AdminGradeSl.CalendarDateService.GetCalendarDateByDate(cDate.Day));
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.CalendarDateService.GetLastDays(sy.Id, false, null, null).Count, 2);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.CalendarDateService.GetLastDays(sy.Id, true, null, null).Count, 1);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.CalendarDateService.GetLastDays(sy.Id, false, cDate2.Day.AddDays(3), null).Count, 0);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.CalendarDateService.GetLastDays(sy.Id, false, null, cDate.Day.AddDays(-2)).Count, 0);
            Assert.AreEqual(FirstSchoolContext.AdminGradeSl.CalendarDateService.GetLastDays(sy.Id, false, cDate.Day.Date, cDate.Day.AddDays(2)).Count, 2);

            
            AssertForDeny(sl => sl.CalendarDateService.Delete(cDate.Day), FirstSchoolContext
                          , SchoolContextRoles.AdminGrade | SchoolContextRoles.AdminEditor | SchoolContextRoles.FirstTeacher 
                          | SchoolContextRoles.FirstStudent | SchoolContextRoles.FirstParent | SchoolContextRoles.Checkin | SchoolContextRoles.AdminViewer);
            
            dateService.Delete(cDate.Day);
            Assert.AreEqual(dateService.GetLastDays(sy.Id, false, null, null).Count, 1);
            Assert.IsNull(dateService.GetCalendarDateByDate(cDate.Day));
        }
    }
}
