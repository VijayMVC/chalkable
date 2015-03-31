using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoMarkingPeriodStorage : BaseDemoIntStorage<MarkingPeriod>
    {
        public DemoMarkingPeriodStorage()
            : base(x => x.Id)
        {
        }

        public MarkingPeriod GetLast(DateTime tillDate)
        {
            return data.First(x => x.Value.StartDate <= tillDate).Value;
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            return data.Where(x => x.Value.SchoolYearRef == schoolYearId).Select(x => x.Value).ToList();
        }

        public MarkingPeriod GetMarkingPeriod(DateTime date)
        {
            return data.Where(x => x.Value.StartDate <= date && x.Value.EndDate >= date).Select(x => x.Value).First();
        }

        public bool IsOverlaped(int id, DateTime startDate, DateTime endDate, int? i)
        {
            return false;
        }

        public bool Exists(IList<int> markingPeriodIds)
        {
            return data.Any(x => markingPeriodIds.Contains(x.Key));
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            Delete(markingPeriods);
        }

        public void ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            var mp = GetById(markingPeriodId);

            return
                data.Where(x => x.Value.SchoolYearRef == mp.SchoolYearRef && x.Value.StartDate > mp.StartDate)
                    .Select(x => x.Value)
                    .First();
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting)
        {
            var res = GetMarkingPeriod(date);
            if (res != null)
                return res;
            if (useLastExisting)
                return GetLastMarkingPeriod(date);
            return null;
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate)
        {
            return GetLast(tillDate ?? Context.NowSchoolTime);
        }

        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Update(markingPeriods);
        }

        public IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId)
        {
            var res = GetMarkingPeriods(schoolYearId).AsEnumerable();
            return res.Where(x => (x.StartDate <= fromDate && x.EndDate >= fromDate)
                                      || (x.StartDate <= toDate && x.EndDate >= toDate)).ToList();
        }

        public IList<MarkingPeriod> AddMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Add(markingPeriods);
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            return GetById(id);
        }
    }   

    public class DemoMarkingPeriodService : DemoSchoolServiceBase, IMarkingPeriodService
    {
        private DemoMarkingPeriodStorage MarkingPeriodStorage { get; set; }
        private DemoMarkingPeriodClassStorage MarkingPeriodClassStorage { get; set; }

        public DemoMarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            MarkingPeriodStorage = new DemoMarkingPeriodStorage();
            MarkingPeriodClassStorage = new DemoMarkingPeriodClassStorage();
        }

        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            return MarkingPeriodStorage.Edit(markingPeriods);
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            return MarkingPeriodStorage.GetMarkingPeriodById(id);
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            return MarkingPeriodStorage.GetLastMarkingPeriod(tillDate);
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            return MarkingPeriodClassStorage.GetMarkingPeriodClass(classId, markingPeriodId);
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            return MarkingPeriodStorage.GetMarkingPeriods(schoolYearId);
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            return MarkingPeriodStorage.GetMarkingPeriodByDate(date, useLastExisting);
        }

        public IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId)
        {
            return MarkingPeriodStorage.GetMarkingPeriodsByDateRange(fromDate, toDate, schoolYearId);
        }
        
        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            return MarkingPeriodStorage.AddMarkingPeriods(markingPeriods);
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            MarkingPeriodStorage.DeleteMarkingPeriods(markingPeriods);
        }

    }
}
