using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class MealTypeDataAccess : DataAccessBase<MealType, int>
    {
        public MealTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<MealType> GetAll()
        {
            var dbQuery = new DbQuery();
            dbQuery.Sql.Append($"SELECT * FROM [{nameof(MealType)}] WHERE [{nameof(MealType.IsActive)}] = 1 ORDER BY [{nameof(MealType.Name)}]");
            return ReadMany<MealType>(dbQuery);
        }
    }
}
