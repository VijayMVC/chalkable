using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPersonSettingService
    {
        void SetSettingsForPerson(int personId, int schoolYearId, IDictionary<string, object> settings);
        void SetSettingsForPerson(IDictionary<string, object> settings, int? personId = null, int? schooLYearId = null, int? classId = null);
        IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId);
        IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId, IList<string> keys);
        IDictionary<string, string> GetSettingsForPerson(IList<string> keys, int? personId = null, int? schoolYearId = null, int? classId = null);
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
            SetSettingsForPerson(settings, personId, schoolYearId);
        }

        public void SetSettingsForPerson(IDictionary<string, object> settings, int? personId = null, int? schoolYearId = null, int? classId = null)
        {
            var keys = settings.Keys.ToList();
            if (!IsAvailableSettings(keys))
                throw new ArgumentException("Not recognized settings");

            var ps = GetSettingsForPerson(keys, personId, schoolYearId, classId);
            var toSet = ps.Select(x => new PersonSetting
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
            DoUpdate(u => new PersonSettingDataAccess(u).AddPersonSettings(settings, personId, schoolYearId, classId));
        }

        public IDictionary<string, string> GetSettingsForPerson(int personId, int schoolYearId)
        {
            return GetSettingsForPerson(personId, schoolYearId, new List<string>());
        }

        public IDictionary<string , string> GetSettingsForPerson(int personId, int schoolYearId, IList<string> keys)
        {
            return GetSettingsForPerson(keys, personId, schoolYearId);
        }
        public IDictionary<string, string> GetSettingsForPerson(IList<string> keys, int? personId = null, int? schoolYearId = null, int? classId = null)
        {
            return DoRead(u => new PersonSettingDataAccess(u).GetPersonSettings(keys, personId, schoolYearId, classId));
        }
    }
}
