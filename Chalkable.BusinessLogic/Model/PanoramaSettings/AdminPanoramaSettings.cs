using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.PanoramaSettings
{
    public class AdminPanoramaSettings : BaseSettingModel
    {
        public int PreviousYearsCount { get; set; }
        public IList<CourseTypeSetting> CourseTypeDefaultSettings { get; set; }
        public IList<StandardizedTestFilter> StudentDefaultSettings { get; set; }
    }

    public class CourseTypeSetting
    {
        public int CourseTypeId { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }
}
