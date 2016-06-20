using Chalkable.BusinessLogic.Model.PanoramaSettings;

namespace Chalkable.Web.Models.Settings
{
    public class ClassProfilePanoramaSettingsViewData : BasePanoramaSettingsViewData
    {
        protected ClassProfilePanoramaSettingsViewData(ClassProfilePanoramaSetting setting) : base(setting.SchoolYearIds, setting.StandardizedTestFilters)
        {
        }
        public static ClassProfilePanoramaSettingsViewData Create(ClassProfilePanoramaSetting setting)
        {
            return setting == null ? null : new ClassProfilePanoramaSettingsViewData(setting);
        }
    }
}