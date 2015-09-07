using System;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;

namespace Chalkable.Data.Master.DataAccess
{
    public class PreferenceDataAccess : DataAccessBase<Preference, Guid>
    {
        public PreferenceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Preference GetPreferenceOrNull(string key)
        {
            var conds = new AndQueryCondition {{"Key", key}};
            return  SelectOneOrNull<Preference>(conds);
        }
    }
}
