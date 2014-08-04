using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IMarkingPeriodService
    {
        IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods);
        void Delete(int id);
        void DeleteMarkingPeriods(IList<int> ids);
        IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods); 
        MarkingPeriod GetMarkingPeriodById(int id);
        MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null);
        MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId);
        IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
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
                return  da.GetLast(tillDate ?? Context.NowSchoolYearTime, Context.SchoolYearId);
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
                var res = da.GetMarkingPeriod(date, Context.SchoolYearId);
                if (res != null)
                    return res;
                if (useLastExisting)
                    return GetLastMarkingPeriod(date);
            }
            return null;
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
