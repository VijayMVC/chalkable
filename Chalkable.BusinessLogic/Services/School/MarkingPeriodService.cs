using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IMarkingPeriodService
    {
        MarkingPeriod GetMarkingPeriodById(Guid id);
        MarkingPeriod GetLastMarkingPeriod(DateTime tillDate);
        MarkingPeriodClass GetMarkingPeriodClass(Guid classId, Guid markingPeriodId);
        IList<MarkingPeriod> GetMarkingPeriods(Guid schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
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
                var da = new MarkingPeriodClassDataAccess(uow);
                return  da.Get(classId, markingPeriodId);
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
    }
}
