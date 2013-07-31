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
        MarkingPeriod Add(Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays, bool generatePeriods = false, int? sisId = null);
        void Delete(Guid id);
        MarkingPeriod Edit(Guid id, Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);
        MarkingPeriod GetMarkingPeriodById(Guid id);
        MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null);
        MarkingPeriod GetNextMarkingPeriodInYear(Guid markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(Guid classId, Guid markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(Guid markingPeriodClassId);
        IList<MarkingPeriod> GetMarkingPeriods(Guid? schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
        bool ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays);
        bool ChangeMarkingPeriodsCount(Guid schoolYearId, int count, out string error);
       
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

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                return  da.GetLast(tillDate ?? Context.NowSchoolTime);
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

        public IList<MarkingPeriod> GetMarkingPeriods(Guid? schoolYearId)
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

        public MarkingPeriod Add(Guid schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays, bool generatePeriods = false, int? sisId = null)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if (!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                IStateMachine machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
                if (!machine.CanApply(StateActionEnum.MarkingPeriodsAdd))
                    throw new InvalidSchoolStatusException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_STATUS);

                var da = new MarkingPeriodDataAccess(uow);
                var sy = new SchoolYearDataAccess(uow).GetById(schoolYearId);
                var mp = AddMarkingPeriod(da, sy, startDate, endDate, name, description, weekDays);
                mp.SisId = sisId;
                da.Insert(mp);
                uow.Commit();
                machine.Apply(StateActionEnum.MarkingPeriodsAdd);
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
            Delete(new List<Guid> {id});
        }

        private void Delete(IList<Guid> markingPeriodIds)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                if (new MarkingPeriodClassDataAccess(uow).Exists(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_ASSIGNED_TO_CLASS);
                if (new DateDataAccess(uow).Exists(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_DELETE);
                if (!ServiceLocator.ScheduleSectionService.CanDeleteSections(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_SCHEDULE_SECTION_CANT_DELETE);
                
                new MarkingPeriodDataAccess(uow).DeleteMarkingPeriods(markingPeriodIds);
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
        //TODO : needs testing
        public bool ChangeWeekDays(IList<Guid> markingPeriodIds, int weekDays)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var mpDa = new MarkingPeriodDataAccess(uow);
                if (new DateDataAccess(uow).Exists(markingPeriodIds))
                    throw new ChalkableException("Can't change markingPeriod week days ");
                
                mpDa.ChangeWeekDays(markingPeriodIds, weekDays);
                uow.Commit();
                return true;
            }
        }

        //TODO: needs test
        public bool ChangeMarkingPeriodsCount(Guid schoolYearId, int count, out string error)
        {
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            if(!Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            error = null;
            var year = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            IStateMachine machine = new SchoolStateMachine(Context.SchoolId.Value, ServiceLocator.ServiceLocatorMaster);
            if (!machine.CanApply(StateActionEnum.MarkingPeriodsAdd))
                throw new InvalidSchoolStatusException(ChlkResources.ERR_MARKING_PERIOD_CHANGE_INVALID_STATUS);
            if (count <= 0)
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_COUNT);

            var mps = GetMarkingPeriods(year.Id);
            if (mps.Count != count)
            {
                using (var uow = Update())
                {
                    var mpDa = new MarkingPeriodDataAccess(uow);
                    var syDa = new SchoolYearDataAccess(uow);
                    var oldYearEndDate = year.EndDate;
                    var changeMpCount = Math.Abs(count - mps.Count);
                    try
                    {
                        if (count > mps.Count)
                        {
                            const int mpLength = 30;
                            var endDate = year.StartDate.AddDays(1);
                            if (mps.Count > 0)
                                endDate = mps.Last().EndDate;
                            var mpsForInsert = new List<MarkingPeriod>();
                            for (var i = 0; i < changeMpCount; i++)
                            {
                                var startDate = endDate.AddDays(1);
                                endDate = endDate.AddDays(mpLength);
                                mpsForInsert.Add(AddMarkingPeriod(mpDa, year, startDate, endDate, "MP" + (mps.Count + i + 1), "", 62));
                            }
                            if (year.EndDate <= endDate)
                                year.EndDate = endDate.AddDays(1);
                            mpDa.Insert(mpsForInsert);
                        }
                        else
                        {
                            var mpsForDelete = mps.Skip(count).Take(changeMpCount).Select(x => x.Id).ToList();
                            Delete(mpsForDelete);
                            year.EndDate = mps[count - 1].EndDate.AddDays(1);
                        }
                        if (year.EndDate != oldYearEndDate)
                            syDa.Update(year);
                        uow.Commit();
                    }
                    catch (Exception e)
                    {
                        error = e.Message;
                    }
                }
            }
            machine.Apply(StateActionEnum.MarkingPeriodsAdd);
            return true;
        }


        public MarkingPeriodClass GetMarkingPeriodClass(Guid markingPeriodClassId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow).GetById(markingPeriodClassId);
            }
        }
    

        public MarkingPeriod GetNextMarkingPeriodInYear(Guid markingPeriodId)
        {
            using (var uow = Read())
            {
               return new MarkingPeriodDataAccess(uow).GetNextInYear(markingPeriodId);
            }
        }
    }
}
