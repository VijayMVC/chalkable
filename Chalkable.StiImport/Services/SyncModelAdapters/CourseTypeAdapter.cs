using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class CourseTypeAdapter : SyncModelAdapter<CourseType>
    {
        public CourseTypeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<CourseType> entities)
        {
            var chalkableCourseTypes = entities.Select(courseType => new Data.School.Model.CourseType
            {
                Id = courseType.CourseTypeID,
                Name = courseType.Name,
                Description = courseType.Description,
                Code = courseType.Code,
                IsActive = courseType.IsActive,
                IsSystem = courseType.IsSystem,
                NCESCode = courseType.NCESCode,
                SIFCode = courseType.SIFCode,
                StateCode = courseType.StateCode
            }).ToList();
            ServiceLocatorSchool.CourseTypeService.Add(chalkableCourseTypes);
        }

        protected override void UpdateInternal(IList<CourseType> entities)
        {
            var chalkableCourseTypes = entities.Select(courseType => new Data.School.Model.CourseType
            {
                Id = courseType.CourseTypeID,
                Name = courseType.Name,
                Description = courseType.Description,
                Code = courseType.Code,
                IsActive = courseType.IsActive,
                IsSystem = courseType.IsSystem,
                NCESCode = courseType.NCESCode,
                SIFCode = courseType.SIFCode,
                StateCode = courseType.StateCode
            }).ToList();
            ServiceLocatorSchool.CourseTypeService.Edit(chalkableCourseTypes);
        }

        protected override void DeleteInternal(IList<CourseType> entities)
        {
            var courseTypes = entities.Select(x => new Data.School.Model.CourseType { Id = x.CourseTypeID }).ToList();
            ServiceLocatorSchool.CourseTypeService.Delete(courseTypes);
        }
    }

    public class SystemSettingAdapter : SyncModelAdapter<SystemSetting>
    {
        public SystemSettingAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<SystemSetting> entities)
        {
            var sysSettings = entities.Select(x => new Data.School.Model.SystemSetting
            {
                Category = x.Category,
                Setting = x.Setting,
                Value = x.Value
            }).ToList();
            ServiceLocatorSchool.SettingsService.AddSettings(sysSettings);
        }

        protected override void UpdateInternal(IList<SystemSetting> entities)
        {
            var sysSettings = entities.Select(x => new Data.School.Model.SystemSetting
            {
                Category = x.Category,
                Setting = x.Setting,
                Value = x.Value
            }).ToList();
            ServiceLocatorSchool.SettingsService.Edit(sysSettings);
        }

        protected override void DeleteInternal(IList<SystemSetting> entities)
        {
            var systemSettings = entities.Select(x => new Data.School.Model.SystemSetting
                    {
                        Category = x.Category,
                        Setting = x.Setting,
                        Value = x.Value
                    }).ToList();
            ServiceLocatorSchool.SettingsService.Delete(systemSettings);
        }
    }
}