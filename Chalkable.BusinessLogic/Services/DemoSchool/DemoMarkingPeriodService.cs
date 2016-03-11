using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
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

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            var mp = GetById(markingPeriodId);

            return
                data.Where(x => x.Value.SchoolYearRef == mp.SchoolYearRef && x.Value.StartDate > mp.StartDate)
                    .Select(x => x.Value)
                    .First();
        }
    }

    public class DemoMarkingPeriodClassStorage : BaseDemoIntStorage<MarkingPeriodClass>
    {
        public DemoMarkingPeriodClassStorage()
            : base(null, true)
        {
        }

        public MarkingPeriodClass GetMarkingPeriodClassOrNull(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            return GetMarkingPeriodClasses(markingPeriodClassQuery).FirstOrDefault();
        }

        public void Delete(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            var mpcs = GetMarkingPeriodClasses(markingPeriodClassQuery);
            foreach (var markingPeriodClass in mpcs)
            {
                var item = data.First(x => x.Value == markingPeriodClass);
                data.Remove(item.Key);
            }
        }

        private IEnumerable<MarkingPeriodClass> GetMarkingPeriodClasses(MarkingPeriodClassQuery markingPeriodClassQuery)
        {
            var mpcs = data.Select(x => x.Value);


            if (markingPeriodClassQuery.ClassId.HasValue)
                mpcs = mpcs.Where(x => x.ClassRef == markingPeriodClassQuery.ClassId);
            if (markingPeriodClassQuery.MarkingPeriodId.HasValue)
                mpcs = mpcs.Where(x => x.MarkingPeriodRef == markingPeriodClassQuery.MarkingPeriodId);

            return mpcs.ToList();
        }

        public bool Exists(int? classId, int? markingPeriodId)
        {
            var mpc = GetMarkingPeriodClasses(new MarkingPeriodClassQuery
            {
                ClassId = classId,
                MarkingPeriodId = markingPeriodId
            });

            return mpc.ToList().Count > 0;
        }

        public new void Delete(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            foreach (var mpc in markingPeriodClasses)
            {
                Delete(new MarkingPeriodClassQuery
                {
                    MarkingPeriodId = mpc.MarkingPeriodRef,
                    ClassId = mpc.ClassRef
                });
            }
        }

        public IEnumerable<MarkingPeriodClass> GetByClassId(int? classId)
        {
            return GetMarkingPeriodClasses(new MarkingPeriodClassQuery
            {
                ClassId = classId
            });
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            return GetMarkingPeriodClassOrNull(new MarkingPeriodClassQuery
            {
                MarkingPeriodId = markingPeriodId,
                ClassId = classId
            });
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
            return MarkingPeriodStorage.Update(markingPeriods);
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            return MarkingPeriodStorage.GetById(id);
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            return MarkingPeriodStorage.GetLast(tillDate ?? Context.NowSchoolTime);
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            return MarkingPeriodClassStorage.GetMarkingPeriodClass(classId, markingPeriodId);
        }

        public IEnumerable<MarkingPeriodClass> GetMarkingPeriodClassById(int classId)
        {
            return MarkingPeriodClassStorage.GetByClassId(classId);
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            return MarkingPeriodStorage.GetMarkingPeriods(schoolYearId);
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            var res = MarkingPeriodStorage.GetMarkingPeriod(date);
            if (res != null)
                return res;
            if (useLastExisting)
                return GetLastMarkingPeriod(date);
            return null;
        }

        public IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId)
        {
            var res = GetMarkingPeriods(schoolYearId).AsEnumerable();
            return res.Where(x => (x.StartDate <= fromDate && x.EndDate >= fromDate)
                                      || (x.StartDate <= toDate && x.EndDate >= toDate)).ToList();
        }

        public MarkingPeriod GetLastClassMarkingPeriod(int classId, DateTime? date)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            return MarkingPeriodStorage.Add(markingPeriods);
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            throw new NotImplementedException();
        }

        public void AddMarkingPeriodClasses(IList<MarkingPeriodClass> markingPeriodClasses)
        {
            MarkingPeriodClassStorage.Add(markingPeriodClasses);
        }
    }
}
