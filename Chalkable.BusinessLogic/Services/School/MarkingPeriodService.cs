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
        MarkingPeriod GetById(Guid id);
        MarkingPeriod GetLastMarkingPeriod(DateTime tillDate);
        MarkingPeriodClass GetMarkingPeriodClass(Guid classId, Guid markingPeriodId);
    }

    public class MarkingPeriodService : SchoolServiceBase, IMarkingPeriodService
    {
        public MarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public MarkingPeriod GetById(Guid id)
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
    }
}
