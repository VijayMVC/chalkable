﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models.Settings
{
    public class BasePanoramaSettingsViewData
    {
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilterViewData> StandardizedTestFilters { get; set; }

        protected BasePanoramaSettingsViewData(IList<int> schoolYearIds, IList<StandardizedTestFilter> filters)
        {
            SchoolYearIds = schoolYearIds;
            StandardizedTestFilters = StandardizedTestFilterViewData.Create(filters);
        }
    }
}