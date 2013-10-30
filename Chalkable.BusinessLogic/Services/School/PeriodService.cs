﻿using System;
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
        void Delete(int id);
        Period Edit(int id, int startTime, int endTime);
        Period GetPeriod(int time, DateTime date);
        Period GetPeriod(int time);
        IList<Period> GetPeriods(int schoolYearId);
        IList<Period> ReGeneratePeriods(IList<Guid> markingPeriodIds, int? startTime = null, int? length = null, int? lengthBetweenPeriods = null, int? periodCount = null);
    }

    public class PeriodService : SchoolServiceBase, IPeriodService
    {
        public PeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        private bool ExistsOverlapping(IList<Period> periods, int startTime, int endTime)
        {
            if (startTime <= endTime)
            {
                foreach (var period in periods)
                {
                    int left = Math.Max(period.StartTime, startTime);
                    int right = Math.Min(period.EndTime, endTime);
                    if (left <= right)
                        return true;
                }
            }
            return false;
        }
     
        public Period Add(int periodId, int schoolYearId, int startTime, int endTime, int order)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            
            using (var uow = Update())
            {
                var da = new PeriodDataAccess(uow, Context.SchoolLocalId);
                var periods = da.GetPeriods(schoolYearId);
                if (ExistsOverlapping(periods, startTime, endTime))
                    throw new ChalkableException(ChlkResources.ERR_PERIODS_CANT_OVERLAP);

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
                var periods = da.GetPeriods(period.SchoolYearRef).Where(x => x.Id != period.Id).ToList();
                if(ExistsOverlapping(periods, startTime, endTime))
                    throw new ChalkableException(ChlkResources.ERR_PERIODS_CANT_OVERLAP);

                period.StartTime = startTime;
                period.EndTime = endTime;
                da.Update(period);
                uow.Commit();
                return period;
            }
        }
        public Period GetPeriod(int time)
        {
            return GetPeriod(time, Context.NowSchoolTime.Date);
        }


        //TODO: remove those methods later
        public Period GetPeriod(int time, DateTime date)
        {
            using (var uow = Read())
            {
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetByDate(date);
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


        public IList<Period> ReGeneratePeriods(IList<Guid> markingPeriodIds, int? startTime = null, int? length = null, int? lengthBetweenPeriods = null, int? periodCount = null)
        {
            throw new NotImplementedException();

            //startTime = startTime ?? 475;
            //length = length ?? 45;
            //lengthBetweenPeriods = lengthBetweenPeriods ?? 2;
            //periodCount = periodCount ?? 9;

            //if (!(markingPeriodIds != null && markingPeriodIds.Count > 0))
            //    throw new ChalkableException();

            //using (var uow = Update())
            //{
            //    var sections = new DateTypeDataAccess(uow).GetSections(markingPeriodIds);
            //    if (markingPeriodIds.Any(mpId => sections.All(x => x.MarkingPeriodRef != mpId)))
            //    {
            //        throw new ChalkableException(ChlkResources.ERR_PERIOD_NO_SCHEDULE_SECTION);
            //    }
            //    if (!BaseSecurity.IsAdminEditor(Context))
            //        throw new ChalkableSecurityException();
            //    if (!Context.SchoolId.HasValue)
            //        throw new UnassignedUserException();
                
            //    IList<Period> res = new List<Period>();
            //    if (ServiceLocator.ScheduleSectionService.CanDeleteSections(markingPeriodIds.ToList()))
            //    {

            //        var da = new PeriodDataAccess(uow);
            //        da.Delete(markingPeriodIds);
            //        foreach (var markingPeriodId in markingPeriodIds)
            //        {
            //            foreach (var scheduleSection in sections)
            //            {
            //                if (scheduleSection.MarkingPeriodRef == markingPeriodId)
            //                {
            //                    for (int i = 0; i < periodCount; i++)
            //                    {
            //                        var periodStartTime = startTime + (length + lengthBetweenPeriods) * i;
            //                        var periodEndTime = periodStartTime + length;
            //                        var period = new Period
            //                        {
            //                            Id = Guid.NewGuid(),
            //                            SectionRef = scheduleSection.Id,
            //                            StartTime = periodStartTime.Value,
            //                            EndTime = periodEndTime.Value,
            //                            MarkingPeriodRef = markingPeriodId
            //                        };
            //                        res.Add(period);
            //                    }
            //                }
            //            }
            //        }
            //        da.Insert(res);
            //        uow.Commit();
            //    }
            //    else
            //    {
            //        throw new ChalkableException(ChlkResources.ERR_CANT_CHANGE_GENERAL_PERIODS);
            //    }
            //    return res;
            //}

        }
    }
}
