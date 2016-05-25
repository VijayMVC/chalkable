﻿using System;
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


        private static IDictionary<Type, IBasePanoramaSettingsHandler<BaseSettingModel>> _settingHandlers = new Dictionary
            <Type, IBasePanoramaSettingsHandler<BaseSettingModel>>
        {
            {
                typeof (ClassProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<ClassProfilePanoramaSetting>(
                    PersonSetting.CLASS_PROFILE_PANORAMA_SETTING)
            },
            {
                typeof (ClassProfilePanoramaSetting),
                new DefaultPanoramaSettingHandler<ClassProfilePanoramaSetting>(
                    PersonSetting.STUDENT_PROFILE_PANORAMA_SETTING)
            }
        };

        private IBasePanoramaSettingsHandler<BaseSettingModel> GetSettingHandler(Type settingType)
        {
            if(!_settingHandlers.ContainsKey(settingType))
                throw new ChalkableException();
            return _settingHandlers[settingType];
        }
        
        private TSettings InternalGet<TSettings>(int? personId, int? classId) where TSettings : BaseSettingModel
        {
            return (GetSettingHandler(typeof(TSettings)).GetSettings(ServiceLocator, personId, classId)) as TSettings;
        }
        private void InternalSet<TSettings>(TSettings settings, int? personId, int? classId) where TSettings : BaseSettingModel
        {
            GetSettingHandler(typeof(TSettings)).SetSettings(ServiceLocator, personId, classId, settings);
        }


        public void Save<TSettings>(TSettings settings, int? classId) where TSettings: BaseSettingModel
        {
            InternalSet(settings, Context.PersonId, classId);
        }
        public TSettings Get<TSettings>(int? classId) where TSettings: BaseSettingModel
        {
            return InternalGet<TSettings>(Context.PersonId, classId);
        }
        public TSettings Restore<TSettings>(int? classId) where TSettings : BaseSettingModel
        {
            var handler = GetSettingHandler(typeof (TSettings));
            var res = (handler.GetDefault(ServiceLocator, Context.PersonId, classId) as TSettings);
            handler.SetSettings(ServiceLocator, Context.PersonId, classId, res);
            return res;
        }
        public void SaveDefault<TDefaultSettings>(TDefaultSettings settings) where TDefaultSettings : BaseSettingModel
        {
            InternalSet(settings, null, null);
        }
        public TDefaultSettings GetDefaultSettings<TDefaultSettings>() where TDefaultSettings : BaseSettingModel
        {
            return InternalGet<TDefaultSettings>(null, null);
        }
    }


 

}
