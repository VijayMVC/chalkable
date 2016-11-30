using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICalendarDateService
    {
        DateTime GetDbDateTime();
        IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = int.MaxValue);
        void Add(IList<Date> days);
        void Edit(IList<Date> dates);
        void Delete(IList<Date> dates);
        void PrepareToDelete(IList<Date> dates);
    }

    public class CalendarDateService : SchoolServiceBase, ICalendarDateService
    {
        public CalendarDateService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public DateTime GetDbDateTime()
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow).GetDbDateTime();
            }
        }


        public IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new DateDataAccess(uow);
                return da.GetDates(new DateQuery
                    {
                        SchoolYearId = schoolYearId,
                        FromDate = fromDate,
                        ToDate = tillDate,
                        Count = count,
                        SchoolDaysOnly = schoolDaysOnly
                    });
            }
        }

        public void Add(IList<Date> days)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DateDataAccess(u).Insert(days));
        }
        
        public void Edit(IList<Date> dates)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            DoUpdate(u => new DateDataAccess(u).Update(dates));
        }

        public void Delete(IList<Date> dates)
        {
            DoUpdate(uow => new DateDataAccess(uow).Delete(dates));
        }

        public void PrepareToDelete(IList<Date> dates)
        {
            DoUpdate(uow => new DateDataAccess(uow).PrepareToDelete(dates));
        }
    }
}
