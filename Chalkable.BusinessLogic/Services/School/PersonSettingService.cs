using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Chalkable.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.DataAccess;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPersonSettingService
    {
        void SetSettingsForPerson(int personId, int schoolYearId, IDictionary<string, object> settings);
        IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId);
        IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId, IList<string> keys);
    }

    class PersonSettingService : SchoolServiceBase, IPersonSettingService
    {
        private static HashSet<string> availableSettings; 
        static PersonSettingService()
        {
            availableSettings = new HashSet<string>();

            var consts = typeof(PersonSetting).GetFields(BindingFlags.Public | BindingFlags.Static |
               BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly).ToList();
            foreach (var setting in consts)
                availableSettings.Add(setting.GetValue(setting).ToString());
        }
        public PersonSettingService(IServiceLocatorSchool serviceLocator) : base(serviceLocator){ }
        private bool IsAvailableSettings(IList<string> settings)
        {
            return settings.All(x => availableSettings.Contains(x));
        }
        public void SetSettingsForPerson(int personId, int schoolYearId, IDictionary<string, object> settings)
        {
            var keys = settings.Keys.ToList();
            if (!IsAvailableSettings(keys))
                throw new ArgumentException("Not recognized settings");
            
            var ps = GetSettingsForPerson(personId, schoolYearId, keys);
            var toSet = ps.Select( x => new PersonSetting
                        {
                            PersonRef = personId,
                            SchoolYearRef = schoolYearId,
                            Key = x.Key,
                            Value = x.Value
                        }).ToList();

            foreach (var set in toSet)
            {
                set.Value = (settings[set.Key] as DateTime?)?
                    .ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture) 
                    ?? settings[set.Key]?.ToString();
            }
            DoUpdate(u => new PersonSettingDataAccess(u).Update(toSet));

            settings = settings.Where(s => ps.All(x => x.Key != s.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            DoUpdate(u => new PersonSettingDataAccess(u).AddPersonSettings(personId, schoolYearId, settings));
        }
        public IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId)
        {
            return DoRead(u => new PersonSettingDataAccess(u).GetPersonSettings(personId, schoolYearId, new List<string>()));
        }
        public IDictionary<string , string> GetSettingsForPerson(int personId, int schoolYearId, IList<string> keys)
        {
            return DoRead(u => new PersonSettingDataAccess(u).GetPersonSettings(personId, schoolYearId, keys));
        }
    }
}
