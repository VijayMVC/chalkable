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
        void SetSettingsForPerson(int personId, IDictionary<string, object> settings);
        IList<PersonSetting> GetSettingsForPerson(int personId);
        IList<PersonSetting> GetSettingsForPerson(int personId, IList<string> keys);
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
        public void SetSettingsForPerson(int personId, IDictionary<string, object> settings)
        {
            var keys = settings.Keys.ToList();
            if (!IsAvailableSettings(keys))
                throw new ArgumentException("Not recognized settings");

            var ps = GetSettingsForPerson(personId, keys);

            foreach (var set in ps)
            {
                set.Value = (settings[set.Key] as DateTime?)?
                    .ToString(Constants.DATE_FORMAT, CultureInfo.InvariantCulture) 
                    ?? settings[set.Key]?.ToString();
            }
            DoUpdate(u => new PersonSettingDataAccess(u).Update(ps));

            settings = settings.Where(s => ps.All(x => x.Key != s.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            DoUpdate(u => new PersonSettingDataAccess(u).AddPersonSettings(personId, settings));
        }

        public IList<PersonSetting> GetSettingsForPerson(int personId)
        {
            return DoRead(u => new PersonSettingDataAccess(u).GetAll(new SimpleQueryCondition("PersonRef", personId, ConditionRelation.Equal)));
        }

        public IList<PersonSetting> GetSettingsForPerson(int personId, IList<string> keys)
        {
            return DoRead(u => new PersonSettingDataAccess(u).GetPersonSettings(personId, keys));
        }
    }
}
