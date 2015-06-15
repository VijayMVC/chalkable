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
        void AddSettings(IList<SystemSetting> settings);
        void Edit(IList<SystemSetting> settings);
        SystemSetting GetSetting(string category, string setting);
        void Delete(IList<SystemSetting> settings);
    }

    public class SettingsService : SchoolServiceBase, ISettingsService
    {
        public SettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddSettings(IList<SystemSetting> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SystemSetting>(u).Insert(settings));
        }

        public void Edit(IList<SystemSetting> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SystemSetting>(u).Update(settings));
        }

        public SystemSetting GetSetting(string category, string setting)
        {
            var conds = new AndQueryCondition
            {
                {SystemSetting.CATEGORY_FIELD, category, ConditionRelation.Equal},
                {SystemSetting.SETTING_FIELD, setting, ConditionRelation.Equal},
            };

            return DoRead(u => new DataAccessBase<SystemSetting>(u).GetAll(conds).FirstOrDefault());
        }

        public void Delete(IList<SystemSetting> settings)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SystemSetting>(u).Delete(settings));
        }
    }
}
