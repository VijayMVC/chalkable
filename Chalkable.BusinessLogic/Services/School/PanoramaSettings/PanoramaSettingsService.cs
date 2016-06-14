using System;
using System.Collections.Generic;
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


        private static readonly IDictionary<Type, IBasePanoramaSettingsHandler<BaseSettingModel>> _settingHandlers = new Dictionary
            <Type, IBasePanoramaSettingsHandler<BaseSettingModel>>
        {
            {
                typeof (ClassProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<ClassProfilePanoramaSetting>(
                    PersonSetting.CLASS_PROFILE_PANORAMA_SETTING)
            },
            {
                typeof (StudentProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<StudentProfilePanoramaSetting>(
                    PersonSetting.STUDENT_PROFILE_PANORAMA_SETTING)
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
            var res = handler.GetDefault(ServiceLocator, Context.PersonId, classId);
            handler.SetSettings(ServiceLocator, Context.PersonId, classId, res);
            return res;
        }
        public void SaveDefault<TDefaultSettings>(TDefaultSettings settings) where TDefaultSettings : BaseSettingModel
        {
            SetInternal(settings, null, null);
        }
        public TDefaultSettings GetDefaultSettings<TDefaultSettings>() where TDefaultSettings : BaseSettingModel
        {
            return GetInternal<TDefaultSettings>(null, null);
        }
    }


 

}
