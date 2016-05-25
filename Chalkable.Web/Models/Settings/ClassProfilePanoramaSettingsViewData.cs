﻿using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.Web.Models.Settings
{
    public class ClassProfilePanoramaSettingsViewData : BasePanoramaSettingsViewData
    {
        protected ClassProfilePanoramaSettingsViewData(ClassProfilePanoramaSetting setting) : base(setting.SchoolYearIds, setting.StandardizedTestFilters)
        {
        }
        public static ClassProfilePanoramaSettingsViewData Create(ClassProfilePanoramaSetting setting)
        {
            return new ClassProfilePanoramaSettingsViewData(setting);
        }

    }
}