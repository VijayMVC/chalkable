using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public  class GradingScaleDataAccess : BaseSchoolDataAccess<GradingScale>
    {
        public GradingScaleDataAccess(UnitOfWork unitOfWork, int? localSchoolId) : base(unitOfWork, localSchoolId)
        {
        }

        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new GradingScale {Id = x}).ToList());
        }
    }
}
