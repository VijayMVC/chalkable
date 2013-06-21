using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.Master.Model;

namespace Chalkable.Data.Master.DataAccess
{
    public class PreferenceDataAccess : DataAccessBase
    {
        public PreferenceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void Create(Preference preference)
        {
            SimpleInsert(preference);
        }
        public void Update(Preference preference)
        {
            SimpleUpdate(preference);
        }

        public Preference GetPreferenceOrNull(string key)
        {
            var conds = new Dictionary<string, object> {{"Key", key}};
            return  SelectOneOrNull<Preference>(conds);
        }
        public IList<Preference> GetList()
        {
            return SelectMany<Preference>(new Dictionary<string, object>());
        }
    }
}
