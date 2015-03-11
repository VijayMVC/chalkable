using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class DayTypeDataAccess : DataAccessBase<DayType, int>
    {
        public DayTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Delete(IList<DayType> dayTypes)
        {
            SimpleDelete(dayTypes);
        }
    }
}
