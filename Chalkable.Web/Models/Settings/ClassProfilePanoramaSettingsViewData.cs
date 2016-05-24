using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models.Settings
{
    public class ClassProfilePanoramaSettingsViewData : BasePanoramaSettingsViewData
    {
        protected ClassProfilePanoramaSettingsViewData(ClassProfilePanoramaSettings settings) : base(settings.SchoolYearIds, settings.StandardizedTestFilters)
        {
        }
        public static ClassProfilePanoramaSettingsViewData Create(ClassProfilePanoramaSettings settings)
        {
            return new ClassProfilePanoramaSettingsViewData(settings);
        }

    }
}