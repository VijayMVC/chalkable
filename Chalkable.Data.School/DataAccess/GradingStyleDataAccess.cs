using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingStyleDataAccess : DataAccessBase<GradingStyle>
    {
        public GradingStyleDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void DeleteAll()
        {
            SimpleDelete<GradingStyle>(new Dictionary<string, object>());
        }
    }
}
