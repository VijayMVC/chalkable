﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School.PanoramaSettings
{

    public interface IPanoramaSettingsService
    {
        void Save<TSettings>(TSettings settings, int? classId) where TSettings : BaseSettingModel;
        TSettings Get<TSettings>(int? classId) where TSettings : BaseSettingModel;
        TSettings Restore<TSettings>(int? classId) where TSettings : BaseSettingModel;
        void SaveDefault<TDefaultSettings>(TDefaultSettings settings) where TDefaultSettings : BaseSettingModel;
        TDefaultSettings GetDefaultSettings<TDefaultSettings>() where TDefaultSettings : BaseSettingModel;
    }

    public class PanoramaSettingsService : SchoolServiceBase, IPanoramaSettingsService
    {
        public PanoramaSettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        private static ClassProfilePanoramaSetting GetDefaultClassPanoramaSettings(IServiceLocatorSchool serviceLocator, int? classId)
        {
            var currentSchoolYear = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            if (!classId.HasValue)
                return new ClassProfilePanoramaSetting {SchoolYearIds = new List<int> {currentSchoolYear.Id}};

            var adminPanoramaSettings = serviceLocator.PanoramaSettingsService.Get<AdminPanoramaSettings>(null);
            var @class = serviceLocator.ClassService.GetById(classId.Value);
            
            var previousSchoolYear = serviceLocator.SchoolYearService.GetPreviousSchoolYears(adminPanoramaSettings.PreviousYearsCount);

            var res = new ClassProfilePanoramaSetting
            {
                SchoolYearIds = previousSchoolYear.Select(x => x.Id).ToList(),
                StandardizedTestFilters = adminPanoramaSettings.CourseTypeDefaultSettings.FirstOrDefault(x => x.CourseTypeId == @class.Id)?.StandardizedTestFilters
                    ?? new List<StandardizedTestFilter>()
            };

            res.SchoolYearIds.Add(currentSchoolYear.Id);

            return res;
        }

        private static StudentProfilePanoramaSetting GetDefaultStudentPanoramaSettings(IServiceLocatorSchool serviceLocator, int? classId)
        {
            var currentSchoolYear = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            var adminPanoramaSettings = serviceLocator.PanoramaSettingsService.Get<AdminPanoramaSettings>(null);
            var previousSchoolYear = serviceLocator.SchoolYearService.GetPreviousSchoolYears(adminPanoramaSettings.PreviousYearsCount);

            var res = new StudentProfilePanoramaSetting
            {
                SchoolYearIds = previousSchoolYear.Select(x => x.Id).ToList(),
                StandardizedTestFilters = adminPanoramaSettings.StudentDefaultSettings
                    ?? new List<StandardizedTestFilter>()
            };

            res.SchoolYearIds.Add(currentSchoolYear.Id);

            return res;
        }

        private static readonly IDictionary<Type, IBasePanoramaSettingsHandler<BaseSettingModel>> _settingHandlers = new Dictionary
            <Type, IBasePanoramaSettingsHandler<BaseSettingModel>>
        {
            {
                typeof (ClassProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<ClassProfilePanoramaSetting>(PersonSetting.CLASS_PROFILE_PANORAMA_SETTING, GetDefaultClassPanoramaSettings)
            },
            {
                typeof (StudentProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<StudentProfilePanoramaSetting>(PersonSetting.STUDENT_PROFILE_PANORAMA_SETTING, GetDefaultStudentPanoramaSettings)
            },
            {
                typeof(AdminPanoramaSettings),
                new AdminPanoramaSettingsHandler(PersonSetting.ADMIN_PANORAMA_SETTINGS)
            }
        };

        private IBasePanoramaSettingsHandler<TSettings> GetSettingHandler<TSettings>() where TSettings : BaseSettingModel
        {
            if(!_settingHandlers.ContainsKey(typeof(TSettings)))
                throw new ChalkableException("Panorama Settings Handler not found");
            return _settingHandlers[typeof(TSettings)] as IBasePanoramaSettingsHandler<TSettings>;
        }
        
        private TSettings GetInternal<TSettings>(int? personId, int? classId) where TSettings : BaseSettingModel
        {
            return GetSettingHandler<TSettings>().GetSettings(ServiceLocator, personId, classId);
        }
        private void SetInternal<TSettings>(TSettings settings, int? personId, int? classId) where TSettings : BaseSettingModel
        {
            GetSettingHandler<TSettings>().SetSettings(ServiceLocator, personId, classId, settings);
        }

        public void Save<TSettings>(TSettings settings, int? classId) where TSettings: BaseSettingModel
        {
            SetInternal(settings, Context.PersonId, classId);
        }
        public TSettings Get<TSettings>(int? classId) where TSettings: BaseSettingModel
        {
            return GetInternal<TSettings>(Context.PersonId, classId);
        }
        public TSettings Restore<TSettings>(int? classId) where TSettings : BaseSettingModel
        {
            var handler = GetSettingHandler<TSettings>();
            var res = handler.GetDefault(ServiceLocator, classId);
            handler.SetSettings(ServiceLocator, Context.PersonId, classId, res);
            return res;
        }
        public void SaveDefault<TDefaultSettings>(TDefaultSettings settings) where TDefaultSettings : BaseSettingModel
        {
            SetInternal(settings, null, null);
        }
        public TDefaultSettings GetDefaultSettings<TDefaultSettings>() where TDefaultSettings : BaseSettingModel
        {
            return GetSettingHandler<TDefaultSettings>().GetDefault(ServiceLocator, null);
        }
    }
}
