using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAttendanceMonthService : DemoSchoolServiceBase, IAttendanceMonthService
    {
        public DemoAttendanceMonthService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void Add(IList<AttendanceMonth> attendanceMonths)
        {
            throw new NotImplementedException();
        }

        public void Edit(IList<AttendanceMonth> attendanceMonths)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<AttendanceMonth> attendanceMonths)
        {
            throw new NotImplementedException();
        }

        public IList<AttendanceMonth> GetAttendanceMonths(int schoolYearId, DateTime? fromDate = null, DateTime? endDate = null)
        {
            var res = Storage.AttendanceMonthStorage.GetAll().Where(x=>x.SchoolYearRef == schoolYearId);
            if (fromDate.HasValue)
                res = res.Where(x => x.EndDate >= fromDate);
            if (endDate.HasValue)
                res = res.Where(x => x.StartDate <= endDate);
            return res.ToList();
        }
    }
}
