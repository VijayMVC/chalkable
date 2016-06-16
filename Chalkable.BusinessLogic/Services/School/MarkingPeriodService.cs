using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IMarkingPeriodService
    {
        IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods);
        void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods);
        IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods); 
        MarkingPeriod GetMarkingPeriodById(int id);
        MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null);
        MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId);
        IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
        IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId);
        MarkingPeriod GetLastClassMarkingPeriod(int classId, DateTime? date);
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
                var da = new MarkingPeriodDataAccess(uow);
                return da.GetById(id);
            }
        }

        public IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId)
        {
            var res = GetMarkingPeriods(schoolYearId);
            return res.Where(x =>(x.StartDate <= fromDate && x.EndDate >= fromDate)
                            || (x.StartDate <= toDate && x.EndDate >= toDate)).ToList();
        }

        public MarkingPeriod GetLastClassMarkingPeriod(int classId, DateTime? date)
        {
            return DoRead(u => new MarkingPeriodDataAccess(u).GetLastClassMarkingPeriod(classId, date));
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                return  da.GetLast(tillDate ?? Context.NowSchoolYearTime, Context.SchoolYearId.Value);
            }
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow)
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
                return new MarkingPeriodDataAccess(uow).GetMarkingPeriods(schoolYearId);
            }
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                var res = da.GetMarkingPeriod(date, Context.SchoolYearId);
                if (res != null)
                    return res;
                if (useLastExisting)
                    return GetLastMarkingPeriod(date);
            }
            return null;
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                if (!BaseSecurity.IsDistrictAdmin(Context))
                    throw new ChalkableSecurityException();
                new MarkingPeriodDataAccess(uow).DeleteMarkingPeriods(markingPeriods);
                uow.Commit();
            }
        }

        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodDataAccess(uow).Insert(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }


        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new MarkingPeriodDataAccess(uow).Update(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }

        
    }
}
