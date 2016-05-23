using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.Settings;

namespace Chalkable.Web.Models
{
    public class ClassPanoramaViewData: ShortClassViewData
    {
        public ClassProfilePanoramaSettingsViewData FilterSettings { get; set; }

        protected ClassPanoramaViewData(Class cClass) : base(cClass)
        {
        }
        
        public static ClassPanoramaViewData Create(Class cClass, ClassProfilePanoramaSettings filterSettings)
        {
            return new ClassPanoramaViewData(cClass)
            {
                FilterSettings = filterSettings != null ? ClassProfilePanoramaSettingsViewData.Create(filterSettings) : null
            };
        }


    }
}