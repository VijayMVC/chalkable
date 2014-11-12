using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICalendarDateService
    {
        Date GetCalendarDateByDate(DateTime date);
        DateTime GetDbDateTime();
        IList<Date> GetDays(int markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null, int count = Int32.MaxValue);
        IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = int.MaxValue);
        Date Add(DateTime date, bool schoolDay, int schoolYearId, int? dateTypeId);
        void Add(IList<Date> days);
        void Edit(IList<Date> dates);
        void Delete(DateTime date);
        void Delete(IList<DateTime> date);
    }

    //TODO: needs tests
    public class CalendarDateService : SchoolServiceBase, ICalendarDateService
    {
        public CalendarDateService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Date GetCalendarDateByDate(DateTime date)
        {
            using (var uow = Update())
            {
                var da = new DateDataAccess(uow, Context.SchoolLocalId);
                var dateQuery = new DateQuery {FromDate = date, ToDate = date};
                if (Context.SchoolYearId.HasValue)
                    dateQuery.SchoolYearId = Context.SchoolYearId.Value;
                return da.GetDateOrNull(dateQuery);
            }
        }


        public DateTime GetDbDateTime()
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow, Context.SchoolLocalId).GetDbDateTime();
            }
        }

        public IList<Date> GetDays(int markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow, Context.SchoolLocalId)
                    .GetDates(new DateQuery
                        {
                            MarkingPeriodId = markingPeriodId,
                            SchoolDaysOnly = schoolDaysOnly,
                            FromDate = fromDate,
                            ToDate = tillDate,
                            Count = count
                        });

            }
        }

        public IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new DateDataAccess(uow, Context.SchoolLocalId);
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
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DateDataAccess(uow, Context.SchoolLocalId).Insert(days);
                uow.Commit();
            }
        }

        public void Delete(DateTime date)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Delete(new DateQuery { FromDate = date, ToDate = date});
        }
        
        private void Delete(DateQuery query)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DateDataAccess(uow, Context.SchoolLocalId).Delete(query);
                uow.Commit();
            }
        }
        
        public Date Add(DateTime date, bool schoolDay, int schoolYearId, int? dateTypeId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId);
                var res = new Date
                    {
                        Day = date,
                        IsSchoolDay = schoolDay,
                        SchoolYearRef = schoolYearId,
                        DayTypeRef = dateTypeId,
                        SchoolRef = sy.SchoolRef
                    };
                new DateDataAccess(uow, Context.SchoolLocalId).Insert(res);
                uow.Commit();
                return res;
            }
        }

        public void Edit(IList<Date> dates)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DateDataAccess(uow, Context.SchoolLocalId).Update(dates);
                uow.Commit();
            }
        }

        public void Delete(IList<DateTime> dates)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            foreach (var date in dates)
            {
                Delete(date);
            }
        }



    }
}
