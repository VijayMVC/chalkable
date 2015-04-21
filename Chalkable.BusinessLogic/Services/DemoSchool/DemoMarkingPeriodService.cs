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
            return Storage.MarkingPeriodStorage.Update(markingPeriods);
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            return Storage.MarkingPeriodStorage.GetById(id);
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

        public IList<MarkingPeriod> GetMarkingPeriodsByDateRange(DateTime fromDate, DateTime toDate, int? schoolYearId)
        {
            var res = GetMarkingPeriods(schoolYearId).AsEnumerable();
            return res.Where(x => (x.StartDate <= fromDate && x.EndDate >= fromDate)
                                      || (x.StartDate <= toDate && x.EndDate >= toDate)).ToList();
        }
        
        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            return Storage.MarkingPeriodStorage.Add(markingPeriods);
        }

        public void DeleteMarkingPeriods(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.MarkingPeriodStorage.DeleteMarkingPeriods(markingPeriods.Select(x=>x.Id).ToList());
        }

    }
}
