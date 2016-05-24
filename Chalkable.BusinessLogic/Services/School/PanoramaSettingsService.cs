using System;
using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.Data.School.Model;
using Newtonsoft.Json;

namespace Chalkable.BusinessLogic.Services.School
{

    public interface IPanoramaSettingsService
    {
        void Save<TSettings>(TSettings settings, int? classId) where TSettings : IBasePanoramaSetting;
        TSettings Get<TSettings>(int? classId);
        TSettings Restore<TSettings>(int? classId);
        void SaveDefault<TDefaultSettings>();
        TDefaultSettings GetDefaultSettings<TDefaultSettings>();
        ClassProfilePanoramaSetting GetClassPanoramaSettings(int classId);

    }

    public class PanoramaSettingsService : SchoolServiceBase, IPanoramaSettingsService
    {
        public PanoramaSettingsService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Save<TSettings>(TSettings settings, int? classId) where TSettings: IBasePanoramaSetting
        {
            InternalSet(settings, Context.PersonId,classId);
        }
        public TSettings Get<TSettings>(int? classId)
        {
            return default(TSettings);
            //return InternalGet<TSettings>(Context.PersonId, classId);
            //if (res == null)
            //{
            //    GetDefaultSettings<>()
            //}
            //return res;
            throw new System.NotImplementedException();
        }
        public TSettings Restore<TSettings>(int? classId)
        {
            return default(TSettings);
            throw new System.NotImplementedException();
        }
        public void SaveDefault<TDefaultSettings>()
        {
            throw new System.NotImplementedException();
        }
        public TDefaultSettings GetDefaultSettings<TDefaultSettings>()
        {
            throw new System.NotImplementedException();
        }

        public ClassProfilePanoramaSetting GetClassPanoramaSettings(int classId)
        {
            return Get<ClassProfilePanoramaSetting>(classId);
//          var c = ServiceLocator.ClassService.GetById(classId);
//          throw new System.NotImplementedException();
        }

        private TSettings InternalGet<TSettings>(int? personId, int? classId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var key = nameof(TSettings).ToLower();
            var settings = ServiceLocator.PersonSettingService.GetSettingsForPerson(new List<string> { key }, personId, classId);
            return settings.ContainsKey(key) ? JsonConvert.DeserializeObject<TSettings>(settings[key]) : default(TSettings);
        }

        private void InternalSet<TSettings>(TSettings settings, int? personId, int? classId) where TSettings : IBasePanoramaSetting
        {
            var key = settings.Key;
            var obj = JsonConvert.SerializeObject(settings);
            var dic = new Dictionary<string, object> {{key, obj}};
            ServiceLocator.PersonSettingService.SetSettingsForPerson(dic, personId, null, classId);
        }
    }


    public abstract class BasePanoramaSettingsHandler<TSetting>
    {
        public abstract string SettigKey { get; }
        public abstract TSetting GetSetting(int? personId, int? classId, string key);
        public abstract TSetting SetSetting(int? personId, int? classId, TSetting setting);

    }

    public  interface IBasePanoramaSetting
    {
        string Key { get; }
    }

    //public abstract class BasePanoramaSetting : IBasePanoramaSetting
    //{
    //    public abstract IList<string> Keys { get; }
    //    public override string ToString()
    //    {
    //        return JsonConvert.SerializeObject(this);
    //    }
        
    //}

    public class ClassProfilePanoramaSetting : IBasePanoramaSetting
    {
        public int ClassId { get; set; }
        public IList<int> SchoolYearIds { get; set; } 
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
        //public override IList<string> Keys => new List<string>{PersonSetting.CLASS_PROFILE_PANORAMA_SETTING};
        public string Key => PersonSetting.CLASS_PROFILE_PANORAMA_SETTING;
    }

    public class StudentProfilePanoramaSetting : IBasePanoramaSetting
    {
        public int CourseTypeId { get; set; }
        public IList<int> SchoolYearIds { get; set; }
        public IList<StandardizedTestFilter> StandardizedTestFilters { get; set; }
        //public override IList<string> Keys => new List<string> {PersonSetting.STUDENT_PROFILE_PANORAMA_SETTING};
        public string Key => PersonSetting.STUDENT_PROFILE_PANORAMA_SETTING;
    }

    public class StandardizedTestFilter
    {
        public int StandardizedTestId { get; set; }
        public int ComponentId { get; set; }
        public int ScoreTypeId { get; set; }
    }
}
