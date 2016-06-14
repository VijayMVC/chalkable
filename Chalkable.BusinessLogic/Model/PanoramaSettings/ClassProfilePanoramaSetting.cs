using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.PanoramaSettings
{
    public class ClassProfilePanoramaSetting : BaseSettingModel
    {
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }
}
