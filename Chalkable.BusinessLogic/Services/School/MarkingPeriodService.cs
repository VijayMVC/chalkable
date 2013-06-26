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
    public interface IMarkingPeriodService
    {
        MarkingPeriod Add(Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays, bool generatePeriods = false);
        void Delete(Guid id);
        MarkingPeriod Edit(Guid id, Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);


        MarkingPeriod GetMarkingPeriodById(Guid id);
        MarkingPeriod GetLastMarkingPeriod(DateTime tillDate);
        MarkingPeriodClass GetMarkingPeriodClass(Guid classId, Guid markingPeriodId);
        IList<MarkingPeriod> GetMarkingPeriods(Guid schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
        bool ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays);
    }

    public class MarkingPeriodService : SchoolServiceBase, IMarkingPeriodService
    {
        public MarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public MarkingPeriod GetMarkingPeriodById(Guid id)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                return da.GetById(id);
            }
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime tillDate)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                return  da.GetLast(tillDate);
            }
        }

        public MarkingPeriodClass GetMarkingPeriodClass(Guid classId, Guid markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow)
                    .GetMarkingPeriodClass(new MarkingPeriodClassQuery
                        {
                            MarkingPeriodId = markingPeriodId,
                            ClassId = classId
                        });
            }
        }

        public IList<MarkingPeriod> GetMarkingPeriods(Guid schoolYearId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodDataAccess(uow).GetMarkingPeriods(schoolYearId);
            }
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                var res = da.GetMarkingPeriod(date);
                if (res != null)
                    return res;
                if (useLastExisting)
                    return GetLastMarkingPeriod(date);
            }
            return null;
        }

        public MarkingPeriod Add(Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays, bool generatePeriods = false)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                IStateMachine machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                if (!machine.CanApply(StateActionEnum.MarkingPeriodsAdd))
                    throw new InvalidSchoolStatusException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_STATUS);

                var da = new MarkingPeriodDataAccess(uow);
                var sy = new SchoolYearDataAccess(uow).GetById(schoolYearId);
                var mp = AddMarkingPeriod(da, sy, startDate, endDate, name, description, weekDays);
                da.Insert(mp);
                machine.Apply(StateActionEnum.MarkingPeriodsAdd);
                uow.Commit();
                if (generatePeriods)
                {
                    var names = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                    ServiceLocator.ScheduleSectionService.GenerateScheduleSectionsWithDefaultPeriods(mp.Id, names);
                }
                return mp;
            }
        }


        private MarkingPeriod AddMarkingPeriod(MarkingPeriodDataAccess dataAccess, SchoolYear sy, DateTime startDate, DateTime endDate, 
            string name, string description, int weekDays, MarkingPeriod markingPeriod = null)
        {
            Guid? id = null;
            if (markingPeriod != null)
                id = markingPeriod.Id;
            else markingPeriod = new MarkingPeriod {Id = Guid.NewGuid()};

            if (startDate >= endDate)
                throw new ChalkableException("MarkingPeriod invalid date");

            if (dataAccess.IsOverlaped(startDate, endDate, id))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_OVERLAP);
            if (!(sy.StartDate <= startDate && sy.EndDate >= endDate))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_YEAR);

            markingPeriod.Description = description;
            markingPeriod.Name = name;
            markingPeriod.SchoolYearRef = sy.Id;
            markingPeriod.StartDate = startDate;
            markingPeriod.EndDate = endDate;
            markingPeriod.WeekDays = weekDays;
            return markingPeriod;
        }
        
        public void Delete(Guid id)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var mpcDa = new MarkingPeriodClassDataAccess(uow);
                if(mpcDa.Exists(new MarkingPeriodClassQuery{MarkingPeriodId = id}))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_ASSIGNED_TO_CLASS);

                var cdDa = new DateDataAccess(uow);
                if(cdDa.Exists(new DateQuery{MarkingPeriodId = id}))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_DELETE);

                var sections = ServiceLocator.ScheduleSectionService.GetSections(id).ToList();
                foreach (var scheduleSection in sections)
                {
                    ServiceLocator.ScheduleSectionService.Delete(scheduleSection.Id);
                }
                var mpDa = new MarkingPeriodDataAccess(uow);
                mpDa.Delete(id);
                uow.Commit();
            }
        }

        public MarkingPeriod Edit(Guid id, Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var da = new MarkingPeriodDataAccess(uow);
                var sy = new SchoolYearDataAccess(uow).GetById(schoolYearId);
                var mp = da.GetById(id);
                mp = AddMarkingPeriod(da, sy, startDate, endDate, name, description, weekDays, mp);
                da.Update(mp);
                uow.Commit();
                ServiceLocator.CalendarDateService.ClearCalendarDates(id);
                return mp;
            }
        }


        //TODO : think how to rewrite this for better performance
        public bool ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var mpDa = new MarkingPeriodDataAccess(uow);
                var cdDa = new DateDataAccess(uow);
                foreach (var markingPeriodId in markingPeriodIds)
                {
                    if(cdDa.Exists(new DateQuery{MarkingPeriodId = markingPeriodId}))
                        throw new ChalkableException("Can't change markingPeriod week days ");
                }
                mpDa.ChangeWeekDays(markingPeriodIds, weekDays);
                uow.Commit();
                return true;
            }
        }
    }
}
