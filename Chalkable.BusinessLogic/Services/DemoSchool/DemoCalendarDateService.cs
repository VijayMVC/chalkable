﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    //TODO: needs tests
    public class DemoCalendarDateService : DemoSchoolServiceBase, ICalendarDateService
    {
        public DemoCalendarDateService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }


        public DateTime GetDbDateTime()
        {
            return Storage.DateStorage.GetDbDateTime();
        }
        
        public IList<Date> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
        {
            return Storage.DateStorage.GetDates(new DateQuery
            {
                SchoolYearId = schoolYearId,
                FromDate = fromDate,
                ToDate = tillDate,
                Count = count,
                SchoolDaysOnly = schoolDaysOnly
            });
        }

        public void Add(IList<Date> days)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.DateStorage.Add(days);
        }

        private void ValidateDate(Date date)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            if (!date.IsSchoolDay && date.DayTypeRef.HasValue)
                throw new ChalkableException("Incorrect parameters data");
            if (date.DayTypeRef.HasValue && !Storage.DayTypeStorage.Exists(date.SchoolYearRef))
                throw new ChalkableException("day type is not assigned to current school year");
        }

        public void Edit(IList<Date> dates)
        {
            foreach (var date in dates)
                ValidateDate(date);
            Storage.DateStorage.Update(dates);
        }

        public void Delete(IList<Date> dates)
        {
            Storage.DateStorage.Delete(dates);
        }
    }
}
