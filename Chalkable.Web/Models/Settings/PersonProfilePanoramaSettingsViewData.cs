using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models.Settings
{
    public class PersonProfilePanoramaSettingViewData : BasePanoramaSettingsViewData
    {
        public int? CourseType { get; set; }
        protected PersonProfilePanoramaSettingViewData(StudentProfilePanoramaSetting settings) : base(settings.SchoolYearIds, settings.StandardizedTestFilters)
        {
        }
        public static PersonProfilePanoramaSettingViewData Create(StudentProfilePanoramaSetting settings)
        {
            return new PersonProfilePanoramaSettingViewData(settings);
        }
    }
}