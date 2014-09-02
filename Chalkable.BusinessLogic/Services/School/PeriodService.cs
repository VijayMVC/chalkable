using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPeriodService
    {
        Period Add(int periodId, int schoolYearId, int startTime, int endTime, int order);
        IList<Period> AddPeriods(IList<Period> periods); 
        void Delete(int id);
        void Delete(IList<int> ids);
        Period Edit(int id, int startTime, int endTime);
        IList<Period> Edit(IList<Period> periods); 
        Period GetPeriod(int time, DateTime date);
        Period GetPeriod(int time);
        IList<Period> GetPeriods(int schoolYearId);
    }

    public class PeriodService : SchoolServiceBase, IPeriodService
    {
        public PeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Period Add(int periodId, int schoolYearId, int startTime, int endTime, int order)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var da = new PeriodDataAccess(uow, Context.SchoolLocalId);
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId);
                var period = new Period
                    {
                        Id = periodId,
                        EndTime = endTime,
                        StartTime = startTime,
                        SchoolYearRef = schoolYearId,
                        Order = order, 
                        SchoolRef = sy.SchoolRef
                    };
                da.Insert(period);
                uow.Commit();
                return period;
            }
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new PeriodDataAccess(uow, Context.SchoolLocalId).Delete(id);
                uow.Commit();
            }
        }

        public Period Edit(int id, int startTime, int endTime)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PeriodDataAccess(uow, Context.SchoolLocalId);
                var period = da.GetById(id);
                period.StartTime = startTime;
                period.EndTime = endTime;
                da.Update(period);
                uow.Commit();
                return period;
            }
        }
        public Period GetPeriod(int time)
        {
            return GetPeriod(time, Context.NowSchoolYearTime.Date);
        }
        
        //TODO: remove those methods later
        public Period GetPeriod(int time, DateTime date)
        {
            using (var uow = Read())
            {
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetByDate(date);
                if (sy == null) return null;
                return new PeriodDataAccess(uow, Context.SchoolLocalId).GetPeriodOrNull(time, sy.Id);
            }
        }

        public IList<Period> GetPeriods(int schoolYearId)
        {
            using (var uow = Read())
            {
                return new PeriodDataAccess(uow, Context.SchoolLocalId).GetPeriods(schoolYearId);
            }
        }

        public IList<Period> AddPeriods(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            ValidatePeriods(periods);
            using (var uow = Update())
            {
                new PeriodDataAccess(uow, Context.SchoolLocalId).Insert(periods);
                uow.Commit();
                return periods;
            }
        }

        public IList<Period> Edit(IList<Period> periods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            ValidatePeriods(periods);
            using (var uow = Update())
            {
                new PeriodDataAccess(uow, Context.SchoolLocalId).Update(periods);
                uow.Commit();
                return periods;
            }
        }

        private void ValidatePeriods(IEnumerable<Period> periods)
        {
            if (periods.Any(period => period.StartTime >= period.EndTime))
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
        }

        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                new PeriodDataAccess(uow, Context.SchoolLocalId).Delete(ids);
                uow.Commit();
            }
        }
    }
}
