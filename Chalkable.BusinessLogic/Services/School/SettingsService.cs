using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface ISettingsService
    {
        void AddSettings(IList<DistrictSettings> settings);
        void Edit(IList<DistrictSettings> settings);
        DistrictSettings GetSetting(string category, string setting);
        void Delete(IList<DistrictSettings> settings);
    }

    public class SettingsService : SchoolServiceBase, ISettingsService
    {
        public SettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddSettings(IList<DistrictSettings> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<DistrictSettings>(u).Insert(settings));
        }

        public void Edit(IList<DistrictSettings> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<DistrictSettings>(u).Update(settings));
        }

        public DistrictSettings GetSetting(string category, string setting)
        {
            var conds = new AndQueryCondition
            {
                {DistrictSettings.CATEGORY_FIELD, category, ConditionRelation.Equal},
                {DistrictSettings.SETTING_FIELD, setting, ConditionRelation.Equal},
            };

            return DoRead(u => new DataAccessBase<DistrictSettings>(u).GetAll(conds).First());
        }

        public void Delete(IList<DistrictSettings> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<DistrictSettings>(u).Delete(settings));
        }
    }
}
