using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAttendanceMonthStorage : BaseDemoIntStorage<AttendanceMonth>
    {
        public DemoAttendanceMonthStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoAttendanceMonthService : DemoSchoolServiceBase, IAttendanceMonthService
    {
        private DemoAttendanceMonthStorage AttendanceMonthStorage { get; set; }
        public DemoAttendanceMonthService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AttendanceMonthStorage = new DemoAttendanceMonthStorage();
        }

        public void Add(IList<AttendanceMonth> attendanceMonths)
        {
            AttendanceMonthStorage.Add(attendanceMonths);
        }

        public void Edit(IList<AttendanceMonth> attendanceMonths)
        {
            AttendanceMonthStorage.Update(attendanceMonths);
        }

        public void Delete(IList<AttendanceMonth> attendanceMonths)
        {
            AttendanceMonthStorage.Delete(attendanceMonths);
        }

        public IList<AttendanceMonth> GetAttendanceMonths(int schoolYearId, DateTime? fromDate = null, DateTime? endDate = null)
        {
            var res = AttendanceMonthStorage.GetAll().Where(x=>x.SchoolYearRef == schoolYearId);
            if (fromDate.HasValue)
                res = res.Where(x => x.EndDate >= fromDate);
            if (endDate.HasValue)
                res = res.Where(x => x.StartDate <= endDate);
            return res.ToList();
        }
    }
}
