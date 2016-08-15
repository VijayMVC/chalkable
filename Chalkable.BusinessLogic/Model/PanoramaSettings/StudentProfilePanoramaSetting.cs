using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.PanoramaSettings
{
    public class StudentProfilePanoramaSetting : BaseSettingModel
    {
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }
}