using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Logic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPeriodService
    {
        Period Add(Guid markingPeriodId, int startTime, int endTime, Guid sectionId);
        void Delete(Guid id);
        Period Edit(Guid id, int startTime, int endTime);
        IList<Period> GetPeriods(Guid? markingPeriodId, Guid? sectionId);
        Period GetPeriod(int time, DateTime date);
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
     
        public Period Add(Guid markingPeriodId, int startTime, int endTime, Guid sectionId)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PeriodDataAccess(uow);
                var periods = da.GetPeriods(sectionId);
                if (ExistsOverlapping(periods, startTime, endTime))
                    throw new ChalkableException(ChlkResources.ERR_PERIODS_CANT_OVERLAP);
               var machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                if (!machine.CanApply(StateActionEnum.SectionsAdd))
                    throw new InvalidSchoolStatusException(ChlkResources.ERR_PERIOD_INVALID_SCHOOL_STATUS);
                
                var period = new Period
                    {
                        Id = Guid.NewGuid(),
                        EndTime = endTime,
                        StartTime = startTime,
                        MarkingPeriodRef = markingPeriodId,
                        SectionRef = sectionId
                    };
                da.Insert(period);
                uow.Commit();
                return period;
            }
        }

        public void Delete(Guid id)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new PeriodDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public Period Edit(Guid id, int startTime, int endTime)
        {
            if (startTime >= endTime)
                throw new ChalkableException(ChlkResources.ERR_PERIOD_INVALID_TIME);
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new PeriodDataAccess(uow);
                var period = da.GeComplextById(id);
                var periods = da.GetPeriods(period.SectionRef).Where(x => x.Id != period.Id).ToList();
                if(ExistsOverlapping(periods, startTime, endTime))
                    throw new ChalkableException(ChlkResources.ERR_PERIODS_CANT_OVERLAP);

                period.StartTime = startTime;
                period.EndTime = endTime;
                da.Update(period);
                uow.Commit();
                return period;
            }
        }

        public IList<Period> GetPeriods(Guid? markingPeriodId, Guid? sectionId)
        {
            using (var uow = Read())
            {
                return new PeriodDataAccess(uow).GetComplexPeriods(sectionId, markingPeriodId);
            }
        }

        public Period GetPeriod(int time, DateTime date)
        {
            using (var uow = Read())
            {
                var calendarDate = ServiceLocator.CalendarDateService.GetCalendarDateByDate(date);
                if(calendarDate.ScheduleSectionRef.HasValue)
                    return new PeriodDataAccess(uow).GetPeriodOrNull(calendarDate.ScheduleSectionRef.Value, time);
            }
            return null;
        }

        public IList<Period> ReGeneratePeriods(IList<Guid> markingPeriodIds, int? startTime = null, int? length = null, int? lengthBetweenPeriods = null, int? periodCount = null)
        {
            startTime = startTime ?? 475;
            length = length ?? 45;
            lengthBetweenPeriods = lengthBetweenPeriods ?? 2;
            periodCount = periodCount ?? 9;

            if (!(markingPeriodIds != null && markingPeriodIds.Count > 0))
                throw new ChalkableException();

            using (var uow = Update())
            {
                var sections = new ScheduleSectionDataAccess(uow).GetSections(markingPeriodIds);
                if (markingPeriodIds.Any(mpId => sections.All(x => x.MarkingPeriodRef != mpId)))
                {
                    throw new ChalkableException(ChlkResources.ERR_PERIOD_NO_SCHEDULE_SECTION);
                }
                if (!BaseSecurity.IsAdminEditor(Context))
                    throw new ChalkableSecurityException();

                IStateMachine machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                if (!machine.CanApply(StateActionEnum.SectionsAdd))
                    throw new InvalidSchoolStatusException();

                IList<Period> res = new List<Period>();
                if (ServiceLocator.ScheduleSectionService.CanDeleteSections(markingPeriodIds.ToList()))
                {

                    var da = new PeriodDataAccess(uow);
                    da.Delete(markingPeriodIds);
                    foreach (var markingPeriodId in markingPeriodIds)
                    {
                        foreach (var scheduleSection in sections)
                        {
                            if (scheduleSection.MarkingPeriodRef == markingPeriodId)
                            {
                                for (int i = 0; i < periodCount; i++)
                                {
                                    var periodStartTime = startTime + (length + lengthBetweenPeriods) * i;
                                    var periodEndTime = periodStartTime + length;
                                    var period = new Period
                                    {
                                        SectionRef = scheduleSection.Id,
                                        StartTime = periodStartTime.Value,
                                        EndTime = periodEndTime.Value,
                                        MarkingPeriodRef = markingPeriodId
                                    };
                                    res.Add(period);
                                }
                            }
                        }
                    }
                    da.Insert(res);
                    uow.Commit();
                }
                else
                {
                    throw new ChalkableException(ChlkResources.ERR_CANT_CHANGE_GENERAL_PERIODS);
                }
                machine.Apply(StateActionEnum.DailyPeriodsAdd);
                return res;
            }

        }
    }
}
