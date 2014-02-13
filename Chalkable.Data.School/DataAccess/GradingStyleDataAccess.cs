using System;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class GradingStyleDataAccess : DataAccessBase<GradingStyle, Guid>
    {
        public GradingStyleDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void DeleteAll()
        {
            SimpleDelete(new AndQueryCondition());
        }
    }
}
