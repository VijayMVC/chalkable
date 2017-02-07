using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SystemSettingAdapter : SyncModelAdapter<SystemSetting>
    {
        public SystemSettingAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.SystemSetting Selector(SystemSetting x)
        {
            return new Data.School.Model.SystemSetting
            {
                Category = x.Category,
                Setting = x.Setting,
                Value = x.Value
            };
        }

        protected override void InsertInternal(IList<SystemSetting> entities)
        {
            var sysSettings = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SettingsService.AddSettings(sysSettings);
        }

        protected override void UpdateInternal(IList<SystemSetting> entities)
        {
            var sysSettings = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SettingsService.Edit(sysSettings);
        }

        protected override void DeleteInternal(IList<SystemSetting> entities)
        {
            var systemSettings = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SettingsService.Delete(systemSettings);
        }

        protected override void PrepareToDeleteInternal(IList<SystemSetting> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}