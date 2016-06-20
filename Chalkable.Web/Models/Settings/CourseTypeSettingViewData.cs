using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models.Settings
{
    public class CourseTypeSettingViewData
    {
        public ShortCourseTypeViewData CourseType { get; set; }
        public IList<StandardizedTestFilterViewData> StandardizedTestFilters { get; set; }

        public static CourseTypeSettingViewData Create(CourseTypeSetting settings, CourseType courseType)
        {
            return new CourseTypeSettingViewData
            {
                CourseType = ShortCourseTypeViewData.Create(courseType),
                StandardizedTestFilters = settings.StandardizedTestFilters.Select(StandardizedTestFilterViewData.Create).ToList()
            };
        }

        public static IList<CourseTypeSettingViewData> Create(IList<CourseTypeSetting> settings, IList<CourseType> courseTypes)
        {
            var res = new List<CourseTypeSettingViewData>();

            settings = settings.Where(x => courseTypes.Any(y => x.CourseTypeId == y.Id)).ToList();
            foreach (var courseTypeSetting in settings)
            {
                var courseType = courseTypes.First(x => x.Id == courseTypeSetting.CourseTypeId);
                res.Add(Create(courseTypeSetting, courseType));
            }

            return res;
        }
    }
}