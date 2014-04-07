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
    public interface IMarkingPeriodService
    {
        MarkingPeriod Add(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);
        IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods);
        void Delete(int id);
        void DeleteMarkingPeriods(IList<int> ids);
        MarkingPeriod Edit(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);
        IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods); 
        MarkingPeriod GetMarkingPeriodById(int id);
        MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null);
        MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId);
        IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
        bool ChangeWeekDays(IList<int> markingPeriodIds, int weekDays);
    }

    public class MarkingPeriodService : SchoolServiceBase, IMarkingPeriodService
    {
        public MarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                return da.GetById(id);
            }
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                return  da.GetLast(tillDate ?? Context.NowSchoolTime);
            }
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId)
                    .GetMarkingPeriodClassOrNull(new MarkingPeriodClassQuery
                        {
                            MarkingPeriodId = markingPeriodId,
                            ClassId = classId
                        });
            }
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).GetMarkingPeriods(schoolYearId);
            }
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                var res = da.GetMarkingPeriod(date);
                if (res != null)
                    return res;
                if (useLastExisting)
                    return GetLastMarkingPeriod(date);
            }
            return null;
        }

        public MarkingPeriod Add(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            var sy = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            var mp = new MarkingPeriod()
                {
                    Description = description,
                    EndDate = endDate,
                    Id = id,
                    Name = name,
                    SchoolRef = sy.SchoolRef,
                    SchoolYearRef = schoolYearId,
                    StartDate = startDate,
                    WeekDays = weekDays
                };
            return Add(new List<MarkingPeriod> {mp}).First();
        }
        
        public void Delete(int id)
        {
            DeleteMarkingPeriods(new List<int> { id });
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                if (new MarkingPeriodClassDataAccess(uow, null).Exists(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_ASSIGNED_TO_CLASS);
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).DeleteMarkingPeriods(markingPeriodIds);
                uow.Commit();
            }
        }


        public MarkingPeriod Edit(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            var sy = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            var mp = new MarkingPeriod()
            {
                Description = description,
                EndDate = endDate,
                Id = id,
                Name = name,
                SchoolRef = sy.SchoolRef,
                SchoolYearRef = schoolYearId,
                StartDate = startDate,
                WeekDays = weekDays
            };
            return Edit(new List<MarkingPeriod>{mp}).First();
        }


        //TODO : think how to rewrite this for better performance
        //TODO : needs testing
        public bool ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var mpDa = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                if (new DateDataAccess(uow, Context.SchoolLocalId).Exists(markingPeriodIds))
                    throw new ChalkableException("Can't change markingPeriod week days ");
                
                mpDa.ChangeWeekDays(markingPeriodIds, weekDays);
                uow.Commit();
                return true;
            }
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId).GetById(markingPeriodClassId);
            }
        }
    

        public MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).GetNextInYear(markingPeriodId);
            }
        }


        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).Insert(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }


        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).Update(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }
    }
}
