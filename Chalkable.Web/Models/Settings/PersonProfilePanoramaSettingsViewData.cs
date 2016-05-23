using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models.Settings
{
    public class PersonProfilePanoramaSettingsViewData : BasePanoramaSettingsViewData
    {
        public int? CourseType { get; set; }
        protected PersonProfilePanoramaSettingsViewData(PersonProfilePanoramaSettings settings) : base(settings.SchoolYearIds, settings.StandardizedTestFilters)
        {
        }
        public static PersonProfilePanoramaSettingsViewData Create(PersonProfilePanoramaSettings settings)
        {
            return new PersonProfilePanoramaSettingsViewData(settings)
            {
                CourseType = settings.CourseTypeId
            };
        }
    }
}