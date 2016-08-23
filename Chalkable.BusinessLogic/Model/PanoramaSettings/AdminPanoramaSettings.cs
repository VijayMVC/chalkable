using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
