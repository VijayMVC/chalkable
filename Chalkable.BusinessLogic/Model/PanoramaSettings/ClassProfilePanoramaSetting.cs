﻿using System.Collections.Generic;

namespace Chalkable.BusinessLogic.Model.PanoramaSettings
{
    public class ClassProfilePanoramaSetting : BaseSettingModel
    {
        public IList<int> AcadYears { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
    }
}
