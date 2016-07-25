using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Chalkable.Data.School.Model;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services.School.PanoramaSettings
{
    public interface IBasePanoramaSettingsHandler<out TSettings> where TSettings : BaseSettingModel
    {
        TSettings GetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId);
        void SetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId, BaseSettingModel settings);
        TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? classId);
    }
    public abstract class BasePanoramaSettingsHandler<TSettings> : IBasePanoramaSettingsHandler<TSettings> where TSettings : BaseSettingModel
    {
        public virtual TSettings GetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId)
        {
            var settings = serviceLocator.PersonSettingService.GetSettingsForPerson(new List<string> { SettingKey }, personId, null, classId);
            return settings.ContainsKey(SettingKey)
                ? JsonConvert.DeserializeObject<TSettings>(settings[SettingKey])
                : GetDefault(serviceLocator, classId);
        }
        public virtual void SetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId, BaseSettingModel settings)
        {
            var obj = JsonConvert.SerializeObject(settings);
            var dic = new Dictionary<string, object> { { SettingKey, obj } };
            serviceLocator.PersonSettingService.SetSettingsForPerson(dic, personId, null, classId);
        }
        public virtual TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? classId)
        {
            return default(TSettings);
        }
        protected abstract string SettingKey { get; }
    }

    public class DefaultPanoramaSettingHandler<TSettings> : BasePanoramaSettingsHandler<TSettings> where TSettings : BaseSettingModel
    {
        private readonly Func<IServiceLocatorSchool, int?, TSettings> _getDefaultAction;
        public DefaultPanoramaSettingHandler(string settingKey, Func<IServiceLocatorSchool, int?, TSettings> getDefaultAction = null)
        {
            SettingKey = settingKey;
            _getDefaultAction = getDefaultAction;
        }
        protected override string SettingKey { get; }
        public override TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? classId)
        {
            return _getDefaultAction?.Invoke(serviceLocator, classId);
        }
    }

    public class AdminPanoramaSettingsHandler : BasePanoramaSettingsHandler<AdminPanoramaSettings>
    {
        protected override string SettingKey => PersonSetting.ADMIN_PANORAMA_SETTINGS;
        public override AdminPanoramaSettings GetDefault(IServiceLocatorSchool serviceLocator, int? classId)
        {
            return new AdminPanoramaSettings
            {
                PreviousYearsCount = 1,
                StudentDefaultSettings = new List<StandardizedTestFilter>(),
                CourseTypeDefaultSettings = new List<CourseTypeSetting>()
            };
        }

        public override AdminPanoramaSettings GetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId)
        {
            return base.GetSettings(serviceLocator, null, null);
        }

        public override void SetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId, BaseSettingModel settings)
        {
            base.SetSettings(serviceLocator, null, null, settings);
        }
    }
}
