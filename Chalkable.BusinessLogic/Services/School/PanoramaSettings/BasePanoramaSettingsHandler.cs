using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model.PanoramaSettings;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services.School.PanoramaSettings
{
    public interface IBasePanoramaSettingsHandler<out TSettings> where TSettings : BaseSettingModel
    {
        TSettings GetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId);
        void SetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId, BaseSettingModel settings);
        TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? personId, int? classId);
    }
    public abstract class BasePanoramaSettingsHandler<TSettings> : IBasePanoramaSettingsHandler<TSettings> where TSettings : BaseSettingModel
    {
        public virtual TSettings GetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId)
        {
            var settings = serviceLocator.PersonSettingService.GetSettingsForPerson(new List<string> { SettingKey }, personId, null, classId);
            return settings.ContainsKey(SettingKey)
                ? JsonConvert.DeserializeObject<TSettings>(settings[SettingKey])
                : GetDefault(serviceLocator, personId, classId);
        }
        public virtual void SetSettings(IServiceLocatorSchool serviceLocator, int? personId, int? classId, BaseSettingModel settings)
        {
            var obj = JsonConvert.SerializeObject(settings);
            var dic = new Dictionary<string, object> { { SettingKey, obj } };
            serviceLocator.PersonSettingService.SetSettingsForPerson(dic, personId, null, classId);
        }
        public virtual TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? personId, int? classId)
        {
            return default(TSettings);
        }
        protected abstract string SettingKey { get; }
    }

    public class DefaultPanoramaSettingHandler<TSettings> : BasePanoramaSettingsHandler<TSettings> where TSettings : BaseSettingModel
    {
        private readonly Func<IServiceLocatorSchool, int?, int?, TSettings> _getDefaultAction;
        public DefaultPanoramaSettingHandler(string settingKey, Func<IServiceLocatorSchool, int?, int?, TSettings> getDefaultAction = null)
        {
            SettingKey = settingKey;
            _getDefaultAction = getDefaultAction;
        }
        protected override string SettingKey { get; }
        public override TSettings GetDefault(IServiceLocatorSchool serviceLocator, int? personId, int? classId)
        {
            return _getDefaultAction?.Invoke(serviceLocator, personId, classId);
        }

    }
}
