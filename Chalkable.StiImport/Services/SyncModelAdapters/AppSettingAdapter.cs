using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    class AppSettingAdapter : SyncModelAdapter<AppSetting>
    {
        public AppSettingAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.AppSetting Selector(AppSetting x)
        {
            return new Data.School.Model.AppSetting
            {
                Name = x.Name,
                Value = x.Value
            };
        }

        protected override void InsertInternal(IList<AppSetting> entities)
        {
            var appSetting = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AppSettingService.AddAppSettings(appSetting);
        }

        protected override void UpdateInternal(IList<AppSetting> entities)
        {
            var appSetting = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AppSettingService.EditAppSettings(appSetting);
        }

        protected override void DeleteInternal(IList<AppSetting> entities)
        {
            var appSetting = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AppSettingService.DeleteAppSettings(appSetting);
        }

        protected override void PrepareToDeleteInternal(IList<AppSetting> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
