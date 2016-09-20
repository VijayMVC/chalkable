using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.Settings
{
    public class AdminPanoramaSettingsViewData
    {
        public int PreviousYearsCount { get; set; }
        public IList<CourseTypeSettingViewData> CourseTypeDefaultSettings { get; set; }
        public IList<StandardizedTestFilterViewData> StudentDefaultSettings { get; set; }

        public static AdminPanoramaSettingsViewData Create(AdminPanoramaSettings settings, IList<CourseType> courseTypes)
        {
            return new AdminPanoramaSettingsViewData
            {
                PreviousYearsCount = settings.PreviousYearsCount,
                StudentDefaultSettings = settings.StudentDefaultSettings?.Select(StandardizedTestFilterViewData.Create).ToList(),
                CourseTypeDefaultSettings = CourseTypeSettingViewData.Create(settings.CourseTypeDefaultSettings, courseTypes)
            };
        }
    }
}