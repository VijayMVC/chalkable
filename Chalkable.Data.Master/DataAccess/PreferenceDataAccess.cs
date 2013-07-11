using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class PreferenceDataAccess : DataAccessBase<Preference>
    {
        public PreferenceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Preference GetPreferenceOrNull(string key)
        {
            var conds = new Dictionary<string, object> {{"Key", key}};
            return  SelectOneOrNull<Preference>(conds);
        }
    }
}
