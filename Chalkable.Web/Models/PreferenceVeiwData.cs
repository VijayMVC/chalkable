using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class PreferenceVeiwData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsPublic { get; set; }
        public int Category { get; set; }
        public int Type { get; set; }
        public string Hint { get; set; }

        private PreferenceVeiwData(){}

        public static PreferenceVeiwData Create(Preference preference)
        {
            var res = new PreferenceVeiwData
                          {
                              Key = preference.Key,
                              Value = preference.Value,
                              IsPublic = preference.IsPublic,
                              Category = (int) preference.Category,
                              Type = (int) preference.Type,
                              Hint = preference.Hint
                          };
            return res;
        }

        public static IList<PreferenceVeiwData> Create(IList<Preference> preferences)
        {
            var res = preferences.Select(Create);
            return res.ToList();
        }
    }
}