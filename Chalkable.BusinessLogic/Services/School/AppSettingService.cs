using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAppSettingService
    {
        void AddAppSettings(IList<AppSetting> appSettings);
        void EditAppSettings(IList<AppSetting> appSettings);
        void DeleteAppSettings(IList<AppSetting> appSettings);
        AppSetting GetByName(string name);
        int? GetLoginTimeOut();
    }

    class AppSettingService : SchoolServiceBase, IAppSettingService
    {
        public AppSettingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAppSettings(IList<AppSetting> appSettings)
        {
            DoUpdate(u => new DataAccessBase<AppSetting>(u).Insert(appSettings));
        }

        public void EditAppSettings(IList<AppSetting> appSettings)
        {
            DoUpdate(u => new DataAccessBase<AppSetting>(u).Update(appSettings));
        }

        public void DeleteAppSettings(IList<AppSetting> appSettings)
        {
            DoUpdate(u => new DataAccessBase<AppSetting>(u).Delete(appSettings));
        }

        public AppSetting GetByName(string name)
        {
            return DoRead(u => new DataAccessBase<AppSetting, string>(u).GetByIdOrNull(name));
        }

        public int? GetLoginTimeOut()
        {
            int temp;
            return int.TryParse(GetByName("LoginTimeOut")?.Value, out temp) ? temp : (int?) null;
        }
    }
}
