using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPeriodService : DemoSchoolServiceBase, IPeriodService
    {
        public DemoPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public Period Add(int periodId, int schoolYearId, int startTime, int endTime, int order)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var sy = Storage.SchoolYearStorage.GetById(schoolYearId);
            var period = new Period
            {
                Id = periodId,
                EndTime = endTime,
                StartTime = startTime,
                SchoolYearRef = schoolYearId,
                Order = order,
                SchoolRef = sy.SchoolRef
            };
            Storage.PeriodStorage.Add(period);
            return period;
        }

        public IList<Period> AddPeriods(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            foreach (var period in periods)
            {
                if (period.StartTime >= period.EndTime)
                    throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            }
            Storage.PeriodStorage.Add(periods);
            return periods;
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Storage.PeriodStorage.Delete(id);

        }

        public void Delete(IList<int> ids)
        {
            Storage.PeriodStorage.Delete(ids);
        }

        public Period Edit(int id, int startTime, int endTime)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var period = Storage.PeriodStorage.GetById(id);
            period.StartTime = startTime;
            period.EndTime = endTime;
            Storage.PeriodStorage.Update(period);
            return period;
        }

        public IList<Period> Edit(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            ValidatePeriods(periods);
            return Storage.PeriodStorage.Update(periods);
        }

        private void ValidatePeriods(IEnumerable<Period> periods)
        {
            if (periods.Any(period => period.StartTime >= period.EndTime))
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
        }

        public Period GetPeriod(int time)
        {
            return GetPeriod(time, Context.NowSchoolTime.Date);
        }

        //TODO: remove those methods later
        public Period GetPeriod(int time, DateTime date)
        {
            var sy = Storage.SchoolYearStorage.GetByDate(date);

            if (sy == null) return null;
            return Storage.PeriodStorage.GetPeriodOrNull(time, sy.Id);
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            return Storage.PeriodStorage.GetPeriods(schoolYearId);
        }

        public IList<Period> ReGeneratePeriods(IList<Guid> markingPeriodIds, int? startTime = null, int? length = null, int? lengthBetweenPeriods = null, int? periodCount = null)
        {
            throw new NotImplementedException();
        }
    }
}
