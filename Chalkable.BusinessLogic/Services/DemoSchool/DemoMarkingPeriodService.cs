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
  
    public class DemoMarkingPeriodService : DemoSchoolServiceBase, IMarkingPeriodService
    {
        public DemoMarkingPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            var sys = Storage.SchoolYearStorage.GetAll();
            foreach (var markingPeriod in markingPeriods)
            {
                var sy = sys.First(x => x.Id == markingPeriod.SchoolYearRef);
                ValidateMarkingPeriodData(sy, markingPeriod.StartDate, markingPeriod.EndDate, markingPeriod.Id);
            }
            return Storage.MarkingPeriodStorage.Update(markingPeriods);

        }

        private void ValidateMarkingPeriodData(SchoolYear sy, DateTime startDate,
                                              DateTime endDate, int? id)
        {
            if (startDate > endDate)
                throw new ChalkableException("Invalida date params. StartDate is bigger than EndDate");
            if (Storage.MarkingPeriodStorage.IsOverlaped(sy.Id, startDate, endDate, id))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_OVERLAP);
            if (!(sy.StartDate <= startDate && sy.EndDate >= endDate))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_YEAR);
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            return Storage.MarkingPeriodStorage.getById(id);
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            return Storage.MarkingPeriodStorage.GetLast(tillDate ?? Context.NowSchoolTime);
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            return Storage.MarkingPeriodClassStorage.GetMarkingPeriodClassOrNull(new MarkingPeriodClassQuery
            {
                MarkingPeriodId = markingPeriodId,
                ClassId = classId
            });
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            return Storage.MarkingPeriodStorage.GetMarkingPeriods(schoolYearId);
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            var res = Storage.MarkingPeriodStorage.GetMarkingPeriod(date);
            if (res != null)
                return res;
            if (useLastExisting)
                return GetLastMarkingPeriod(date);
            return null;
        }

        public MarkingPeriod Add(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();


            var sy = Storage.SchoolYearStorage.GetById(schoolYearId);
            var mp = UpdateMarkingPeriod(sy, startDate, endDate, name, description, weekDays, new MarkingPeriod{Id = id});
            Storage.MarkingPeriodStorage.Add(mp);
            return mp;
        }

        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.MarkingPeriodStorage.Add(markingPeriods);
        }


        private MarkingPeriod UpdateMarkingPeriod(SchoolYear sy, DateTime startDate, DateTime endDate, 
            string name, string description, int weekDays, MarkingPeriod markingPeriod = null)
        {
            int? id = null;
            if (markingPeriod != null)
                id = markingPeriod.Id;
            else markingPeriod = new MarkingPeriod();
            
            if(startDate > endDate)
                throw new ChalkableException("Invalid date params. StartDate is bigger than EndDate");
            if (Storage.MarkingPeriodStorage.IsOverlaped(sy.Id, startDate, endDate, id))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_OVERLAP);
            if (!(sy.StartDate <= startDate && sy.EndDate >= endDate))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_YEAR);

            markingPeriod.Description = description;
            markingPeriod.Name = name;
            markingPeriod.SchoolYearRef = sy.Id;
            markingPeriod.StartDate = startDate;
            markingPeriod.EndDate = endDate;
            markingPeriod.WeekDays = weekDays;
            markingPeriod.SchoolRef = sy.SchoolRef;
            return markingPeriod;
        }
        
        public void Delete(int id)
        {
            Delete(new List<int> {id});
        }

        public void DeleteMarkingPeriods(IList<int> ids)
        {
            throw new NotImplementedException();
        }

        private void Delete(IList<int> markingPeriodIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            if (Storage.MarkingPeriodStorage.Exists(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_ASSIGNED_TO_CLASS);
                Storage.MarkingPeriodStorage.DeleteMarkingPeriods(markingPeriodIds);
        }


        public MarkingPeriod Edit(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            var sy = Storage.SchoolYearStorage.GetById(schoolYearId);
            var mp = Storage.MarkingPeriodStorage.GetById(id);
            mp = UpdateMarkingPeriod(sy, startDate, endDate, name, description, weekDays, mp);
            Storage.MarkingPeriodStorage.Update(mp);
            return mp;
            
        }


        //TODO : think how to rewrite this for better performance
        //TODO : needs testing
        public bool ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            if (Storage.MarkingPeriodStorage.Exists(markingPeriodIds))
                throw new ChalkableException("Can't change markingPeriod week days ");
                
            Storage.MarkingPeriodStorage.ChangeWeekDays(markingPeriodIds, weekDays);
            return true;
            
        }

        //TODO: remove this method
        public bool ChangeMarkingPeriodsCount(int schoolYearId, int count, out string error)
        {
            throw new NotImplementedException();
        }


        public MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId)
        {
            return Storage.MarkingPeriodClassStorage.GetById(markingPeriodClassId);
        }
    

        public MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId)
        {
            return Storage.MarkingPeriodStorage.GetNextInYear(markingPeriodId);
        }
    }
}
