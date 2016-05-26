using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.PanoramaSettings
{
    public class ClassProfilePanoramaSetting : BaseSettingModel
    {
        public int ClassId { get; set; }
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }
}
