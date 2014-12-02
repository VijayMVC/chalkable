using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class BellScheduleDataAccess : BaseSchoolDataAccess<BellSchedule>
    {
        public BellScheduleDataAccess(UnitOfWork unitOfWork, int? localSchoolId)
            : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<BellSchedule> bellSchedules)
        {
            SimpleDelete(bellSchedules);
        }
    }
}