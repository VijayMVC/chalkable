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
        MarkingPeriod GetById(string id);
    }

    public class MarkingPeriodService : SchoolServiceBase, IMarkingPeriodService
    {
        public MarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public MarkingPeriod GetById(string id)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow);
                return da.GetById(Guid.Parse(id));
            }
        }
    }
}
